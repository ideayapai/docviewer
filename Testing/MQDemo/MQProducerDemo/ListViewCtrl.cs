using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace MQDemoProducer
{
    static class ListViewCtrl
    {
        public static void SetListViewItemForeColor(Color color, string strKey, ListView lv, ref ListViewItem item)
        {
            try
            {
                int subItemIndex;
                if (lv == null || item == null || strKey == "") return;

                subItemIndex = lv.Columns[strKey].Index;
                if (subItemIndex >= 0)
                {
                    item.SubItems[subItemIndex].ForeColor = color;
                }
            }
            catch (System.Exception e)
            {
                //    GlobalFunction.MsgBoxException(e.Message);
            }
        }

        public static void SetListViewItemBackColor(Color color, string strKey, ListView lv, ref ListViewItem item)
        {
            try
            {
                int subItemIndex;
                if (lv == null || item == null || strKey == "") return;

                subItemIndex = lv.Columns[strKey].Index;
                if (subItemIndex >= 0)
                {
                    item.SubItems[subItemIndex].BackColor = color;
                }
            }
            catch (System.Exception e)
            {
                //    GlobalFunction.MsgBoxException(e.Message);
            }
        }

        /// <summary>
        ///   设置item的subitem的值
        /// </summary>
        public static void SetListViewValue(string strValue, string strKey, ListView lv, ref ListViewItem item)
        {
            try
            {
                int subItemIndex;
                if (lv == null || item == null || strKey == "") return;

                subItemIndex = lv.Columns[strKey].Index;
                if (subItemIndex >= 0 && item.SubItems[subItemIndex].Text != strValue)
                {
                    item.SubItems[subItemIndex].Text = strValue;
                }
            }
            catch (System.Exception e)
            {
                Debug.PrintWithTime("{0}:{1},key={2},value={3}", e.Message, "ListViewCtrl.SetListViewValue", strKey, strValue);
                //    GlobalFunction.MsgBoxException(e.Message);
            }
        }

        /// <summary>
        ///   设置item的subitem的值，值为数值，需要格式化
        /// </summary>
        /// <param name="decimalNum">格式化数字的小数位个数</param>
        public static void SetListViewValue(string strValue, string strKey, ListView lv, ref ListViewItem item, int decimalNum)
        {
            try
            {
                int subItemIndex;
                if (lv == null || item == null || strKey == "") return;

                subItemIndex = lv.Columns[strKey].Index;
                if (subItemIndex >= 0)
                {
                    //if (GlobalFunction.IsNumeric(strValue))
                    {
                        string val = "";
                        if (!GlobalFunction.FormatNumber(strValue, decimalNum, ref val))
                        {
                            return;
                        }
                        strValue = val;
                        //strValue = String.Format("{0:N" + decimalNum.ToString() + "}", Convert.ToDouble(strValue));
                    }
                    if (item.SubItems[subItemIndex].Text != strValue)
                    {
                        item.SubItems[subItemIndex].Text = strValue;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.PrintWithTime("{0}:{1},key={2},value={3}", e.Message, "ListViewCtrl.SetListViewValue", strKey, strValue);
                //    GlobalFunction.MsgBoxException(e.Message);
            }
        }

        public static string GetListViewValue(string strKey, ListView lv, int iRow)
        {
            try
            {
                int subItemIndex;
                if (lv == null || iRow < 0 || strKey == "") return "";
                if (iRow >= lv.Items.Count) return "";

                subItemIndex = lv.Columns[strKey].Index;
                if (subItemIndex >= 0)
                {
                    return lv.Items[iRow].SubItems[subItemIndex].Text;
                }
                else
                {
                    return "";
                }
            }
            catch (System.Exception e)
            {
                //Debug.PrintWithTime("{0}:{1},key={2}", e.Message, "ListViewCtrl.GetListViewValue", strKey);
                //    GlobalFunction.MsgBoxException(e.Message);
                return "";
            }
        }

        public static void InitSubItems(ref ListViewItem item, ListView lv)
        {
            for (int i = 1; i < lv.Columns.Count; i++)
            {
                item.SubItems.Add("");
            }
        }

        public static bool AddNewListViewItem(ref ListViewItem item, ref ListView lv)
        {
            try
            {
                item = new ListViewItem();
                for (int i = 1; i < lv.Columns.Count; i++)
                {
                    item.SubItems.Add("");
                }
                lv.Items.Add(item);
                return true;
            }
            catch (System.Exception e)
            {
                GlobalFunction.MsgBoxException(e.Message, "ListViewCtrl.AddNewListViewItem");
                return false;
            }
        }

        public static bool AddNewListViewItem(ref ListViewItem item, ref ListView lv, string strKey)
        {
            try
            {
                item = lv.Items.Add(strKey, "", 0);

                if (item != null)
                {
                    for (int i = 1; i < lv.Columns.Count; i++)
                    {
                        item.SubItems.Add("");
                    }
                }
                return true;
            }
            catch (System.Exception e)
            {
                GlobalFunction.MsgBoxException(e.Message, "ListViewCtrl.AddNewListViewItem");
                return false;
            }
        }

        public static bool InsertNewListViewItem(int index, ref ListViewItem item, ref ListView lv)
        {
            try
            {
                item = new ListViewItem();
                for (int i = 1; i < lv.Columns.Count; i++)
                {
                    item.SubItems.Add("");
                }
                lv.Items.Insert(index, item);
                return true;
            }
            catch (System.Exception e)
            {
                GlobalFunction.MsgBoxException(e.Message, "ListViewCtrl.InsertNewListViewItem");
                return false;
            }
        }

        public static bool InsertNewListViewItem(int index, ref ListViewItem item, ref ListView lv, string strKey)
        {
            try
            {
                item = lv.Items.Insert(index, strKey, "", 0);

                if (item != null)
                {
                    for (int i = 1; i < lv.Columns.Count; i++)
                    {
                        item.SubItems.Add("");
                    }
                }
                return true;
            }
            catch (System.Exception e)
            {
                GlobalFunction.MsgBoxException(e.Message, "ListViewCtrl.InsertNewListViewItem");
                return false;
            }
        }

        public static void Sort(ref ListView lv, ref ListViewColumnSorter lvwColumnSorter, ColumnClickEventArgs e)
        {
            try
            {
                if (lvwColumnSorter.SortColumn == e.Column)
                {
                    if (lvwColumnSorter.Order == SortOrder.Ascending)
                    {
                        lvwColumnSorter.Order = SortOrder.Descending;
                    }
                    else
                    {
                        lvwColumnSorter.Order = SortOrder.Ascending;
                    }
                }
                else
                {
                    lvwColumnSorter.SortColumn = e.Column;
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }

                lv.Sort();
            }
            catch (System.Exception e1)
            {
                GlobalFunction.MsgBoxException(e1.Message, "ListViewCtrl.Sort");
            }
        }

        public static void SetListViewStyle(ref ListView lv)
        {
            lv.View = View.Details;//定义列表显示的方式 
            lv.GridLines = true;//显示各个记录的分隔线 
            lv.FullRowSelect = true;//要选择就是一行 
            lv.Scrollable = true;//需要时候显示滚动条 
            lv.MultiSelect = false; // 不可以多行选择 
            lv.HideSelection = false; //总是显示选中行

            //           lv.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        }

        public static void SetListViewStyle(ref ListView lv, bool blnGridLines, bool blnMultiSelect)
        {
            lv.View = View.Details;//定义列表显示的方式 
            lv.GridLines = blnGridLines;//显示各个记录的分隔线 
            lv.FullRowSelect = true;//要选择就是一行 
            lv.Scrollable = true;//需要时候显示滚动条 
            lv.MultiSelect = blnMultiSelect; // 不可以多行选择 
            lv.HideSelection = false; //总是显示选中行

            //           lv.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        }
    }
}
