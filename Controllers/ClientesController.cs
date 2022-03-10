using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http;

namespace JsonWebTokensAPI.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("~/[controller]")]
    [Authorize]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("GetId")]
        public IHttpActionResult GetId(int id)
        {
            var customerFake = "customer-fake";
            return (IHttpActionResult)Ok(customerFake);
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("GetAll")]
        public IHttpActionResult GetAll()
        {
            var customersFake = new string[] { "customer-1", "customer-2", "customer-3" };
            return (IHttpActionResult)Ok(customersFake);
        }
    }
}
