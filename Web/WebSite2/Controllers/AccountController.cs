using System;
using System.Collections.Generic;
using System.Configuration;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Common.Logging;
using Infrasturcture.MD5;
using Infrasturcture.Web;
using Microsoft.Web.WebPages.OAuth;
using Services.Context;
using Services.Spaces;
using SSOLib;
using SSOLib.Service;
using WebMatrix.WebData;
using WebSite2.Models;

namespace WebSite2.Controllers
{
    public class AccountController : BaseController
    {
        private readonly SpaceTreeService _spaceService;
        private readonly ContextService _contextService;
        private readonly IFormsAuthentication _formsAuthentication;

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public AccountController(SpaceTreeService spaceService, ContextService contextService, IFormsAuthentication formsAuthentication)
        {
            _spaceService = spaceService;
            _contextService = contextService;
            _formsAuthentication = formsAuthentication;
        }

        //
        // GET: /Account/Login
        //[AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {

            _logger.Info("登录页面：访问IP：" + CerCommon.GetIp());

            if (CurrentUser != null && !(CurrentUser is EmptyUserContract))
            {
                _formsAuthentication.SetAuthCookie(CurrentUser.UserName, false);
                _contextService.SetCookie("role", CurrentUser.RoleId.ToString());
                _contextService.NickName = CurrentUser.NickName;
                _contextService.DepId = CurrentUser.DepId.ToString();
                _contextService.UserPhoto = ConfigurationManager.AppSettings["USER_AVATAR"] + CurrentUser.UserInfoPhoto;
                _logger.Info(CurrentUser.Id + "登录成功" + "文档管理系统");

                return Redirect("/home/index");
            }
            var model = new LoginModel();
            //读取保存的Cookie信息
            HttpCookie cookies = Request.Cookies["USER_COOKIE"];
            if (cookies != null && !string.IsNullOrEmpty(cookies.Value))
            {
                //如果Cookie不为空，则将Cookie里面的用户名和密码读取出来赋值给前台的文本框。
                model.UserName = Md5Util.Decrypt(cookies["UserName"]);
                model.Password = Md5Util.Decrypt(cookies["UserPassword"]);

                //这里依然把记住密码的选项给选中。
                model.RememberMe = true;
                ViewBag.ReturnUrl = returnUrl;
                if (model.AutoLogin)
                {
                    return Login(model, returnUrl);
                }

                return View(model);
            }

            //if (!string.IsNullOrEmpty(returnUrl) && returnUrl.EndsWith("/account/logoff"))
            //{
            //    returnUrl = returnUrl.Replace("/account/logoff", "/home/index");
            //}
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {

            if (string.IsNullOrEmpty(model.UserName))
            {
                ModelState.AddModelError("", "账号不能为空！");
                return View(model);
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("", "密码不能为空！");
                return View(model);
            }

            string errorMessage = string.Empty;
            var loginSuccessful = Login(model.UserName.Trim(), model.Password.Trim(), out errorMessage);
            if (loginSuccessful == null)
            {
                ModelState.AddModelError("", errorMessage);
                return View(model);
            }

            _formsAuthentication.SetAuthCookie(model.UserName, true);
            _contextService.SetCookie("role", CurrentUser.RoleId.ToString());
            _contextService.NickName = CurrentUser.NickName;
            _contextService.DepId = CurrentUser.DepId.ToString();
            _contextService.UserId = CurrentUser.Id.ToString();
            _contextService.UserPhoto = ConfigurationManager.AppSettings["USER_AVATAR"] + CurrentUser.UserInfoPhoto;
            _contextService.SpaceTreeHtml = _spaceService.GetOrSetSpaceTree(_contextService.UserId);

            HttpCookie cookie = new HttpCookie("USER_COOKIE");
            if (model.RememberMe)
            {
                //所有的验证信息检测之后，如果用户选择的记住密码，则将用户名和密码写入Cookie里面保存起来。
                cookie.Values.Add("UserName", Md5Util.Encrypt(model.UserName.Trim()));
                cookie.Values.Add("UserPassword", Md5Util.Encrypt(model.Password.Trim()));
                //这里是设置Cookie的过期时间，这里设置30天的时间，过了时间之后状态保持自动清空。
                cookie.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Add(cookie);
            }
            else
            {
                //如果用户没有选择记住密码，那么立即将Cookie里面的信息情况，并且设置状态保持立即过期。
                var httpCookie = Response.Cookies["USER_COOKIE"];
                if (httpCookie != null)
                {
                    httpCookie.Value = null;
                    httpCookie.Expires = DateTime.Now;
                }
            }

            _logger.Info("登录成功：user：" + model.UserName);

            return loginSuccessful;
        }

