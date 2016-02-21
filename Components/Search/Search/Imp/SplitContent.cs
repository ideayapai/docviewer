using PanGu;

namespace Search.Imp {
    public class SplitContent {

        /// <summary>
        /// 标题加亮
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string HightLightTitle(string keyword, string title)
        {
            if (title.Contains(keyword) && !string.IsNullOrWhiteSpace(keyword))
            {
                title = title.Replace(keyword, "<font style=\"font-style:normal;color:#cc0000;\"><b>" + keyword + "</b></font>");
            }
            else
            {
                string[] keywords = System.Text.RegularExpressions.Regex.Split(keyword, @"\s+");
                foreach (var word in keywords)
                {
                    if (title.Contains(word) && !string.IsNullOrWhiteSpace(word))
                    {
                        title = title.Replace(word,
                            "<font style=\"font-style:normal;color:#cc0000;\"><b>" + word + "</b></font>");
                    }
                }
            }
            return title;
        }

        //需要添加PanGu.HighLight.dll的引用
        /// <summary>
        /// 搜索结果高亮显示
        /// </summary>
        /// <param name="keyword"> 关键字 </param>
        /// <param name="content"> 搜索结果 </param>
        /// <returns> 高亮后结果 </returns>
        /// 
        public static string HightLight(string keyword, string content) {
            //创建HTMLFormatter,参数为高亮单词的前后缀
            PanGu.HighLight.SimpleHTMLFormatter simpleHTMLFormatter =
                new PanGu.HighLight.SimpleHTMLFormatter("<font style=\"font-style:normal;color:#cc0000;\"><b>", "</b></font>");
            
            //创建 Highlighter ，输入HTMLFormatter 和 盘古分词对象Semgent
            PanGu.HighLight.Highlighter highlighter =
                            new PanGu.HighLight.Highlighter(simpleHTMLFormatter,
                            new Segment());

            //设置每个摘要段的字符数
            highlighter.FragmentSize = 255;
            //获取最匹配的摘要段
            return highlighter.GetBestFragment(keyword, content);
        }
    }
}