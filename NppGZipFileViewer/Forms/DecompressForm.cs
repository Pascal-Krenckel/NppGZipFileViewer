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
public partial class DecompressForm : Form
{
    public DecompressForm()
    {
        InitializeComponent();
    }

    private void CompressForm_Load(object sender, EventArgs e)
    {
        lstCompressors.DataSource = Preferences.Default.EnumerateCompressions().ToArray();
        lstCompressors.DisplayMember = nameof(Settings.CompressionSettings.CompressionAlgorithm);
    }

    private void lstCompressors_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private void button1_Click(object sender, EventArgs e)
    {

    }

    public Settings.CompressionSettings CompressionSettings
    { get => lstCompressors.SelectedItem as Settings.CompressionSettings; }
}
