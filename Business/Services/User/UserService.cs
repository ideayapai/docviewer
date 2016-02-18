using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrasturcture.Cryptographer;
using Infrasturcture.QueryableExtension;
using Repository;
using Repository.Business;
using Services.Common;
using Services.Contracts;
using Services.Enums;

namespace Services.User
{
    public class UserService : IDataAuthorizeByDepServiceProvider<UserContract>, IServiceProvider<UserContract>
    {
        private readonly IBaseRepository<BASE_USER_INFO> _userRepository;
        private readonly IBaseRepository<V_USER_DEPT> _userDepRepository;
        private readonly IBaseRepository<V_USER_ROLE_DEPT> _userRoleDepartmentViewRepository;
        private readonly IDataAuthorizeByDepRepository<V_USER_ROLE_DEPT> _userRoleDepartmentViewAuthorizeRepository;

        public UserService(IBaseRepository<BASE_USER_INFO> userRepository, IBaseRepository<V_USER_DEPT> userDepRepository, IBaseRepository<V_USER_ROLE_DEPT> userRoleDepartmentViewRepository, IDataAuthorizeByDepRepository<V_USER_ROLE_DEPT> userRoleDepartmentViewAuthorizeRepository)
        {
            _userRepository = userRepository;
            _userDepRepository = userDepRepository;
            _userRoleDepartmentViewRepository = userRoleDepartmentViewRepository;
            _userRoleDepartmentViewAuthorizeRepository = userRoleDepartmentViewAuthorizeRepository;
        }

        public UserContract Validate(string userName, string password)
        {
            var user = _userRoleDepartmentViewRepository.Get(p => p.USER_INFO_ISDEL == 0 && p.DEPT_INFO_ISDEL == 0
                && p.USER_INFO_NAME == userName.ToLower());
            if (user != null)
            {
                var rs = Md5Cryptographer.CompareHash(password, user.USER_INFO_PWD);
                if (rs)
                {
                    var records = user.ToUserContract();
                    records.Token = Md5Cryptographer.GetGuidHash();
                    return records;
                }
            }
            return new EmptyUserContract() { ErrorMessage = "用户名或密码错误！" };
        }


        public ResultStatus Create(UserContract user)
        {
            if (user.IsValid())
            {
                var count = _userRepository.Count(p => p.USER_INFO_ISDEL == 0 && p.USER_INFO_NAME == user.UserName.ToLower());

                if (count == 0)
                {
                    user.Password = Md5Cryptographer.CreateHash(user.Password);
                    user.CreatedDate = DateTime.Now;
                    var entity = user.ToEntity();

                    _userRepository.Add(entity);
                    user.Id = entity.USER_INFO_ID;
                    return ResultStatus.Success;
                }
                return ResultStatus.Duplicate;

            }
            return ResultStatus.Failed;
        }

        public ResultStatus Update(UserContract user)
        {
            if (user.IsValid())
            {
                _userRepository.Update(u => u.USER_INFO_ID == user.Id, (u) =>
                                                                 {
                                                                     u.DEPID = user.DepId;
                                                                     u.USER_INFO_CREATEDDATE = DateTime.Now;
                                                                     u.USER_INFO_NICKNAME = user.NickName;
                                                                     u.ROLE_INFO_ID = user.RoleId;
                                                                     u.USER_INFO_TEL = user.UserInfoTel;
                                                                     u.USER_INFO_EMAIL = user.UserInfoEmail;
                                                                     u.USER_INFO_PHOTO = user.UserInfoPhoto;
                                                                 });
                return ResultStatus.Success;
            }

            return ResultStatus.Failed;
        }

        public bool IsExist(string userName)
        {
            return _userRepository.Count(p => p.USER_INFO_ISDEL == 0 && p.USER_INFO_NAME == userName.ToLower()) != 0;
        }


