using System;
using System.Xml.Serialization;

namespace MonoGame.Extended.BitmapFonts
{
	// ---- AngelCode BmFont XML serializer ----------------------
	// ---- By DeadlyDan @ deadlydan@gmail.com -------------------
	// ---- There's no license restrictions, use as you will. ----
	// ---- Credits to http://www.angelcode.com/ -----------------	
    public class BitmapFontInfo
	{
		[XmlAttribute ( "face" )]
		public String Face { get; set; }

		[XmlAttribute ( "size" )]
		public Int32 Size { get; set; }
		
		[XmlAttribute ( "bold" )]
		public Int32 Bold { get; set; }
		
		[XmlAttribute ( "italic" )]
		public Int32 Italic { get; set; }
		
		[XmlAttribute ( "charset" )]
		public String CharSet { get; set; }
		
		[XmlAttribute ( "unicode" )]
		public Int32 Unicode { get; set; }
		
		[XmlAttribute ( "stretchH" )]
		public Int32 StretchHeight { get; set; }
		
		[XmlAttribute ( "smooth" )]
		public Int32 Smooth { get; set; }

		[XmlAttribute ( "aa" )]
		public Int32 SuperSampling { get; set; }

		[XmlAttribute ( "padding" )]
		public String Padding { get; set; }

		[XmlAttribute ( "spacing" )]
		public String Spacing { get; set; }
		
		[XmlAttribute ( "outline" )]
		public Int32 OutLine { get; set; }
	}	
}
