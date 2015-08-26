using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Samples.Extended.Samples;

namespace Samples.Extended
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public Game Sample { get; private set; }

        private void BitmapFontsButton_Click(object sender, EventArgs e)
        {
            Sample = new BitmapFontsSample();
            DialogResult = DialogResult.OK;
        }

        private void SpritesButton_Click(object sender, EventArgs e)
        {
            Sample = new SpritesSample();
            DialogResult = DialogResult.OK;
        }
    }
}
