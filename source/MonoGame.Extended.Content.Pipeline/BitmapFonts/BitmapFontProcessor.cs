using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Content.BitmapFonts;

namespace MonoGame.Extended.Content.Pipeline.BitmapFonts
{
    [ContentProcessor(DisplayName = "BMFont Processor - MonoGame.Extended")]
    public class BitmapFontProcessor : ContentProcessor<ContentImporterResult<BitmapFontFileContent>, BitmapFontProcessorResult>
    {
        public override BitmapFontProcessorResult Process(ContentImporterResult<BitmapFontFileContent> importerResult, ContentProcessorContext context)
        {
            try
            {
                BitmapFontFileContent bmfFile = importerResult.Data;

                ValidateKernings(bmfFile, context);

                var result = new BitmapFontProcessorResult(bmfFile);

                foreach (var page in bmfFile.Pages)
                {
                    context.AddDependency(Path.GetFileName(page));
                    result.TextureAssets.Add(Path.GetFileNameWithoutExtension(page));
                }

                return result;
            }
            catch (Exception ex)
            {
                context.Logger.LogMessage("Error {0}", ex);
                throw;
            }
        }

        private readonly record struct KerningPair(uint First, uint Second);
        private readonly record struct KerningEntry(short Amount, int Index);
        private readonly record struct DuplicateKerning(uint First, uint Second, short FirstAmount, short DuplicateAmount, int FirstIndex, int DuplicateIndex);

        private void ValidateKernings(BitmapFontFileContent bmfFile, ContentProcessorContext context)
        {
            if(bmfFile.Kernings.Count == 0)
            {
                return;
            }

            Dictionary<KerningPair, KerningEntry> seenPairs = [];
            List<DuplicateKerning> duplicates = [];

            for(int i = 0; i < bmfFile.Kernings.Count; i++)
            {
                BitmapFontFileContent.KerningPairsBlock kerning = bmfFile.Kernings[i];
                KerningPair pair = new KerningPair(kerning.First, kerning.Second);

                if(seenPairs.TryGetValue(pair, out KerningEntry existing))
                {
                    duplicates.Add(new DuplicateKerning(pair.First, pair.Second, existing.Amount, kerning.Amount, existing.Index, i));
                }
                else
                {
                    seenPairs[pair] = new KerningEntry(kerning.Amount, i);
                }
            }

            if(duplicates.Count > 0)
            {
                context.Logger.LogWarning(
                    string.Empty,
                    new ContentIdentity(bmfFile.Path),
                    $"""
                    BMFont file contains {duplicates.Count} duplicate kerning pair(s).
                    This may cause runtime errors.  Each character pair should only have one kerning entry.
                    Please regenerate or fix the font file
                    """
                );

                foreach(var duplicate in duplicates)
                {
                    char firstChar = duplicate.First >= 32  && duplicate.First < 127
                                     ? (char)duplicate.First
                                     : '?';

                    char secondChar = duplicate.Second >= 32 && duplicate.Second < 127
                                      ? (char)duplicate.Second
                                      : '?';

                    context.Logger.LogWarning(
                        string.Empty,
                        new ContentIdentity(bmfFile.Path),
                        $"""
                          Duplicate kerning:  Character {duplicate.First} ('{firstChar}') -> {duplicate.Second} ('{secondChar}')
                          First entry (index {duplicate.FirstIndex}): amount={duplicate.FirstAmount}
                          Duplicate etnry (index {duplicate.DuplicateIndex}): amount={duplicate.DuplicateIndex}")

                        """
                    );
                }
            }
        }
    }

}
