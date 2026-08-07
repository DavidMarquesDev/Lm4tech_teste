using System.ComponentModel;
using ProductChallenge.Application;
using ProductChallenge.Desktop.Common;
using ProductChallenge.Domain;
using ProductChallenge.Desktop.ViewModels;

namespace ProductChallenge.Desktop.Views;

public partial class MainForm : Form
{
    private readonly ProductListViewModel _viewModel;
    private readonly Func<ExportColumnsDialog> _exportDialogFactory;
    private readonly BindingSource _productsBinding = new();
    private readonly BindingSource _editorBinding = new();
    private readonly BindingSource _listBinding = new();
    private readonly Dictionary<string, Control> _errorTargets;

    private readonly System.Windows.Forms.Timer _searchDebounce = new() { Interval = 300 };

    private bool _syncingCategory;

    public MainForm(ProductListViewModel viewModel, Func<ExportColumnsDialog> exportDialogFactory)
    {
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        _exportDialogFactory = exportDialogFactory ?? throw new ArgumentNullException(nameof(exportDialogFactory));

        InitializeComponent();

        _errorTargets = new Dictionary<string, Control>(StringComparer.Ordinal)
        {
            [nameof(ProductInput.Name)] = txtName,
            [nameof(ProductInput.Description)] = txtDescription,
            [nameof(ProductInput.Category)] = cboCategory,
            [nameof(ProductInput.Price)] = txtPrice,
            [nameof(ProductInput.StockQuantity)] = txtStockQuantity
        };

        BindGrid();
        BindEditor();
        BindStatus();
        BindCommands();
        BindSearch();
        BindPaging();

        _viewModel.OperationFailed += OnOperationFailed;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        _viewModel.Editor.PropertyChanged += OnEditorPropertyChanged;

        _viewModel.Editor.StartNew();

        // O ComboBox seleciona o primeiro item ao receber a origem de dados, e StartNew não
        // notifica quando a categoria já era nula.
        cboCategory.SelectedIndex = -1;
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await _viewModel.InitializeAsync();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        // A ordem importa: descartar o BindingSource com o grid ainda ligado dispara
        // SelectionChanged sobre uma linha que já não existe, e DataBoundItem lança.
        gridProducts.SelectionChanged -= OnGridSelectionChanged;
        gridProducts.CellFormatting -= OnGridCellFormatting;
        gridProducts.CellDoubleClick -= OnGridCellDoubleClick;
        cboCategory.SelectedIndexChanged -= OnCategorySelectionChanged;

        _viewModel.OperationFailed -= OnOperationFailed;
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        _viewModel.Editor.PropertyChanged -= OnEditorPropertyChanged;

        gridProducts.DataSource = null;

        _searchDebounce.Dispose();
        _productsBinding.Dispose();
        _editorBinding.Dispose();
        _listBinding.Dispose();

        base.OnFormClosed(e);
    }

    private void BindGrid()
    {
        _productsBinding.DataSource = _viewModel.Products;
        gridProducts.DataSource = _productsBinding;

        gridProducts.CellFormatting += OnGridCellFormatting;
        gridProducts.SelectionChanged += OnGridSelectionChanged;
        gridProducts.CellDoubleClick += OnGridCellDoubleClick;
    }

    private void BindEditor()
    {
        _editorBinding.DataSource = _viewModel.Editor;

        cboCategory.DisplayMember = nameof(CategoryOption.Label);
        cboCategory.DataSource = _viewModel.Editor.Categories;

        txtName.DataBindings.Add(
            nameof(TextBox.Text), _editorBinding, nameof(ProductEditorViewModel.Name),
            false, DataSourceUpdateMode.OnPropertyChanged);

        txtDescription.DataBindings.Add(
            nameof(TextBox.Text), _editorBinding, nameof(ProductEditorViewModel.Description),
            false, DataSourceUpdateMode.OnPropertyChanged);

        txtPrice.DataBindings.Add(
            nameof(TextBox.Text), _editorBinding, nameof(ProductEditorViewModel.Price),
            false, DataSourceUpdateMode.OnPropertyChanged);

        txtStockQuantity.DataBindings.Add(
            nameof(TextBox.Text), _editorBinding, nameof(ProductEditorViewModel.StockQuantity),
            false, DataSourceUpdateMode.OnPropertyChanged);

        // Sem DataBindings: o WinForms não expõe SelectedItemChanged, então um binding em
        // SelectedItem só grava na perda de foco — e acionar Salvar por atalho não move o foco.
        cboCategory.SelectedIndexChanged += OnCategorySelectionChanged;
    }

    private void BindStatus()
    {
        _listBinding.DataSource = _viewModel;

        lblStatus.DataBindings.Add(
            nameof(Label.Text), _listBinding, nameof(ProductListViewModel.StatusMessage),
            false, DataSourceUpdateMode.Never);
    }

    private void BindCommands()
    {
        CommandBinder.Bind(btnSave, _viewModel.SaveCommand);
        CommandBinder.Bind(btnCancel, _viewModel.CancelEditCommand);
        CommandBinder.Bind(btnNew, _viewModel.StartNewCommand);
        CommandBinder.Bind(btnEdit, _viewModel.StartEditCommand);

        // Sem o binder: a confirmação precisa ocorrer antes do comando.
        CommandBinder.BindEnabled(btnDelete, _viewModel.DeleteCommand);
        btnDelete.Click += OnDeleteClick;
        btnExport.Click += OnExportClick;
    }

