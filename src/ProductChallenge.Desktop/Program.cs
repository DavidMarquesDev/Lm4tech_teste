using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductChallenge.Desktop.Composition;
using ProductChallenge.Desktop.Views;
using ProductChallenge.Infrastructure;
using ProductChallenge.Infrastructure.Messaging;
using ProductChallenge.Infrastructure.Persistence;
using Serilog;

namespace ProductChallenge.Desktop;

internal static class Program
{
    private const string SeedArgument = "--seed";
    private const int SampleSize = 100;

    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        try
        {
            var services = new ServiceCollection();
            services.AddProductChallenge(DatabaseLocation.ConnectionString);

            using var provider = services.BuildServiceProvider();
            provider.MigrateDatabase();
            provider.SubscribeAuditLog();

            RegisterGlobalHandlers(provider);

            if (args.Contains(SeedArgument, StringComparer.OrdinalIgnoreCase))
            {
                provider.SeedSampleData(SampleSize);
            }

            // Qualificado porque o namespace ProductChallenge.Application deixa "Application"
            // ambíguo neste escopo.
            System.Windows.Forms.Application.Run(provider.GetRequiredService<MainForm>());
        }
        finally
        {
            // As últimas linhas antes de um encerramento anormal são justamente as que
            // interessam depois, e sem o flush elas podem não chegar ao disco.
            Log.CloseAndFlush();
        }
    }

    /// <summary>
    /// Sem estes ganchos, uma exceção não tratada derruba a aplicação sem deixar registro.
    /// </summary>
    private static void RegisterGlobalHandlers(IServiceProvider provider)
    {
        var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger("Program");

        System.Windows.Forms.Application.ThreadException += (_, args) =>
        {
            logger.LogCritical(args.Exception, "Exceção não tratada na thread da interface.");
            ShowUnexpectedError();
        };

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            logger.LogCritical(args.ExceptionObject as Exception, "Exceção não tratada no domínio.");

        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            logger.LogError(args.Exception, "Exceção de Task não observada.");
            args.SetObserved();
        };
    }

    private static void ShowUnexpectedError() =>
        MessageBox.Show(
            $"Ocorreu um erro inesperado.{Environment.NewLine}{Environment.NewLine}"
            + $"Detalhes registrados em:{Environment.NewLine}{LogLocation.Directory}",
            "Cadastro de Produtos", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
