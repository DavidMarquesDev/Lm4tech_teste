namespace ProductChallenge.Desktop.Views
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tlpRoot = new System.Windows.Forms.TableLayoutPanel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.pnlGrid = new System.Windows.Forms.Panel();
            this.gridProducts = new System.Windows.Forms.DataGridView();
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStockQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlEditor = new System.Windows.Forms.Panel();
            this.tlpEditor = new System.Windows.Forms.TableLayoutPanel();
            this.lblEditorTitle = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblCategory = new System.Windows.Forms.Label();
            this.lblPrice = new System.Windows.Forms.Label();
            this.lblStockQuantity = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.cboCategory = new System.Windows.Forms.ComboBox();
            this.txtPrice = new System.Windows.Forms.TextBox();
            this.txtStockQuantity = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.tlpButtons = new System.Windows.Forms.TableLayoutPanel();
            this.flpRecordActions = new System.Windows.Forms.FlowLayoutPanel();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.flpFormActions = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.tlpRoot.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridProducts)).BeginInit();
            this.pnlEditor.SuspendLayout();
            this.tlpEditor.SuspendLayout();
            this.tlpButtons.SuspendLayout();
            this.flpRecordActions.SuspendLayout();
            this.flpFormActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            //
            // tlpRoot
            //
            this.tlpRoot.BackColor = System.Drawing.Color.White;
            this.tlpRoot.ColumnCount = 1;
            this.tlpRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRoot.Controls.Add(this.pnlHeader, 0, 0);
            this.tlpRoot.Controls.Add(this.pnlGrid, 0, 1);
            this.tlpRoot.Controls.Add(this.pnlEditor, 0, 2);
            this.tlpRoot.Controls.Add(this.lblStatus, 0, 3);
            this.tlpRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpRoot.Location = new System.Drawing.Point(0, 0);
            this.tlpRoot.Name = "tlpRoot";
            this.tlpRoot.Padding = new System.Windows.Forms.Padding(20, 16, 20, 10);
            this.tlpRoot.RowCount = 4;
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpRoot.Size = new System.Drawing.Size(1020, 700);
            this.tlpRoot.TabIndex = 0;
            //
            // pnlHeader
            //
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.lblSearch);
            this.pnlHeader.Controls.Add(this.txtSearch);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHeader.Location = new System.Drawing.Point(23, 19);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(3, 3, 3, 12);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(974, 40);
            this.pnlHeader.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblTitle.Location = new System.Drawing.Point(0, 4);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(180, 28);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Produtos";
            //
            // lblSearch
            //
            this.lblSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSearch.AutoSize = true;
            this.lblSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblSearch.Location = new System.Drawing.Point(651, 11);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(52, 15);
            this.lblSearch.TabIndex = 1;
            this.lblSearch.Text = "&Localizar";
            //
            // txtSearch
            //
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Location = new System.Drawing.Point(709, 8);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.PlaceholderText = "nome ou descrição";
            this.txtSearch.Size = new System.Drawing.Size(265, 23);
            this.txtSearch.TabIndex = 2;
            //
            // pnlGrid
            //
            this.pnlGrid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.pnlGrid.Controls.Add(this.gridProducts);
            this.pnlGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGrid.Location = new System.Drawing.Point(23, 74);
            this.pnlGrid.Name = "pnlGrid";
            this.pnlGrid.Padding = new System.Windows.Forms.Padding(1);
            this.pnlGrid.Size = new System.Drawing.Size(974, 344);
            this.pnlGrid.TabIndex = 1;
            //
            // gridProducts
            //
            this.gridProducts.AllowUserToAddRows = false;
            this.gridProducts.AllowUserToDeleteRows = false;
            this.gridProducts.AllowUserToResizeRows = false;
            this.gridProducts.AutoGenerateColumns = false;
            this.gridProducts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridProducts.BackgroundColor = System.Drawing.Color.White;
            this.gridProducts.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridProducts.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.gridProducts.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(250)))), ((int)(((byte)(251)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(250)))), ((int)(((byte)(251)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.gridProducts.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridProducts.ColumnHeadersHeight = 38;
            this.gridProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridProducts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colId,
            this.colName,
            this.colCategory,
            this.colPrice,
            this.colStockQuantity});
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(234)))), ((int)(((byte)(254)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(58)))), ((int)(((byte)(138)))));
            this.gridProducts.DefaultCellStyle = dataGridViewCellStyle2;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(234)))), ((int)(((byte)(254)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(58)))), ((int)(((byte)(138)))));
            this.gridProducts.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.gridProducts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridProducts.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.gridProducts.EnableHeadersVisualStyles = false;
            this.gridProducts.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(241)))), ((int)(((byte)(244)))));
            this.gridProducts.Location = new System.Drawing.Point(1, 1);
            this.gridProducts.MultiSelect = false;
            this.gridProducts.Name = "gridProducts";
            this.gridProducts.ReadOnly = true;
            this.gridProducts.RowHeadersVisible = false;
            this.gridProducts.RowTemplate.Height = 34;
            this.gridProducts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridProducts.Size = new System.Drawing.Size(972, 342);
            this.gridProducts.TabIndex = 0;
            //
            // colId
            //
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.colId.DataPropertyName = "Id";
            this.colId.DefaultCellStyle = dataGridViewCellStyle4;
            this.colId.FillWeight = 8F;
            this.colId.HeaderText = "Código";
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            //
            // colName
            //
            this.colName.DataPropertyName = "Name";
            this.colName.FillWeight = 46F;
            this.colName.HeaderText = "Produto";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            //
            // colCategory
            //
            this.colCategory.DataPropertyName = "Category";
            this.colCategory.FillWeight = 18F;
            this.colCategory.HeaderText = "Categoria";
            this.colCategory.Name = "colCategory";
            this.colCategory.ReadOnly = true;
            //
            // colPrice
            //
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Format = "C2";
            dataGridViewCellStyle5.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.colPrice.DataPropertyName = "Price";
            this.colPrice.DefaultCellStyle = dataGridViewCellStyle5;
            this.colPrice.FillWeight = 14F;
            this.colPrice.HeaderText = "Preço";
            this.colPrice.Name = "colPrice";
            this.colPrice.ReadOnly = true;
            //
            // colStockQuantity
            //
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "N0";
            dataGridViewCellStyle6.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.colStockQuantity.DataPropertyName = "StockQuantity";
            this.colStockQuantity.DefaultCellStyle = dataGridViewCellStyle6;
            this.colStockQuantity.FillWeight = 14F;
            this.colStockQuantity.HeaderText = "Estoque";
            this.colStockQuantity.Name = "colStockQuantity";
            this.colStockQuantity.ReadOnly = true;
            //
            // pnlEditor
            //
            this.pnlEditor.AutoSize = true;
            this.pnlEditor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlEditor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(250)))), ((int)(((byte)(251)))));
            this.pnlEditor.Controls.Add(this.tlpEditor);
            this.pnlEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEditor.Location = new System.Drawing.Point(23, 424);
            this.pnlEditor.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.pnlEditor.Name = "pnlEditor";
            this.pnlEditor.Padding = new System.Windows.Forms.Padding(16, 12, 16, 12);
            this.pnlEditor.Size = new System.Drawing.Size(974, 230);
            this.pnlEditor.TabIndex = 2;
            //
            // tlpEditor
            //
            this.tlpEditor.AutoSize = true;
            this.tlpEditor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpEditor.BackColor = System.Drawing.Color.Transparent;
            this.tlpEditor.ColumnCount = 4;
            this.tlpEditor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tlpEditor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24F));
            this.tlpEditor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18F));
            this.tlpEditor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18F));
            this.tlpEditor.Controls.Add(this.lblEditorTitle, 0, 0);
            this.tlpEditor.Controls.Add(this.lblName, 0, 1);
            this.tlpEditor.Controls.Add(this.lblCategory, 1, 1);
            this.tlpEditor.Controls.Add(this.lblPrice, 2, 1);
            this.tlpEditor.Controls.Add(this.lblStockQuantity, 3, 1);
            this.tlpEditor.Controls.Add(this.txtName, 0, 2);
            this.tlpEditor.Controls.Add(this.cboCategory, 1, 2);
            this.tlpEditor.Controls.Add(this.txtPrice, 2, 2);
            this.tlpEditor.Controls.Add(this.txtStockQuantity, 3, 2);
            this.tlpEditor.Controls.Add(this.lblDescription, 0, 3);
            this.tlpEditor.Controls.Add(this.txtDescription, 0, 4);
            this.tlpEditor.Controls.Add(this.tlpButtons, 0, 5);
            this.tlpEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpEditor.Location = new System.Drawing.Point(16, 12);
            this.tlpEditor.Name = "tlpEditor";
            this.tlpEditor.RowCount = 6;
            this.tlpEditor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpEditor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpEditor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpEditor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpEditor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpEditor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpEditor.Size = new System.Drawing.Size(942, 206);
            this.tlpEditor.TabIndex = 0;
            //
            // lblEditorTitle
            //
            this.lblEditorTitle.AutoSize = true;
            this.tlpEditor.SetColumnSpan(this.lblEditorTitle, 4);
            this.lblEditorTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblEditorTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblEditorTitle.Location = new System.Drawing.Point(3, 0);
            this.lblEditorTitle.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.lblEditorTitle.Name = "lblEditorTitle";
            this.lblEditorTitle.Size = new System.Drawing.Size(120, 19);
            this.lblEditorTitle.TabIndex = 0;
            this.lblEditorTitle.Text = "Novo produto";
            //
            // lblName
            //
            this.lblName.AutoSize = true;
            this.lblName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.lblName.Location = new System.Drawing.Point(3, 29);
            this.lblName.Margin = new System.Windows.Forms.Padding(3, 0, 3, 4);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(43, 15);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "&Nome";
            //
            // lblCategory
            //
            this.lblCategory.AutoSize = true;
            this.lblCategory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.lblCategory.Location = new System.Drawing.Point(379, 29);
            this.lblCategory.Margin = new System.Windows.Forms.Padding(3, 0, 3, 4);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(58, 15);
            this.lblCategory.TabIndex = 2;
            this.lblCategory.Text = "C&ategoria";
            //
            // lblPrice
            //
            this.lblPrice.AutoSize = true;
            this.lblPrice.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.lblPrice.Location = new System.Drawing.Point(605, 29);
            this.lblPrice.Margin = new System.Windows.Forms.Padding(3, 0, 3, 4);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(37, 15);
            this.lblPrice.TabIndex = 3;
            this.lblPrice.Text = "&Preço";
            //
            // lblStockQuantity
            //
            this.lblStockQuantity.AutoSize = true;
            this.lblStockQuantity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.lblStockQuantity.Location = new System.Drawing.Point(774, 29);
            this.lblStockQuantity.Margin = new System.Windows.Forms.Padding(3, 0, 3, 4);
            this.lblStockQuantity.Name = "lblStockQuantity";
            this.lblStockQuantity.Size = new System.Drawing.Size(50, 15);
            this.lblStockQuantity.TabIndex = 4;
            this.lblStockQuantity.Text = "&Estoque";
            //
            // txtName
            //
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtName.Location = new System.Drawing.Point(3, 51);
            this.txtName.Margin = new System.Windows.Forms.Padding(3, 3, 26, 10);
            this.txtName.MaxLength = 120;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(347, 23);
            this.txtName.TabIndex = 5;
            //
            // cboCategory
            //
            this.cboCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCategory.FormattingEnabled = true;
            this.cboCategory.Location = new System.Drawing.Point(379, 51);
            this.cboCategory.Margin = new System.Windows.Forms.Padding(3, 3, 26, 10);
            this.cboCategory.Name = "cboCategory";
            this.cboCategory.Size = new System.Drawing.Size(197, 23);
            this.cboCategory.TabIndex = 6;
            //
            // txtPrice
            //
            this.txtPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPrice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPrice.Location = new System.Drawing.Point(605, 51);
            this.txtPrice.Margin = new System.Windows.Forms.Padding(3, 3, 26, 10);
            this.txtPrice.MaxLength = 15;
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.Size = new System.Drawing.Size(140, 23);
            this.txtPrice.TabIndex = 7;
            this.txtPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //
            // txtStockQuantity
            //
            this.txtStockQuantity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStockQuantity.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtStockQuantity.Location = new System.Drawing.Point(774, 51);
            this.txtStockQuantity.Margin = new System.Windows.Forms.Padding(3, 3, 26, 10);
            this.txtStockQuantity.MaxLength = 10;
            this.txtStockQuantity.Name = "txtStockQuantity";
            this.txtStockQuantity.Size = new System.Drawing.Size(140, 23);
            this.txtStockQuantity.TabIndex = 8;
            this.txtStockQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //
            // lblDescription
            //
            this.lblDescription.AutoSize = true;
            this.tlpEditor.SetColumnSpan(this.lblDescription, 4);
            this.lblDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.lblDescription.Location = new System.Drawing.Point(3, 87);
            this.lblDescription.Margin = new System.Windows.Forms.Padding(3, 0, 3, 4);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(150, 15);
            this.lblDescription.TabIndex = 9;
            this.lblDescription.Text = "&Descrição / ficha técnica";
            //
            // txtDescription
            //
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tlpEditor.SetColumnSpan(this.txtDescription, 4);
            this.txtDescription.Location = new System.Drawing.Point(3, 109);
            this.txtDescription.Margin = new System.Windows.Forms.Padding(3, 3, 26, 12);
            this.txtDescription.MaxLength = 1000;
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.PlaceholderText = "Especificações, medidas, observações...";
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(913, 62);
            this.txtDescription.TabIndex = 10;
            //
            // tlpButtons
            //
            this.tlpButtons.AutoSize = true;
            this.tlpButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpButtons.ColumnCount = 2;
            this.tlpEditor.SetColumnSpan(this.tlpButtons, 4);
            this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpButtons.Controls.Add(this.flpRecordActions, 0, 0);
            this.tlpButtons.Controls.Add(this.flpFormActions, 1, 0);
            this.tlpButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpButtons.Location = new System.Drawing.Point(0, 186);
            this.tlpButtons.Margin = new System.Windows.Forms.Padding(0);
            this.tlpButtons.Name = "tlpButtons";
            this.tlpButtons.RowCount = 1;
            this.tlpButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpButtons.Size = new System.Drawing.Size(942, 35);
            this.tlpButtons.TabIndex = 11;
            //
            // flpRecordActions
            //
            this.flpRecordActions.AutoSize = true;
            this.flpRecordActions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpRecordActions.Controls.Add(this.btnNew);
            this.flpRecordActions.Controls.Add(this.btnEdit);
            this.flpRecordActions.Controls.Add(this.btnDelete);
            this.flpRecordActions.Location = new System.Drawing.Point(0, 0);
            this.flpRecordActions.Margin = new System.Windows.Forms.Padding(0);
            this.flpRecordActions.Name = "flpRecordActions";
            this.flpRecordActions.Size = new System.Drawing.Size(318, 35);
            this.flpRecordActions.TabIndex = 0;
            this.flpRecordActions.WrapContents = false;
            //
            // btnNew
            //
            this.btnNew.BackColor = System.Drawing.Color.White;
            this.btnNew.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(213)))), ((int)(((byte)(219)))));
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnNew.Location = new System.Drawing.Point(0, 3);
            this.btnNew.Margin = new System.Windows.Forms.Padding(0, 3, 8, 3);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(98, 29);
            this.btnNew.TabIndex = 0;
            this.btnNew.Text = "N&ovo";
            this.btnNew.UseVisualStyleBackColor = false;
            //
            // btnEdit
            //
            this.btnEdit.BackColor = System.Drawing.Color.White;
            this.btnEdit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(213)))), ((int)(((byte)(219)))));
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnEdit.Location = new System.Drawing.Point(106, 3);
            this.btnEdit.Margin = new System.Windows.Forms.Padding(0, 3, 8, 3);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(98, 29);
            this.btnEdit.TabIndex = 1;
            this.btnEdit.Text = "&Editar";
            this.btnEdit.UseVisualStyleBackColor = false;
            //
            // btnDelete
            //
            this.btnDelete.BackColor = System.Drawing.Color.White;
            this.btnDelete.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(165)))), ((int)(((byte)(165)))));
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.btnDelete.Location = new System.Drawing.Point(212, 3);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(0, 3, 8, 3);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(98, 29);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "E&xcluir";
            this.btnDelete.UseVisualStyleBackColor = false;
            //
            // flpFormActions
            //
            this.flpFormActions.AutoSize = true;
            this.flpFormActions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpFormActions.Controls.Add(this.btnCancel);
            this.flpFormActions.Controls.Add(this.btnSave);
            this.flpFormActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpFormActions.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpFormActions.Location = new System.Drawing.Point(318, 0);
            this.flpFormActions.Margin = new System.Windows.Forms.Padding(0);
            this.flpFormActions.Name = "flpFormActions";
            this.flpFormActions.Size = new System.Drawing.Size(624, 35);
            this.flpFormActions.TabIndex = 1;
            this.flpFormActions.WrapContents = false;
            //
            // btnCancel
            //
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(213)))), ((int)(((byte)(219)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnCancel.Location = new System.Drawing.Point(526, 3);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(8, 3, 0, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(98, 29);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "&Cancelar";
            this.btnCancel.UseVisualStyleBackColor = false;
            //
            // btnSave
            //
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(420, 3);
            this.btnSave.Margin = new System.Windows.Forms.Padding(8, 3, 0, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(98, 29);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "&Salvar";
            this.btnSave.UseVisualStyleBackColor = false;
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblStatus.Location = new System.Drawing.Point(23, 666);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(974, 15);
            this.lblStatus.TabIndex = 3;
            //
            // errorProvider
            //
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1020, 700);
            this.Controls.Add(this.tlpRoot);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.MinimumSize = new System.Drawing.Size(900, 640);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cadastro de Produtos";
            this.tlpRoot.ResumeLayout(false);
            this.tlpRoot.PerformLayout();
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridProducts)).EndInit();
            this.pnlEditor.ResumeLayout(false);
            this.pnlEditor.PerformLayout();
            this.tlpEditor.ResumeLayout(false);
            this.tlpEditor.PerformLayout();
            this.tlpButtons.ResumeLayout(false);
            this.tlpButtons.PerformLayout();
            this.flpRecordActions.ResumeLayout(false);
            this.flpFormActions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpRoot;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Panel pnlGrid;
        private System.Windows.Forms.DataGridView gridProducts;
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStockQuantity;
        private System.Windows.Forms.Panel pnlEditor;
        private System.Windows.Forms.TableLayoutPanel tlpEditor;
        private System.Windows.Forms.Label lblEditorTitle;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.Label lblStockQuantity;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.ComboBox cboCategory;
        private System.Windows.Forms.TextBox txtPrice;
        private System.Windows.Forms.TextBox txtStockQuantity;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.TableLayoutPanel tlpButtons;
        private System.Windows.Forms.FlowLayoutPanel flpRecordActions;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.FlowLayoutPanel flpFormActions;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
