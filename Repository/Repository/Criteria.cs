using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public class Criteria : IEnumerable
    {
        private readonly Dictionary<string, object> _dics = new Dictionary<string, object>();
        private StringBuilder _conditionBuilder = new StringBuilder(" where 1=1 ");
        private StringBuilder _setBuilder = new StringBuilder("SET ");
        private string _tableName = string.Empty;

        public Criteria(string tableName)
        {
            _tableName = tableName;
        }

        public void AddCondition(string key, object value, string operate)
        {
            if (!_dics.ContainsKey(key))
            {
                _dics.Add(key, value);
                _conditionBuilder.AppendFormat("AND [{0}]{1}{2} ", key, operate, value);
            }
        }

        public void SetStatement(string key, object value)
        {
            if (!_dics.ContainsKey(key))
            {
                _dics.Add(key, value);
                _setBuilder.AppendFormat("[{0}]={1},", key, value);
            }

        }

        public string GetUpdateString()
        {
            return string.Format("UPDATE {0} {1} {2}", _tableName, _setBuilder.ToString().TrimEnd(','), _conditionBuilder);
        }


        public IEnumerator GetEnumerator()
        {
            return _dics.GetEnumerator();
        }

        public bool ContainsKey(string key)
        {
            return _dics.ContainsKey(key);
        }
    }
}
