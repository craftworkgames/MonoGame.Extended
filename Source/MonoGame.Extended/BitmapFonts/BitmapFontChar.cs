using System;
using System.Xml.Serialization;

namespace MonoGame.Extended.BitmapFonts
{
	// ---- AngelCode BmFont XML serializer ----------------------
	// ---- By DeadlyDan @ deadlydan@gmail.com -------------------
	// ---- There's no license restrictions, use as you will. ----
	// ---- Credits to http://www.angelcode.com/ -----------------	
	public class BitmapFontChar
	{
		[XmlAttribute ("id")]
		public Int32 ID { get; set; }
		
		[XmlAttribute ("x")]
		public Int32 X { get; set; }
		
		[XmlAttribute ("y")]
		public Int32 Y { get; set; }
		
		[XmlAttribute ("width")]
		public Int32 Width { get; set; }
		
		[XmlAttribute ("height")]
		public Int32 Height { get; set; }
		
		[XmlAttribute ("xoffset")]
		public Int32 XOffset { get; set; }
		
		[XmlAttribute ("yoffset")]
		public Int32 YOffset { get; set; }
		
		[XmlAttribute ("xadvance")]
		public Int32 XAdvance { get; set; }
		
		[XmlAttribute ("page")]
		public Int32 Page { get; set; }
		
		[XmlAttribute ("chnl")]
		public Int32 Channel { get; set; }
	}	
}
