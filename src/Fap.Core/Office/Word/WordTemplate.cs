using Fap.Core.Infrastructure.Domain;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Fap.Core.Office.Word
{
    /// <summary>
    /// word 模板
    /// </summary>
    public class WordTemplate
    {
        public void ReplaceTemplate(string templateFileName, string wordFileName, IDictionary<string, string> keyValues)
        {
            using FileStream fileStream = File.OpenRead(templateFileName);
            //打开07（.docx）以上的版本的文档
            XWPFDocument docx = new XWPFDocument(fileStream);

            IList<XWPFParagraph> paragraphs = docx.Paragraphs;
            ReplaceParagraphList(paragraphs);
            IList<XWPFTable> tables = docx.Tables;
            ReplaceTableList(tables);
            using FileStream output = new FileStream(wordFileName, FileMode.Create);
            docx.Write(output);

            void ReplaceParagraphList(IList<XWPFParagraph> paragraphs)
            {
                //遍历段落
                foreach (var paragraph in paragraphs)
                {
                    ReplaceParagraph(paragraph);
                }
            }
            void ReplaceTableList(IList<XWPFTable> tables)
            {
                foreach (var table in tables)
                {
                    foreach (var row in table.Rows)
                    {
                        foreach (var cell in row.GetTableCells())
                        {
                            ReplaceParagraphList(cell.Paragraphs);
                            ReplaceTableList(cell.Tables);
                        }
                    }
                }
            }

            void ReplaceParagraph(XWPFParagraph paragraph)
            {
                string text = paragraph.ParagraphText;
                Regex rgx = new Regex(FapPlatformConstants.VariablePattern);
                MatchCollection matchs = rgx.Matches(text);
                foreach (var mtch in matchs)
                {
                    string sc = mtch.ToString();
                    string key = sc.Substring(2).TrimEnd('}');
                    if (keyValues.TryGetValue(key, out string value))
                    {
                        paragraph.ReplaceText(sc, value);
                    }
                }
            }
        }
    }
}
