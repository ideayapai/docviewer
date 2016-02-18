using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Infrasturcture.QueryableExtension;

namespace Services.Contracts
{
    [DataContract]
    public class AreaContract : ContractBase
    {
        [DataMember]
        [Mapping(To = "GEO_INFO_ID")]
        [Required]
        public Guid Id { get; set; }

        [DataMember]
        [Mapping(To = "GEO_INFO_CODE")]
        [StringLength(50, ErrorMessage = "区域编号最大长度为50字符")]
        [Required]
        public string AreaNo { get; set; }

        [DataMember]
        [Mapping(To = "GEO_INFO_NAME")]
        [StringLength(25, ErrorMessage = "区域名称最大长度为25字符")]
        [Required]
        public string AreaName { get; set; }

        [DataMember]
        [Mapping(To = "GEO_INFO_PARENT")]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [DataMember]
        [Mapping(To = "GEO_INFO_ISDEL")]
        public int IsDel { get; set; }


        [DataMember]
        [Mapping(To = "GEO_INFO_ORDER")]
        [Required]
        public int OrderBy { get; set; }
    }
}
