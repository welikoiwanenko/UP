using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using UkrPravdaService.DataObjects;
using UkrPravdaService.Models;

namespace UkrPravdaService.Controllers
{
    public class UndefElementsController : TableController<UndefElements>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            UkrPravdaContext context = new UkrPravdaContext();
            DomainManager = new EntityDomainManager<UndefElements>(context, Request, Services);
        }

        // GET tables/UndefElements
        public IQueryable<UndefElements> GetAllUndefElements()
        {
            return Query();
        }

        // GET tables/UndefElements/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<UndefElements> GetUndefElements(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/UndefElements/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<UndefElements> PatchUndefElements(string id, Delta<UndefElements> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/UndefElements
        //IHttpActionResult
        public async Task<IHttpActionResult> PostUndefElements(UndefElements item)
        {
            UkrPravdaContext context = new UkrPravdaContext();
            var a = Query().Where(d => d.OuterHtml == item.OuterHtml).Count<UndefElements>();
            if (a != 0)
            {
                return BadRequest();
            }
            UndefElements current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/UndefElements/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteUndefElements(string id)
        {
            return DeleteAsync(id);
        }

    }
}