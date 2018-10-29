
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MonoGame.Extended.Gui.Controls
{
    public class Label : ContentControl
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; Content = _text; IsLayoutRequired = true; }
        }
        public Label()
        {
            BackgroundColor = Color.Transparent;
        }

        public Label(string text = null)
        {
            Content = text ?? string.Empty;
            Text = text ?? string.Empty;
            BackgroundColor = Color.Transparent;
        }

        /// <summary>
        /// If true, then the font width will be fit to be within the bounds of the parent
        /// </summary>
        public bool AutoFit { get; set; }

        public Size RefreshSize(IGuiContext context)
        {
            Size = new Size(0, 0);
            return CalculateActualSize(context);
        }

        public override Size GetContentSize(IGuiContext context)
        {

            var fixedSize = Size;
            var desiredSize = Size2.Empty;

            var font = Font ?? context.DefaultFont;

            if (font != null && Text != null)
            {
                var textSize = font.MeasureString(Text, TextScale);
                desiredSize.Width = textSize.Width;
                desiredSize.Height = textSize.Height ;
            }

            var availableWidth = ClippingRectangle.Width == 0 ? Width- Margin.Width : ClippingRectangle.Width;
            if (AutoFit && desiredSize.Width > availableWidth && availableWidth > 0)
            {
                VerticalAlignment = VerticalAlignment.Top;
                VerticalTextAlignment = VerticalAlignment.Top;

                var lastSplitWidth = 0f;
                string mLineText = "";
                var glyphs = font.GetGlyphs(Text);
                var words = new List<Tuple<string, float>>();
                var right = 0f;

                foreach (var glyph in glyphs)
                {
                    mLineText += char.ConvertFromUtf32(glyph.Character);
                    if (glyph.FontRegion != null)
                    {
                        right = glyph.Position.X + glyph.FontRegion.Width - lastSplitWidth;

                        if (glyph.Character == ' ' || glyph.Character == '\n')
                        {
                            words.Add(Tuple.Create<string, float>(mLineText, right));
                            mLineText = "";
                            lastSplitWidth += right;
                        }
                    }
                }

                words.Add(Tuple.Create<string, float>(mLineText, right));

                var currentLineWidth = 0f;
                mLineText = "";
                var finalText = "";
                lastSplitWidth = 0f;
                foreach (var word in words)
                {
                    if (word.Item2 > availableWidth) //the single word is bigger than our available area. 
                    {
                        //Split the word up into the available width
                        glyphs = font.GetGlyphs(word.Item1);
                        foreach (var glyph in glyphs)
                        {
                            if (glyph.FontRegion != null)
                            {
                                right = glyph.Position.X + glyph.FontRegion.Width - lastSplitWidth;

                                if (right > availableWidth)
                                {
                                    finalText += mLineText + "\n";
                                    mLineText = "";
                                    lastSplitWidth += right;
                                }

                                mLineText += char.ConvertFromUtf32(glyph.Character);
                            }

                        }
                        finalText += mLineText + " ";
                        currentLineWidth += word.Item2 - lastSplitWidth;
                    }
                    else
                    {
                        var testWidth = currentLineWidth + word.Item2;
                        if (testWidth > availableWidth)
                        {
                            finalText += "\n";
                            currentLineWidth = 0;
                        }

                        finalText += word.Item1;
                        currentLineWidth += word.Item2;
                    }

                }


                Content = finalText;
                var lineReturns = Regex.Matches(finalText, "\n").Count + 1;
                var origTextHeight = font.MeasureString(Text, TextScale).Height;
                var origLineCount = Regex.Matches(Text, "\n").Count+1;
                var lineHeight = origTextHeight / origLineCount; 
                desiredSize = new Size2(availableWidth, lineReturns * lineHeight);
            }

            float imageWidth = Image != null ? ImageSize == Size2.Empty ? Image.Size.Width : ImageSize.Width : 0;
            float imageHeight = Image != null ? ImageSize == Size2.Empty ? Image.Size.Height : ImageSize.Height : 0;
            desiredSize.Width += imageWidth;
            desiredSize.Height = Math.Max(desiredSize.Height, imageHeight);

            return new Size((int)desiredSize.Width, (int)desiredSize.Height);

        }

        public override void InvalidateMeasure()
        {
            if (AutoFit && this.ActualSize.Height > ClippingRectangle.Height)
                ActualSize = new Size(ActualSize.Width, ClippingRectangle.Height);
            if (AutoFit && this.Size.Height > ClippingRectangle.Height)
                ActualSize = new Size(Size.Width, Size.Height);

            base.InvalidateMeasure();
        }

        public override void DrawText(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            var text = Content?.ToString();
            var textInfo = GetTextInfo(context, text, ContentRectangle, HorizontalTextAlignment, VerticalTextAlignment);
            if (!string.IsNullOrWhiteSpace(textInfo.Text))
                renderer.DrawText(textInfo.Font, textInfo.Text, textInfo.Position + TextOffset, textInfo.Color, TextScale, ClippingRectangle);
        }
    }
}