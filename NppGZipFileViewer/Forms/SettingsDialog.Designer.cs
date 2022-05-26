
namespace NppGZipFileViewer.Forms
{
    partial class SettingsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsDialog));
            this.lst_Suffixes = new System.Windows.Forms.ListBox();
            this.chk_DecompressAll = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Add = new System.Windows.Forms.Button();
            this.txt_Suffix = new System.Windows.Forms.TextBox();
            this.btn_Delete = new System.Windows.Forms.Button();
            this.btn_OK = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.chk_OpenAsUTF8 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lst_Suffixes
            // 
            this.lst_Suffixes.FormattingEnabled = true;
            this.lst_Suffixes.ItemHeight = 20;
            this.lst_Suffixes.Location = new System.Drawing.Point(32, 107);
            this.lst_Suffixes.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lst_Suffixes.Name = "lst_Suffixes";
            this.lst_Suffixes.Size = new System.Drawing.Size(348, 304);
            this.lst_Suffixes.TabIndex = 0;
            // 
            // chk_DecompressAll
            // 
            this.chk_DecompressAll.AutoSize = true;
            this.chk_DecompressAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chk_DecompressAll.Location = new System.Drawing.Point(32, 14);
            this.chk_DecompressAll.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chk_DecompressAll.Name = "chk_DecompressAll";
            this.chk_DecompressAll.Size = new System.Drawing.Size(209, 24);
            this.chk_DecompressAll.TabIndex = 1;
            this.chk_DecompressAll.Text = "Try to decompress all files";
            this.chk_DecompressAll.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 82);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "GZip-suffixes";
            // 
            // btn_Add
            // 
            this.btn_Add.Location = new System.Drawing.Point(167, 419);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(102, 31);
            this.btn_Add.TabIndex = 3;
            this.btn_Add.Text = "Add suffix";
            this.btn_Add.UseVisualStyleBackColor = true;
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
            // 
            // txt_Suffix
            // 
            this.txt_Suffix.Location = new System.Drawing.Point(32, 421);
            this.txt_Suffix.Name = "txt_Suffix";
            this.txt_Suffix.Size = new System.Drawing.Size(129, 26);
            this.txt_Suffix.TabIndex = 4;
            // 
            // btn_Delete
            // 
            this.btn_Delete.Location = new System.Drawing.Point(275, 419);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(105, 31);
            this.btn_Delete.TabIndex = 5;
            this.btn_Delete.Text = "Delete";
            this.btn_Delete.UseVisualStyleBackColor = true;
            this.btn_Delete.Click += new System.EventHandler(this.btn_Delete_Click);
            // 
            // btn_OK
            // 
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Location = new System.Drawing.Point(32, 473);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(102, 31);
            this.btn_OK.TabIndex = 6;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Location = new System.Drawing.Point(278, 473);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(102, 31);
            this.btn_Cancel.TabIndex = 7;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // chk_OpenAsUTF8
            // 
            this.chk_OpenAsUTF8.AutoSize = true;
            this.chk_OpenAsUTF8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chk_OpenAsUTF8.Location = new System.Drawing.Point(32, 48);
            this.chk_OpenAsUTF8.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chk_OpenAsUTF8.Name = "chk_OpenAsUTF8";
            this.chk_OpenAsUTF8.Size = new System.Drawing.Size(179, 24);
            this.chk_OpenAsUTF8.TabIndex = 8;
            this.chk_OpenAsUTF8.Text = "Open ANSI as UTF-8";
            this.chk_OpenAsUTF8.UseVisualStyleBackColor = true;
            // 
            // SettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 536);
            this.Controls.Add(this.chk_OpenAsUTF8);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.btn_Delete);
            this.Controls.Add(this.txt_Suffix);
            this.Controls.Add(this.btn_Add);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chk_DecompressAll);
            this.Controls.Add(this.lst_Suffixes);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsDialog";
            this.ShowInTaskbar = false;
            this.Text = "SettingsDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lst_Suffixes;
        private System.Windows.Forms.CheckBox chk_DecompressAll;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_Add;
        private System.Windows.Forms.TextBox txt_Suffix;
        private System.Windows.Forms.Button btn_Delete;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.CheckBox chk_OpenAsUTF8;
    }
}