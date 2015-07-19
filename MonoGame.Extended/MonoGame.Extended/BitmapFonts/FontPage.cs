using System;
using System.Xml.Serialization;

namespace MonoGame.Extended.BitmapFonts
{
	// ---- AngelCode BmFont XML serializer ----------------------
	// ---- By DeadlyDan @ deadlydan@gmail.com -------------------
	// ---- There's no license restrictions, use as you will. ----
	// ---- Credits to http://www.angelcode.com/ -----------------	
    public class FontPage
	{
		[XmlAttribute ( "id" )]
        public Int32 ID { get; set; }
		
		[XmlAttribute ( "file" )]
		public String File { get; set; }
	}	
}
