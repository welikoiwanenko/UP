using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UPravda.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Шаблон всплывающего элемента параметров задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=273769

namespace UPravda
{
    public sealed partial class SettingFlyout : SettingsFlyout
    {
        public SettingFlyout()
        {
            this.InitializeComponent();
            ElementTheme newsItemPageTheme = SettingHelper.GetSetting<ElementTheme>(Settings.NewsItemPageTheme, ElementTheme.Light);
            switch (newsItemPageTheme)
            {
                case ElementTheme.Dark:
                    NewsItemPageThemeComboBox.SelectedIndex = 1;
                    break;
                case ElementTheme.Light:
                    NewsItemPageThemeComboBox.SelectedIndex = 0;
                    break;
                default:
                    break;
            }

            ElementTheme appTheme = SettingHelper.GetSetting<ElementTheme>(Settings.AppTheme, ElementTheme.Dark);
            switch (appTheme)
            {
                case ElementTheme.Dark:
                    AppThemeComboBox.SelectedIndex = 1;
                    break;
                case ElementTheme.Light:
                    AppThemeComboBox.SelectedIndex = 0;
                    break;
                default:
                    break;
            }

            string language = SettingHelper.GetSetting<string>(Settings.Language, "uk-UA");
            switch (language)
            {
                case "uk-UA":
                    LanguageComboBox.SelectedIndex = 0;
                    break;
                case "ru-RU":
                    LanguageComboBox.SelectedIndex = 1;
                    break;
                default:
                    break;
            }
        }

        private void NewsItemPageThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems.FirstOrDefault() as ComboBoxItem).Content.ToString() == "Світла")
            {
                SettingHelper.SetSetting<ElementTheme>(ElementTheme.Light, Settings.NewsItemPageTheme);
            }
            else
            {
                SettingHelper.SetSetting<ElementTheme>(ElementTheme.Dark, Settings.NewsItemPageTheme);
            }
        }

        private void AppThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems.FirstOrDefault() as ComboBoxItem).Content.ToString() == "Світла")
            {
                SettingHelper.SetSetting<ElementTheme>(ElementTheme.Light, Settings.AppTheme);
            }
            else
            {
                SettingHelper.SetSetting<ElementTheme>(ElementTheme.Dark, Settings.AppTheme);
            }
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems.FirstOrDefault() as ComboBoxItem).Content.ToString().ToLower() == "українська")
            {
                SettingHelper.SetSetting<string>("uk-UA", Settings.Language);
            }
            else
            {
                SettingHelper.SetSetting<string>("ru-RU", Settings.Language);
            }
        }


    }
}