        public List<ShortUserContract> Get(string userName)
        {
            return _userDepRepository.GetAll(p => p.USER_INFO_ISDEL == 0 && p.DEPT_INFO_ISDEL == 0 && (p.USER_INFO_NICKNAME.Contains(userName) || p.USER_INFO_NAME.Contains(userName))).ConvertAll(p => p.ToShortContract());
        }

        public List<ShortUserContract> GetApprovalUser(Guid roleid, string areocode, string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return _userDepRepository.GetAll(p => p.GEO_INFO_CODE == areocode && p.ROLE_INFO_ID == roleid).ConvertAll(p => p.ToShortContract());
            }
            return _userDepRepository.GetAll(p => p.GEO_INFO_CODE == areocode && p.ROLE_INFO_ID == roleid && (p.USER_INFO_NICKNAME.Contains(search) || p.DEPT_INFO_NAME.Contains(search))).ConvertAll(p => p.ToShortContract());

        }

        public ShortUserContract GetUserById(Guid id)
        {
            return _userDepRepository.Get(p => p.USER_INFO_ID == id).ToShortContract();
        }

        public ShortUserContract GetDepartment(string userName)
        {
            return _userDepRepository.Get(p => p.DEPT_INFO_ISDEL == 0 && p.USER_INFO_NAME == userName).ToShortContract();
        }


        public List<ShortUserContract> Get(string userName, string[] cateogy)
        {
            return
                _userRoleDepartmentViewRepository.GetAll(
                    p => p.USER_INFO_ISDEL == 0 && p.DEPT_INFO_ISDEL == 0 && (p.USER_INFO_NICKNAME.Contains(userName) || p.USER_INFO_NAME.Contains(userName))).ConvertAll(p => p.ToShortContract());
        }

        public UserContract GetByUserName(string userName)
        {
            var user = _userRepository.Get(p => p.USER_INFO_ISDEL == 0 && p.USER_INFO_NAME == userName);
            if (user != null)
                return user.ToContract();
            return null;
        }

        public UserContract GetByUniqueId(string uniqueId)
        {
            var user = _userRepository.Get(p => p.USER_INFO_ISDEL == 0 && p.USER_INFO_ID.ToString() == uniqueId);
            if (user != null)
                return user.ToContract();
            return null;
        }

        public UserContract GetUser(Guid id)
        {
            var user = _userRepository.Get(p => p.USER_INFO_ISDEL == 0 && p.USER_INFO_ID == id);
            return user != null ? user.ToContract() : null;
        }



        public int Count(ExpressionCriteriaBase[] criteria)
        {
            Expression<Func<V_USER_ROLE_DEPT, bool>> condition = ExpressionUtils.BuildCondition<V_USER_ROLE_DEPT>(criteria);
            return _userRoleDepartmentViewRepository.Count(condition);
        }

        public bool Delete(List<Guid> ids)
        {
            if (ids.Count > 0)
            {
                return _userRepository.Delete(p => ids.Contains(p.USER_INFO_ID));
            }
            return false;
        }

        public bool Delete(Guid id)
        {
            return _userRepository.Delete(p => id == p.USER_INFO_ID);
        }
        public bool SetDelete(Guid id)
        {
            BASE_USER_INFO user = _userRepository.Update(u => u.USER_INFO_ID == id, (u) =>
            {
                u.USER_INFO_ISDEL = 1;
            });
            if (user != null)

                return true;
            else
                return false;
        }

        public bool SetStatus(Guid id, string status)
        {
            BASE_USER_INFO user = _userRepository.Update(u => u.USER_INFO_ID == id, (u) =>
            {
                u.USER_INFO_STATE = (status);
            });
            if (user != null)

                return true;
            else
                return false;
        }

        public bool SetStatus(string status)
        {
            List<BASE_USER_INFO> user = null;
            if (status == "0")
            {
                user = _userRepository.UpdateAll(u => u.USER_INFO_STATE == "1", (u) =>
            {
                u.USER_INFO_STATE = (status);
            });
            }
            if (status == "1")
            {
                user = _userRepository.UpdateAll(u => u.USER_INFO_STATE == "0", (u) =>
            {
                u.USER_INFO_STATE = (status);
            });
            }
            if (user != null && user.Count > 0)

                return true;
            else
                return false;
        }

