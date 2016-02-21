using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Infrasturcture.QueryableExtension;

namespace Services.Contracts
{
    [DataContract]
    public class UserContract : ContractBase
    {
        [DataMember]
        [Mapping(To = "USER_INFO_ID")]
        public Guid Id { get; set; }

        /// <summary>
        /// 角色编号
        /// </summary>
        [DataMember]
        [Mapping(To = "ROLE_INFO_ID")]
        public Guid RoleId { get; set; }

        [DataMember]
        [Mapping(To = "ROLE_INFO_NAME")]
        public string RoleName { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [DataMember]
        [Mapping(To = "USER_INFO_NAME")]
        [Required(ErrorMessage = "用户名不能为空")]
        [StringLength(50, ErrorMessage = "用户名最长为50个字符")]
        public string UserName { get; set; }


        /// <summary>
        /// 昵称
        /// </summary>
        [DataMember]
        [Mapping(To = "USER_INFO_NICKNAME")]
        [Required(ErrorMessage = "昵称不能为空")]
        [StringLength(10, ErrorMessage = "昵称最长为10个字符")]
        public string NickName { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        [DataMember]
        [Mapping(To = "DEPID")]
        [Required(ErrorMessage = "部门不能为空")]
        public Guid DepId { get; set; }

        [DataMember]
        [Mapping(To = "DEPT_INFO_NAME")]
        public string DepName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [DataMember]
        [Mapping(To = "USER_INFO_PWD")]
        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [DataMember]
        [Mapping(To = "USER_INFO_EMAIL")]
        public string UserInfoEmail { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [DataMember]
        [Mapping(To = "USER_INFO_TEL")]
        public string UserInfoTel { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [DataMember]
        [Mapping(To = "USER_INFO_PHOTO")]
        public string UserInfoPhoto { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        [Mapping(To = "USER_INFO_CREATEDDATE")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [DataMember]
        [Mapping(To = "USER_INFO_CREATEDBY")]
        [Required(ErrorMessage = "创建者不能为空")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [DataMember]
        [Mapping(To = "USER_INFO_ISDEL")]
        public int IsDel { get; set; }

        public string Token { get; set; }

        /// <summary>
        /// 是否为供应商
        /// </summary>
        [DataMember]
        [Mapping(To = "USER_INFO_ISSUP")]
        public int UserInfoIssup { get; set; }

        /// <summary>
        /// 是否为供应商
        /// </summary>
        [DataMember]
        [Mapping(To = "DEPT_INFO_ISSUP")]
        public Nullable<int> DeptInfoIssup { get; set; }

        /// <summary>
        /// 是否为黑名单
        /// </summary>
        [DataMember]
        [Mapping(To = "USER_INFO_STATE")]
        public string UserInfoState { get; set; }

    }

    [DataContract]
    public class EmptyUserContract : UserContract
    {
        [DataMember]
        public string ErrorMsg { get; set; }
    }

    [DataContract]
    public class DuplicateUserContract : UserContract
    {

    }
}
