using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Infrasturcture.QueryableExtension;

namespace Services.Contracts
{
    [DataContract]
    public class SelectValueContract : ContractBase
    {
        [DataMember]
        [Mapping(To = "Id")]
        [Required]
        public Guid Id { get; set; }

        [DataMember]
        [Mapping(To = "Name")]
        [Required(ErrorMessage = "名称不能为空")]
        public string Name { get; set; }

        [DataMember]
        [Mapping(To = "Belong")]
        [Required(ErrorMessage = "类型不能为空")]
        public string Belong { get; set; }

        [DataMember]
        [Mapping(To = "SOrder")]
        [Required(ErrorMessage = "排序不能为空")]
        public int? SOrder { get; set; }

        /// <summary>
        /// 用于标注道路类型或道路等级的标示编码
        /// </summary>
        [DataMember]
        [Mapping(To = "Code")]
        [Required(ErrorMessage = "数值不能为空")]
        public int? Code { get; set; }
    }
}
