using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Infrasturcture.QueryableExtension;
using Services.Contracts;
using Services.Enums;
using Services.Exceptions;
using Services.Role;
using WebSite2.Models;

namespace WebSite2.Controllers
{
    //[MenuHightlight(CurrentItem = "role_tt", CurrentParentItem = "user_home")]
    public class RoleController : BaseController
    {
        private readonly RoleService _roleService;

        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        public ActionResult Index()
        {
            var expression = new[]
            {
                new ExpressionCriteria{ PropertyName = "IsDel", Value = 0, Operate = Operator.Equal },
            };
            var rs = _roleService.Get(expression);
            var viewModel = new RoleListViewModel
            {
                RoleContracts = rs,
            };
            
            return View(viewModel);
        }

        [HttpPost]
        //[AllowAnonymous]
        public ActionResult List(int page, int rows)
        {
            var searchString = Request.Params["searchString"];
            var rs = _roleService.GetByUser(CurrentUser.UserName, searchString, page, rows, Request.Params["sort"], Request.Params["order"]);
            var data = new Dictionary<string, object>()
            {
                {"rows", rs.Items},
                {"total", rs.TotalCount}
            };
            return Json(data);
        }

        public ActionResult Create()
        {
            var item = new RoleContract()
            {
                Id = Guid.NewGuid(),
                ApplicationId = Guid.Parse(ConfigurationManager.AppSettings["ApplicationId"])
            };
            return View(new RoleViewModel(item));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoleViewModel item)
        {
            if (item.RoleContract.IsValid())
            {
                item.RoleContract.Type = "custom";
                item.RoleContract.Createby = CurrentUser.UserName;
                var rs = _roleService.Create(item.RoleContract);
                if (rs != ResultStatus.Success)
                {
                    ModelState.AddModelError("", rs == ResultStatus.Duplicate ? "记录已经存在" : "添加失败");
                    return View(new RoleViewModel(item.RoleContract));
                }
                return RedirectToAction("Index");

            }
            ModelState.AddModelError("", item.RoleContract.ErrorMessage);
            return View(new RoleViewModel(item.RoleContract));
        }

        public ActionResult Update(Guid id)
        {
            var roleInfo = _roleService.Get(id);

            if (roleInfo != null)
            {
                return View(new RoleViewModel(roleInfo));
            }
            throw new ItemNotExistException("指定角色不存在!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(RoleViewModel viewModel)
        {
            if (viewModel.RoleContract.IsValid())
            {
                var status = _roleService.Update(viewModel.RoleContract);
                switch (status)
                {
                    case ResultStatus.Success:
                        return RedirectToAction("Index");
                    case ResultStatus.Failed:
                        ModelState.AddModelError("", string.Format("修改角色信息{0}失败", viewModel.RoleContract.Name));
                        break;
                }
            }

            ModelState.AddModelError("", viewModel.RoleContract.ErrorMessage);
            var roleInfo = _roleService.Get(viewModel.RoleContract.Id);
            if (roleInfo != null)
            {
                return View(new RoleViewModel(roleInfo));
            }
            throw new ItemNotExistException("指定角色不存在!");
        }

        //[HttpPost]
        //[AllowAnonymous]
        public ActionResult Delete(Guid id)
        {
            var rs = _roleService.SetDelete(id);
            return RedirectToAction("Index");
        }
    }
}
