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
public partial class XZSettingsDialog : Form
{
    public XZSettingsDialog()
    {
        InitializeComponent();
        numThreads.Maximum = Environment.ProcessorCount;
    }

    private void XZSettings_Load(object sender, EventArgs e)
    {
        toolTip1.SetToolTip(numBufferSize, "Buffer size of the internal deflater, minimum 512");
        toolTip1.SetToolTip(numCompressionLevel, "The compression level from 0 (no compression) to 9 (highest)");
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


    public XZSettings XZSettings
    {
        get
        {
            return new XZSettings()
            {
                BufferSize = (int)numBufferSize.Value,
                CompressionLevel = (Joveler.Compression.XZ.LzmaCompLevel)(int)numCompressionLevel.Value,
                Extensions = lst_Suffixes.Items.Cast<string>().ToList(),
                Threads = (int)numThreads.Value,
                MultiThread = chkMultiThreading.Checked,
            };
        }

        set
        {
            numBufferSize.Value = value.BufferSize;
            numThreads.Value = value.Threads;
            chkMultiThreading.Checked = value.MultiThread;
            numCompressionLevel.Value = (int)value.CompressionLevel;
            lst_Suffixes.Items.Clear();
            lst_Suffixes.Items.AddRange(value.Extensions.ToArray());
        }
    }

    private void btnDefault_Click(object sender, EventArgs e)
    {
        this.XZSettings = Preferences.Default.XZSettings;
    }

    private void toolTip1_Popup(object sender, PopupEventArgs e)
    {

    }
}
