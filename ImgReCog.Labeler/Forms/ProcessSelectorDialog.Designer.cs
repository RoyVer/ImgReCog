namespace ImgReCog.Labeler.Forms
{
  partial class ProcessSelectorDialog
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;



    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      Grid = new DataGridView();
      BtnSelect = new Button();
      BtnCancel = new Button();
      ((System.ComponentModel.ISupportInitialize)Grid).BeginInit();
      SuspendLayout();
      // 
      // Grid
      // 
      Grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
      Grid.BackgroundColor = SystemColors.ButtonHighlight;
      Grid.CellBorderStyle = DataGridViewCellBorderStyle.Sunken;
      Grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      Grid.GridColor = SystemColors.InactiveCaptionText;
      Grid.Location = new Point(6, 5);
      Grid.MultiSelect = false;
      Grid.Name = "Grid";
      Grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Sunken;
      Grid.RowHeadersWidth = 51;
      Grid.RowTemplate.Height = 29;
      Grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      Grid.Size = new Size(782, 377);
      Grid.TabIndex = 0;
      Grid.SelectionChanged += Grid_SelectionChanged;
      // 
      // BtnSelect
      // 
      BtnSelect.Font = new Font("Segoe UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point);
      BtnSelect.Location = new Point(628, 388);
      BtnSelect.Name = "BtnSelect";
      BtnSelect.Size = new Size(160, 50);
      BtnSelect.TabIndex = 1;
      BtnSelect.Text = "&Select";
      BtnSelect.TextImageRelation = TextImageRelation.ImageAboveText;
      BtnSelect.UseVisualStyleBackColor = true;
      BtnSelect.Click += BtnSelect_Click;
      // 
      // BtnCancel
      // 
      BtnCancel.Font = new Font("Segoe UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point);
      BtnCancel.Location = new Point(6, 388);
      BtnCancel.Name = "BtnCancel";
      BtnCancel.Size = new Size(160, 50);
      BtnCancel.TabIndex = 2;
      BtnCancel.Text = "&Cancel";
      BtnCancel.TextImageRelation = TextImageRelation.ImageAboveText;
      BtnCancel.UseVisualStyleBackColor = true;
      BtnCancel.Click += BtnCancel_Click;
      // 
      // ProcessSelectorDialog
      // 
      AutoScaleDimensions = new SizeF(8F, 20F);
      AutoScaleMode = AutoScaleMode.Font;
      CancelButton = BtnCancel;
      ClientSize = new Size(800, 450);
      Controls.Add(BtnCancel);
      Controls.Add(BtnSelect);
      Controls.Add(Grid);
      FormBorderStyle = FormBorderStyle.FixedDialog;
      MaximizeBox = false;
      MinimizeBox = false;
      Name = "ProcessSelectorDialog";
      ShowIcon = false;
      StartPosition = FormStartPosition.CenterScreen;
      Text = "Process selector";
      TopMost = true;
      ((System.ComponentModel.ISupportInitialize)Grid).EndInit();
      ResumeLayout(false);
    }

    #endregion

    private DataGridView Grid;
    private Button BtnSelect;
    private Button BtnCancel;
  }
}