using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace UkrPravdaService.Models
{
	[XmlTypeAttribute(AnonymousType = true)]
	[XmlRoot(ElementName = "item")]
	public class NewsItem
	{
		#region Fields
		private uint idField;
		private string titleField;
		private string annotationField;
		private string descriptionField;
		private string pubDateField;
		private string thumbnailField;
		private string linkField;
		private uint highlightedField;
		private string descriptionImageField;
		private string authorField;
		#endregion

		[XmlElement(ElementName = "id")]
		public uint ID
		{
			get
			{
				return this.idField;
			}
			set
			{
				this.idField = value;
			}
		}

		[XmlElement(ElementName = "title")]
		public string Title
		{
			get
			{
				return this.titleField;
			}
			set
			{
				this.titleField = value;
			}
		}

		[XmlElement(ElementName = "annotation")]
		public string Annotation
		{
			get
			{
				return this.annotationField;
			}
			set
			{
				this.annotationField = value;
			}
		}

		[XmlElement(ElementName = "description")]
		public string Description
		{
			get
			{
				return this.descriptionField;
			}
			set
			{
				this.descriptionField = value;
			}
		}

		[XmlElement(ElementName = "description-image")]
		public string DescriptionImage
		{
			get
			{
				return this.descriptionImageField;
			}
			set
			{
				this.descriptionImageField = value;
			}
		}

		[XmlElement(ElementName = "pubDate")]
		public string PubDate
		{
			get
			{
				return this.pubDateField;
			}
			set
			{
				this.pubDateField = value;
			}
		}

		[XmlElement(ElementName = "thumbnail")]
		public string Thumbnail
		{
			get
			{
				return this.thumbnailField;
			}
			set
			{
				this.thumbnailField = value;
			}
		}

		[XmlElement(ElementName = "link")]
		public string Link
		{
			get
			{
				return this.linkField;
			}
			set
			{
				this.linkField = value;
			}
		}

		[XmlElement(ElementName = "highlighted")]
		public uint Highlighted
		{
			get
			{
				return this.highlightedField;
			}
			set
			{
				this.highlightedField = value;
			}
		}

		[XmlElement(ElementName = "author")]
		public string Author
		{
			get
			{
				return this.authorField;
			}
			set
			{
				this.authorField = value;
			}
		}

		public int Size { get; set; }
	}
}
