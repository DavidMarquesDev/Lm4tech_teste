using System.ComponentModel;
using System.Data.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductChallenge.Application;
using ProductChallenge.Application.Abstractions;
using ProductChallenge.Application.Messaging;
using ProductChallenge.Application.Reporting;
using ProductChallenge.Desktop.Common;
using ProductChallenge.Domain;

namespace ProductChallenge.Desktop.ViewModels;

public partial class ProductListViewModel : ObservableObject
{
    public const int DefaultPageSize = 10;

    // Array concreto porque o ComboBox exige uma origem de dados que implemente IList.
    private static readonly int[] AvailablePageSizes = [10, 15, 30, 50, 100];

    public static IReadOnlyList<int> PageSizeOptions => AvailablePageSizes;

    private readonly IProductService _productService;
    private readonly IProductExportService _exportService;
    private readonly IServiceBus<ProductChangedNotification> _bus;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartEditCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
    private Product? _selectedProduct;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _searchTerm = string.Empty;

    [ObservableProperty]
    private int _pageSize = DefaultPageSize;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GoToPreviousPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(GoToNextPageCommand))]
    private int _pageNumber = 1;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GoToPreviousPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(GoToNextPageCommand))]
    private int _pageCount = 1;

    [ObservableProperty]
    private string _pageSummary = string.Empty;

    public ProductListViewModel(
        IProductService productService,
        IProductExportService exportService,
        IServiceBus<ProductChangedNotification> bus)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
    }

    /// <summary>
    /// Assina as mudanças e faz a primeira carga. Fora do construtor porque trabalho assíncrono
    /// ali deixaria o objeto observável antes de estar pronto.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _bus.SubscribeAsync(_ => LoadAsync());
        await LoadAsync();
    }

    public IReadOnlyList<ExportField> GetExportableFields() => _exportService.GetAvailableFields();

    public Task<int> ExportAsync(IReadOnlyList<string> fieldNames, Stream destination) =>
        _exportService.ExportAsync(fieldNames, SearchTerm, destination);

    /// <summary>
    /// Como apresentar a falha é decisão da View, o que mantém este ViewModel independente de
    /// Windows Forms.
    /// </summary>
    public event EventHandler<string>? OperationFailed;

    public BindingList<Product> Products { get; } = [];

    public ProductEditorViewModel Editor { get; } = new();

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;

        try
        {
            var page = await _productService.ListAsync(SearchTerm, PageNumber, PageSize);

            Products.ReplaceAll(page.Items);

            PageNumber = page.PageNumber;
            PageCount = page.PageCount;
            PageSummary = DescribeResult(page, SearchTerm.Trim());
        }
        catch (Exception exception) when (exception is DataAccessException or DbException)
        {
            OperationFailed?.Invoke(this, $"Não foi possível carregar os produtos: {exception.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void StartNew()
    {
        Editor.StartNew();
        StatusMessage = "Preencha os dados do novo produto.";
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private void StartEdit()
    {
        if (SelectedProduct is null)
        {
            return;
        }

        Editor.StartEdit(SelectedProduct);
        StatusMessage = $"Editando \"{SelectedProduct.Name}\".";
    }

    [RelayCommand]
    private void CancelEdit()
    {
        Editor.StartNew();
        StatusMessage = "Edição cancelada.";
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var draft = Editor.TryBuildDraft();

        if (draft is null)
        {
            StatusMessage = "Corrija os campos destacados para continuar.";
            return;
        }

        IsBusy = true;

        try
        {
            if (Editor.EditingId is { } productId)
            {
                await _productService.UpdateAsync(productId, draft);
                StatusMessage = $"Produto \"{draft.Name}\" atualizado.";
            }
            else
            {
                await _productService.CreateAsync(draft);
                StatusMessage = $"Produto \"{draft.Name}\" adicionado.";
            }

            Editor.StartNew();
        }
        catch (KeyNotFoundException)
        {
            OperationFailed?.Invoke(this, "O produto não existe mais. A lista será atualizada.");
            Editor.StartNew();
            await LoadAsync();
        }
        catch (Exception exception) when (exception is DataAccessException or ArgumentException)
        {
            OperationFailed?.Invoke(this, $"Não foi possível salvar o produto: {exception.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task DeleteAsync()
    {
        if (SelectedProduct is null)
        {
            return;
        }

        var productId = SelectedProduct.Id;
        var productName = SelectedProduct.Name;
        IsBusy = true;

        try
        {
            await _productService.DeleteAsync(productId);
            StatusMessage = $"Produto \"{productName}\" excluído.";
        }
        catch (KeyNotFoundException)
        {
            StatusMessage = "O produto já havia sido excluído.";
        }
        catch (DataAccessException exception)
        {
            OperationFailed?.Invoke(this, $"Não foi possível excluir o produto: {exception.Message}");
            IsBusy = false;
            return;
        }

        if (Editor.EditingId == productId)
        {
            Editor.StartNew();
        }

        IsBusy = false;
    }

    [RelayCommand(CanExecute = nameof(CanGoToPreviousPage))]
    private async Task GoToPreviousPageAsync()
    {
        PageNumber--;
        await LoadAsync();
    }

    [RelayCommand(CanExecute = nameof(CanGoToNextPage))]
    private async Task GoToNextPageAsync()
    {
        PageNumber++;
        await LoadAsync();
    }

    // Trocar o filtro ou o tamanho da página invalida a posição atual: a página 4 do resultado
    // anterior não corresponde a nada no novo.
    partial void OnSearchTermChanged(string value) => PageNumber = 1;

    partial void OnPageSizeChanged(int value) => PageNumber = 1;

    private bool CanGoToPreviousPage() => PageNumber > 1;

    private bool CanGoToNextPage() => PageNumber < PageCount;

    private bool HasSelection() => SelectedProduct is not null;

    private static string DescribeResult(PagedResult<Product> page, string term)
    {
        var subject = term.Length > 0
            ? page.TotalCount switch
            {
                0 => $"nenhum resultado para \"{term}\"",
                1 => $"1 resultado para \"{term}\"",
                _ => $"{page.TotalCount} resultados para \"{term}\""
            }
            : page.TotalCount switch
            {
                0 => "nenhum produto cadastrado",
                1 => "1 produto cadastrado",
                _ => $"{page.TotalCount} produtos cadastrados"
            };

        return $"Página {page.PageNumber} de {page.PageCount} · {subject}";
    }
}