        public bool IsExist(Guid id, string name)
        {
            return _userRepository.Count(p => p.USER_INFO_ISDEL == 0 && p.DEPID != id && p.USER_INFO_NAME == name) > 0;
        }

        public PaginationList<UserContract> Get(ExpressionCriteriaBase[] criteria, int page, int pageSize, string sort, string orderBy = "desc")
        {
            Expression<Func<V_USER_ROLE_DEPT, bool>> condition = ExpressionUtils.BuildCondition<V_USER_ROLE_DEPT>(criteria);
            var sortMap = QueryableBuilderExtensions.GetMappingPropertyName<UserContract, V_USER_ROLE_DEPT>(sort);

            var pager = _userRoleDepartmentViewRepository.GetPager(condition, sortMap ?? "USER_INFO_CREATEDDATE", orderBy ?? "desc", Math.Max(page - 1, 0) * pageSize, pageSize);
            return new PaginationList<UserContract>(pager.Items.ConvertAll(p => p.ToUserContract()))
            {
                TotalCount = pager.TotalCount,
                PageSize = pageSize
            };
        }

        public PaginationList<UserContract> Get(ExpressionCriteriaBase[] criteria, string[] properties, int page, int pageSize, string sort, string orderBy = "desc")
        {
            Expression<Func<V_USER_ROLE_DEPT, bool>> condition = ExpressionUtils.BuildCondition<V_USER_ROLE_DEPT>(criteria);
            var sortMap = QueryableBuilderExtensions.GetMappingPropertyName<UserContract, V_USER_ROLE_DEPT>(sort);
            if (properties != null && properties.Length > 0)
            {
                Expression<Func<V_USER_ROLE_DEPT, UserContract>> selector =
                    QueryableBuilderExtensions.BuildSelectorClause<V_USER_ROLE_DEPT, UserContract>(properties);
                return _userRoleDepartmentViewRepository.GetPager(selector, condition, sortMap ?? "USER_INFO_CREATEDDATE", orderBy ?? "desc",
                    Math.Max(page - 1, 0) * pageSize, pageSize);
            }
            var pager = _userRoleDepartmentViewRepository.GetPager(condition, sortMap ?? "USER_INFO_CREATEDDATE", orderBy ?? "desc", Math.Max(page - 1, 0) * pageSize, pageSize);

            return new PaginationList<UserContract>(pager.Items.ConvertAll(p => p.ToUserContract()))
            {
                TotalCount = pager.TotalCount,
                PageSize = pageSize
            };
        }
        /// <summary>
        /// 正常用户列表
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="searchString"></param>
        /// <param name="properties"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public PaginationList<UserContract> Get(string userName, string searchString, string[] properties, int page, int pageSize, string sort, string orderBy = "desc")
        {
            Expression<Func<V_USER_ROLE_DEPT, bool>> condition = p => p.USER_INFO_ISDEL == 0 && p.DEPT_INFO_ISDEL == 0 && p.USER_INFO_ISSUP == 0
                && p.DEPT_INFO_ISSUP == 0 && p.USER_INFO_STATE == "0";

            if (!string.IsNullOrEmpty(searchString))
            {
                condition = (p => p.USER_INFO_ISDEL == 0 && p.DEPT_INFO_ISDEL == 0 && p.USER_INFO_ISSUP == 0 && p.DEPT_INFO_ISSUP == 0 && p.USER_INFO_STATE == "0"
                    && (p.USER_INFO_NICKNAME.Contains(searchString) || p.DEPT_INFO_NAME.Contains(searchString) || p.ROLE_INFO_NAME.Contains(searchString)
                    || p.USER_INFO_NAME.Contains(searchString)));
            }

            var sortMap = QueryableBuilderExtensions.GetMappingPropertyName<UserContract, V_USER_ROLE_DEPT>(sort);
            if (properties != null && properties.Length > 0)
            {
                var selector = QueryableBuilderExtensions.BuildSelectorClause<V_USER_ROLE_DEPT, UserContract>(properties);
                return _userRoleDepartmentViewAuthorizeRepository.GetPagerByUser(userName, selector, condition, sortMap ?? "USER_INFO_CREATEDDATE", orderBy ?? "desc", Math.Max(page - 1, 0) * pageSize, pageSize);
            }
            var pager = _userRoleDepartmentViewAuthorizeRepository.GetPagerByUser(userName, condition, sortMap ?? "USER_INFO_CREATEDDATE", orderBy ?? "desc", Math.Max(page - 1, 0) * pageSize, pageSize);

            return new PaginationList<UserContract>(pager.Items.ConvertAll(p => p.ToUserContract()))
            {
                TotalCount = pager.TotalCount,
                PageSize = pageSize
            };
        }
        /// <summary>
        /// 所有正常用户列表
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="searchString"></param>
        /// <param name="properties"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public PaginationList<UserContract> GetAllUserList(string userName, string searchString, int isSup, string[] properties, int page, int pageSize, string sort, string orderBy = "desc")
        {
            Expression<Func<V_USER_ROLE_DEPT, bool>> condition = p => p.USER_INFO_ISDEL == 0 && p.DEPT_INFO_ISDEL == 0 && p.USER_INFO_ISSUP == isSup
                && p.DEPT_INFO_ISSUP == isSup && p.USER_INFO_STATE == "0";

            if (!string.IsNullOrEmpty(searchString))
            {
                condition = (p => p.USER_INFO_ISDEL == 0 && p.DEPT_INFO_ISDEL == 0 && p.USER_INFO_ISSUP == isSup && p.DEPT_INFO_ISSUP == isSup && p.USER_INFO_STATE == "0"
                    && (p.USER_INFO_NICKNAME.Contains(searchString) || p.DEPT_INFO_NAME.Contains(searchString) || p.ROLE_INFO_NAME.Contains(searchString)
                    || p.USER_INFO_NAME.Contains(searchString)));
            }

            var sortMap = QueryableBuilderExtensions.GetMappingPropertyName<UserContract, V_USER_ROLE_DEPT>(sort);
            if (properties != null && properties.Length > 0)
            {
                var selector = QueryableBuilderExtensions.BuildSelectorClause<V_USER_ROLE_DEPT, UserContract>(properties);
                return _userRoleDepartmentViewAuthorizeRepository.GetPagerByUser(userName, selector, condition, sortMap ?? "USER_INFO_CREATEDDATE", orderBy ?? "desc", Math.Max(page - 1, 0) * pageSize, pageSize);
            }
            var pager = _userRoleDepartmentViewAuthorizeRepository.GetPagerByUser(userName, condition, sortMap ?? "USER_INFO_CREATEDDATE", orderBy ?? "desc", Math.Max(page - 1, 0) * pageSize, pageSize);

            return new PaginationList<UserContract>(pager.Items.ConvertAll(p => p.ToUserContract()))
            {
                TotalCount = pager.TotalCount,
                PageSize = pageSize
            };
        }
        /// <summary>
        /// 用户黑名单
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="searchString"></param>
        /// <param name="properties"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public PaginationList<UserContract> GetHList(string userName, string searchString, int isSup, string[] properties, int page, int pageSize, string sort, string orderBy = "desc")
        {
            Expression<Func<V_USER_ROLE_DEPT, bool>> condition = p => p.USER_INFO_ISDEL == 0 && p.DEPT_INFO_ISDEL == 0 && p.USER_INFO_ISSUP == isSup
                && p.DEPT_INFO_ISSUP == isSup && p.USER_INFO_STATE == "1";

            if (!string.IsNullOrEmpty(searchString))
            {
                condition = (p => p.USER_INFO_ISDEL == 0 && p.DEPT_INFO_ISDEL == 0 && p.USER_INFO_ISSUP == isSup && p.DEPT_INFO_ISSUP == isSup && p.USER_INFO_STATE == "1"
                    && (p.USER_INFO_NICKNAME.Contains(searchString) || p.DEPT_INFO_NAME.Contains(searchString)
                    || p.USER_INFO_NAME.Contains(searchString)));
            }

            var sortMap = QueryableBuilderExtensions.GetMappingPropertyName<UserContract, V_USER_ROLE_DEPT>(sort);
            if (properties != null && properties.Length > 0)
            {
                var selector = QueryableBuilderExtensions.BuildSelectorClause<V_USER_ROLE_DEPT, UserContract>(properties);
                return _userRoleDepartmentViewAuthorizeRepository.GetPagerByUser(userName, selector, condition, sortMap ?? "USER_INFO_CREATEDDATE", orderBy ?? "desc", Math.Max(page - 1, 0) * pageSize, pageSize);
            }
            var pager = _userRoleDepartmentViewAuthorizeRepository.GetPagerByUser(userName, condition, sortMap ?? "USER_INFO_CREATEDDATE", orderBy ?? "desc", Math.Max(page - 1, 0) * pageSize, pageSize);

            return new PaginationList<UserContract>(pager.Items.ConvertAll(p => p.ToUserContract()))
            {
                TotalCount = pager.TotalCount,
                PageSize = pageSize
            };
        }
        /// <summary>
        /// 供应商账号分页列表
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="properties"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public PaginationList<UserContract> GetSup(string searchString, string[] properties, int page, int pageSize, string sort, string orderBy = "desc")
        {
            Expression<Func<V_USER_ROLE_DEPT, bool>> condition = p => p.USER_INFO_ISDEL == 0 && p.DEPT_INFO_ISDEL == 0
                && p.DEPT_INFO_ISSUP == 1 && p.USER_INFO_ISSUP == 1 && p.USER_INFO_STATE == "0";

            if (!string.IsNullOrEmpty(searchString))
            {
                condition = (p => p.USER_INFO_ISDEL == 0 && p.DEPT_INFO_ISSUP == 1 && p.USER_INFO_ISSUP == 1
                    && p.DEPT_INFO_ISDEL == 0 && p.USER_INFO_STATE == "0" && (p.USER_INFO_NICKNAME.Contains(searchString) || p.DEPT_INFO_NAME.Contains(searchString)));
            }

            var sortMap = QueryableBuilderExtensions.GetMappingPropertyName<UserContract, V_USER_ROLE_DEPT>(sort);
            if (properties != null && properties.Length > 0)
            {
                var selector = QueryableBuilderExtensions.BuildSelectorClause<V_USER_ROLE_DEPT, UserContract>(properties);
                return _userRoleDepartmentViewRepository.GetPager(selector, condition, sortMap ?? "USER_INFO_CREATEDDATE", orderBy ?? "desc", Math.Max(page - 1, 0) * pageSize, pageSize);
            }
            var pager = _userRoleDepartmentViewRepository.GetPager(condition, sortMap ?? "USER_INFO_CREATEDDATE", orderBy ?? "desc", Math.Max(page - 1, 0) * pageSize, pageSize);

            return new PaginationList<UserContract>(pager.Items.ConvertAll(p => p.ToUserContract()))
            {
                TotalCount = pager.TotalCount,
                PageSize = pageSize
            };
        }

