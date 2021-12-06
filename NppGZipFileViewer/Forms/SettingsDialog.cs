using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NppGZipFileViewer.Forms
{
    public partial class SettingsDialog : Form
    {
        public SettingsDialog()
        {
            InitializeComponent();
            Icon = Icon.FromHandle(NppGZipFileViewer.Properties.Resources.gzip_filled64.GetHicon());
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            lst_Suffixes.Items.Add(txt_Suffix.Text);
            txt_Suffix.Text = "";
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            if (lst_Suffixes.SelectedIndex >= 0)
                lst_Suffixes.Items.RemoveAt(lst_Suffixes.SelectedIndex);
        }

        public Preferences Preferences
        {
            get
            {
                return new Preferences(chk_DecompressAll.Checked, lst_Suffixes.Items.Cast<string>());
            }
            set
            {
                chk_DecompressAll.Checked = value.DecompressAll;
                lst_Suffixes.Items.AddRange(value.Extensions.ToArray());
            }
        }
    }
}
