using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Infrasturcture.QueryableExtension;

namespace Services.Contracts
{
    [DataContract]
    public  class RoleContract : ContractBase
    {
        [DataMember]
        [Mapping(To = "ROLE_INFO_ID")]
        public Guid Id { get; set; }

        [DataMember]
        [Mapping(To = "ROLE_INFO_NAME")]
        [Required(ErrorMessage = "角色名称不能为空")]
        [StringLength(50, ErrorMessage = "角色名称最大长度为50字符")]
        public string Name { get; set; }

        [DataMember]
        [Mapping(To = "ROLE_INFO_TYPE")]
        public string Type { get; set; }

        [DataMember]
        [Mapping(To = "ROLE_INFO_REMARK")]
        [StringLength(100, ErrorMessage = "角色描述最大长度为100字符")]
        public string Remark { get; set; }

        [DataMember]
        [Mapping(To = "ROLE_INFO_CREATEDDATE")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Mapping(To = "GLOBAL_TYPE_ID")]
        public Guid ApplicationId { get; set; }

        [DataMember]
        [Mapping(To = "IsDel")]
        public int IsDel { get; set; }

        [DataMember]
        [Mapping(To = "CREATEBY")]
        public string Createby { get; set; }

        [IgnoreDataMember]
        public bool CanDelete
        {
            get { return string.Compare(Type, ServiceConstants.SystemRoleType, true) != 0; }
        }
    }


}
