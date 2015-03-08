using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.UI.Xaml;

namespace UPravda.Helpers
{
    public static class SettingHelper
    {
        static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        static SettingHelper()
        {
            localSettings.CreateContainer("settingsContainer", ApplicationDataCreateDisposition.Always);
        }

        public static T GetSetting<T>(Settings setting, T defaultValue)
        {
            T set;
            bool hasContainer = localSettings.Containers.ContainsKey("settingsContainer");
            if (hasContainer)
            {
                if (localSettings.Containers["settingsContainer"].Values.ContainsKey(setting.ToString()))
                {
                    set = Deserialize<T>(localSettings.Containers["settingsContainer"].Values[setting.ToString()].ToString());
                }
                else
                {
                    set = defaultValue;
                }
            }
            else
            {
                set = defaultValue;
            }

            //localSettings.Containers["settingsContainer"].Values[setting.ToString()]

            return set;
        }

        public static void SetSetting<T>(T value, Settings setting)
        {
            bool hasContainer = localSettings.Containers.ContainsKey("settingsContainer");
            if (hasContainer)
            {
                if (localSettings.Containers["settingsContainer"].Values.ContainsKey(setting.ToString()))
                {
                    localSettings.Containers["settingsContainer"].Values[setting.ToString()] = Serialize(value);
                }
                else
                {
                    localSettings.Containers["settingsContainer"].Values.Add(setting.ToString(), Serialize(value));
                }
            }
        }

        private static string Serialize(object obj)
        {
            using (var sw = new StringWriter())
            {
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(sw, obj);
                return sw.ToString();
            }
        }

        private static T Deserialize<T>(string xml)
        {
            using (var sw = new StringReader(xml))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(sw);
            }
        }

    }

    public enum Settings
    {
        NewsItemPageTheme, AppTheme, Language
    }
}
