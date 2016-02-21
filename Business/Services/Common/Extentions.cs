using System;
using Infrasturcture.Utils;
using Repository;
using Services.Contracts;

namespace Services.Common
{
    public static class Extentions
    {

        public static MenuItemContract ToContract(this V_ROLE_FUNCTION source)
        {
            if (source == null)
                throw new InvalidOperationException("source is null");

            return new MenuItemContract
            {
                RoleId = source.ROLE_INFO_ID,
                RoleName = source.ROLE_INFO_NAME,
                Name = source.FUNCTIONS_NAME,
                Action = source.FUNCTIONS_ACTION,
                MenuId = source.FUNCTIONS_ID,
                ParentMenuId = source.FUNCTIONS_PARENTID,
                //TODO:
                OrderBy = source.FUNCTIONS_ORDERBY.Value,
                Category = source.FUNCTIONS_CATEGORY,
                Attributes = JsonFormaterUtils.Deserialize<MenuItemAttributes>(source.FUNCTIONS_ATTRIBUTES) ?? new MenuItemAttributes()
            };
        }

        public static MenuItemContract ToContract(this BASE_FUNCTIONS source)
        {
            if (source == null)
                throw new InvalidOperationException("source is null");
            return new MenuItemContract
            {
                Name = source.FUNCTIONS_NAME,
                Action = source.FUNCTIONS_ACTION,
                MenuId = source.FUNCTIONS_ID,
                ParentMenuId = source.FUNCTIONS_PARENTID,
                //TODO:
                OrderBy = source.FUNCTIONS_ORDERBY.Value,
                Category = source.FUNCTIONS_CATEGORY,
                Attributes = JsonFormaterUtils.Deserialize<MenuItemAttributes>(source.FUNCTIONS_ATTRIBUTES) ?? new MenuItemAttributes()
            };
        }

        public static BASE_FUNCTIONS ToEntity(this MenuItemContract source)
        {
            if (source == null)
                throw new InvalidOperationException("source is null");
            return new BASE_FUNCTIONS
            {
                FUNCTIONS_NAME = source.Name,
                FUNCTIONS_ACTION = source.Action,
                FUNCTIONS_ID = source.MenuId,
                FUNCTIONS_PARENTID = source.ParentMenuId,
                FUNCTIONS_ORDERBY = source.OrderBy,
                FUNCTIONS_CATEGORY = source.Category,
                FUNCTIONS_ATTRIBUTES = JsonFormaterUtils.Serialize(source.Attributes)
            };
        }

        public static RoleContract ToContract(this BASE_ROLE_INFO source)
        {
            if (source == null)
                throw new InvalidOperationException("source is null");
            return new RoleContract
            {
                Id = source.ROLE_INFO_ID,
                Name = source.ROLE_INFO_NAME,
                CreatedDate = source.ROLE_INFO_CREATEDDATE ?? DateTime.MinValue,
                ApplicationId = source.GLOBAL_TYPE_ID,
                Remark = source.ROLE_INFO_REMARK,
                Type = source.ROLE_INFO_TYPE,
                Createby = source.CREATEDBY,
                IsDel = source.IsDel ?? 0
            };
        }

        public static BASE_ROLE_INFO ToEntity(this RoleContract source)
        {
            if (source == null)
                throw new InvalidOperationException("source is null");
            return new BASE_ROLE_INFO
            {
                ROLE_INFO_ID = source.Id,
                ROLE_INFO_NAME = source.Name,
                ROLE_INFO_CREATEDDATE = source.CreatedDate,
                GLOBAL_TYPE_ID = source.ApplicationId,
                ROLE_INFO_REMARK = source.Remark,
                ROLE_INFO_TYPE = source.Type,
                CREATEDBY = source.Createby,
                IsDel = source.IsDel
            };
        }

        public static RoleFunctionContract ToContract(this BASE_ROLEFUNCTIONS source)
        {
            if (source == null)
                throw new InvalidOperationException("source is null");
            return new RoleFunctionContract
            {
                Id = source.ROLEFUNCTIONS_ID,
                RoleId = source.ROLE_INFO_ID,
                FunctionId = source.FUNCTIONS_ID,
                CreatedDate = source.ROLEFUNCTIONS_CREATEDDATE,

            };
        }

