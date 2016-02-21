using System.Collections.Generic;

namespace Infrasturcture.QueryObjects
{
    public class QueryObject
    {
        public string Name { get; set; }

        public string PropertyName { get; set; }

        public string Javascript { get; set; }

        public string Type { get; set; }

        public List<NameValuePair> NameValues { get; set; }
    }

    public class QueryObjectItem
    {
        public string Name { get; set; }

        public string PropertyName { get; set; }

        public string Operate { get; set; }

        public string OperateText { get; set; }

        public object Value { get; set; }

        public string ValueType { get; set; }

        public string BinaryOperator { get; set; }

        public string BinaryOperatorText { get; set; }
    }

    public class NameValuePair
    {
        public string Name { get; set; }

        public object Value { get; set; }
    }
}