        //
        // POST: /Account/LogOff
        //[ValidateAntiForgeryToken]
        public ActionResult LogOut()
        {
            if (CurrentUser != null)
            {
                _logger.Info("注销退出：user：" + CurrentUser.UserName);
                //_userLogService.Log(new UserLogContract() { IpAddress = CerCommon.GetIp(), Message = "注销退出", UserId = CurrentUser.Id, FromClient = "主系统" });

            }
            //WebSecurity.Logout();
            
            HttpCookie cookie = new HttpCookie("USER_COOKIE");

            //读取保存的Cookie信息
            HttpCookie cookies = Request.Cookies["USER_COOKIE"];
            var model = new LoginModel();
            if (cookies != null && !string.IsNullOrEmpty(cookies.Value))
            {
                //如果Cookie不为空，则将Cookie里面的用户名和密码读取出来赋值给前台的文本框。
                model.UserName = Md5Util.Decrypt(cookies["UserName"]);
                model.Password = Md5Util.Decrypt(cookies["UserPassword"]);
                if (!string.IsNullOrEmpty(cookies["AutoLogin"]))
                {
                    model.AutoLogin = bool.Parse(Md5Util.Decrypt(cookies["AutoLogin"]));
                }
                //这里依然把记住密码的选项给选中。
                model.RememberMe = true;

            }
            if (model.RememberMe)
            {
                //所有的验证信息检测之后，如果用户选择的记住密码，则将用户名和密码写入Cookie里面保存起来。
                cookie.Values.Add("UserName", Md5Util.Encrypt(model.UserName.Trim()));
                cookie.Values.Add("UserPassword", Md5Util.Encrypt(model.Password.Trim()));
                cookie.Values.Add("AutoLogin", Md5Util.Encrypt(false.ToString()));
                //这里是设置Cookie的过期时间，这里设置7天的时间，过了时间之后状态保持自动清空。
                cookie.Expires = DateTime.Now.AddDays(7);
                Response.Cookies.Add(cookie);
            }

            ActionResult logOff = Logout();

            _formsAuthentication.SignOut();
            _contextService.SetCookie("role", "");
            _contextService.NickName = null;
            _contextService.DepId = string.Empty;

            return logOff;
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // 尝试注册用户
                try
                {
                    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", true);
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    WebSecurity.Login(model.UserName, model.Password, true);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            var ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // 只有在当前登录用户是所有者时才取消关联帐户
            if (ownerAccount == User.Identity.Name)
            {
                // 使用事务来防止用户删除其上次使用的登录凭据
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "已更改你的密码。"
                : message == ManageMessageId.SetPasswordSuccess ? "已设置你的密码。"
                : message == ManageMessageId.RemoveLoginSuccess ? "已删除外部登录。"
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // 在某些失败方案中，ChangePassword 将引发异常，而不是返回 false。
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "当前密码不正确或新密码无效。");
                    }
                }
            }
            else
            {
                // 用户没有本地密码，因此将删除由于缺少
                // OldPassword 字段而导致的所有验证错误
                var state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            var result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // 如果当前用户已登录，则添加新帐户
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }

            // 该用户是新用户，因此将要求该用户提供所需的成员名称
            var loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
        }

        //
        // POST: /Account/ExternalLoginConfirmation



        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            var accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            var externalLogins = new List<ExternalLogin>();
            foreach (var account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }



        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // 请参见 http://go.microsoft.com/fwlink/?LinkID=177550 以查看
            // 状态代码的完整列表。
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "用户名已存在。请输入其他用户名。";

                case MembershipCreateStatus.DuplicateEmail:
                    return "该电子邮件地址的用户名已存在。请输入其他电子邮件地址。";

                case MembershipCreateStatus.InvalidPassword:
                    return "提供的密码无效。请输入有效的密码值。";

                case MembershipCreateStatus.InvalidEmail:
                    return "提供的电子邮件地址无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidAnswer:
                    return "提供的密码取回答案无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidQuestion:
                    return "提供的密码取回问题无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidUserName:
                    return "提供的用户名无效。请检查该值并重试。";

                case MembershipCreateStatus.ProviderError:
                    return "身份验证提供程序返回了错误。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";

                case MembershipCreateStatus.UserRejected:
                    return "已取消用户创建请求。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";

                default:
                    return "发生未知错误。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";
            }
        }
    }

}
