using System.Web.Http;
using System.Web.Http.Cors;

namespace WebAPI2.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    /// <summary>
    /// 继承基类
    /// </summary>
    public abstract class BaseApiController : ApiController
    {
        

       
    }

}