    private void BindPaging()
    {
        cboPageSize.DataSource = ProductListViewModel.PageSizeOptions;
        cboPageSize.SelectedItem = _viewModel.PageSize;

        cboPageSize.SelectedIndexChanged += async (_, _) =>
        {
            if (cboPageSize.SelectedItem is int size && size != _viewModel.PageSize)
            {
                _viewModel.PageSize = size;
                await _viewModel.LoadCommand.ExecuteAsync(null);
            }
        };

        CommandBinder.Bind(btnPreviousPage, _viewModel.GoToPreviousPageCommand);
        CommandBinder.Bind(btnNextPage, _viewModel.GoToNextPageCommand);

        lblPageSummary.DataBindings.Add(
            nameof(Label.Text), _listBinding, nameof(ProductListViewModel.PageSummary),
            false, DataSourceUpdateMode.Never);
    }

    private void BindSearch()
    {
        txtSearch.TextChanged += (_, _) =>
        {
            _searchDebounce.Stop();
            _searchDebounce.Start();
        };

        _searchDebounce.Tick += async (_, _) =>
        {
            _searchDebounce.Stop();
            _viewModel.SearchTerm = txtSearch.Text;
            await _viewModel.LoadCommand.ExecuteAsync(null);
        };
    }

    private async void OnExportClick(object? sender, EventArgs e)
    {
        using var dialog = _exportDialogFactory();
        dialog.LoadFields(_viewModel.GetExportableFields());

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        using var save = new SaveFileDialog
        {
            Title = "Salvar exportação",
            Filter = "Arquivo CSV (*.csv)|*.csv",
            FileName = $"produtos_{DateTime.Now:yyyy-MM-dd_HHmm}.csv",
            DefaultExt = "csv",
            AddExtension = true
        };

        if (save.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        btnExport.Enabled = false;

        try
        {
            // useAsync habilita I/O realmente assíncrona no Windows.
            await using var file = new FileStream(
                save.FileName, FileMode.Create, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true);

            var rowCount = await _viewModel.ExportAsync(dialog.SelectedFieldNames, file);

            MessageBox.Show(
                this, $"{rowCount} produto(s) exportado(s) para:{Environment.NewLine}{save.FileName}",
                "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or InvalidOperationException)
        {
            MessageBox.Show(
                this, $"Não foi possível exportar: {exception.Message}",
                "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            btnExport.Enabled = true;
        }
    }

    private void OnCategorySelectionChanged(object? sender, EventArgs e)
    {
        if (_syncingCategory)
        {
            return;
        }

        _viewModel.Editor.SelectedCategory = cboCategory.SelectedItem as CategoryOption;
    }

    private void SyncCategorySelection()
    {
        var selected = _viewModel.Editor.SelectedCategory;

        if (Equals(cboCategory.SelectedItem, selected))
        {
            return;
        }

        _syncingCategory = true;

        try
        {
            if (selected is null)
            {
                cboCategory.SelectedIndex = -1;
            }
            else
            {
                cboCategory.SelectedItem = selected;
            }
        }
        finally
        {
            _syncingCategory = false;
        }
    }

    private void OnGridCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.ColumnIndex == colCategory.Index && e.Value is ProductCategory category)
        {
            e.Value = ProductCategoryCatalog.LabelFor(category);
            e.FormattingApplied = true;
        }
    }

    private void OnGridSelectionChanged(object? sender, EventArgs e)
    {
        _viewModel.SelectedProduct = gridProducts.CurrentRow?.DataBoundItem as Product;
    }

    private void OnGridCellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && _viewModel.StartEditCommand.CanExecute(null))
        {
            _viewModel.StartEditCommand.Execute(null);
            txtName.Focus();
        }
    }

    private async void OnDeleteClick(object? sender, EventArgs e)
    {
        var product = _viewModel.SelectedProduct;

        if (product is null)
        {
            return;
        }

        var answer = MessageBox.Show(
            this,
            $"Excluir o produto \"{product.Name}\"?",
            "Confirmar exclusão",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (answer == DialogResult.Yes)
        {
            await _viewModel.DeleteCommand.ExecuteAsync(null);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ProductListViewModel.IsBusy))
        {
            UseWaitCursor = _viewModel.IsBusy;
        }
    }

    private void OnEditorPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ProductEditorViewModel.Errors):
                ShowValidationErrors();
                break;

            case nameof(ProductEditorViewModel.SelectedCategory):
                SyncCategorySelection();
                break;

            case nameof(ProductEditorViewModel.IsEditing):
                lblEditorTitle.Text = _viewModel.Editor.IsEditing ? "Editar produto" : "Novo produto";
                break;
        }
    }

    private void ShowValidationErrors()
    {
        foreach (var control in _errorTargets.Values)
        {
            errorProvider.SetError(control, string.Empty);
        }

        foreach (var error in _viewModel.Editor.Errors)
        {
            if (_errorTargets.TryGetValue(error.PropertyName, out var control))
            {
                errorProvider.SetError(control, error.Message);
            }
        }

        var firstInvalid = _viewModel.Editor.Errors
            .Select(error => _errorTargets.TryGetValue(error.PropertyName, out var control) ? control : null)
            .FirstOrDefault(control => control is not null);

        firstInvalid?.Focus();
    }

    private void OnOperationFailed(object? sender, string message)
    {
        MessageBox.Show(this, message, "Cadastro de Produtos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
