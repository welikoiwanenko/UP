using UPravda.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using UPravda.Models;
using UPravda.Helpers;
using UPravda.DataSources;
using System.Collections.ObjectModel;
using System.Collections;
using Windows.ApplicationModel.Resources;

// Шаблон элемента страницы элементов задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234233

namespace UPravda
{
    /// <summary>
    /// Страница, на которой отображается коллекция эскизов элементов.  В приложении с разделением эта страница
    /// служит для отображения и выбора одной из доступных групп.
    /// </summary>
    public sealed partial class DetailPage : Page
    {
        Dictionary<NewsType, string> _sectionDictionary;
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        ResourceLoader loader;

        /// <summary>
        /// Эту настройку можно изменить на модель строго типизированных представлений.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper используется на каждой странице для облегчения навигации и 
        /// управление жизненным циклом процесса
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public DetailPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            loader = new ResourceLoader();
            _sectionDictionary = new Dictionary<NewsType, string>();
            _sectionDictionary.Add(NewsType.News, loader.GetString("NewsHubSection/Header"));
            _sectionDictionary.Add(NewsType.Article, loader.GetString("ArticlesHubSection/Header"));
            _sectionDictionary.Add(NewsType.Column, loader.GetString("ColumnsHubSection/Header"));
            _sectionDictionary.Add(NewsType.Photo, loader.GetString("PhotosHubSection/Header"));
            _sectionDictionary.Add(NewsType.Video, loader.GetString("VideosHubSection/Header"));
            ExpandButton.ItemsSource = _sectionDictionary.Values;
        }

        /// <summary>
        /// Заполняет страницу содержимым, передаваемым в процессе навигации.  Также предоставляется любое сохраненное состояние
        /// при повторном создании страницы из предыдущего сеанса.
        /// </summary>
        /// <param name="sender">
        /// Источник события; как правило, <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Данные события, предоставляющие параметр навигации, который передается
        /// <see cref="Frame.Navigate(Type, Object)"/> при первоначальном запросе этой страницы и
        /// словарь состояний, сохраненных этой страницей в ходе предыдущего
        /// сеанса.  Это состояние будет равно NULL при первом посещении страницы.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Назначение привязываемой коллекции элементов объекту this.DefaultViewModel["Items"]
        }

        #region Регистрация NavigationHelper

        /// Методы, предоставленные в этом разделе, используются исключительно для того, чтобы
        /// NavigationHelper для отклика на методы навигации страницы.
        /// 
        /// Логика страницы должна быть размещена в обработчиках событий для 
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// и <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// Параметр навигации доступен в методе LoadState 
        /// в дополнение к состоянию страницы, сохраненному в ходе предыдущего сеанса.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NewsType newsType = (NewsType)e.Parameter;
            BuildPage(newsType);
            //ExpandButton.SelectedItem = ExpandButton.Items.Where(x => (x as ComboBoxItem).Content.ToString() == _sectionDictionary[newsType].ToString()).FirstOrDefault();
            ExpandButton.SelectedItem = ExpandButton.Items.Where(x => x.ToString() == _sectionDictionary[newsType].ToString()).FirstOrDefault();
            this.DefaultViewModel["Theme"] = SettingHelper.GetSetting<ElementTheme>(Settings.AppTheme, ElementTheme.Dark);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
        }

        #endregion

        private void itemGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(NewsItemPage), e.ClickedItem);
        }


        async private void BuildPage(NewsType newsType)
        {
            switch (newsType)
            {
                case NewsType.News:
                    this.DefaultViewModel["Items"] = App.IsConnectedToInternet() ? (await NewsDataSource.GetDataSourceAsync()).NewsData.NewsItems : (await NewsDataSource.GetOfflineDataSourceAsync()).NewsData.NewsItems;
                    break;
                case NewsType.Article:
                    this.DefaultViewModel["Items"] = (await ArticlesDataSource.GetDataSourceAsync()).NewsData.NewsItems;
                    break;
                case NewsType.Column:
                    this.DefaultViewModel["Items"] = (await ColumnsDataSource.GetDataSourceAsync()).NewsData.NewsItems;
                    break;
                case NewsType.Photo:
                    this.DefaultViewModel["Items"] = App.IsConnectedToInternet() ? (await PhotoDataSource.GetDataSourceAsync()).NewsData.NewsItems : (await PhotoDataSource.GetOfflineDataSourceAsync()).NewsData.NewsItems;
                    break;
                case NewsType.Video:
                    this.DefaultViewModel["Items"] = (await VideoDataSource.GetDataSourceAsync()).NewsData.NewsItems;
                    break;
                default:
                    break;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;
            BuildPage(this._sectionDictionary.FirstOrDefault(x => x.Value.ToString() == e.AddedItems.FirstOrDefault().ToString()).Key);
        }

    }
}
