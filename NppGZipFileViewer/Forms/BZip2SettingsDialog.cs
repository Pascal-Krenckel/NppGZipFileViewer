using NppGZipFileViewer.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NppGZipFileViewer.Forms;

public partial class BZip2SettingsDialog : Form
{
    public BZip2SettingsDialog()
    {
        InitializeComponent();
    }

    private void BZip2Settings_Load(object sender, EventArgs e)
    {
        toolTip1.SetToolTip(numCompressionLevel, "Compression level from 1 (lowest) to 9 (highest)");
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

    public BZip2Settings BZip2Settings
    {
        get => new BZip2Settings()
        { CompressionLevel = (int)numCompressionLevel.Value, Extensions = lst_Suffixes.Items.Cast<string>().ToList() };
        set
        {
            numCompressionLevel.Value = value.CompressionLevel;
            lst_Suffixes.Items.Clear();
            lst_Suffixes.Items.AddRange(value.Extensions.ToArray());
        }
    }

    private void btnDefault_Click(object sender, EventArgs e)
    {
        BZip2Settings = Preferences.Default.BZip2Settings;
    }
}
