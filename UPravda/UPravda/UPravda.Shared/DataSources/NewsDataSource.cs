using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using UPravda.Models;
using Windows.ApplicationModel;
using Windows.Storage;
using System.IO;
using UPravda.Helpers;

namespace UPravda.DataSources
{
	public class NewsDataSource : IDataSource
	{
		private static NewsDataSource _dataSource = new NewsDataSource();

		private News _newsData = new News();

		private News _offlineNewsData = new News();
		private News _onlineNewsData = new News();

		public News NewsData
		{
			get { return this._newsData; }
			set { this._newsData = value; }
		}

		public static async Task<NewsDataSource> GetDataSourceAsync()
		{
			await _dataSource.LoadingOnlineData();
			_dataSource._newsData = _dataSource._onlineNewsData;

			return _dataSource;
		}

		public static async Task<NewsDataSource> GetOfflineDataSourceAsync()
		{
			await _dataSource.LoadingOfflineData();
			_dataSource._newsData = _dataSource._offlineNewsData;

			return _dataSource;
		}

		async private Task LoadingOfflineData()
		{
			string xml = null;
			xml = await OfflineHelper.GetFromStorage(NewsType.News);
			var data = await Parse(xml);
			this._offlineNewsData = new News() { NewsItems = new ObservableCollection<NewsItem>(data), NewsType = NewsType.News };
		}

		private async Task LoadingOnlineData()
		{
			if (this._onlineNewsData.NewsItems.Count != 0)
				return;

			string xml = null;
			HttpResponseMessage request = null;

			HttpClient a = new HttpClient();
			var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
			var str = loader.GetString("Language");
			request = await a.GetAsync(new Uri("http://www.pravda.com.ua/rssiphone/mode_news/?language=" + str));
			if (request.StatusCode == System.Net.HttpStatusCode.BadGateway)
			{
				throw new HttpRequestException("Сервер не відповідає");
			}
			xml = await request.Content.ReadAsStringAsync();

			var data = await Parse(xml);
			this._onlineNewsData = new News() { NewsItems = new ObservableCollection<NewsItem>(data), NewsType = NewsType.News };
		}

		private static async Task<IEnumerable<NewsItem>> Parse(string xml)
		{
			XDocument loadedData = XDocument.Parse(xml);
			await OfflineHelper.SaveToStorage(NewsType.News, xml);

			var data = from query in loadedData.Descendants("item")
					   select new NewsItem
					   {
						   ID = (uint)query.Element("id"),
						   Title = (string)query.Element("title"),
						   Description = (string)query.Element("description"),
						   Annotation = (string)query.Element("annotation"),
						   Link = (string)query.Element("link"),
						   Highlighted = (uint)query.Element("highlighted"),
						   Thumbnail = (string)query.Element("thumbnail") == null ? "ms-appx:///Assets/Images/thumb.png" : (string)query.Element("thumbnail")
					   };
			return data;
		}
	}
}
