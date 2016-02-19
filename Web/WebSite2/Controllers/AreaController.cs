using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Infrasturcture.QueryableExtension;
using Services.Area;
using Services.Contracts;
using Services.Enums;
using Services.Exceptions;
using WebSite2.Models;

namespace WebSite2.Controllers
{
    //[MenuHightlight(CurrentItem = "area_tt", CurrentParentItem = "user_home")]
    public class AreaController : BaseController
    {
        private readonly AreaService _areaService;

        public AreaController(AreaService areaService)
        {
            _areaService = areaService;
        }

        //[CustomerAuthorize]
        public ActionResult Index()
        {
            var areaListViewModel = new AreaListViewModel();
            var expression = new[]
            {
                new ExpressionCriteria{ PropertyName = "GEO_INFO_ISDEL", Value = 0, Operate = Operator.Equal },
 
            };
            areaListViewModel.AreaContracts = _areaService.Get(expression, properties: new[] { "Id", "AreaNo", "AreaName" });
            return View(areaListViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult List(int page, int rows)
        {
            var searchString = Request.Params["searchString"];
            var rs = _areaService.Get(searchString, page, rows, Request.Params["sort"], Request.Params["order"]);
            var data = new Dictionary<string, object>()
            {
                {"rows", rs.Items},
                {"total", rs.TotalCount}
            };
            return Json(data);
        }

        public ActionResult Create()
        {
            var exists = _areaService.Get(properties: new[] { "Id", "AreaName" }).Select(p => new SelectListItem
            {
                Text = p.AreaName,
                Value = p.Id.ToString(),
            }).ToList();
            ViewData["Areas"] = exists;

            var item = new AreaContract()
            {
                Id = Guid.NewGuid()
            };
            var areaViewModel = new AreaViewModel(item);
            return View(areaViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AreaViewModel item)
        {
            var exists = _areaService.Get(properties: new[] { "Id", "AreaName" }).Select(p => new SelectListItem
            {
                Text = p.AreaName,
                Value = p.Id.ToString(),
            }).ToList();
            ViewData["Areas"] = exists;

            if (item.AreaContract.IsValid())
            {
                var rs = _areaService.Create(item.AreaContract);
                if (rs != ResultStatus.Success)
                {
                    ModelState.AddModelError("", rs == ResultStatus.Duplicate ? "记录已经存在" : "添加失败");
                    return View(new AreaViewModel(item.AreaContract));
                }
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", item.AreaContract.ErrorMessage);
            return View(new AreaViewModel(item.AreaContract));
        }

        public ActionResult Update(Guid id)
        {
            var area = _areaService.Get(id);

            var exists = _areaService.Get(properties: new[] { "Id", "AreaName" }).Select(p => new SelectListItem
            {
                Text = p.AreaName,
                Value = p.Id.ToString(),
            }).ToList();

            ViewData["Areas"] = exists;
            ViewData["AreaItem"] = area.ParentId;

            if (area != null)
            {
                return View(new AreaViewModel(area));
            }
            throw new ItemNotExistException("指定区域不存在!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(AreaViewModel area)
        {
            if (area.AreaContract.IsValid())
            {
                var status = _areaService.Update(area.AreaContract);
                switch (status)
                {
                    case ResultStatus.Success:
                        return RedirectToAction("Index");

                    case ResultStatus.Failed:
                        ModelState.AddModelError("", string.Format("修改区域信息{0}失败", area.AreaContract.AreaName));
                        break;
                }
            }

            var exists = _areaService.Get(properties: new[] { "Id", "AreaName" }).Select(p => new SelectListItem
            {
                Text = p.AreaName,
                Value = p.Id.ToString(),
            }).ToList();
            ViewData["Areas"] = exists;
            ViewData["AreaItem"] = area.AreaContract.ParentId;

            ModelState.AddModelError("Error", area.AreaContract.ErrorMessage);

            var areaInfo = _areaService.Get(area.AreaContract.Id);

            if (areaInfo != null)
            {
                return View(new AreaViewModel(areaInfo));
            }
            throw new ItemNotExistException("指定区域不存在!");
        }

        [AllowAnonymous]
        public ActionResult Delete(Guid id)
        {
            var rs = _areaService.SetDelete(id);
            return RedirectToAction("Index");
        }

    }
}
