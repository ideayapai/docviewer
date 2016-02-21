using System.Collections;
using System.Windows.Forms;
using System;


namespace MQDemoProducer
{
    class ListViewColumnSorter : IComparer
    {
        private int ColumnToSort;
        private SortOrder OrderOfSort;
        private CaseInsensitiveComparer ObjectCompare;

        public ListViewColumnSorter()
        {
            // 默认按第一列排序
            ColumnToSort = 0;

            // 排序方式为不排序
            OrderOfSort = SortOrder.None;

            // 初始化CaseInsensitiveComparer类对象
            ObjectCompare = new CaseInsensitiveComparer();
        }

        /**/
        /// <summary>
        /// 重写IComparer接口.
        /// </summary>
        /// <param name="x">要比较的第一个对象</param>
        /// <param name="y">要比较的第二个对象</param>
        /// <returns>比较的结果.如果相等返回0，如果x大于y返回1，如果x小于y返回-1</returns>
        public int Compare(object x, object y)
        {
            try
            {
                int compareResult;
                string strX, strY;
                ListViewItem listviewX, listviewY;

                // 将比较对象转换为ListViewItem对象
                listviewX = (ListViewItem)x;
                listviewY = (ListViewItem)y;

                strX = listviewX.SubItems[ColumnToSort].Text;
                strY = listviewY.SubItems[ColumnToSort].Text;

                strX = strX.Replace(",", "");
                strY = strY.Replace(",", "");

                if (!GlobalFunction.IsNumeric(strX))
                {
                    strX = listviewX.SubItems[ColumnToSort].Text;
                    strY = listviewY.SubItems[ColumnToSort].Text;
                }

                if (GlobalFunction.IsNumeric(strX) && GlobalFunction.IsNumeric(strY))
                {
                    //简直是画蛇添足，是数字的话直接转换为数字比较就可以了。而且这样的话如果是数字还会出错
                    //strX = string.Format("{0:000000000000000.000000}", Convert.ToDouble(strX));   
                    //strY = string.Format("{0:000000000000000.000000}", Convert.ToDouble(strY));

                    // 比较
                    compareResult = ObjectCompare.Compare(Convert.ToDouble(strX), Convert.ToDouble(strY));

                    //    Debug.Print("strX={0} strY={1}", strX, strY);
                }
                else
                {
                    // 比较
                    compareResult = ObjectCompare.Compare(strX, strY);
                }


                // 根据上面的比较结果返回正确的比较结果
                if (OrderOfSort == SortOrder.Ascending)
                {
                    // 因为是正序排序，所以直接返回结果
                    return compareResult;
                }
                else if (OrderOfSort == SortOrder.Descending)
                {
                    // 如果是反序排序，所以要取负值再返回
                    return (-compareResult);
                }
                else
                {
                    // 如果相等返回0
                    return 0;
                }
            }
            catch (System.Exception e)
            {
                //GlobalFunction.MsgBoxException(e.Message, "ListViewColumnSorter.Compare");
                return 0;
            }
        }

        /**/
        /// <summary>
        /// 获取或设置按照哪一列排序.
        /// </summary>
        public int SortColumn
        {
            set
            {
                ColumnToSort = value;
            }
            get
            {
                return ColumnToSort;
            }
        }

        /**/
        /// <summary>
        /// 获取或设置排序方式.
        /// </summary>
        public SortOrder Order
        {
            set
            {
                OrderOfSort = value;
            }
            get
            {
                return OrderOfSort;
            }
        }
    }
}