        public static BASE_ROLEFUNCTIONS ToEntity(this RoleFunctionContract source)
        {
            if (source == null)
                throw new InvalidOperationException("source is null");
            return new BASE_ROLEFUNCTIONS
            {
                ROLEFUNCTIONS_ID = source.Id,
                ROLE_INFO_ID = source.RoleId,
                FUNCTIONS_ID = source.FunctionId,
                ROLEFUNCTIONS_CREATEDDATE = source.CreatedDate
            };
        }

        public static UserInfoContract ToUserInfoContract(this V_USER_ROLE_DEPT source)
        {
            if (source == null)
                throw new InvalidOperationException("source is null");
            return new UserInfoContract
            {
                Id = source.USER_INFO_ID,
                UserName = source.USER_INFO_NAME,
                NickName = source.USER_INFO_NICKNAME,
                DepId = source.DEPID,
                DepName = source.DEPT_INFO_NAME,
                CreatedDate = source.USER_INFO_CREATEDDATE == null ? DateTime.MinValue.ToString() : source.USER_INFO_CREATEDDATE.ToString(),
                TelNum = source.USER_INFO_TEL,
                AreaId = source.GEO_INFO_ID.Value,
            };
        }

        public static UserContract ToUserContract(this V_USER_ROLE_DEPT source)
        {
            if (source == null)
                throw new InvalidOperationException("source is null");
            return new UserContract
            {
                Id = source.USER_INFO_ID,
                RoleId = source.ROLE_INFO_ID,
                UserName = source.USER_INFO_NAME,
                NickName = source.USER_INFO_NICKNAME,
                DepId = source.DEPID,
                DepName = source.DEPT_INFO_NAME,
                Password = source.USER_INFO_PWD,
                RoleName = source.ROLE_INFO_NAME,
                CreatedDate = source.USER_INFO_CREATEDDATE ?? DateTime.MinValue,
                CreatedBy = source.USER_INFO_CREATEDBY,
                IsDel = source.USER_INFO_ISDEL ?? 0,
                DeptInfoIssup = source.DEPT_INFO_ISSUP,
                UserInfoIssup = source.USER_INFO_ISSUP ?? 0,
                UserInfoTel = source.USER_INFO_TEL,
                UserInfoEmail = source.USER_INFO_EMAIL,
                UserInfoPhoto = source.USER_INFO_PHOTO,
                UserInfoState = source.USER_INFO_STATE
            };
        }

        public static BASE_USER_INFO ToEntity(this UserContract source)
        {
            if (source == null)
                throw new InvalidOperationException("source is null");
            return new BASE_USER_INFO
            {
                USER_INFO_ID = source.Id,
                USER_INFO_NAME = source.UserName.ToLower(),
                USER_INFO_NICKNAME = source.NickName,
                DEPID = source.DepId,
                ROLE_INFO_ID = source.RoleId,
                USER_INFO_CREATEDDATE = source.CreatedDate,
                USER_INFO_CREATEDBY = source.CreatedBy,
                USER_INFO_PWD = source.Password,
                USER_INFO_ISDEL = source.IsDel,
                USER_INFO_ISSUP = source.UserInfoIssup,
                USER_INFO_TEL = source.UserInfoTel,
                USER_INFO_EMAIL = source.UserInfoEmail,
                USER_INFO_PHOTO = source.UserInfoPhoto,
                USER_INFO_STATE = source.UserInfoState
            };
        }

        public static ShortUserContract ToShortContract(this V_USER_DEPT source)
        {
            if (source == null)
                throw new InvalidOperationException("source is null");
            return new ShortUserContract
            {
                Id = source.USER_INFO_ID,
                NickName = source.USER_INFO_NICKNAME,
                DepId = source.DEPT_INFO_ID,
                Name = source.USER_INFO_NAME,
                DepName = source.DEPT_INFO_NAME,
                RoleId = source.ROLE_INFO_ID,
                AreoCode = source.GEO_INFO_CODE,
                AreoName = source.GEO_INFO_NAME,
                UserInfoIssup = source.USER_INFO_ISSUP ?? 0,
                DeptInfoIssup = source.DEPT_INFO_ISSUP,
                UserInfoTel = source.USER_INFO_TEL,
                UserInfoEmail = source.USER_INFO_EMAIL,
                UserInfoPhoto = source.USER_INFO_PHOTO,
                UserInfoState = source.USER_INFO_STATE
            };
        }

