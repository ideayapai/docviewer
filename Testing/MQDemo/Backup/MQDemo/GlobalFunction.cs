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
using System.Runtime.InteropServices;


namespace MQDemoSubscriber
{
    static class GlobalFunction
    {
        public static bool IsNumeric(string value)
        {
            bool ret;
            ret = Regex.IsMatch(value, @"^\d+(\.\d+)?$");
            if (!ret)
            {
                ret = Regex.IsMatch(value, @"^((-\d+(\.\d+)?)|(0+(\.0+)?))$");
            }

            return ret;
        }

        public static bool IsInt(string value)
        {
            bool ret;
            ret = Regex.IsMatch(value, @"^\d+$");
            if (!ret)
            {
                ret = Regex.IsMatch(value, @"^((-\d+)|(0+))$");
            }

            return ret;
        }

        public static bool IsDate(string value)
        {
            DateTime d;
            return DateTime.TryParse(value, out d);
        }

        public static bool IsDigit(KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool FormatNumber(string value, int decimalNum, ref string result)
        {
            try
            {
                result = String.Format("{0:N" + decimalNum.ToString() + "}", Convert.ToDecimal(Decimal.Parse(value, System.Globalization.NumberStyles.Float)));
                return true;
            }
            catch (System.Exception ex)
            {
                //MsgBoxException(ex.Message, "FormatNumber");
                return false;
            }
        }

        public static string FormatNumberPercent(string value, int decimalNum)
        {
            try
            {
                value = String.Format("{0:N" + decimalNum.ToString() + "}%", 100 * Convert.ToDecimal(Decimal.Parse(value, System.Globalization.NumberStyles.Float)));
                return value;
            }
            catch (System.Exception ex)
            {
                MsgBoxException(ex.Message, "FormatNumber");
                return value;
            }
        }

        public static DialogResult MsgBox(string strText)
        {
            return MessageBox.Show(strText, "操作提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static DialogResult MsgBoxExclamation(string strText)
        {
            return MessageBox.Show(strText, "操作提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        public static DialogResult MsgBoxException(string strText, string strFuncName)
        {
            string strMsg;
            strMsg = strFuncName + "\n" + strText;

            return MessageBox.Show(strMsg, "操作提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static bool CheckControlInput(Control ctrl, string strCaption, int iLength, bool blnNumber)
        {
            try
            {
                if (ctrl.GetType().ToString() == "System.Windows.Forms.TextBox")
                {
                    if (ctrl.Text.Trim() == "")
                    {
                        MsgBoxExclamation(strCaption + "不能为空，请输入" + strCaption);
                        ctrl.Focus();
                        ((TextBox)ctrl).SelectAll();
                        return false;
                    }
                    else if (iLength > 0 && ctrl.Text.Length != iLength)
                    {
                        MsgBoxExclamation("请输入" + iLength.ToString() + "位" + strCaption);
                        ctrl.Focus();
                        ((TextBox)ctrl).SelectAll();
                        return false;
                    }

                    if (blnNumber && !IsNumeric(ctrl.Text))
                    {
                        MsgBoxExclamation(strCaption + "必须为数字，请重新输入");
                        ctrl.Focus();
                        ((TextBox)ctrl).SelectAll();
                        return false;
                    }
                }
                else if (ctrl.GetType().ToString() == "System.Windows.Forms.ComboBox")
                {
                    if (ctrl.Text.Trim() == "")
                    {
                        MsgBoxExclamation(strCaption + "不能为空，请选择" + strCaption);
                        ctrl.Focus();
                        return false;
                    }
                }
                else
                {
                    if (ctrl.Text.Trim() == "")
                    {
                        MsgBoxExclamation(strCaption + "不能为空，请输入" + strCaption);
                        ctrl.Focus();
                        return false;
                    }
                }

                return true;
            }
            catch (System.Exception e)
            {
                MsgBoxException(e.Message, "GlobalFunction.CheckControlInput");
                return false;
            }
        }
    }
}
