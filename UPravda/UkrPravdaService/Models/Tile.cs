using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace UkrPravdaService.Models
{
	//	<tile>
	//		<visual version="2">
	//			<binding template="TileSquare150x150Text04" fallback="TileSquareText04">
	//				<text id="1">Hello world!</text>
	//			</binding>  
	//		</visual>
	//	</tile>
	[XmlType(TypeName="tile")]
	public class Tile
	{
		[XmlElement(ElementName = "visual")]
		public Visual Visual { get; set; }
	}
}