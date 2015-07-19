using System;
using System.Xml.Serialization;

namespace MonoGame.Extended.BitmapFonts
{
	// ---- AngelCode BmFont XML serializer ----------------------
	// ---- By DeadlyDan @ deadlydan@gmail.com -------------------
	// ---- There's no license restrictions, use as you will. ----
	// ---- Credits to http://www.angelcode.com/ -----------------		
    public class FontCommon
	{
        [XmlAttribute("lineHeight")]
		public Int32 LineHeight { get; set; }

        [XmlAttribute("base")]
		public Int32 Base { get; set; }

        [XmlAttribute("scaleW")]
		public Int32 ScaleW { get; set; }

        [XmlAttribute("scaleH")]
		public Int32 ScaleH { get; set; }

        [XmlAttribute("pages")]
		public Int32 Pages { get; set; }

        [XmlAttribute("packed")]
		public Int32 Packed { get; set; }

        [XmlAttribute("alphaChnl")]
		public Int32 AlphaChannel { get; set; }

        [XmlAttribute("redChnl")]
		public Int32 RedChannel { get; set; }

        [XmlAttribute("greenChnl")]
		public Int32 GreenChannel { get; set; }

        [XmlAttribute("blueChnl")]
		public Int32 BlueChannel { get; set; }
	}	
}
