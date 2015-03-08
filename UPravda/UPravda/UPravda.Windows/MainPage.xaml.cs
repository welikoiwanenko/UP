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
using UPravda.DataSources;
using UPravda.Helpers;

// Документацию по шаблону элемента "Основная страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234237

namespace UPravda
{
    /// <summary>
    /// Основная страница, которая обеспечивает характеристики, являющимися общими для большинства приложений.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

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


        public MainPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Заполняет страницу содержимым, передаваемым в процессе навигации. Любое сохраненное состояние также является
        /// при повторном создании страницы из предыдущего сеанса.
        /// </summary>
        /// <param name="sender">
        /// Источник события; как правило, <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Данные события, предоставляющие параметр навигации, который передается
        /// <see cref="Frame.Navigate(Type, Object)"/> при первоначальном запросе этой страницы, и
        /// словарь состояния, сохраненного этой страницей в ходе предыдущего
        /// сеансом. Состояние будет равно значению NULL при первом посещении страницы.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Сохраняет состояние, связанное с данной страницей, в случае приостановки приложения или
        /// удаления страницы из кэша навигации.  Значения должны соответствовать требованиям сериализации
        /// <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">Источник события; как правило, <see cref="NavigationHelper"/></param>
        /// <param name="e">Данные события, которые предоставляют пустой словарь для заполнения
        /// сериализуемым состоянием.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {

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

        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.RemoveBackEntry();
            navigationHelper.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.New)
            {
                progressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                this.DefaultViewModel["Theme"] = App.Theme;

                if ((bool)e.Parameter) // если offline-данные были в Storage - отобразить их
                {
                    var dataSource = await MainDataSource.GetAllOfflineNewsData();
                    this.DefaultViewModel["DataSource"] = dataSource;
                }

                if (App.IsConnectedToInternet())
                {
                    progressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    var dataSource = await MainDataSource.GetAllNewsData();
                    this.DefaultViewModel["DataSource"] = null;
                    this.DefaultViewModel["DataSource"] = dataSource;
                }
                else
                {
                    captionGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {

            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void NewsItem_Click(object sender, ItemClickEventArgs e)
        {
            //bool r = HtmlTools.Helpers.HtmlHelpers.IsFullyPhotoNews((e.ClickedItem as NewsItem).Description);
            this.Frame.Navigate(typeof(NewsItemPage), e.ClickedItem);
        }

        private void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            var items = e.Section.DataContext;
            this.Frame.Navigate(typeof(DetailPage), (items as IDataSource).NewsData.NewsType);
        }

        private void CloseCaptionButton_Click(object sender, RoutedEventArgs e)
        {
            captionGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
