using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UPravda.Models;

namespace UPravda.Helpers
{
    /// <summary>
    /// Getting ExpandButton Content
    /// </summary>
    public class ExpandButtonHelper
    {
        private static ExpandButtonHelper _expandButtonHelper;
        private string _expandButtonContent;
        private ObservableCollection<string> _expandButtonFlyoutSource;
        static Dictionary<NewsType, string> _sectionDictionaty;

        public static Dictionary<NewsType, string> SectionDictionary
        {
            get { return _sectionDictionaty; }
        }

        static ExpandButtonHelper()
        {
            _expandButtonHelper = new ExpandButtonHelper();
        }

        public ExpandButtonHelper()
        {
            _sectionDictionaty = new Dictionary<NewsType, string>() 
            { 
                { NewsType.News, "Новини" }, 
                { NewsType.Article, "Публікації" }, 
                { NewsType.Column, "Колонки" }, 
                { NewsType.Photo, "Фото" }, 
                { NewsType.Video, "Відео" } 
            };
        }

        public string ExpandButtonContent
        {
            get { return _expandButtonContent; }
            private set { _expandButtonContent = value; }
        }

        public ObservableCollection<string> ExpandButtonFlyoutSource
        {
            get { return _expandButtonFlyoutSource; }
            set { _expandButtonFlyoutSource = value; }
        }

        public static ExpandButtonHelper GetExpandButtonData(NewsType newsType)
        {
            _expandButtonHelper.GetExpandButtonContents(newsType);
            return _expandButtonHelper;
        }

        private void GetExpandButtonContents(NewsType newsType)
        {
            this.ExpandButtonContent = _sectionDictionaty[newsType];
            this.ExpandButtonFlyoutSource = new ObservableCollection<string>(_sectionDictionaty.Values);
        }
    }
}
