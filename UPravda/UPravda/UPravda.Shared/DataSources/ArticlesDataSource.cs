﻿using System;
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
	public class ArticlesDataSource : IDataSource
	{
		private static ArticlesDataSource _dataSource = new ArticlesDataSource();
		
		private News _newsData = new News();

		private News _offlineNewsData = new News();
		private News _onlineNewsData = new News();

		public News NewsData
		{
			get { return this._newsData; }
			set { this._newsData = value; }
		}

		public static async Task<ArticlesDataSource> GetDataSourceAsync()
		{
			await _dataSource.LoadingOnlineData();
			_dataSource._newsData = _dataSource._onlineNewsData;

			return _dataSource;
		}

		public static async Task<ArticlesDataSource> GetOfflineDataSourceAsync()
		{
			await _dataSource.LoadingOfflineData();
			_dataSource._newsData = _dataSource._offlineNewsData;
			
			return _dataSource;
		}

		private async Task LoadingOfflineData()
		{
			string xml = null;
			xml = await OfflineHelper.GetFromStorage(NewsType.Article);
			var data = await Parse(xml);
			this._offlineNewsData = new News() { NewsItems = new ObservableCollection<NewsItem>(data) };
			int j = 0;
			foreach (var item in this._offlineNewsData.NewsItems)
			{
				if (item.DescriptionImage != null && j < 2)
				{
					item.Size = 2;
					++j;
				}
				else
				{
					continue;
				}
			}
			this._offlineNewsData.NewsType = NewsType.Article;
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
			request = await a.GetAsync(new Uri("http://www.pravda.com.ua/rssiphone/mode_articles/?language=" + str));
			if (request.StatusCode == System.Net.HttpStatusCode.BadGateway)
			{
				throw new HttpRequestException("Сервер не відповідає");
			}
			xml = await request.Content.ReadAsStringAsync();

			var data = await Parse(xml);
			this._onlineNewsData = new News() { NewsItems = new ObservableCollection<NewsItem>(data) };
			int j = 0;
			foreach (var item in this._onlineNewsData.NewsItems)
			{
				if (item.DescriptionImage != null && j < 2)
				{
					item.Size = 2;
					++j;
				}
				else
				{
					continue;
				}
			}
			this._onlineNewsData.NewsType = NewsType.Article;
		}

		private static async Task<IEnumerable<NewsItem>> Parse(string xml)
		{
			XDocument loadedData = XDocument.Parse(xml);
			await OfflineHelper.SaveToStorage(NewsType.Article, xml);
			var data = from query in loadedData.Descendants("item")
					   select new NewsItem
					   {
						   ID = (uint)query.Element("id"),
						   Title = (string)query.Element("title"),
						   Description = (string)query.Element("description"),
						   DescriptionImage = (string)query.Element("description-image"),
						   Annotation = (string)query.Element("annotation"),
						   Link = (string)query.Element("link"),
						   Highlighted = (uint)query.Element("highlighted"),
						   Thumbnail = (string)query.Element("thumbnail"),
						   Size = 1
					   };
			return data;
		}

	}
}
