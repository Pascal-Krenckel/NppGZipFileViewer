namespace NppGZipFileViewer.Forms;

partial class BZip2SettingsDialog
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
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_OK = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.numCompressionLevel = new System.Windows.Forms.NumericUpDown();
            this.btn_Delete = new System.Windows.Forms.Button();
            this.txt_Suffix = new System.Windows.Forms.TextBox();
            this.btn_Add = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lst_Suffixes = new System.Windows.Forms.ListBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnDefault = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numCompressionLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Location = new System.Drawing.Point(298, 490);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(102, 31);
            this.btn_Cancel.TabIndex = 27;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // btn_OK
            // 
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Location = new System.Drawing.Point(21, 490);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(102, 31);
            this.btn_OK.TabIndex = 26;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(141, 20);
            this.label2.TabIndex = 24;
            this.label2.Text = "Compression level:";
            // 
            // numCompressionLevel
            // 
            this.numCompressionLevel.Location = new System.Drawing.Point(276, 16);
            this.numCompressionLevel.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.numCompressionLevel.Name = "numCompressionLevel";
            this.numCompressionLevel.Size = new System.Drawing.Size(120, 26);
            this.numCompressionLevel.TabIndex = 22;
            // 
            // btn_Delete
            // 
            this.btn_Delete.Location = new System.Drawing.Point(291, 387);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(105, 31);
            this.btn_Delete.TabIndex = 21;
            this.btn_Delete.Text = "Delete";
            this.btn_Delete.UseVisualStyleBackColor = true;
            this.btn_Delete.Click += new System.EventHandler(this.btn_Delete_Click);
            // 
            // txt_Suffix
            // 
            this.txt_Suffix.Location = new System.Drawing.Point(17, 389);
            this.txt_Suffix.Name = "txt_Suffix";
            this.txt_Suffix.Size = new System.Drawing.Size(160, 26);
            this.txt_Suffix.TabIndex = 20;
            // 
            // btn_Add
            // 
            this.btn_Add.Location = new System.Drawing.Point(183, 387);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(102, 31);
            this.btn_Add.TabIndex = 19;
            this.btn_Add.Text = "Add suffix";
            this.btn_Add.UseVisualStyleBackColor = true;
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 50);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 20);
            this.label1.TabIndex = 18;
            this.label1.Text = "BZip2-suffixes";
            // 
            // lst_Suffixes
            // 
            this.lst_Suffixes.FormattingEnabled = true;
            this.lst_Suffixes.ItemHeight = 20;
            this.lst_Suffixes.Location = new System.Drawing.Point(17, 75);
            this.lst_Suffixes.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lst_Suffixes.Name = "lst_Suffixes";
            this.lst_Suffixes.Size = new System.Drawing.Size(379, 304);
            this.lst_Suffixes.TabIndex = 17;
            // 
            // btnDefault
            // 
            this.btnDefault.Location = new System.Drawing.Point(129, 490);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(102, 31);
            this.btnDefault.TabIndex = 28;
            this.btnDefault.Text = "Default";
            this.btnDefault.UseVisualStyleBackColor = true;
            this.btnDefault.Click += new System.EventHandler(this.btnDefault_Click);
            // 
            // BZip2SettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 533);
            this.Controls.Add(this.btnDefault);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numCompressionLevel);
            this.Controls.Add(this.btn_Delete);
            this.Controls.Add(this.txt_Suffix);
            this.Controls.Add(this.btn_Add);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lst_Suffixes);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "BZip2SettingsDialog";
            this.Text = "BZip2Settings";
            this.Load += new System.EventHandler(this.BZip2Settings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numCompressionLevel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btn_Cancel;
    private System.Windows.Forms.Button btn_OK;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.NumericUpDown numCompressionLevel;
    private System.Windows.Forms.Button btn_Delete;
    private System.Windows.Forms.TextBox txt_Suffix;
    private System.Windows.Forms.Button btn_Add;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ListBox lst_Suffixes;
    private System.Windows.Forms.ToolTip toolTip1;
    private System.Windows.Forms.Button btnDefault;
}