        public UserContract Get(Guid id)
        {
            var user = _userRoleDepartmentViewRepository.Get(p => p.USER_INFO_ISDEL == 0 && p.DEPT_INFO_ISDEL == 0 && p.USER_INFO_ID == id);
            if (user != null)
                return user.ToUserContract();
            return null;
        }

        public List<UserContract> Get(ExpressionCriteriaBase[] criteria = null, string[] properties = null)
        {
            Expression<Func<V_USER_ROLE_DEPT, bool>> condition = ExpressionUtils.BuildCondition<V_USER_ROLE_DEPT>(criteria);
            if (properties != null && properties.Length > 0)
            {
                Expression<Func<V_USER_ROLE_DEPT, UserContract>> selector =
                    QueryableBuilderExtensions.BuildSelectorClause<V_USER_ROLE_DEPT, UserContract>(properties);
                return _userRoleDepartmentViewRepository.Get(selector, condition);
            }
            return _userRoleDepartmentViewRepository.GetAll(condition, p => p.USER_INFO_ID).ConvertAll(p => p.ToUserContract());

        }

        //public bool ChangePassword(string userName, string oldPassword, string newPassword)
        //{
        //    if (String.IsNullOrEmpty(userName)) throw new ArgumentException("值不能为 null 或为空。", "userName");
        //    if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("值不能为 null 或为空。", "oldPassword");
        //    if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("值不能为 null 或为空。", "newPassword");

