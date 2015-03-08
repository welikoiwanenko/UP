using Microsoft.WindowsAzure.MobileServices;
using HtmlAgilityPack;
using HtmlTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media.Imaging;

namespace HtmlTools
{
	[Windows.UI.Xaml.Markup.ContentProperty(Name = "RichTextContent")]
	public sealed class RichTextColumns : Panel
	{

		/// <summary>
		/// Identifies the <see cref="RichTextContent"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty RichTextContentProperty =
			DependencyProperty.Register("RichTextContent", typeof(RichTextBlock),
			typeof(RichTextColumns), new PropertyMetadata(null, ResetOverflowLayout));

		/// <summary>
		/// Identifies the <see cref="ColumnTemplate"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ColumnTemplateProperty =
			DependencyProperty.Register("ColumnTemplate", typeof(DataTemplate),
			typeof(RichTextColumns), new PropertyMetadata(null, ResetOverflowLayout));

		/// <summary>
		/// Identifies the <see cref="Source"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.Register("Source", typeof(string),
			typeof(RichTextColumns), new PropertyMetadata(String.Empty, SourceChanged));

		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register("Title", typeof(string),
			typeof(RichTextColumns), new PropertyMetadata(String.Empty, TitleChanged));

		public static readonly DependencyProperty HtmlContentProperty =
			DependencyProperty.Register("HtmlContent", typeof(string),
			typeof(RichTextColumns), new PropertyMetadata(String.Empty, HtmlContentChanged));


		
		static RichTextColumns()
        {
            InitializeClient();
        }

        public static MobileServiceClient MobileService;

        public static void InitializeClient()
        {
            MobileService = new MobileServiceClient(
				@"https://ukrpravda.azure-mobile.net/",
				@"ionBKlJgbLUlzIHsZDLllIhpAQKAQo55"
            );
        }



		/// <summary>
		/// Событие нажатия на картинку для воспроизведения видео
		/// </summary>
		public event EventHandler<VideoEventArgs> VideoPlayEvent;

		/// <summary>
		/// Lists overflow columns already created.  Must maintain a 1:1 relationship with
		/// instances in the <see cref="Panel.Children"/> collection following the initial
		/// RichTextBlock child.
		/// </summary>
		private List<RichTextBlockOverflow> _overflowColumns = null;
		private RichTextBlock _richContentBlock;

		/// <summary>
		/// Initializes a new instance of the <see cref="RichTextColumns"/> class.
		/// </summary>
		public RichTextColumns()
		{
			this.HorizontalAlignment = HorizontalAlignment.Left;
			_richContentBlock = new RichTextBlock() { Width = 480, Margin = new Thickness(0, 0, 0, 0), IsTextSelectionEnabled = true, TextAlignment = TextAlignment.Justify };

			//Выделение места под:
			// 0 - изображение новости
			// 1 - заглавие
			// 2 - текст
			for (int i = 0; i < 3; i++)
			{
				_richContentBlock.Blocks.Add(new Paragraph());
			}

			this.RichTextContent = _richContentBlock;
		}



