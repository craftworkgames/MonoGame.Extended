using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace Samples.Extended
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public Game Sample { get; private set; }

        private void BitmapFontButton_Click(object sender, EventArgs e)
        {
            Sample = new BitmapFontSample();
            DialogResult = DialogResult.OK;
        }
    }
}