        //    try
        //    {
        //        var currentUser = Validate(userName, oldPassword);
        //        if (currentUser is EmptyUserContract)
        //        {
        //            return false;
        //        }
        //        _userRepository.Update(p => p.USER_INFO_NAME == userName,
        //                               (u) => u.USER_INFO_PWD = Md5Cryptographer.CreateHash(newPassword));
        //        return true;
        //    }
        //    catch (ArgumentException)
        //    {
        //        return false;
        //    }
        //    catch (Exception e)
        //    {
        //        return false;
        //    }
        //}

        public PaginationList<UserContract> Get(string userName, ExpressionCriteriaBase[] criteria, int page, int pageSize, string sort,
            string orderBy = "desc")
        {
            Expression<Func<V_USER_ROLE_DEPT, bool>> condition = ExpressionUtils.BuildCondition<V_USER_ROLE_DEPT>(criteria);
            var sortMap = QueryableBuilderExtensions.GetMappingPropertyName<UserContract, V_USER_ROLE_DEPT>(sort);

            var pager = _userRoleDepartmentViewAuthorizeRepository.GetPagerByUser(userName, condition, sortMap ?? "USER_INFO_CREATEDDATE", orderBy ?? "desc", Math.Max(page - 1, 0) * pageSize, pageSize);
            return new PaginationList<UserContract>(pager.Items.ConvertAll(p => p.ToUserContract()))
            {
                TotalCount = pager.TotalCount,
                PageSize = pageSize
            };
        }

