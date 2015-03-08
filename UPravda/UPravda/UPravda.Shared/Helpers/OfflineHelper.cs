using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UPravda.Models;
using Windows.Storage;

namespace UPravda.Helpers
{
    public static class OfflineHelper
    {
        /// <summary>
        /// Получение сохраненной в хранилище новостей определенного типа
        /// </summary>
        /// <param name="newsType">Тип получаемых новостей</param>
        /// <returns>Сохраненный XML</returns>
        async public static Task<string> GetFromStorage(NewsType newsType)
        {
            StorageFile file;
            try
            {
                file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(newsType.ToString());
            }
            catch (FileNotFoundException ex)
            {
                return null;
            }
            return await Windows.Storage.FileIO.ReadTextAsync(file);
        }

        /// <summary>
        /// Сохранение в хранилище новости определенного типа
        /// </summary>
        /// <param name="newsType">Тип сохраняемой новостей</param>
        /// <param name="data">Сохраняемая XML</param>
        /// <returns></returns>
        async public static Task SaveToStorage(NewsType newsType, string data)
        {
            var file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(newsType.ToString(), Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(file, data);
        }

        async public static Task<bool> IsNewsDataConatins()
        {
            return await IsNewsDataConatins(NewsType.Article, NewsType.Column, NewsType.News, NewsType.Photo, NewsType.Video);
        }
        async public static Task<bool> IsNewsDataConatins(params NewsType[] newsTypes)
        {
            foreach (var item in newsTypes)
            {
                try
                {
                    var file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(item.ToString());
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
    }
}
