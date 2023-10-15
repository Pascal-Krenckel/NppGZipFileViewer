namespace NppGZipFileViewer.Forms;

partial class ZstdSettingsDialog
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
            this.btn_Delete = new System.Windows.Forms.Button();
            this.txt_Suffix = new System.Windows.Forms.TextBox();
            this.btn_Add = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lst_Suffixes = new System.Windows.Forms.ListBox();
            this.numCompressionLevel = new System.Windows.Forms.NumericUpDown();
            this.numBufferSize = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_OK = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnDefault = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numCompressionLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBufferSize)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Delete
            // 
            this.btn_Delete.Location = new System.Drawing.Point(287, 420);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(105, 31);
            this.btn_Delete.TabIndex = 10;
            this.btn_Delete.Text = "Delete";
            this.btn_Delete.UseVisualStyleBackColor = true;
            this.btn_Delete.Click += new System.EventHandler(this.btn_Delete_Click);
            // 
            // txt_Suffix
            // 
            this.txt_Suffix.Location = new System.Drawing.Point(13, 422);
            this.txt_Suffix.Name = "txt_Suffix";
            this.txt_Suffix.Size = new System.Drawing.Size(160, 26);
            this.txt_Suffix.TabIndex = 9;
            // 
            // btn_Add
            // 
            this.btn_Add.Location = new System.Drawing.Point(179, 420);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(102, 31);
            this.btn_Add.TabIndex = 8;
            this.btn_Add.Text = "Add suffix";
            this.btn_Add.UseVisualStyleBackColor = true;
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 83);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 20);
            this.label1.TabIndex = 7;
            this.label1.Text = "Zstd-suffixes";
            // 
            // lst_Suffixes
            // 
            this.lst_Suffixes.FormattingEnabled = true;
            this.lst_Suffixes.ItemHeight = 20;
            this.lst_Suffixes.Location = new System.Drawing.Point(13, 108);
            this.lst_Suffixes.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lst_Suffixes.Name = "lst_Suffixes";
            this.lst_Suffixes.Size = new System.Drawing.Size(379, 304);
            this.lst_Suffixes.TabIndex = 6;
            // 
            // numCompressionLevel
            // 
            this.numCompressionLevel.Location = new System.Drawing.Point(273, 12);
            this.numCompressionLevel.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numCompressionLevel.Name = "numCompressionLevel";
            this.numCompressionLevel.Size = new System.Drawing.Size(120, 26);
            this.numCompressionLevel.TabIndex = 11;
            // 
            // numBufferSize
            // 
            this.numBufferSize.Location = new System.Drawing.Point(273, 52);
            this.numBufferSize.Maximum = new decimal(new int[] {
            0,
            256,
            0,
            0});
            this.numBufferSize.Name = "numBufferSize";
            this.numBufferSize.Size = new System.Drawing.Size(120, 26);
            this.numBufferSize.TabIndex = 12;
            this.numBufferSize.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(141, 20);
            this.label2.TabIndex = 13;
            this.label2.Text = "Compression level:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 20);
            this.label3.TabIndex = 14;
            this.label3.Text = "Buffer size:";
            // 
            // btn_OK
            // 
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Location = new System.Drawing.Point(13, 523);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(102, 31);
            this.btn_OK.TabIndex = 15;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Location = new System.Drawing.Point(290, 523);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(102, 31);
            this.btn_Cancel.TabIndex = 16;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // btnDefault
            // 
            this.btnDefault.Location = new System.Drawing.Point(121, 523);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(102, 31);
            this.btnDefault.TabIndex = 17;
            this.btnDefault.Text = "Default";
            this.btnDefault.UseVisualStyleBackColor = true;
            this.btnDefault.Click += new System.EventHandler(this.btnDefault_Click);
            // 
            // ZstdSettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 566);
            this.Controls.Add(this.btnDefault);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numBufferSize);
            this.Controls.Add(this.numCompressionLevel);
            this.Controls.Add(this.btn_Delete);
            this.Controls.Add(this.txt_Suffix);
            this.Controls.Add(this.btn_Add);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lst_Suffixes);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ZstdSettingsDialog";
            this.Text = "ZstSettings";
            this.Load += new System.EventHandler(this.ZstSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numCompressionLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBufferSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btn_Delete;
    private System.Windows.Forms.TextBox txt_Suffix;
    private System.Windows.Forms.Button btn_Add;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ListBox lst_Suffixes;
    private System.Windows.Forms.NumericUpDown numCompressionLevel;
    private System.Windows.Forms.NumericUpDown numBufferSize;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button btn_OK;
    private System.Windows.Forms.Button btn_Cancel;
    private System.Windows.Forms.ToolTip toolTip1;
    private System.Windows.Forms.Button btnDefault;
}