using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Infrasturcture.QueryableExtension;
using Services.Contracts;
using Services.Department;
using Services.Enums;
using Services.Exceptions;
using Services.Role;
using Services.User;
using WebSite2.Models;

namespace WebSite2.Controllers
{
    public class UserController : BaseController
    {
        private readonly UserService _userService;
        private readonly RoleService _roleService;
        private readonly DepartmentService _departmentService;

        public UserController(UserService userService, RoleService roleService, DepartmentService departmentService)
        {
            _userService = userService;
            _roleService = roleService;
            _departmentService = departmentService;
        }

        //[MenuHightlight(CurrentItem = "user_tt", CurrentParentItem = "user_home")]
        //[CustomerAuthorize]
        public ActionResult Index()
        {
            var userListViewModel = new UserListViewModel();
            var expression = new[]
            {
                new ExpressionCriteria{ PropertyName = "USER_INFO_ISDEL", Value = 0, Operate = Operator.Equal },
 
            };

            userListViewModel.UserContracts = _userService.Get(expression);
            return View(userListViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult List(int page, int rows)
        {
            var columns = Request.Params["columns"].Split(',');
            var searchString = Request.Params["searchString"];
            var userName = CurrentUser.UserName;
            var rs = _userService.Get(userName, searchString, columns, page, rows, Request.Params["sort"], Request.Params["order"]);
            var data = new Dictionary<string, object>()
            {
                {"rows", rs.Items},
                {"total", rs.TotalCount}
            };
            return Json(data);
        }

        //[MenuHightlight(CurrentItem = "user_tt", CurrentParentItem = "user_home")]
        public ActionResult Create()
        {
            var roleExpression = new[]
            {
                new ExpressionCriteria{ PropertyName = "IsDel", Value = 0, Operate = Operator.Equal },
            };
            var roles = _roleService.Get(roleExpression, properties: new[] { "Id", "Name" }).Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString(),
            }).ToList();
            //var roles = _roleService.GetByUser(CurrentUser.UserName, properties: new[] { "Id", "Name" }).Select(p => new SelectListItem
            //{
            //    Text = p.Name,
            //    Value = p.Id.ToString(),
            //}).ToList();
            ViewData["Roles"] = roles;


            var deptExpression = new[]
            {
                new ExpressionCriteria{ PropertyName = "DEPT_INFO_ISDEL", Value = 0, Operate = Operator.Equal },
            };

            var departs = _departmentService.Get(deptExpression, properties: new[] {"Id", "Name"}).Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString(),
                }).ToList();
            //var departs = _departmentService.GetByUser(CurrentUser.UserName, properties: new[] { "Id", "Name" }).Select(p => new SelectListItem
            //{
            //    Text = p.Name,
            //    Value = p.Id.ToString(),
            //}).ToList();
            ViewData["Departments"] = departs;

