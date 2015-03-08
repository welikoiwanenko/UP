using Microsoft.WindowsAzure.Mobile.Service.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;
using UkrPravdaService.Models;

namespace UkrPravdaService.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.Anonymous)]
    public class TileController : ApiController
    {
        [AuthorizeLevel(AuthorizationLevel.Anonymous)]
        // GET: api/Tile
        public async Task<HttpResponseMessage> Get()
        {
            return await this.Get(0);
        }

        // GET: api/Tile/5
        public async Task<HttpResponseMessage> Get(int id)
        {
            string xml = null;
            HttpResponseMessage request = null;

            HttpClient a = new HttpClient();
            request = await a.GetAsync(new Uri("http://www.pravda.com.ua/rssiphone/mode_news/?language=" + "uk"));
            if (request.StatusCode == System.Net.HttpStatusCode.BadGateway)
            {
                throw new HttpRequestException("Сервер не відповідає");
            }
            xml = await request.Content.ReadAsStringAsync();

            var data = await Parse(xml);
            var listData = data.ToList<NewsItem>();
            //добавить обработку id
            var xmlString = "<tile><visual version=\"2\"><binding template=\"TileWide310x150Text04\" fallback=\"TileWideText04\"><text id=\"1\">" + listData[id].Title + "</text></binding> <binding template=\"TileSquare150x150Text04\" fallback=\"TileSquareText04\"><text id=\"1\">" + listData[id].Title + "</text></binding>  </visual></tile>";

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    xmlString,
                    Encoding.UTF8,
                    "text/xml"
                )
            };
        }


        private static async Task<IEnumerable<NewsItem>> Parse(string xml)
        {
            XDocument loadedData = XDocument.Parse(xml);
            var data = from query in loadedData.Descendants("item")
                       select new NewsItem
                       {
                           ID = (uint)query.Element("id"),
                           Title = (string)query.Element("title"),
                           Description = (string)query.Element("description"),
                           Annotation = (string)query.Element("annotation"),
                           Link = (string)query.Element("link"),
                           Highlighted = (uint)query.Element("highlighted")
                       };
            return data;
        }
    }

}
