namespace Samples.Extended
{
    partial class MainForm
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
            this.BitmapFontsButton = new System.Windows.Forms.Button();
            this.SpritesButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BitmapFontsButton
            // 
            this.BitmapFontsButton.Location = new System.Drawing.Point(12, 12);
            this.BitmapFontsButton.Name = "BitmapFontsButton";
            this.BitmapFontsButton.Size = new System.Drawing.Size(134, 58);
            this.BitmapFontsButton.TabIndex = 0;
            this.BitmapFontsButton.Text = "Bitmap Fonts";
            this.BitmapFontsButton.UseVisualStyleBackColor = true;
            this.BitmapFontsButton.Click += new System.EventHandler(this.BitmapFontsButton_Click);
            // 
            // SpritesButton
            // 
            this.SpritesButton.Location = new System.Drawing.Point(152, 12);
            this.SpritesButton.Name = "SpritesButton";
            this.SpritesButton.Size = new System.Drawing.Size(134, 58);
            this.SpritesButton.TabIndex = 1;
            this.SpritesButton.Text = "Sprites";
            this.SpritesButton.UseVisualStyleBackColor = true;
            this.SpritesButton.Click += new System.EventHandler(this.SpritesButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(568, 304);
            this.Controls.Add(this.SpritesButton);
            this.Controls.Add(this.BitmapFontsButton);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MonoGame.Extended Samples";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BitmapFontsButton;
        private System.Windows.Forms.Button SpritesButton;
    }
}