        public static ShortUserContract ToShortContract(this V_USER_ROLE_DEPT source)
        {
            if (source == null)
                throw new InvalidOperationException("source is null");
            return new ShortUserContract
            {
                Id = source.USER_INFO_ID,
                NickName = source.USER_INFO_NICKNAME,
                DepId = source.DEPID,
                Name = source.DEPT_INFO_NAME,
                DepName = source.DEPT_INFO_NAME,
                UserInfoIssup = source.USER_INFO_ISSUP ?? 0,
                DeptInfoIssup = source.DEPT_INFO_ISSUP,
                UserInfoTel = source.USER_INFO_TEL,
                UserInfoEmail = source.USER_INFO_EMAIL,
                UserInfoPhoto = source.USER_INFO_PHOTO,
                UserInfoState = source.USER_INFO_STATE
            };
        }

        public static UserContract ToContract(this BASE_USER_INFO source)
        {
            if (source == null)
                throw new InvalidOperationException("source is null");
            return new UserContract
            {
                Id = source.USER_INFO_ID,

                UserName = source.USER_INFO_NAME,
                NickName = source.USER_INFO_NICKNAME,
                DepId = source.DEPID,
                RoleId = source.ROLE_INFO_ID,
                CreatedDate = source.USER_INFO_CREATEDDATE ?? DateTime.MinValue,
                CreatedBy = source.USER_INFO_CREATEDBY,
                Password = source.USER_INFO_PWD,
                IsDel = source.USER_INFO_ISDEL ?? 0,
                UserInfoIssup = source.USER_INFO_ISSUP ?? 0,
                UserInfoTel = source.USER_INFO_TEL,
                UserInfoEmail = source.USER_INFO_EMAIL,
                UserInfoPhoto = source.USER_INFO_PHOTO,
                UserInfoState = source.USER_INFO_STATE
            };
        }

        public static BASE_DEPARTMENT_INFO ToEntity(this DepartmentContract source)
        {
            if (source == null)
                throw new InvalidOperationException("source is null");
            return new BASE_DEPARTMENT_INFO
            {
                DEPT_INFO_ID = source.Id,
                DEPT_INFO_NAME = source.Name,
                PARENT_ID = source.ParentId,
                DEPT_INFO_CODE = source.Code,
                DEPT_INFO_ADDRESS = source.Address,
                DEPT_INFO_PRINCIPALMAN = source.PrincipalMan,
                DEPT_INFO_TEL = source.Tel,
                DEPT_INFO_FAX = source.Fax,
                DEPT_INFO_MAIL = source.Mail,
                DEPT_INFO_CAPACITY = source.Capacity,
                GEO_INFO_ID = source.AreaId,
                DEPT_INFO_CREATEDBY = source.CreatedBy,
                DEPT_INFO_CREATEDDATE = source.CreatedTime,
                DEPT_INFO_ISDEL = source.IsDel,
                DEPT_INFO_ISSUP = source.DeptInfoIssup,
                SUP_ADD_ZC = source.SupAddZc,
                SUP_PROJECT = source.SupProject,
                SUP_XZ = source.SupXz,
                SUP_ZJ = source.SupZj
            };
        }

