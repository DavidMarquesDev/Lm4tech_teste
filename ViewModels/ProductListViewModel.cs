using System.ComponentModel;
using System.Data.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using ProductChallenge.Common;
using ProductChallenge.Data;
using ProductChallenge.Models;

namespace ProductChallenge.ViewModels;

public partial class ProductListViewModel : ObservableObject
{
    private readonly Func<AppDbContext> _contextFactory;

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

    public ProductListViewModel(Func<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
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
            await using var context = _contextFactory();

            var query = context.Products.AsNoTracking();
            var term = SearchTerm.Trim();

            if (term.Length > 0)
            {
                var pattern = $"%{SearchNormalizer.Normalize(term)}%";
                query = query.Where(product => EF.Functions.Like(product.SearchText, pattern));
            }

            var products = await query.OrderBy(product => product.Name).ToListAsync();

            Products.ReplaceAll(products);
            StatusMessage = DescribeResult(products.Count, term);
        }
        catch (Exception exception) when (exception is DbException or InvalidOperationException)
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
            await using var context = _contextFactory();

            if (Editor.EditingId is { } productId)
            {
                var product = await context.Products.FirstOrDefaultAsync(item => item.Id == productId);

                if (product is null)
                {
                    OperationFailed?.Invoke(this, "O produto não existe mais. A lista será atualizada.");
                    Editor.StartNew();
                    await LoadAsync();
                    return;
                }

                product.SetDetails(
                    draft.Name, draft.Description, draft.Price, draft.Category, draft.StockQuantity);
                await context.SaveChangesAsync();
                StatusMessage = $"Produto \"{product.Name}\" atualizado.";
            }
            else
            {
                var product = Product.Create(
                    draft.Name, draft.Description, draft.Price, draft.Category, draft.StockQuantity);
                context.Products.Add(product);
                await context.SaveChangesAsync();
                StatusMessage = $"Produto \"{product.Name}\" adicionado.";
            }

            Editor.StartNew();
            await LoadAsync();
        }
        catch (Exception exception) when (exception is DbUpdateException or ArgumentException)
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
            await using var context = _contextFactory();

            var affectedRows = await context.Products
                .Where(product => product.Id == productId)
                .ExecuteDeleteAsync();

            StatusMessage = affectedRows > 0
                ? $"Produto \"{productName}\" excluído."
                : "O produto já havia sido excluído.";

            if (Editor.EditingId == productId)
            {
                Editor.StartNew();
            }

            await LoadAsync();
        }
        catch (DbUpdateException exception)
        {
            OperationFailed?.Invoke(this, $"Não foi possível excluir o produto: {exception.Message}");
        }
        finally
        {
            IsBusy = false;
        }
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
