using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.NuclexGui.Visuals.Flat
{
    /// <summary>
    ///     Locates the opening between characters in a string that is nearest
    ///     to a user-defined location
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This is a class rather than a static class to prevent garbage production
    ///         which would then have to be cleaned up again by the garbage collector.
    ///         If you create an instance of it and keep reusing it, garbage and allocation
    ///         will amortize.
    ///     </para>
    ///     <para>
    ///         The method used to calculate the openings seems to be not terribly accurate.
    ///         As of XNA 3.1, SpriteFonts don't do kerning, so the only thing left would be
    ///         a variable space appended to the end of characters. This could be compensated
    ///         for by always appending character with a known length for which no kerning is
    ///         possible, for example, the pipe sign (|).
    ///     </para>
    /// </remarks>
    internal class OpeningLocator
    {
        /// <summary>Used by GetClosestOpening() to avoid garbage production</summary>
        private readonly StringBuilder _textBuilder;

        /// <summary>Initializes a new text opening locator</summary>
        public OpeningLocator()
        {
            _textBuilder = new StringBuilder(64);
        }

        /// <summary>
        ///     Locates the opening between two letters that is closest to
        ///     the specified position
        /// </summary>
        /// <param name="font">Font that opening search will use</param>
        /// <param name="text">Text that will be searched for the opening</param>
        /// <param name="x">X coordinate closest to which an opening will be found</param>
        /// <returns>The opening closest to the specified X coordinate</returns>
        public int FindClosestOpening(SpriteFont font, string text, float x)
        {
            // Measure the size of the whole string
            _textBuilder.Remove(0, _textBuilder.Length);
            _textBuilder.Append(text);
            var textSize = font.MeasureString(_textBuilder);

            // Run a binary search until to close in on the nearest opening
            var left = 0;
            var leftX = 0.0f;
            var right = text.Length;
            var rightX = textSize.X;
            for (;;)
            {
                // Is the provided coordinate outside of our search range?
                // -> Opening is to the far left or to the far right.
                if (x <= leftX)
                    return left;
                if (x >= rightX)
                    return right;

                // Do we have only one character left to check?
                // -> Opening is either to its left or to its right.
                if (right - left <= 1)
                    if (x - leftX <= rightX - x)
                        return left;
                    else return right;

                // The position of the opening is still not absolutely clear, cut the string
                // in the middle of the search range so we can close in further.
                var middle = (right + left)/2;
                _textBuilder.Remove(middle, right - middle);
                textSize = font.MeasureString(_textBuilder);

                // Depending on whether the searched-for position was on the left or right
                // of our cut, adjust appropriate side and prepare for another run
                if (x < textSize.X)
                {
                    right = middle;
                    rightX = textSize.X;
                }
                else
                {
                    _textBuilder.Append(text, middle, right - middle);
                    left = middle;
                    leftX = textSize.X;
                }
            }
        }
    }
}