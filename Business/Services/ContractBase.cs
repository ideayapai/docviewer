using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Services
{
    [Serializable]
    [DataContract]
    public abstract class ContractBase
    {
        [DataMember]
        public string ErrorMessage { get; set; }


        private string _errorCode = "0";
        [DataMember]
        public string ErrorCode
        {
            get { return _errorCode; }
            set
            {
                if (_errorCode != null && _errorCode != value)
                    _errorCode = value;
            }
        }

        public virtual bool IsValid()
        {
            Type type = GetType();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var attrs = property.GetCustomAttributes(typeof(ValidationAttribute), true);
                for (int i = 0; i < attrs.Length; i++)
                {
                    var attr = (ValidationAttribute)attrs[i];
                    var rs = attr.IsValid(property.GetValue(this, null));
                    if (!rs)
                    {
                        ErrorMessage += (attr.ErrorMessage + "\r\n");
                    }
                }
            }
            return string.IsNullOrEmpty(ErrorMessage);
        }
    }
}