        public PaginationList<UserContract> Get(string userName, ExpressionCriteriaBase[] criteria, string[] properties, int page, int pageSize,
            string sort, string orderBy = "desc")
        {
            Expression<Func<V_USER_ROLE_DEPT, bool>> condition = ExpressionUtils.BuildCondition<V_USER_ROLE_DEPT>(criteria);
            var sortMap = QueryableBuilderExtensions.GetMappingPropertyName<UserContract, V_USER_ROLE_DEPT>(sort);
            if (properties != null && properties.Length > 0)
            {
                Expression<Func<V_USER_ROLE_DEPT, UserContract>> selector =
                    QueryableBuilderExtensions.BuildSelectorClause<V_USER_ROLE_DEPT, UserContract>(properties);
                return _userRoleDepartmentViewAuthorizeRepository.GetPagerByUser(userName, selector, condition, sortMap ?? "USER_INFO_CREATEDDATE", orderBy ?? "desc",
                    Math.Max(page - 1, 0) * pageSize, pageSize);
            }
            var pager = _userRoleDepartmentViewAuthorizeRepository.GetPagerByUser(userName, condition, sortMap ?? "USER_INFO_CREATEDDATE", orderBy ?? "desc", Math.Max(page - 1, 0) * pageSize, pageSize);

            return new PaginationList<UserContract>(pager.Items.ConvertAll(p => p.ToUserContract()))
            {
                TotalCount = pager.TotalCount,
                PageSize = pageSize
            };
        }

