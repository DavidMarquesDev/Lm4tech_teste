namespace ProductChallenge.Desktop.Views
{
    partial class ExportColumnsDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tlpRoot = new System.Windows.Forms.TableLayoutPanel();
            this.lblHint = new System.Windows.Forms.Label();
            this.gridColumns = new System.Windows.Forms.DataGridView();
            this.colSelected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colHeader = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flpActions = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnSelectNone = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.flpDialog = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancelDialog = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.tlpRoot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridColumns)).BeginInit();
            this.flpActions.SuspendLayout();
            this.flpDialog.SuspendLayout();
            this.SuspendLayout();
            //
            // tlpRoot
            //
            this.tlpRoot.ColumnCount = 1;
            this.tlpRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRoot.Controls.Add(this.lblHint, 0, 0);
            this.tlpRoot.Controls.Add(this.gridColumns, 0, 1);
            this.tlpRoot.Controls.Add(this.flpActions, 0, 2);
            this.tlpRoot.Controls.Add(this.flpDialog, 0, 3);
            this.tlpRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpRoot.Location = new System.Drawing.Point(0, 0);
            this.tlpRoot.Name = "tlpRoot";
            this.tlpRoot.Padding = new System.Windows.Forms.Padding(16);
            this.tlpRoot.RowCount = 4;
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpRoot.Size = new System.Drawing.Size(520, 460);
            this.tlpRoot.TabIndex = 0;
            //
            // lblHint
            //
            this.lblHint.AutoSize = true;
            this.lblHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.lblHint.Location = new System.Drawing.Point(19, 16);
            this.lblHint.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(300, 15);
            this.lblHint.TabIndex = 0;
            this.lblHint.Text = "Marque as colunas e use as setas para definir a ordem.";
            //
            // gridColumns
            //
            this.gridColumns.AllowUserToAddRows = false;
            this.gridColumns.AllowUserToDeleteRows = false;
            this.gridColumns.AllowUserToResizeRows = false;
            this.gridColumns.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridColumns.BackgroundColor = System.Drawing.Color.White;
            this.gridColumns.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gridColumns.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.gridColumns.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(250)))), ((int)(((byte)(251)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.gridColumns.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridColumns.ColumnHeadersHeight = 34;
            this.gridColumns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridColumns.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSelected,
            this.colHeader});
            this.gridColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridColumns.EnableHeadersVisualStyles = false;
            this.gridColumns.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(241)))), ((int)(((byte)(244)))));
            this.gridColumns.Location = new System.Drawing.Point(19, 44);
            this.gridColumns.MultiSelect = false;
            this.gridColumns.Name = "gridColumns";
            this.gridColumns.RowHeadersVisible = false;
            this.gridColumns.RowTemplate.Height = 30;
            this.gridColumns.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridColumns.Size = new System.Drawing.Size(422, 290);
            this.gridColumns.TabIndex = 1;
            //
            // colSelected
            //
            this.colSelected.DataPropertyName = "Selected";
            this.colSelected.FillWeight = 18F;
            this.colSelected.HeaderText = "Exportar";
            this.colSelected.Name = "colSelected";
            //
            // colHeader
            //
            this.colHeader.DataPropertyName = "Header";
            this.colHeader.FillWeight = 82F;
            this.colHeader.HeaderText = "Coluna";
            this.colHeader.Name = "colHeader";
            this.colHeader.ReadOnly = true;
            //
            // flpActions
            //
            this.flpActions.AutoSize = true;
            this.flpActions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpActions.Controls.Add(this.btnSelectAll);
            this.flpActions.Controls.Add(this.btnSelectNone);
            this.flpActions.Controls.Add(this.btnMoveUp);
            this.flpActions.Controls.Add(this.btnMoveDown);
            this.flpActions.Location = new System.Drawing.Point(16, 340);
            this.flpActions.Margin = new System.Windows.Forms.Padding(0, 6, 0, 6);
            this.flpActions.Name = "flpActions";
            this.flpActions.Size = new System.Drawing.Size(404, 33);
            this.flpActions.TabIndex = 2;
            this.flpActions.WrapContents = false;
            //
            // btnSelectAll
            //
            this.btnSelectAll.BackColor = System.Drawing.Color.White;
            this.btnSelectAll.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(213)))), ((int)(((byte)(219)))));
            this.btnSelectAll.AutoSize = true;
            this.btnSelectAll.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSelectAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectAll.Location = new System.Drawing.Point(3, 3);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.MinimumSize = new System.Drawing.Size(96, 27);
            this.btnSelectAll.TabIndex = 0;
            this.btnSelectAll.Text = "Marcar todas";
            this.btnSelectAll.UseVisualStyleBackColor = false;
            //
            // btnSelectNone
            //
            this.btnSelectNone.BackColor = System.Drawing.Color.White;
            this.btnSelectNone.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(213)))), ((int)(((byte)(219)))));
            this.btnSelectNone.AutoSize = true;
            this.btnSelectNone.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSelectNone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectNone.Location = new System.Drawing.Point(105, 3);
            this.btnSelectNone.Name = "btnSelectNone";
            this.btnSelectNone.MinimumSize = new System.Drawing.Size(106, 27);
            this.btnSelectNone.TabIndex = 1;
            this.btnSelectNone.Text = "Desmarcar todas";
            this.btnSelectNone.UseVisualStyleBackColor = false;
            //
            // btnMoveUp
            //
            this.btnMoveUp.BackColor = System.Drawing.Color.White;
            this.btnMoveUp.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(213)))), ((int)(((byte)(219)))));
            this.btnMoveUp.AutoSize = true;
            this.btnMoveUp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnMoveUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveUp.Location = new System.Drawing.Point(217, 3);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.MinimumSize = new System.Drawing.Size(90, 27);
            this.btnMoveUp.TabIndex = 2;
            this.btnMoveUp.Text = "Subir";
            this.btnMoveUp.UseVisualStyleBackColor = false;
            //
            // btnMoveDown
            //
            this.btnMoveDown.BackColor = System.Drawing.Color.White;
            this.btnMoveDown.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(213)))), ((int)(((byte)(219)))));
            this.btnMoveDown.AutoSize = true;
            this.btnMoveDown.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnMoveDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveDown.Location = new System.Drawing.Point(313, 3);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.MinimumSize = new System.Drawing.Size(90, 27);
            this.btnMoveDown.TabIndex = 3;
            this.btnMoveDown.Text = "Descer";
            this.btnMoveDown.UseVisualStyleBackColor = false;
            //
            // flpDialog
            //
            this.flpDialog.AutoSize = true;
            this.flpDialog.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpDialog.Controls.Add(this.btnCancelDialog);
            this.flpDialog.Controls.Add(this.btnConfirm);
            this.flpDialog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpDialog.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpDialog.Location = new System.Drawing.Point(16, 385);
            this.flpDialog.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.flpDialog.Name = "flpDialog";
            this.flpDialog.Size = new System.Drawing.Size(428, 33);
            this.flpDialog.TabIndex = 3;
            this.flpDialog.WrapContents = false;
            //
            // btnCancelDialog
            //
            this.btnCancelDialog.BackColor = System.Drawing.Color.White;
            this.btnCancelDialog.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelDialog.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(213)))), ((int)(((byte)(219)))));
            this.btnCancelDialog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelDialog.Location = new System.Drawing.Point(327, 3);
            this.btnCancelDialog.Name = "btnCancelDialog";
            this.btnCancelDialog.Size = new System.Drawing.Size(98, 27);
            this.btnCancelDialog.TabIndex = 1;
            this.btnCancelDialog.Text = "Cancelar";
            this.btnCancelDialog.UseVisualStyleBackColor = false;
            //
            // btnConfirm
            //
            this.btnConfirm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.btnConfirm.FlatAppearance.BorderSize = 0;
            this.btnConfirm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirm.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.btnConfirm.ForeColor = System.Drawing.Color.White;
            this.btnConfirm.Location = new System.Drawing.Point(221, 3);
            this.btnConfirm.Margin = new System.Windows.Forms.Padding(3, 3, 8, 3);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(98, 27);
            this.btnConfirm.TabIndex = 0;
            this.btnConfirm.Text = "Exportar";
            this.btnConfirm.UseVisualStyleBackColor = false;
            //
            // ExportColumnsDialog
            //
            this.AcceptButton = this.btnConfirm;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancelDialog;
            this.ClientSize = new System.Drawing.Size(520, 460);
            this.Controls.Add(this.tlpRoot);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportColumnsDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Exportar produtos";
            this.tlpRoot.ResumeLayout(false);
            this.tlpRoot.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridColumns)).EndInit();
            this.flpActions.ResumeLayout(false);
            this.flpDialog.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpRoot;
        private System.Windows.Forms.Label lblHint;
        private System.Windows.Forms.DataGridView gridColumns;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSelected;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHeader;
        private System.Windows.Forms.FlowLayoutPanel flpActions;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnSelectNone;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.FlowLayoutPanel flpDialog;
        private System.Windows.Forms.Button btnCancelDialog;
        private System.Windows.Forms.Button btnConfirm;
    }
}