        public static DepartmentContract ToContract(this V_DEPT_DEPT source)
        {
            if (source == null)
                throw new InvalidOperationException("source is null");
            return new DepartmentContract
            {
                Id = source.DEPID,
                Name = source.DEPT_INFO_NAME,
                ParentId = source.PARENT_ID,
                Code = source.DEPT_INFO_CODE,
                Address = source.DEPT_INFO_ADDRESS,
                PrincipalMan = source.DEPT_INFO_PRINCIPALMAN,
                Tel = source.DEPT_INFO_TEL,
                Fax = source.DEPT_INFO_FAX,
                Mail = source.DEPT_INFO_MAIL,
                Capacity = source.DEPT_INFO_CAPACITY ?? 0,
                AreaId = source.GEO_INFO_ID.Value,
                ParentName = source.DEPT_INFO_PARENTNAME,
                CreatedTime = source.DEPT_INFO_CREATEDDATE ?? DateTime.MinValue,
                CreatedBy = source.DEPT_INFO_CREATEDBY,
                AreaCode = source.GEO_INFO_CODE,
                AreaName = source.GEO_INFO_NAME,
                IsDel = source.DEPT_INFO_ISDEL ?? 0,
                DeptInfoIssup = source.DEPT_INFO_ISSUP ?? 0,
                SupAddZc = source.SUP_ADD_ZC,
                SupProject = source.SUP_PROJECT,
                SupXz = source.SUP_XZ,
                SupZj = source.SUP_ZJ ?? 0

            };
        }

        public static DepartmentContract ToContract(this BASE_DEPARTMENT_INFO source)
        {
            if (source == null)
                throw new InvalidOperationException("source is null");
            return new DepartmentContract
            {
                Id = source.DEPT_INFO_ID,
                Name = source.DEPT_INFO_NAME,
                ParentId = source.PARENT_ID,
                Code = source.DEPT_INFO_CODE,
                Address = source.DEPT_INFO_ADDRESS,
                PrincipalMan = source.DEPT_INFO_PRINCIPALMAN,
                Tel = source.DEPT_INFO_TEL,
                Fax = source.DEPT_INFO_FAX,
                Mail = source.DEPT_INFO_MAIL,
                Capacity = source.DEPT_INFO_CAPACITY ?? 0,
                AreaId = source.GEO_INFO_ID.Value,
                CreatedBy = source.DEPT_INFO_CREATEDBY,
                IsDel = source.DEPT_INFO_ISDEL ?? 0,
                DeptInfoIssup = source.DEPT_INFO_ISSUP ?? 0,
                SupAddZc = source.SUP_ADD_ZC,
                SupProject = source.SUP_PROJECT,
                SupXz = source.SUP_XZ,
                SupZj = source.SUP_ZJ ?? 0
            };
        }

        public static AreaContract ToContract(this BASE_GEO_INFO source)
        {
            if (source == null)
                throw new InvalidOperationException("source is null");
            return new AreaContract
            {
                Id = source.GEO_INFO_ID,
                AreaNo = source.GEO_INFO_CODE,
                AreaName = source.GEO_INFO_NAME,
                ParentId = source.GEO_INFO_PARENT,
                OrderBy = source.GEO_INFO_ORDER ?? 0,
                IsDel = source.GEO_INFO_ISDEL ?? 0
            };
        }

        public static BASE_GEO_INFO ToEntity(this AreaContract source)
        {
            if (source == null)
                throw new InvalidOperationException("source is null");
            return new BASE_GEO_INFO
            {
                GEO_INFO_ID = source.Id,
                GEO_INFO_CODE = source.AreaNo,
                GEO_INFO_NAME = source.AreaName,
                GEO_INFO_PARENT = source.ParentId,
                GEO_INFO_ORDER = source.OrderBy,
                GEO_INFO_ISDEL = source.IsDel
            };
        }

        public static SelectValueContract ToContract(this BASE_SELECTE_VALUE_INFO source)
        {
            if (source == null)
                throw new InvalidOperationException("source is null");
            return new SelectValueContract
            {
                Id = source.Id,
                Name = source.Name,
                Belong = source.Belong,
                SOrder = source.SOrder,
                Code = source.Code
            };
        }

        public static BASE_SELECTE_VALUE_INFO ToEntity(this SelectValueContract source)
        {
            if (source == null)
                throw new InvalidOperationException("source is null");
            return new BASE_SELECTE_VALUE_INFO
            {
                Id = source.Id,
                Name = source.Name,
                Belong = source.Belong,
                SOrder = source.SOrder,
                Code = source.Code
            };
        }

       
    }
}