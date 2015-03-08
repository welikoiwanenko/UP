using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using UPravda.Helpers;
using UPravda.Models;

namespace UPravda.ViewModel
{
    /// <summary>
    /// Базовый класс ViewModel
    /// </summary>
    public abstract class ViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        //public ViewModel()
        //{
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
        //}

		/// <summary>
		/// Поле для хранения новостей
		/// </summary>
		private ObservableCollection<object> news;

		/// <summary>
		/// Свойство для хранения новостей
		/// </summary>
		protected ObservableCollection<object> News
		{
			get { return news; }
			set { news = value; }
		}

		private NewsType type;

		protected NewsType Type
		{
			get { return type; }
			set { type = value; }
		}

		/// <summary>
		/// Обновление новостей
		/// </summary>
		protected void Refresh()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Получение новостей из источников 
		/// </summary>
		protected void GetNews()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Получение новостей online
		/// </summary>
		protected void GetOnlineNews(NewsType type)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Получение новостей offline
		/// </summary>
		protected void GetOfflineNews(NewsType type)
		{
			throw new NotImplementedException();
		}
    }
}