using System;
namespace UPravda.Models
{
	interface INewsItem
	{
		string Annotation { get; set; }
		string Description { get; set; }
		string DescriptionImage { get; set; }
		uint Highlighted { get; set; }
		uint ID { get; set; }
		string Link { get; set; }
		string PubDate { get; set; }
		string Thumbnail { get; set; }
		string Title { get; set; }
	}
}
