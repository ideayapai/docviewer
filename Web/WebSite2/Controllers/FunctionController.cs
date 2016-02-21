using System;
using System.Linq;
using System.Web.Mvc;
using Infrasturcture.Utils;
using Services.Contracts;
using Services.Enums;
using Services.Role;

namespace WebSite2.Controllers
{
    public class FunctionController : BaseController
    {
        private readonly RoleFunctionService _roleFunctionService;

        public FunctionController(RoleFunctionService roleFunctionService)
        {
            _roleFunctionService = roleFunctionService;
        }

        //[MenuHightlight(CurrentItem = "MenuSetting_tt")]
        public ActionResult Index()
        {
            var functions = _roleFunctionService.GetFunctions().Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.MenuId.ToString(),
            }).ToList();
            ViewData["functions"] = functions;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult List()
        {
            Guid roleId = CurrentUser.RoleId;

            var functions = _roleFunctionService.GetShortMenu(roleId, true);

            var rs = JsonFormaterUtils.Serialize(functions);
            return Content(rs);

        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetFunction(Guid Id)
        {
            var function = _roleFunctionService.GetById(Id);
            var rs = JsonFormaterUtils.Serialize(function);
            return Content(rs);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Edite(MenuItemContract item)
        {
            string aId = Request.Params["MAttributes"];
            if (string.IsNullOrEmpty(aId))
            {
                return Json(new
                {
                    result = false,
                    msg = "唯一标识不能为空！"
                });
            }

            item.Attributes = new MenuItemAttributes { Id = aId };
            //新增菜单
            if (item.MenuId == Guid.Empty)
            {
                return CreateMenu(item);
            }
            //编辑菜单
            else
            {
                return EditMenu(item);
            }
        }

        private ActionResult EditMenu(MenuItemContract item)
        {
            if (item.IsValid())
            {
                var rs = _roleFunctionService.Update(item);
                if (rs == ResultStatus.Success)
                {
                    return Json(new
                    {
                        result = true,
                        msg = "菜单" + item.Name + "保存成功！"
                    });
                }
                return Json(new
                {
                    result = false,
                    msg = (rs == ResultStatus.Duplicate ? "记录已经存在" : "保存失败")
                });
            }
            return Json(new
            {
                result = false,
                msg = item.ErrorMessage
            });
        }

        private ActionResult CreateMenu(MenuItemContract item)
        {
            item.MenuId = Guid.NewGuid();
            if (item.IsValid())
            {
                var rs = _roleFunctionService.Create(item);
                if (rs == ResultStatus.Success)
                {
                    return Json(new
                    {
                        result = true,
                        msg = "菜单" + item.Name + "创建成功！"
                    });
                }
                return Json(new
                {
                    result = false,
                    msg = (rs == ResultStatus.Duplicate ? "记录已经存在" : "添加失败")
                });
            }
            return Json(new
            {
                result = false,
                msg = item.ErrorMessage
            });
        }
    }
}
