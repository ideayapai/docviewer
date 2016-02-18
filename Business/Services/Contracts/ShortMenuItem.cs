using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Services.Contracts
{
    [DataContract]
    public class ShortMenuItem
    {
        [DataMember(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember(Name = "parentId")]
        public Guid? ParentId { get; set; }
        
        [DataMember(Name = "text")]
        public string Name { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "checked")]
        public bool IsChecked { get; set; }

        [DataMember(Name = "children")]
        public List<ShortMenuItem> Children { get; set; }
    }
}
