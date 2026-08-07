using System.ComponentModel;
using System.Data.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductChallenge.Application.Abstractions;
using ProductChallenge.Desktop.Common;
using ProductChallenge.Domain;

namespace ProductChallenge.Desktop.ViewModels;

public partial class ProductListViewModel : ObservableObject
{
    private readonly IProductService _productService;

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

    public ProductListViewModel(IProductService productService)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
    }

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
            var products = await _productService.ListAsync(SearchTerm);

            Products.ReplaceAll(products);
            StatusMessage = DescribeResult(products.Count, SearchTerm.Trim());
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
            await LoadAsync();
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
        await LoadAsync();
    }

    private bool HasSelection() => SelectedProduct is not null;

    private static string DescribeResult(int count, string term)
    {
        if (term.Length > 0)
        {
            return count switch
            {
                0 => $"Nenhum produto encontrado para \"{term}\".",
                1 => $"1 produto encontrado para \"{term}\".",
                _ => $"{count} produtos encontrados para \"{term}\"."
            };
        }

        return count switch
        {
            0 => "Nenhum produto cadastrado.",
            1 => "1 produto cadastrado.",
            _ => $"{count} produtos cadastrados."
        };
    }
}