		public string ImageSource
		{
			get { return (string)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ImageSourceProperty =
			DependencyProperty.Register("ImageSource", typeof(string), typeof(RichTextColumns), new PropertyMetadata(String.Empty, ImageSourceChanged));

		private static void ImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != null)
			{
				Paragraph imageParagraph = new Paragraph();
				BitmapImage bitmap = new BitmapImage(new Uri(e.NewValue.ToString()));
				Image image = new Image() { Source = bitmap };
				InlineUIContainer container = new InlineUIContainer() { Child = image };
				imageParagraph.Inlines.Add(container);
				((RichTextColumns)d)._richContentBlock.Blocks[0] = imageParagraph;
			}
		}






		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		private static void TitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Paragraph titleParagraph = new Paragraph() { TextAlignment = TextAlignment.Left };
			Bold bold = new Bold() { FontWeight = Windows.UI.Text.FontWeights.SemiBold };
			bold.Inlines.Add(new Run() { Text = e.NewValue.ToString().TrimStart(new char[] { ' ' }), FontSize = 36 });
			titleParagraph.Inlines.Add(bold);
			titleParagraph.Inlines.Add(new Run() { Text = "\n\n" });
			((RichTextColumns)d)._richContentBlock.Blocks[1] = titleParagraph;
		}



		public string HtmlContent
		{
			get { return (string)GetValue(HtmlContentProperty); }
			set { SetValue(HtmlContentProperty, value); }
		}

		async private static void HtmlContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			//Удаление текста новости
			for (int i = 2; i < ((RichTextColumns)d)._richContentBlock.Blocks.Count - 1; i++)
			{
				((RichTextColumns)d)._richContentBlock.Blocks.RemoveAt(i);
			}

			HtmlAgilityPack.HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(e.NewValue.ToString().Replace("&nbsp;", " ").Replace("&ndash;", "—").Replace("&hellip;", "...").Replace("&rsquo;", "`").TrimStart(new char[] { ' ' }));
			int index = 2;
			var ads = await BuildBlocks(d, doc.DocumentNode);
			foreach (var item in await BuildBlocks(d, doc.DocumentNode))
			{
				((RichTextColumns)d)._richContentBlock.Blocks.Insert(index, item);
				index++;
			}
		}





















		/// <summary>
		/// Gets or sets the initial rich text content to be used as the first column.
		/// </summary>
		public RichTextBlock RichTextContent
		{
			get { return (RichTextBlock)GetValue(RichTextContentProperty); }
			set { SetValue(RichTextContentProperty, value); }
		}

		/// <summary>
		/// 
		/// </summary>
		public string Source
		{
			get { return (string)GetValue(SourceProperty); ; }
			set { SetValue(SourceProperty, value); }
		}


		/// <summary>
		/// Gets or sets the template used to create additional
		/// <see cref="RichTextBlockOverflow"/> instances.
		/// </summary>
		public DataTemplate ColumnTemplate
		{
			get { return (DataTemplate)GetValue(ColumnTemplateProperty); }
			set { SetValue(ColumnTemplateProperty, value); }
		}

		/// <summary>
		/// Invoked when the content or overflow template is changed to recreate the column layout.
		/// </summary>
		/// <param name="d">Instance of <see cref="RichTextColumns"/> where the change
		/// occurred.</param>
		/// <param name="e">Event data describing the specific change.</param>
		private static void ResetOverflowLayout(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var target = d as RichTextColumns;
			if (target != null)
			{
				target._overflowColumns = null;
				target.Children.Clear();
				target.InvalidateMeasure();
			}
		}

		async private static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			//var richTextBlock = new RichTextBlock() { Width = 480, Margin = new Thickness(0, 0, 0, 0), IsTextSelectionEnabled = true, TextAlignment = TextAlignment.Justify };
			//NewsItem news = (NewsItem)e.NewValue;
			//HtmlAgilityPack.HtmlDocument doc = new HtmlDocument();
			//doc.LoadHtml(news.Description.Replace("&nbsp;", " ").Replace("&ndash;", "—").Replace("&hellip;", "..."));

			//if (news.DescriptionImage != null)
			//{
			//    Paragraph imageParagraph = new Paragraph();
			//    BitmapImage bitmap = new BitmapImage(new Uri(news.DescriptionImage));
			//    Image image = new Image();
			//    image.Source = bitmap;
			//    image.Width = 100;
			//    InlineUIContainer container = new InlineUIContainer();
			//    container.Child = image;
			//    imageParagraph.Inlines.Add(container);
			//    richTextBlock.Blocks.Add(imageParagraph);
			//}

			//Paragraph titleParagraph = new Paragraph() { TextAlignment = TextAlignment.Left };
			//Bold bold = new Bold() { FontWeight = Windows.UI.Text.FontWeights.SemiBold };
			//bold.Inlines.Add(new Run() { Text = news.Title, FontSize = 36 });
			//titleParagraph.Inlines.Add(bold);
			//titleParagraph.Inlines.Add(new Run() { Text = "\n\n" });
			//richTextBlock.Blocks.Add(titleParagraph);


			//foreach (var item in await BuildBlocks(d, doc.DocumentNode))
			//{
			//    richTextBlock.Blocks.Add(item);
			//}


			//var target = d as RichTextColumns;
			//if (target != null)
			//{
			//    target.RichTextContent = richTextBlock;
			//}
		}

		/// <summary>
		/// Метод для построение блоков RichTextBlock из тэгов
		/// </summary>
		/// <param name="depobj"></param>
		/// <param name="htmlNodeCollection"></param>
		/// <returns></returns>
		async static private Task<IEnumerable<Block>> BuildBlocks(DependencyObject depobj, HtmlNode htmlNodeCollection)
		{
			int lineHeight;
			List<Block> blocks = new List<Block>();
			foreach (var item in htmlNodeCollection.ChildNodes)
			{
				if (item.ChildNodes.Where(d => d.Name == "object").Count<HtmlNode>() != 0 || item.ChildNodes.Where(d => d.Name == "iframe").Count<HtmlNode>() != 0)
				{
					lineHeight = 320;
				}
				else
				{
					lineHeight = 23;
				}
				switch (item.Name)
				{
					case "div":
						#region div logic
						if (item.ChildNodes.Count==0)
						{
							break;
						}
						//если div - контейнер для facebook-записи
						if (item.Attributes.Contains("class") && item.Attributes["class"].Value == "fb-post")
						{
							var aa = item.Descendants("div").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value == "fb-xfbml-parse-ignore").FirstOrDefault<HtmlNode>();
							Paragraph p5 = new Paragraph() { TextAlignment = TextAlignment.Left, LineHeight = lineHeight };
							foreach (var item2 in await BuildParagraph(depobj, aa))
							{
								p5.Inlines.Add(item2);
							}
							blocks.Add(p5);
						}
						//если внутри div есть контейнер p или table
						else
						{
							foreach (var item2 in item.ChildNodes)
							{
								switch (item2.Name)
								{
									case "p":
									case "table":
										Paragraph p1 = new Paragraph() { TextAlignment = TextAlignment.Left, LineHeight = lineHeight };
										if (item.ChildNodes.Where(d => d.Name == "table").Count<HtmlNode>() != 0)
										{
											p1.TextAlignment = TextAlignment.Center;
										}
										foreach (var item3 in await BuildParagraph(depobj, item2))
										{
											p1.Inlines.Add(item3);
										}
										blocks.Add(p1);
										break;
									case "#text":
										if (item2.InnerText==""||item2.InnerText=="\n")
										{
											break;
										}
										string inntext1 = item2.InnerText.Replace("\n", "").TrimStart(new char[] { '\n', ' ' });
										if (inntext1!="")
										{
											Paragraph p5 = new Paragraph() { TextAlignment = TextAlignment.Left, LineHeight = lineHeight };
											p5.Inlines.Add(new Run() { Text = inntext1, FontSize = 16 });
											blocks.Add(p5);
										}
										break;
									default:
										Paragraph p4 = new Paragraph() { TextAlignment = TextAlignment.Left, LineHeight = lineHeight };
										foreach (var item3 in await BuildParagraph(depobj, item2))
										{
											p4.Inlines.Add(item3);
										}
										blocks.Add(p4);
										break;
								}
							}
						}
						break;
						#endregion
					case "p":
						#region p logic
						if (item.ChildNodes.Count == 0)
						{
							break;
						}
						Paragraph p2 = new Paragraph() { TextAlignment = TextAlignment.Left, LineHeight = lineHeight };
						foreach (var item2 in await BuildParagraph(depobj, item))
						{
							p2.Inlines.Add(item2);
						}
						blocks.Add(p2);
						break;
						#endregion
					case "table":
						if (item.ChildNodes.Count == 0)
						{
							break;
						}
						Paragraph p6 = new Paragraph() { TextAlignment = TextAlignment.Left, LineHeight = lineHeight };
						foreach (var item2 in await BuildParagraph(depobj, item))
						{
							p6.Inlines.Add(item2);
						}
						blocks.Add(p6);
						break;
					default:
						try
						{
							var ue = new UndefElements() 
							{ 
								ElementName = item.Name, 
								ExceptionName = "default", 
								InnerHtml = item.InnerHtml, 
								OuterHtml = item.OuterHtml, 
								ParentElementName = item.ParentNode.Name, 
								StackTrace = "null"
							};
							//await MobileService.GetTable<UndefElements>().InsertAsync(ue);
						}
						catch
						{

						}
						break;
				}
			}
			return blocks;
		}

		/// <summary>
		/// Метод для построения наполения блоков RichTextBlock из тэгов
		/// </summary>
		/// <param name="depobj"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		async private static Task<IEnumerable<Inline>> BuildParagraph(DependencyObject depobj, HtmlNode node)
		{
			List<Inline> inlines = new List<Inline>();
			foreach (var item in node.ChildNodes)
			{
				switch (item.Name)
				{
					case "img":
						try
						{
							BitmapImage img_bitmap = new BitmapImage(new Uri(item.Attributes["src"].Value.ToString()));
							Image img_image = new Image();
							img_image.Source = img_bitmap;
							img_image.Width = 480;
							InlineUIContainer img_container = new InlineUIContainer();
							img_container.Child = img_image;
							inlines.Add(img_container);
						}
						catch (Exception ex)
						{
							AzureThrowException(item, ex);
						}
						break;
					case "a":
						try
						{
							Hyperlink a_link = new Hyperlink();
							var href = item.Attributes["href"].Value.ToString();
							a_link.NavigateUri = new Uri(href.Replace("\n", ""));
							a_link.Inlines.Add(new Run { Text = item.InnerText.Replace("\n", "") });
							a_link.FontSize = 16;
							inlines.Add(a_link);
						}
						catch (Exception ex)
						{
							AzureThrowException(item, ex);
						}
						break;
					case "object":
						try
						{
							if (item.Descendants("param").Where(d => d.GetAttributeValue("name", "") == "src").Count<HtmlNode>() != 0)
							{
								Grid object_imggrid = new Grid();

								var object_vsrc = Embeding(item.Descendants("param").Where(d => d.GetAttributeValue("name", "") == "src").FirstOrDefault<HtmlNode>().Attributes["value"].Value.ToString());
								var object_imgsrc = "http://img.youtube.com/vi/" + YoutubeID(object_vsrc) + "/0.jpg";
								BitmapImage object_bitmap = new BitmapImage(new Uri(object_imgsrc));
								Image object_image = new Image();
								object_image.Tag = object_vsrc;
								object_image.Source = object_bitmap;
								object_image.Width = 480;
								object_imggrid.Children.Add(object_image);

								BitmapImage object_ytbimgbitmap = new BitmapImage(new Uri("ms-appx:///Assets/oie_transparent.png"));
								Image object_ytbimg = new Image();
								object_ytbimg.Source = object_ytbimgbitmap;
								object_ytbimg.Width = 240;
								object_imggrid.Children.Add(object_ytbimg);
								object_imggrid.Tapped += (sender, e) => Video_Tapped(sender, e, depobj);

								InlineUIContainer object_imggridcontainer = new InlineUIContainer();
								object_imggridcontainer.Child = object_imggrid;
								inlines.Add(object_imggridcontainer);
							}
							else
							{
								throw new Exception("Video not found");
							}
						}
						catch (Exception ex)
						{
							AzureThrowException(item, ex);
						}
						break;
					case "iframe":
						try
						{
							Grid iframe_imggrid = new Grid();
							iframe_imggrid.Width = 480;
							iframe_imggrid.Height = 320;

							var iframe_vsrc = Embeding(item.Attributes["src"].Value.ToString());
							var iframe_imgsrc = "http://img.youtube.com/vi/" + YoutubeID(iframe_vsrc) + "/0.jpg";
							BitmapImage iframe_bitmap = new BitmapImage(new Uri(iframe_imgsrc));
							Image iframe_image = new Image();
							iframe_image.Tag = iframe_vsrc;
							iframe_image.Source = iframe_bitmap;
							iframe_image.Width = 480;
							iframe_imggrid.Children.Add(iframe_image);

							BitmapImage iframe_ytbimgbitmap = new BitmapImage(new Uri("ms-appx:///HtmlTools/Assets/VideoThumb350.png"));
							Image iframe_ytbimg = new Image();
							iframe_ytbimg.Source = iframe_ytbimgbitmap;
							iframe_ytbimg.Width = 240;
							iframe_ytbimg.Height = 240;
							iframe_imggrid.Children.Add(iframe_ytbimg);
							iframe_imggrid.Tapped += (sender, e) => Video_Tapped(sender, e, depobj);

							InlineUIContainer iframe_imggridcontainer = new InlineUIContainer();
							iframe_imggridcontainer.Child = iframe_imggrid;
							inlines.Add(iframe_imggridcontainer);
							inlines.Add(new Run() { Text = "\n", FontSize = 16 });
						}
						catch (Exception ex)
						{
							AzureThrowException(item, ex);
						}
						break;
					case "br":
						try
						{
							if (item.InnerText == String.Empty)
							{
								break;
							}
							inlines.Add(new Run() { Text = "\n", FontSize = 16 });
						}
						catch (Exception ex)
						{
							AzureThrowException(item, ex);
						}
						break;
					case "em":
						try
						{
							if (item.InnerText == String.Empty)
							{
								break;
							}
							Italic em_it = new Italic();
							em_it.Inlines.Add(new Run() { Text = item.InnerText.Replace("\n", ""), FontSize = 16 });
							inlines.Add(em_it);
						}
						catch (Exception ex)
						{
							AzureThrowException(item, ex);
						}
						break;
					case "strong":
						try
						{
							if (item.InnerText == String.Empty)
							{
								break;
							}
							Bold strong_b = new Bold();
							strong_b.Inlines.Add(new Run() { Text = item.InnerText.Replace("\n", ""), FontSize = 16 });
							inlines.Add(strong_b);
						}
						catch (Exception ex)
						{
							AzureThrowException(item, ex);
						}
						break;
					case "#text":
						try
						{
							if (item.InnerText == String.Empty)
							{
								break;
							}
							inlines.Add(new Run() { Text = item.InnerText.Replace("\n", ""), FontSize = 16 });
						}
						catch (Exception ex)
						{
							AzureThrowException(item, ex);
						}
						break;
					
					case "tbody":
						try
						{
							string tbody_imgsrc;
							if (item.Descendants("a").Count<HtmlNode>() == 0 || item.Descendants("img").Count<HtmlNode>() == 0)
							{
								//
								// Натуральная таблица
								//
								// В будущем реализовать ее.
								//
								throw new Exception("Need a table");
							}
							if (item.Descendants("a").Count<HtmlNode>() == 0 && item.Descendants("img").Count<HtmlNode>() != 0)
							{
								tbody_imgsrc = item.Descendants("img").FirstOrDefault<HtmlNode>().Attributes["src"].Value.ToString();
							}
							else
							{
								tbody_imgsrc = item.Descendants("a").FirstOrDefault<HtmlNode>().Attributes["href"].Value.ToString();
							}
							string tbody_text = "";
							if (item.Descendants("td").Where(d => d.GetAttributeValue("class", "") == "tb_text").Count<HtmlNode>() != 0)
							{
								tbody_text = item.Descendants("td").Where(d => d.GetAttributeValue("class", "") == "tb_text").FirstOrDefault<HtmlNode>().InnerText;
							}
							BitmapImage tbody_bitmap = new BitmapImage(new Uri(tbody_imgsrc));
							Image tbody_image = new Image();
							tbody_image.Source = tbody_bitmap;
							tbody_image.MaxWidth = 560;
							InlineUIContainer tbody_container = new InlineUIContainer();
							tbody_container.Child = tbody_image;
							inlines.Add(new Run() { Text = "\n", FontSize = 16 });
							inlines.Add(tbody_container);
							Italic tbody_it = new Italic();
							tbody_it.Inlines.Add(new Run() { Text = tbody_text, FontSize = 16 });
							inlines.Add(tbody_it);
							inlines.Add(new Run() { Text = "\n", FontSize = 16 });
						}
						catch (Exception ex)
						{
							AzureThrowException(item, ex);
						}
						break;
					default:
						try
						{
							var ue = new UndefElements()
							{
								ElementName = item.Name,
								ExceptionName = "default",
								InnerHtml = item.InnerHtml,
								OuterHtml = item.OuterHtml,
								ParentElementName = item.ParentNode.Name,
								StackTrace = "null"
							};
							//await MobileService.GetTable<UndefElements>().InsertAsync(ue);
						}
						catch
						{

						}
						break;
				}
			}
			return inlines;
		}

		private static void AzureThrowException(HtmlNode item, Exception ex)
		{
			var ue = new UndefElements()
			{
				ElementName = item.Name,
				ExceptionName = ex.Message,
				InnerHtml = item.InnerHtml,
				OuterHtml = item.OuterHtml,
				ParentElementName = item.ParentNode.Name,
				StackTrace = ex.StackTrace
			};
			try
			{
				//MobileService.GetTable<UndefElements>().InsertAsync(ue);
			}
			catch
			{

			}
		}



		static void Video_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e, DependencyObject depobj)
		{
			var videoGrid = (Grid)sender;
			var videoSource = ((Image)videoGrid.Children[0]).Tag.ToString();
			((RichTextColumns)depobj).VideoEvent(new VideoEventArgs(YoutubeID(videoSource)));
		}

		private void VideoEvent(VideoEventArgs videoEventArgs)
		{
			EventHandler<VideoEventArgs> handler = VideoPlayEvent;

			// Event will be null if there are no subscribers
			if (handler != null)
			{
				// Use the () operator to raise the event.
				handler(typeof(RichTextColumns), videoEventArgs);
			}

		}

		public static string Embeding(string ytbstr)
		{
			return "https://youtube.com/embed/" + YoutubeID(ytbstr);
		}

		public static string YoutubeID(string ytbstr)
		{
			Regex YoutubeVideoRegex = new Regex(@"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)");
			Match youtubeMatch = YoutubeVideoRegex.Match(ytbstr);

			string id = string.Empty;

			if (youtubeMatch.Success)
				id = youtubeMatch.Groups[1].Value;
			return id;
		}

		/// <summary>
		/// Determines whether additional overflow columns are needed and if existing columns can
		/// be removed.
		/// </summary>
		/// <param name="availableSize">The size of the space available, used to constrain the
		/// number of additional columns that can be created.</param>
		/// <returns>The resulting size of the original content plus any extra columns.</returns>
		protected override Size MeasureOverride(Size availableSize)
		{
			if (this.RichTextContent == null) return new Size(0, 0);

			// Make sure the RichTextBlock is a child, using the lack of
			// a list of additional columns as a sign that this hasn't been
			// done yet
			if (this._overflowColumns == null)
			{
				Children.Add(this.RichTextContent);
				this._overflowColumns = new List<RichTextBlockOverflow>();
			}

			// Start by measuring the original RichTextBlock content
			this.RichTextContent.Measure(availableSize);
			var maxWidth = this.RichTextContent.DesiredSize.Width;
			var maxHeight = this.RichTextContent.DesiredSize.Height;
			var hasOverflow = this.RichTextContent.HasOverflowContent;

			// Make sure there are enough overflow columns
			int overflowIndex = 0;
			while (hasOverflow && maxWidth < availableSize.Width && this.ColumnTemplate != null)
			{
				// Use existing overflow columns until we run out, then create
				// more from the supplied template
				RichTextBlockOverflow overflow;
				if (this._overflowColumns.Count > overflowIndex)
				{
					overflow = this._overflowColumns[overflowIndex];
				}
				else
				{
					overflow = (RichTextBlockOverflow)this.ColumnTemplate.LoadContent();
					this._overflowColumns.Add(overflow);
					this.Children.Add(overflow);
					if (overflowIndex == 0)
					{
						this.RichTextContent.OverflowContentTarget = overflow;
					}
					else
					{
						this._overflowColumns[overflowIndex - 1].OverflowContentTarget = overflow;
					}
				}

				// Measure the new column and prepare to repeat as necessary
				overflow.Measure(new Size(availableSize.Width - maxWidth, availableSize.Height));
				maxWidth += overflow.DesiredSize.Width;
				maxHeight = Math.Max(maxHeight, overflow.DesiredSize.Height);
				hasOverflow = overflow.HasOverflowContent;
				overflowIndex++;
			}

			// Disconnect extra columns from the overflow chain, remove them from our private list
			// of columns, and remove them as children
			if (this._overflowColumns.Count > overflowIndex)
			{
				if (overflowIndex == 0)
				{
					this.RichTextContent.OverflowContentTarget = null;
				}
				else
				{
					this._overflowColumns[overflowIndex - 1].OverflowContentTarget = null;
				}
				while (this._overflowColumns.Count > overflowIndex)
				{
					this._overflowColumns.RemoveAt(overflowIndex);
					this.Children.RemoveAt(overflowIndex + 1);
				}
			}

			// Report final determined size
			return new Size(maxWidth, maxHeight);
		}

		/// <summary>
		/// Arranges the original content and all extra columns.
		/// </summary>
		/// <param name="finalSize">Defines the size of the area the children must be arranged
		/// within.</param>
		/// <returns>The size of the area the children actually required.</returns>
		protected override Size ArrangeOverride(Size finalSize)
		{
			double maxWidth = 0;
			double maxHeight = 0;
			foreach (var child in Children)
			{
				child.Arrange(new Rect(maxWidth, 0, child.DesiredSize.Width, finalSize.Height));
				maxWidth += child.DesiredSize.Width;
				maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
			}
			return new Size(maxWidth, maxHeight);
		}
	}
}

