using Fap.Core.Office.Word;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace XUnitTestFapCore
{
    public class WordTest
    {
        [Fact]
        public void ReplaceTemplate()
        {
            WordTemplate template = new WordTemplate();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("a0101", "123");
            dic.Add("b0110", "信息系统不");
            template.ReplaceTemplate("E:\\离职交接表.docx", "E:\\离职交接表1.docx",dic);

            Assert.True(true);
        }
    }
}
