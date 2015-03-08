using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Linq;
using UPravda.DataSources;
using UPravda.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace UPravda
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class SplashPage : Page
    {
        public SplashPage()
        {
            this.InitializeComponent();
        }

        async private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            TileUpdateManager.CreateTileUpdaterForApplication().StartPeriodicUpdateBatch(new List<Uri>() { new Uri("http://ukrpravda.azure-mobile.net/api/tile/0"), new Uri("http://ukrpravda.azure-mobile.net/api/tile/1"), new Uri("http://ukrpravda.azure-mobile.net/api/tile/2") }, PeriodicUpdateRecurrence.HalfHour);

            await LoadData();
        }

        private async Task LoadData()
        {
            if (!await MainDataSource.IsOfflineDataContains())
            {
                if (App.IsConnectedToInternet())
                {
                    await MainDataSource.GetAllNewsData();
                    ///false - не было offline-данных
                    this.Frame.Navigate(typeof(MainPage), false);
                }
                else
                {
                    //MessageDialog md = new MessageDialog("INTERNET ERROR", "Oops");
                    //await md.ShowAsync();
                    loadGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    errorGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
            }
            else
            {
                this.Frame.Navigate(typeof(MainPage), true);
            }
        }

        async private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            loadGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
            errorGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            await LoadData();
        }
    }
}
