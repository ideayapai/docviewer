using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Infrasturcture.QueryableExtension;

namespace Services.Contracts
{
    /// <summary>
    /// 部门基本信息
    /// </summary>
    [DataContract]
    public class DepartmentContract : ContractBase
    {
        /// <summary>
        /// 编号
        /// </summary>
        [DataMember]
        [Mapping(To = "DEPID")]
        public Guid Id { get; set; }

        [DataMember]
        [Mapping(To = "GEO_INFO_ID")]
        public Guid AreaId { get; set; }

        /// <summary>
        /// 部门编号
        /// </summary>
        [DataMember]
        [Mapping(To = "DEPT_INFO_CODE")]
        [Required(ErrorMessage = "请输入编号")]
     
        public string Code { get; set; }


        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        [Mapping(To = "DEPT_INFO_NAME")]
        [Required(ErrorMessage = "请输入名称")]
        [StringLength(64, ErrorMessage = "名称的最大长度为64个字符")]
        public string Name { get; set; }


        /// <summary>
        /// 部门地址
        /// </summary>
        [DataMember]
        [Mapping(To = "DEPT_INFO_ADDRESS")]
        [StringLength(200, ErrorMessage = "地址的最大长度为200个字符")]
        public string Address { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        [DataMember]
        [Mapping(To = "DEPT_INFO_PRINCIPALMAN")]
        [StringLength(64, ErrorMessage = "负责人的最大长度为64个字符")]
        public string PrincipalMan { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [DataMember]
        [Mapping(To = "DEPT_INFO_TEL")]
        [Required(ErrorMessage = "请输入联系电话")]
        [StringLength(12, ErrorMessage = "联系电话最大长度为12个字符")]
        public string Tel { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
        [DataMember]
        [Mapping(To = "DEPT_INFO_FAX")]
        [StringLength(12, ErrorMessage = "传真最大长度为12个字符")]
        public string Fax { get; set; }


        [DataMember]
        [Mapping(To = "DEPT_INFO_MAIL")]
        [StringLength(64, ErrorMessage = "E-Mail最大长度为64个字符")]
        public string Mail { get; set; }

        [DataMember]
        [Mapping(To = "DEPT_INFO_CAPACITY")]
        public int Capacity { get; set; }

        /// <summary>
        /// 父级Id
        /// </summary>
        [DataMember]
        [Mapping(To = "PARENT_ID")]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        [DataMember]
        public string Area { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        [Mapping(To = "DEPT_INFO_CREATEDDATE")]
        public DateTime CreatedTime { get; set; }

        [DataMember]
        [Mapping(To = "DEPT_INFO_PARENTNAME")]
        public string ParentName { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [DataMember]
        [Mapping(To = "DEPT_INFO_ISDEL")]
        public int IsDel { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [DataMember]
        [Mapping(To = "DEPT_INFO_CREATEDBY")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// 所属区域名称
        /// </summary>
        [DataMember]
        [Mapping(To = "GEO_INFO_NAME")]
        public string AreaName { get; set; }

        /// <summary>
        /// 所属区域编号
        /// </summary>
        [DataMember]
        [Mapping(To = "GEO_INFO_CODE")]
        public string AreaCode { get; set; }

        /// <summary>
        /// 是否为供应商
        /// </summary>
        [DataMember]
        [Mapping(To = "DEPT_INFO_ISSUP")]
        public int DeptInfoIssup { get; set; }

        /// <summary>
        /// 单位性质
        /// </summary>
        [DataMember]
        [Mapping(To = "SUP_XZ")]
        public string SupXz { get; set; }
        /// <summary>
        /// 注册地址
        /// </summary>
        [DataMember]
        [Mapping(To = "SUP_ADD_ZC")]
        public string SupAddZc { get; set; }
        /// <summary>
        /// 注册资金
        /// </summary>
        [DataMember]
        [Mapping(To = "SUP_ZJ")]
        public decimal SupZj { get; set; }
        /// <summary>
        /// 从事行业
        /// </summary>
        [DataMember]
        [Mapping(To = "SUP_PROJECT")]
        public string SupProject { get; set; }

    }
}
