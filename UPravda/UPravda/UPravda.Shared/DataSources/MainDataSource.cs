using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UPravda.Helpers;
using UPravda.Models;

namespace UPravda.DataSources
{
	public class MainDataSource
	{
		private static MainDataSource _mainDataSource = new MainDataSource();

		private NewsDataSource _newsDataSource = new NewsDataSource();
		private ArticlesDataSource _articlesDataSource = new ArticlesDataSource();
		private ColumnsDataSource _columnsDataSource = new ColumnsDataSource();
		private PhotoDataSource _photosDataSource = new PhotoDataSource();
		private VideoDataSource _videosDataSource = new VideoDataSource();

		public NewsDataSource NewsDataSource
		{
			get { return _newsDataSource; }
			set { _newsDataSource = value; }
		}

		public ArticlesDataSource ArticlesDataSource
		{
			get { return _articlesDataSource; }
			set { _articlesDataSource = value; }
		}

		public ColumnsDataSource ColumnsDataSource
		{
			get { return _columnsDataSource; }
			set { _columnsDataSource = value; }
		}

		public PhotoDataSource PhotosDataSource
		{
			get { return _photosDataSource; }
			set { _photosDataSource = value; }
		}

		public VideoDataSource VideosDataSource
		{
			get { return _videosDataSource; }
			set { _videosDataSource = value; }
		}


		public MainDataSource()
		{

		}

		public static async Task<MainDataSource> GetAllNewsData()
		{
			await _mainDataSource.GetData();
			return _mainDataSource;
		}
		async public static Task<object> GetAllOfflineNewsData()
		{
			await _mainDataSource.GetOfflineData();
			return _mainDataSource;
		}

		async private Task GetOfflineData()
		{
			this._articlesDataSource = await ArticlesDataSource.GetOfflineDataSourceAsync();
			this._newsDataSource = await NewsDataSource.GetOfflineDataSourceAsync();
			this._photosDataSource = await UPravda.DataSources.PhotoDataSource.GetOfflineDataSourceAsync();
			this._columnsDataSource = await UPravda.DataSources.ColumnsDataSource.GetOfflineDataSourceAsync();
			this._videosDataSource = await UPravda.DataSources.VideoDataSource.GetOfflineDataSourceAsync();
		}
		async private Task GetData()
		{
			try
			{
				this._articlesDataSource = await UPravda.DataSources.ArticlesDataSource.GetDataSourceAsync();
				this._newsDataSource = await UPravda.DataSources.NewsDataSource.GetDataSourceAsync();
				this._photosDataSource = await UPravda.DataSources.PhotoDataSource.GetDataSourceAsync();
				this._columnsDataSource = await UPravda.DataSources.ColumnsDataSource.GetDataSourceAsync();
				this._videosDataSource = await UPravda.DataSources.VideoDataSource.GetDataSourceAsync();
			}
			catch (HttpRequestException ex)
			{
				throw new HttpRequestException(ex.Message);
			}
		}

		async public static Task<bool> IsOfflineDataContains()
		{
			return await OfflineHelper.IsNewsDataConatins();
		}

		
	}
}