            var item = new UserContract()
            {
                Id = Guid.NewGuid(),
                CreatedBy = CurrentUser.UserName
            };
            return View(new UserViewModel(item));
        }


        //[MenuHightlight(CurrentItem = "user_tt", CurrentParentItem = "user_home")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserViewModel viewModel)
        {
            var roleExpression = new[]
            {
                new ExpressionCriteria{ PropertyName = "IsDel", Value = 0, Operate = Operator.Equal },
            };
            var roles = _roleService.Get(roleExpression, properties: new[] { "Id", "Name" }).Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString(),
            }).ToList(); ;
            //var roles = _roleService.GetByUser(CurrentUser.UserName, properties: new[] { "Id", "Name" }).Select(p => new SelectListItem
            //{
            //    Text = p.Name,
            //    Value = p.Id.ToString(),
            //}).ToList();
            ViewData["Roles"] = roles;


            var deptExpression = new[]
            {
                new ExpressionCriteria{ PropertyName = "DEPT_INFO_ISDEL", Value = 0, Operate = Operator.Equal },
            };

            var departs = _departmentService.Get(deptExpression, properties: new[] { "Id", "Name" }).Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString(),
            }).ToList(); 
            //var departs = _departmentService.GetByUser(CurrentUser.UserName, properties: new[] { "Id", "Name" }).Select(p => new SelectListItem
            //{
            //    Text = p.Name,
            //    Value = p.Id.ToString(),
            //}).ToList();
            ViewData["Departments"] = departs;

            if (viewModel.UserContract.DepId == default(Guid))
            {
                ModelState.AddModelError("", "请选择部门");

            }
            else if (viewModel.UserContract.RoleId == default(Guid))
            {
                ModelState.AddModelError("", "请选择角色");

            }
            else if (viewModel.UserContract.IsValid())
            {

                var rs = _userService.Create(viewModel.UserContract);
                if (rs != ResultStatus.Success)
                {
                    ModelState.AddModelError("", rs == ResultStatus.Duplicate ? "记录已经存在" : "添加失败");
                    return View(new UserViewModel(viewModel.UserContract));
                }
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", viewModel.UserContract.ErrorMessage);
            }

            return View(new UserViewModel(viewModel.UserContract));
        }

        //[MenuHightlight(CurrentItem = "user_tt", CurrentParentItem = "user_home")]
        public ActionResult Update(Guid id)
        {
            var userInfo = _userService.GetByUniqueId(id.ToString());

            var roleExpression = new[]
            {
                new ExpressionCriteria{ PropertyName = "IsDel", Value = 0, Operate = Operator.Equal },
            };
            var roles = _roleService.Get(roleExpression, properties: new[] { "Id", "Name" }).Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString(),
            }).ToList();
            //var roles = _roleService.GetByUser(CurrentUser.UserName, properties: new[] { "Id", "Name" }).Select(p => new SelectListItem
            //{
            //    Text = p.Name,
            //    Value = p.Id.ToString(),
            //}).ToList();
            ViewData["Roles"] = roles;
            ViewData["RoleItem"] = userInfo.RoleId;

            var deptExpression = new[]
            {
                new ExpressionCriteria{ PropertyName = "DEPT_INFO_ISDEL", Value = 0, Operate = Operator.Equal },
            };

            var departs = _departmentService.Get(deptExpression, properties: new[] { "Id", "Name" }).Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString(),
            }).ToList(); 
            //var departs = _departmentService.GetByUser(CurrentUser.UserName, properties: new[] { "Id", "Name" }).Select(p => new SelectListItem
            //{
            //    Text = p.Name,
            //    Value = p.Id.ToString(),
            //}).ToList();
            ViewData["Departments"] = departs;
            ViewData["DepartItem"] = userInfo.DepId;

            if (userInfo != null)
            {
                return View(new UserViewModel(userInfo));
            }
            throw new ItemNotExistException("指定用户不存在!");
        }

        //[MenuHightlight(CurrentItem = "user_tt", CurrentParentItem = "user_home")]
        [HttpPost]
        public ActionResult Update(UserViewModel viewModel)
        {
            if (viewModel.UserContract.DepId == default(Guid))
            {
                ModelState.AddModelError("", "请选择部门");
            }
            else if (viewModel.UserContract.RoleId == default(Guid))
            {
                ModelState.AddModelError("", "请选择角色");
            }
            else if (viewModel.UserContract.IsValid())
            {
                var status = _userService.Update(viewModel.UserContract);
                switch (status)
                {
                    case ResultStatus.Success:
                        return RedirectToAction("Index");
                    case ResultStatus.Failed:
                        ModelState.AddModelError("", string.Format("修改用户信息{0}失败", viewModel.UserContract.NickName));
                        break;
                }
            }

            var roleExpression = new[]
            {
                new ExpressionCriteria{ PropertyName = "IsDel", Value = 0, Operate = Operator.Equal },
            };
            var roles = _roleService.Get(roleExpression, properties: new[] { "Id", "Name" }).Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString(),
            }).ToList();
            //var roles = _roleService.GetByUser(CurrentUser.UserName, properties: new[] { "Id", "Name" }).Select(p => new SelectListItem
            //{
            //    Text = p.Name,
            //    Value = p.Id.ToString(),
            //}).ToList();
            ViewData["Roles"] = roles;
            ViewData["RoleItem"] = viewModel.UserContract.RoleId;

            var deptExpression = new[]
            {
                new ExpressionCriteria{ PropertyName = "DEPT_INFO_ISDEL", Value = 0, Operate = Operator.Equal },
            };

            var departs = _departmentService.Get(deptExpression, properties: new[] { "Id", "Name" }).Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString(),
            }).ToList(); 
            //var departs = _departmentService.GetByUser(CurrentUser.UserName, properties: new[] { "Id", "Name" }).Select(p => new SelectListItem
            //{
            //    Text = p.Name,
            //    Value = p.Id.ToString(),
            //}).ToList();
            ViewData["Departments"] = departs;
            ViewData["DepartItem"] = viewModel.UserContract.DepId;

            ModelState.AddModelError("", viewModel.UserContract.ErrorMessage);

            var userInfo = _userService.GetByUniqueId(viewModel.UserContract.Id.ToString());

            if (userInfo != null)
            {
                return View(new UserViewModel(userInfo));
            }
            throw new ItemNotExistException("指定用户不存在!");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Delete(Guid id)
        {
            var rs = _userService.SetDelete(id);
            return Json(rs);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SetToH(Guid id)
        {
            var rs = _userService.SetStatus(id, "1");
            return Json(rs);
        }


        //[MenuHightlight(CurrentItem = "user_infozx")]
        public ActionResult UserInformation()
        {
            var user = _userService.GetUserInfoById(CurrentUser.Id);
            return View(user);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ChangePassword(string username, string oldpwd, string newpwd)
        {
            var rs = _userService.ChangePassword(username, oldpwd, newpwd);
            return Json(rs);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ChangeNickNmae(string username, string newvalue)
        {
            var rs = _userService.UpdateNickName(username, newvalue);
            return Json(rs);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ChangeTelPhone(string username, string newvalue)
        {
            var rs = _userService.UpdateTel(username, newvalue);
            return Json(rs);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ChangeEmail(string username, string newvalue)
        {
            var rs = _userService.UpdateEmail(username, newvalue);
            return Json(rs);
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult ListBlack(int page, int rows)
        {
            var columns = Request.Params["columns"].Split(',');
            var searchString = Request.Params["searchString"];
            var userType = Request.Params["userType"];
            var userName = CurrentUser.UserName;
            var rs = _userService.GetHList(userName, searchString, int.Parse(userType), columns, page, rows, Request.Params["sort"], Request.Params["order"]);
            var data = new Dictionary<string, object>()
            {
                {"rows", rs.Items},
                {"total", rs.TotalCount}
            };
            return Json(data);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RemoveH(Guid id)
        {
            var rs = _userService.SetStatus(id, "0");
            return Json(rs);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ListAll(int page, int rows)
        {
            var columns = Request.Params["columns"].Split(',');
            var searchString = Request.Params["searchString"];
            var userType = Request.Params["userType"];
            var userName = CurrentUser.UserName;
            var rs = _userService.GetAllUserList(userName, searchString, int.Parse(userType), columns, page, rows, Request.Params["sort"], Request.Params["order"]);
            var data = new Dictionary<string, object>()
            {
                {"rows", rs.Items},
                {"total", rs.TotalCount}
            };
            return Json(data);
        }

    }
}