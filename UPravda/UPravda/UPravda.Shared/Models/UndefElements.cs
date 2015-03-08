using System;
using System.Collections.Generic;
using System.Linq;

namespace UPravda.Models
{
	public class UndefElements 
	{
		public string ElementName { get; set; }
		public string ParentElementName { get; set; }
		public string InnerHtml { get; set; }
		public string OuterHtml { get; set; }
		public string StackTrace { get; set; }
		public string ExceptionName { get; set; }
	}
}