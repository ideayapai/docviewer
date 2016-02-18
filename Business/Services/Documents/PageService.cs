
using System;
using System.Configuration;

namespace Services.Documents
{
    public class PageService
    {
        /// <summary>
        /// 每页显示条数
        /// </summary>
        static public int PageShowCount = int.Parse(ConfigurationManager.AppSettings["PageShowCount"]);

        /// <summary>
        /// 获取分页代码
        /// </summary>
        /// <returns></returns>
        static public string GetPageCode(int allCount, int nowPage,string linkType)
        {
            var allPage = CalculateAllPage(allCount);
            if (allPage < 2)
                return string.Empty;
            var result = string.Format("<div class=\"page\"><a href=\"/{0}/{1}\">{2}</a>\n", linkType, 1, "首页");

            if (nowPage > 3)
                result += "<span>…</span>";
            if(nowPage == 2 )
                result += string.Format("<a href=\"/{0}/{1}\">{1}</a>\n", linkType, 1);

            if (nowPage > (nowPage - 2) && nowPage > 2 )
            {
                for (var i = nowPage - 2; i < nowPage; i++)
                {
                    result += string.Format("<a href=\"/{0}/{1}\">{2}</a>\n", linkType, i, i);
                }
            }
            result += string.Format("<a href=\"#\" class=\"checked\">{0}</a>\n", nowPage);
            for (var i = nowPage + 1; i < nowPage + 3; i++)
            {
                if (i < allPage + 1)
                    result += string.Format("<a href=\"/{0}/{1}\">{2}</a>\n", linkType, i, i);
            }
            if ((nowPage + 2) < allPage)
                result += string.Format("<span>…</span>\n");
            if (nowPage < allPage)
                result += string.Format("<a href=\"/{0}/{1}\">{2}</a>\n", linkType, allPage, "尾页");
            result += "<input id=\"pageinput\" type=\"text\" onkeyup=\"pageKeyup(this);\" onafterpaste=\"pageOnafterpaste();\">\n<button onclick=\"JumpPage();\">跳转</button></div>";
            return result;
        }

        /// <summary>
        /// 计算共有多少页
        /// </summary>
        /// <param name="allCount">共有多少条记录</param>
        private static int CalculateAllPage(int allCount)
        {
            var pageCount = (double)allCount / (double)PageShowCount;
            var tempPageCount = pageCount + "";
            try
            {
                if (double.Parse(tempPageCount.Split('.')[1]) > 0)
                {
                    return int.Parse(tempPageCount.Split('.')[0]) + 1;
                }
            }
            catch (Exception e)
            {
                return int.Parse(pageCount.ToString());
            }
            return int.Parse(pageCount.ToString());
        }
    }
}