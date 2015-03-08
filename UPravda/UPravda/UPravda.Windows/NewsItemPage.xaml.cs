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
using MyToolkit;
using MyToolkit.Multimedia;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.DataTransfer;

// Документацию по шаблону элемента "Основная страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234237

namespace UPravda
{
    /// <summary>
    /// Основная страница, которая обеспечивает характеристики, являющимися общими для большинства приложений.
    /// </summary>
    public sealed partial class NewsItemPage : Page
    {
        private DataTransferManager dataTransferManager;
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        NewsItem news;

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


        public NewsItemPage()
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //navigationHelper.OnNavigatedTo(e);
            news = e.Parameter as NewsItem;
            this.DefaultViewModel["News"] = news;

            this.DefaultViewModel["Theme"] = SettingHelper.GetSetting<ElementTheme>(Settings.NewsItemPageTheme, ElementTheme.Light);
            if (SettingHelper.GetSetting<ElementTheme>(Settings.NewsItemPageTheme, ElementTheme.Light) == ElementTheme.Light)
            {
                this.DefaultViewModel["Logo"] = new BitmapImage(new Uri("ms-appx:///Assets/HeadLogo/HeadLogoBlack.png"));
            }
            else
            {
                this.DefaultViewModel["Logo"] = new BitmapImage(new Uri("ms-appx:///Assets/HeadLogo/HeadLogo.png"));
            }
            this.dataTransferManager = DataTransferManager.GetForCurrentView();
            this.dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnDataRequested);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void Video_Played(object sender, HtmlTools.VideoEventArgs e)
        {
            try
            {
                playerGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;

                var url = await YouTube.GetVideoUriAsync(e.VideoSource, YouTubeQuality.QualityHigh);
                player.Source = url.Uri;
            }
            catch (Exception)
            {
                playerGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                MessageDialog a = new MessageDialog("Video not found");
                a.ShowAsync();
            }

        }

        private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            player.Stop();
            playerGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            // Call the scenario specific function to populate the datapackage with the data to be shared.
            if (GetShareContent(e.Request))
            {
                // Out of the datapackage properties, the title is required. If the scenario completed successfully, we need
                // to make sure the title is valid since the sample scenario gets the title from the user.
                if (String.IsNullOrEmpty(e.Request.Data.Properties.Title))
                {
                    //e.Request.FailWithDisplayText(MainPage.MissingTitleError);
                }
            }
        }
        protected bool GetShareContent(DataRequest request)
        {
            bool succeeded = false;

            Uri dataPackageUri = ValidateAndGetUri(news.Link);
            if (dataPackageUri != null)
            {
                DataPackage requestData = request.Data;
                requestData.Properties.Title = news.Title;
                requestData.Properties.Description = news.Annotation; // The description is optional.
                                                                      //requestData.Properties.ContentSourceApplicationLink = ApplicationLink;
                requestData.SetWebLink(dataPackageUri);
                succeeded = true;
            }
            else
            {
                request.FailWithDisplayText("Enter the text you would like to share and try again.");
            }
            return succeeded;
        }
        private Uri ValidateAndGetUri(string uriString)
        {
            Uri uri = null;
            try
            {
                uri = new Uri(uriString);
            }
            catch (FormatException)
            {

            }
            return uri;
        }
    }
}
