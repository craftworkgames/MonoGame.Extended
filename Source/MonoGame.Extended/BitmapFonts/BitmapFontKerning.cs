using System;
using System.Xml.Serialization;

namespace MonoGame.Extended.BitmapFonts
{
	// ---- AngelCode BmFont XML serializer ----------------------
	// ---- By DeadlyDan @ deadlydan@gmail.com -------------------
	// ---- There's no license restrictions, use as you will. ----
	// ---- Credits to http://www.angelcode.com/ -----------------	
    public class BitmapFontKerning
	{
		[XmlAttribute ( "first" )]
		public Int32 First { get; set; }
		
		[XmlAttribute ( "second" )]
		public Int32 Second { get; set; }
		
		[XmlAttribute ( "amount" )]
		public Int32 Amount { get; set; }
	}	
}
