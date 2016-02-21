using System;
using System.Runtime.Serialization;

namespace Services.Contracts
{
    [Serializable]
    [DataContract(Name="users")]
    public class UserInfoContract : ContractBase
    {
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [DataMember]
        public string NickName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [DataMember]
        public string Sex { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        [DataMember]
        public Guid DepId { get; set; }

        [DataMember]
        public string DepName { get; set; }

        [DataMember]
        public Guid AreaId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public string CreatedDate { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        [DataMember]
        public string TelNum { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [DataMember]
        public string HeadImage { get; set; }
    }

    [DataContract]
    public class EmptyUserInfoContract : UserInfoContract
    {

    }
}
