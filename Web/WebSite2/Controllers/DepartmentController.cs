using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Infrasturcture.QueryableExtension;
using Services;
using Services.Area;
using Services.Contracts;
using Services.Department;
using Services.Enums;
using Services.Exceptions;
using WebSite2.Models;

namespace WebSite2.Controllers
{

    public class DepartmentController : BaseController
    {
        private readonly DepartmentService _departmentService;
        private readonly AreaService _areaService;
        private readonly SelectValueService _selectValueService;

        public DepartmentController(DepartmentService departmentService, AreaService areaService, SelectValueService selectValueService)
        {
            _departmentService = departmentService;
            _areaService = areaService;
            _selectValueService = selectValueService;
        }

        //[CustomerAuthorize]
        //[MenuHightlight(CurrentItem = "depart_tt", CurrentParentItem = "user_home")]
        public ActionResult Index()
        {
            var expression = new[]
            {
                new ExpressionCriteria{ PropertyName = "DEPT_INFO_ISDEL", Value = 0, Operate = Operator.Equal },
            };

            var deps = _departmentService.Get(expression);
            return View(new DepartmentListViewModel
            {
                DepartmentContracts = deps,
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult List(int page, int rows)
        {
            var columns = Request.Params["columns"].Split(',');
            var searchString = Request.Params["searchString"];
            var userName = CurrentUser.UserName;
            var rs = _departmentService.Get(userName, searchString, columns, page, rows, Request.Params["sort"], Request.Params["order"]);
            var data = new Dictionary<string, object>()
            {
                {"rows", rs.Items},
                {"total", rs.TotalCount}
            };
            return Json(data);
        }

        //[MenuHightlight(CurrentItem = "depart_tt", CurrentParentItem = "user_home")]
        public ActionResult Create()
        {
            var areas = _areaService.Get(properties: new[] { "Id", "AreaName" }).Select(p => new SelectListItem
            {
                Text = p.AreaName,
                Value = p.Id.ToString(),
            }).ToList();
            ViewData["Areas"] = areas;

            var expression = new[]
            {
                new ExpressionCriteria{ PropertyName = "DEPT_INFO_ISDEL", Value = 0, Operate = Operator.Equal },
            };
            var deps = _departmentService.Get(expression).Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString(),
            }).ToList();
            ViewData["Departments"] = deps;

            var item = new DepartmentContract()
            {
                Id = Guid.NewGuid(),
                CreatedBy = CurrentUser.UserName
            };
            return View(new DepartmentViewModel(item));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[MenuHightlight(CurrentItem = "depart_tt", CurrentParentItem = "user_home")]
        public ActionResult Create(DepartmentViewModel viewModel)
        {
            var areas = _areaService.Get(properties: new[] { "Id", "AreaName" }).Select(p => new SelectListItem
            {
                Text = p.AreaName,
                Value = p.Id.ToString(),
            }).ToList();
            ViewData["Areas"] = areas;

            var expression = new[]
            {
                new ExpressionCriteria{ PropertyName = "DEPT_INFO_ISDEL", Value = 0, Operate = Operator.Equal },
            };
            var deps = _departmentService.Get(expression).Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString(),
            }).ToList();
            ViewData["Departments"] = deps;

            if (viewModel.DepartmentContract.AreaId == default(Guid))
            {
                ModelState.AddModelError("", "请选择区域");
            }
            else if (viewModel.DepartmentContract.IsValid())
            {
                var rs = _departmentService.Create(viewModel.DepartmentContract);
                if (rs != ResultStatus.Success)
                {
                    ModelState.AddModelError("", rs == ResultStatus.Duplicate ? "记录已经存在" : "添加失败");
                    return View(new DepartmentViewModel(viewModel.DepartmentContract));
                }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", viewModel.DepartmentContract.ErrorMessage);
            return View(new DepartmentViewModel(viewModel.DepartmentContract));
        }

        //[MenuHightlight(CurrentItem = "depart_tt", CurrentParentItem = "user_home")]
        public ActionResult Update(Guid id)
        {
            var department = _departmentService.Get(id);

            var areas = _areaService.Get(properties: new[] { "Id", "AreaName" }).Select(p => new SelectListItem
            {
                Text = p.AreaName,
                Value = p.Id.ToString(),
            }).ToList();
            ViewData["Areas"] = areas;
            ViewData["AreaItem"] = department.AreaId;
            //ViewData["Roles"] = JavascriptFormaterUtils.Serialize(roles);
            var expression = new[]
            {
                new ExpressionCriteria{ PropertyName = "DEPT_INFO_ISDEL", Value = 0, Operate = Operator.Equal },
                new ExpressionCriteria{ PropertyName = "ParentId", Value = id.ToString(), Operate = Operator.NotEqual },
            };
            var deps = _departmentService.Get(expression).Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString(),
            }).ToList();
            ViewData["Departments"] = deps;
            ViewData["DepartItem"] = department.ParentId;

            if (department != null)
            {
                return View(new DepartmentViewModel(department));
            }
            throw new ItemNotExistException("指定部门不存在!");
        }

        [HttpPost]
        //[MenuHightlight(CurrentItem = "depart_tt", CurrentParentItem = "user_home")]
        public ActionResult Update(DepartmentViewModel viewModel)
        {
            if (viewModel.DepartmentContract.AreaId == default(Guid))
            {
                ModelState.AddModelError("", "请选择区域");
            }
            else if (viewModel.DepartmentContract.IsValid())
            {
                var status = _departmentService.Update(viewModel.DepartmentContract);
                switch (status)
                {
                    case ResultStatus.Success:
                        return RedirectToAction("Index");
                    case ResultStatus.Failed:
                        ModelState.AddModelError("", string.Format("修改部门信息{0}失败", viewModel.DepartmentContract.Name));
                        break;
                }
            }

            var areas = _areaService.Get(properties: new[] { "Id", "AreaName" }).Select(p => new SelectListItem
            {
                Text = p.AreaName,
                Value = p.Id.ToString(),
            }).ToList();
            ViewData["Areas"] = areas;
            ViewData["AreaItem"] = viewModel.DepartmentContract.AreaId;
            var expression = new[]
            {
                new ExpressionCriteria{ PropertyName = "DEPT_INFO_ISDEL", Value = 0, Operate = Operator.Equal },
                new ExpressionCriteria{ PropertyName = "ParentId", Value = viewModel.DepartmentContract.Id.ToString(), Operate = Operator.NotEqual },
            };
            var deps = _departmentService.Get(expression).Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString(),
            }).ToList();
            ViewData["Departments"] = deps;
            ViewData["DepartItem"] = viewModel.DepartmentContract.ParentId;

            ModelState.AddModelError("", viewModel.DepartmentContract.ErrorMessage);

            var departmentInfo = _departmentService.Get(viewModel.DepartmentContract.Id);

            if (departmentInfo != null)
            {
                return View(new DepartmentViewModel(departmentInfo));
            }
            throw new ItemNotExistException("指定部门不存在!");
        }

        //[HttpPost]
        //[AllowAnonymous]
        public ActionResult Delete(Guid id)
        {
            var rs = _departmentService.SetDelete(id);
            return RedirectToAction("Index");
        }
    }
}