        public List<UserContract> Get(string userName, ExpressionCriteriaBase[] criteria = null, string[] properties = null)
        {
            Expression<Func<V_USER_ROLE_DEPT, bool>> condition = ExpressionUtils.BuildCondition<V_USER_ROLE_DEPT>(criteria);
            if (properties != null && properties.Length > 0)
            {
                Expression<Func<V_USER_ROLE_DEPT, UserContract>> selector =
                    QueryableBuilderExtensions.BuildSelectorClause<V_USER_ROLE_DEPT, UserContract>(properties);
                return _userRoleDepartmentViewRepository.Get(selector, condition).Where(p => p.CreatedBy == userName).ToList();
            }
            return _userRoleDepartmentViewRepository.GetAll(condition, p => p.USER_INFO_ID).Where(p => p.USER_INFO_CREATEDBY == userName).ToList().ConvertAll(p => p.ToUserContract());
        }

        public int Count(string userName, ExpressionCriteriaBase[] criteria)
        {
            Expression<Func<V_USER_ROLE_DEPT, bool>> condition = ExpressionUtils.BuildCondition<V_USER_ROLE_DEPT>(criteria);
            return _userRoleDepartmentViewAuthorizeRepository.CountByUser(userName, condition);
        }
        /// <summary>
        /// 根据ID获取用户视图信息
        /// </summary>
        /// <returns></returns>
        public UserContract GetUserInfoById(Guid userid)
        {
            var user = _userRoleDepartmentViewRepository.Get(p => p.USER_INFO_ID == userid).ToUserContract();
            return user;
        }
        /// <summary>
        /// 手持端登录
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public UserInfoContract LogIn(string username, string pwd)
        {
            var user = _userRoleDepartmentViewRepository.Get(p => p.USER_INFO_ISDEL == 0 && p.DEPT_INFO_ISDEL == 0 && p.USER_INFO_NAME == username.ToLower());
            if (user == null) return null;
            var rs = Md5Cryptographer.CompareHash(pwd, user.USER_INFO_PWD);
            if (!rs) return null;
            var records = user.ToUserInfoContract();

            return records;
        }

        public List<UserInfoContract> UserSynchronization()
        {
            var users = _userRoleDepartmentViewRepository.GetAll(p => p.USER_INFO_ISDEL == 0 && p.DEPT_INFO_ISDEL == 0);
            if (users == null) return null;
            return users.ConvertAll(p => p.ToUserInfoContract());
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("值不能为 null 或为空。", "userName");
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("值不能为 null 或为空。", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("值不能为 null 或为空。", "newPassword");

            try
            {
                var currentUser = Validate(userName, oldPassword);
                if (currentUser is EmptyUserContract)
                {
                    return false;
                }
                _userRepository.Update(p => p.USER_INFO_NAME == userName,
                                       (u) => u.USER_INFO_PWD = Md5Cryptographer.CreateHash(newPassword));
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool UpdateNickName(string username, string nickname)
        {
            BASE_USER_INFO user = _userRepository.Update(u => u.USER_INFO_NAME == username, (u) =>
            {
                u.USER_INFO_NICKNAME = nickname;
            });
            if (user != null)
                return true;
            else
                return false;
        }

        public bool UpdateTel(string username, string telphone)
        {
            BASE_USER_INFO user = _userRepository.Update(u => u.USER_INFO_NAME == username, (u) =>
            {
                u.USER_INFO_TEL = telphone;
            });
            if (user != null)
                return true;
            else
                return false;
        }
        public bool UpdateEmail(string username, string email)
        {
            BASE_USER_INFO user = _userRepository.Update(u => u.USER_INFO_NAME == username, (u) =>
            {
                u.USER_INFO_EMAIL = email;
            });
            if (user != null)
                return true;
            else
                return false;
        }

    }
}
