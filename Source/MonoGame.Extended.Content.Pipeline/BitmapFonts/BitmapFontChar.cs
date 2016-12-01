using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.BitmapFonts
{
	// ---- AngelCode BmFont XML serializer ----------------------
	// ---- By DeadlyDan @ deadlydan@gmail.com -------------------
	// ---- There's no license restrictions, use as you will. ----
	// ---- Credits to http://www.angelcode.com/ -----------------	
	public class BitmapFontChar
	{
		[XmlAttribute ("id")]
		public int Id { get; set; }
		
		[XmlAttribute ("x")]
		public int X { get; set; }
		
		[XmlAttribute ("y")]
		public int Y { get; set; }
		
		[XmlAttribute ("width")]
		public int Width { get; set; }
		
		[XmlAttribute ("height")]
		public int Height { get; set; }
		
		[XmlAttribute ("xoffset")]
		public int XOffset { get; set; }
		
		[XmlAttribute ("yoffset")]
		public int YOffset { get; set; }
		
		[XmlAttribute ("xadvance")]
		public int XAdvance { get; set; }
		
		[XmlAttribute ("page")]
		public int Page { get; set; }
		
		[XmlAttribute ("chnl")]
		public int Channel { get; set; }
	}	
}
