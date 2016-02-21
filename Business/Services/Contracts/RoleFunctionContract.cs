using System;
using System.Runtime.Serialization;

namespace Services.Contracts
{
    [DataContract]
    public class RoleFunctionContract : ContractBase
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public Guid RoleId { get; set; }

        [DataMember]
        public Guid FunctionId { get; set; }

        [DataMember]
        public DateTime CreatedDate { get; set; }
    }
}
