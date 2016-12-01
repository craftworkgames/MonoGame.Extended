using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.BitmapFonts
{
	// ---- AngelCode BmFont XML serializer ----------------------
	// ---- By DeadlyDan @ deadlydan@gmail.com -------------------
	// ---- There's no license restrictions, use as you will. ----
	// ---- Credits to http://www.angelcode.com/ -----------------	
    public class BitmapFontKerning
	{
		[XmlAttribute ( "first" )]
		public int First { get; set; }
		
		[XmlAttribute ( "second" )]
		public int Second { get; set; }
		
		[XmlAttribute ( "amount" )]
		public int Amount { get; set; }
	}	
}
