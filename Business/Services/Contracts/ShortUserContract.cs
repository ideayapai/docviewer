using System;
using System.Runtime.Serialization;
using Infrasturcture.QueryableExtension;

namespace Services.Contracts
{
    [DataContract]
    public class ShortUserContract : ContractBase
    {
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        
        /// <summary>
        /// 昵称
        /// </summary>
        [DataMember]
        public string NickName { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        [DataMember]
        public Guid DepId { get; set; }

        [DataMember]
        public string DepName { get; set; }

        [DataMember]
        public string AreoCode { get; set; }


        [DataMember]
        public string AreoName { get; set; }

        [DataMember]
        public Guid RoleId { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [DataMember]
        public string UserInfoEmail { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [DataMember]
        public string UserInfoTel { get; set; }

        /// <summary>
        /// 是否为供应商
        /// </summary>
        [DataMember]
        public int UserInfoIssup { get; set; }

        /// <summary>
        /// 是否为供应商
        /// </summary>
        [DataMember]
        public Nullable<int> DeptInfoIssup { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [DataMember]
        public string UserInfoPhoto { get; set; }

        /// <summary>
        /// 是否为黑名单
        /// </summary>
        [DataMember]
        [Mapping(To = "USER_INFO_STATE")]
        public string UserInfoState { get; set; }
    }
}
