using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace UkrPravdaService.Models
{
	public class Visual
	{
		[XmlElement(ElementName = "version")]
		public string Version { get; set; }
	}
}
