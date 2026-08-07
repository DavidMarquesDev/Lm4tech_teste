using System.ComponentModel;
using ProductChallenge.Application.Reporting;
using ProductChallenge.Desktop.Common;

namespace ProductChallenge.Desktop.Views;

public partial class ExportColumnsDialog : Form
{
    // BindingList, não List: uma lista comum não notifica o DataGridView, e o CurrencyManager
    // fica dessincronizado depois que o conteúdo muda.
    private readonly BindingList<ColumnChoice> _choices = [];

    public ExportColumnsDialog()
    {
        InitializeComponent();

        gridColumns.AutoGenerateColumns = false;
        gridColumns.DataSource = _choices;

        btnMoveUp.Click += (_, _) => MoveSelection(-1);
        btnMoveDown.Click += (_, _) => MoveSelection(1);
        btnSelectAll.Click += (_, _) => SetAll(selected: true);
        btnSelectNone.Click += (_, _) => SetAll(selected: false);
        btnConfirm.Click += OnConfirmClick;
    }

    /// <summary>Nomes das propriedades escolhidas, na ordem definida pelo usuário.</summary>
    public IReadOnlyList<string> SelectedFieldNames { get; private set; } = [];

    public void LoadFields(IReadOnlyList<ExportField> fields)
    {
        ArgumentNullException.ThrowIfNull(fields);

        _choices.ReplaceAll(fields.Select(field => new ColumnChoice(field.PropertyName, field.Header)));
    }

    private void MoveSelection(int offset)
    {
        var index = gridColumns.CurrentRow?.Index ?? -1;
        var target = index + offset;

        if (index < 0 || target < 0 || target >= _choices.Count)
        {
            return;
        }

        (_choices[index], _choices[target]) = (_choices[target], _choices[index]);

        gridColumns.CurrentCell = gridColumns.Rows[target].Cells[colHeader.Index];
    }

    private void SetAll(bool selected)
    {
        foreach (var choice in _choices)
        {
            choice.Selected = selected;
        }

        // As marcações mudam dentro dos itens, e não na coleção, então o aviso é manual.
        _choices.ResetBindings();
    }

    private void OnConfirmClick(object? sender, EventArgs e)
    {
        gridColumns.EndEdit();

        var selected = _choices.Where(choice => choice.Selected).Select(choice => choice.PropertyName).ToList();

        if (selected.Count == 0)
        {
            MessageBox.Show(
                this, "Selecione ao menos uma coluna.", "Exportar",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        SelectedFieldNames = selected;
        DialogResult = DialogResult.OK;
    }

    private sealed class ColumnChoice(string propertyName, string header)
    {
        public bool Selected { get; set; } = true;

        public string PropertyName { get; } = propertyName;

        public string Header { get; } = header;
    }
}
