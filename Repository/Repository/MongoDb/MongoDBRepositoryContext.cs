using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;

namespace Repository.MongoDb
{
    public abstract class MongoDBRepositoryContext : DataContext
    {
        public MongoDBRepositoryContext(IDbConnection connection)
            : this(connection, false)
        {
        }

        public MongoDBRepositoryContext(string connectionString, MappingSource mappingSource)
            : base(connectionString, mappingSource)
        {
        }

        public MongoDBRepositoryContext(IDbConnection connection, MappingSource mappingSource)
            : base(connection, mappingSource)
        {
        }


        public MongoDBRepositoryContext(IDbConnection connection, bool isReadOnly)
            : base(connection)
        {
            ResetContext(isReadOnly);
        }

        public MongoDBRepositoryContext(string fileOrServerOrConnection)
            : this(fileOrServerOrConnection, false)
        {
        }

        public MongoDBRepositoryContext(string fileOrServerOrConnection, bool isReadOnly)
            : base(fileOrServerOrConnection )
        {
            ResetContext(isReadOnly);
        }


        private bool _isReadOnly = true;

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
        }

        internal void ResetContext(bool isReadOnly)
        {
            _isReadOnly = isReadOnly;
            ObjectTrackingEnabled = !_isReadOnly;
            if (!isReadOnly)
                DeferredLoadingEnabled = true;
        }


        [Function(Name = "NewGuid", IsComposable = true)]
        public Guid NewGuid()
        {
            return ((Guid)(ExecuteMethodCall(this, ((MethodInfo)(MethodBase.GetCurrentMethod()))).ReturnValue));
        }

        public T UpdateEntity<T>(T updateEntity, T sourceEntity)
        {
            return GetUpdated(updateEntity, sourceEntity, new string[] { });
        }

        public T UpdateEntity<T>(T updateEntity, T sourceEntity, string[] includeProperties)
        {
            return GetUpdated(updateEntity, sourceEntity, includeProperties);
        }

        public T UpdateEntity<T>(T updateEntity, Dictionary<string, object> updateField)
        {
            var dic = new SortedDictionary<string, object>();
            foreach (var key in updateField.Keys)
            {
                dic.Add(key.ToLower(), updateField[key]);
            }

            foreach (PropertyInfo t in updateEntity.GetType().GetProperties())
            {
                if (dic.ContainsKey(t.Name.ToLower()))
                {
                    t.SetValue(updateEntity, dic[t.Name.ToLower()], null);
                }
            }
            return updateEntity;
        }

        private T GetUpdated<T>(T updateEntity, T sourceEntity, string[] includeProperties)
        {
            var ps = new SortedDictionary<string, string>();
            includeProperties.ToList().ForEach(p => ps.Add(p.ToLower(), p.ToLower()));

            if (Equals(updateEntity, default(T)))
            {
                return updateEntity;
            }

            var seted = new Dictionary<string, string>();
            foreach (var p1 in sourceEntity.GetType().GetProperties())
            {
                foreach (var p2 in updateEntity.GetType().GetProperties())
                {
                    if (seted.ContainsKey(p2.Name))
                    {
                        continue;
                    }

                    if ((ps.Count > 0 ? ps.ContainsKey(p2.Name.ToLower()) : true) && String.Equals(p1.Name, p2.Name, StringComparison.OrdinalIgnoreCase) && Equals(p1.PropertyType, p2.PropertyType))
                    {
                        p2.SetValue(updateEntity, p1.GetValue(sourceEntity, null), null);
                        seted.Add(p2.Name, p2.Name);
                    }
                }
            }
            return updateEntity;
        }

    }
}
