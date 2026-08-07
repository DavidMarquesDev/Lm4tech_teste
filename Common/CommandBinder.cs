using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace ProductChallenge.Common;

/// <summary>
/// Windows Forms não tem a propriedade Command dos botões. Centralizar a ligação evita repetir
/// a verificação de CanExecute e a sincronização de Enabled em cada handler.
/// </summary>
public static class CommandBinder
{
    public static void Bind(Button button, IAsyncRelayCommand command)
    {
        ArgumentNullException.ThrowIfNull(button);
        ArgumentNullException.ThrowIfNull(command);

        BindEnabled(button, command);
        button.Click += async (_, _) => await command.ExecuteAsync(null);
    }

    public static void Bind(Button button, IRelayCommand command)
    {
        ArgumentNullException.ThrowIfNull(button);
        ArgumentNullException.ThrowIfNull(command);

        BindEnabled(button, command);
        button.Click += (_, _) => command.Execute(null);
    }

    public static void BindEnabled(Button button, ICommand command)
    {
        ArgumentNullException.ThrowIfNull(button);
        ArgumentNullException.ThrowIfNull(command);

        void Synchronize(object? sender, EventArgs args) => button.Enabled = command.CanExecute(null);

        command.CanExecuteChanged += Synchronize;
        Synchronize(null, EventArgs.Empty);
    }
}
