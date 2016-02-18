using System;
using System.Data.Linq.Mapping;

namespace Repository.Base
{
    #region 函数

    public partial class GetUsersByUserNameResult
    {

        private string _USER_INFO_NAME;

        public GetUsersByUserNameResult()
        {
        }

        [Column(Storage = "_USER_INFO_NAME", DbType = "VarChar(50) NOT NULL", CanBeNull = false)]
        public string USER_INFO_NAME
        {
            get
            {
                return this._USER_INFO_NAME;
            }
            set
            {
                if ((this._USER_INFO_NAME != value))
                {
                    this._USER_INFO_NAME = value;
                }
            }
        }
    }

    public partial class GetDepsByUserNameResult
    {

        private Nullable<Guid> _DEPT_INFO_ID;

        public GetDepsByUserNameResult()
        {
        }

        [Column(Storage = "_DEPT_INFO_ID", DbType = "UniqueIdentifier", CanBeNull = false)]
        public Nullable<Guid> DEPT_INFO_ID
        {
            get
            {
                return this._DEPT_INFO_ID;
            }
            set
            {
                if ((this._DEPT_INFO_ID != value))
                {
                    this._DEPT_INFO_ID = value;
                }
            }
        }
    }

    #endregion
}
