using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.BitmapFonts
{
	// ---- AngelCode BmFont XML serializer ----------------------
	// ---- By DeadlyDan @ deadlydan@gmail.com -------------------
	// ---- There's no license restrictions, use as you will. ----
	// ---- Credits to http://www.angelcode.com/ -----------------
	[XmlRoot ( "font" )]
    public class FontFile
	{
		[XmlElement ( "info" )]
		public FontInfo Info { get; set; }
		
		[XmlElement ( "common" )]
		public FontCommon Common { get; set; }

		[XmlArray ( "pages" )]
		[XmlArrayItem ( "page" )]
		public List<FontPage> Pages { get; set; }

		[XmlArray ( "chars" )]
		[XmlArrayItem ( "char" )]
		public List<FontChar> Chars { get; set; }
		
		[XmlArray ( "kernings" )]
		[XmlArrayItem ( "kerning" )]
		public List<FontKerning> Kernings { get; set; }
	}	
}
