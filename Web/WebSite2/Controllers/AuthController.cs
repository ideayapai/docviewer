using System;
using System.Linq;
using System.Web.Mvc;
using Common.Logging;
using Infrasturcture.QueryableExtension;
using Infrasturcture.Utils;
using Services.Role;
using WebSite2.Models;

namespace WebSite2.Controllers
{
    //[CustomerAuthorize]
    //[MenuHightlight(CurrentItem = "authorizes_tt", CurrentParentItem = "user_home")]
    public class AuthController : BaseController
    {
        private readonly RoleService _roleService;
        private readonly RoleFunctionService _roleFunctionService;
        private ILog _logger = LogManager.GetCurrentClassLogger();

        public AuthController(RoleService roleService, RoleFunctionService roleFunctionService)
        {
            _roleService = roleService;
            _roleFunctionService = roleFunctionService;
        }

        public ActionResult Index()
        {
            var exprssion = new ExpressionCriteria
            {
                PropertyName = "ROLE_INFO_TYPE",
                Value = "system",
                Operate = Operator.NotEqual
            };

            //var roles = _roleService.GetByUser(CurrentUser.UserName, new ExpressionCriteriaBase[] { exprssion }, new string[] { "Id", "Name" }).Select(p => new SelectListItem
            //{
            //    Text = p.Name,
            //    Value = p.Id.ToString(),
            //}).ToList();

            var expression = new[]
            {
                new ExpressionCriteria{ PropertyName = "IsDel", Value = 0, Operate = Operator.Equal },
                new ExpressionCriteria  { PropertyName = "ROLE_INFO_TYPE", Value = "system", Operate = Operator.NotEqual }
            };
            var roles = _roleService.Get(expression).Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString(),
            }).ToList(); ;
            ViewData["Roles"] = roles;
            return View(new AuthViewModel());
        }

        //TODO 权限部分
       // [AllowAnonymous]
        public ActionResult List(Guid roleId)
        {
            var functions = _roleFunctionService.GetShortMenu(roleId, true);

            var rs = JsonFormaterUtils.Serialize(functions);
            return Content(rs);
        }

        [HttpPost]
       // [AllowAnonymous]
        public ActionResult Post(Guid roleId, Guid[] ids)
        {

            try
            {
                _roleFunctionService.Update(roleId, ids.ToList());
                return Json(true);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                _logger.Error(e.StackTrace);
                return Json(false);
            }
        }

    }
}
