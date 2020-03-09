
using Fap.Core.Extensions;
using NPOI.HSSF.UserModel;
using NPOI.POIFS.FileSystem;
using NPOI.SS.Format;
using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Fap.Core.Office.Excel
{
    public class ExcelToHTML
    {
        private IWorkbook wb = null;

        private const String DEFAULTS_CLASS = "excelDefaults";
        private const String COL_HEAD_CLASS = "colHeader";
        private const String ROW_HEAD_CLASS = "rowHeader";

        private const int IDX_TABLE_WIDTH = -2;
        private const int IDX_HEADER_COL_WIDTH = -1;


        private int firstColumn;
        private int endColumn;

        private bool gotBounds;

        private List<KeyValuePair<HorizontalAlignment, string>> HALIGN = new List<KeyValuePair<HorizontalAlignment, string>>() {
            new KeyValuePair<HorizontalAlignment, string>(HorizontalAlignment.Left, "left"),
            new KeyValuePair<HorizontalAlignment, string>(HorizontalAlignment.Center, "center"),
            new KeyValuePair<HorizontalAlignment, string>(HorizontalAlignment.Right, "right"),
            new KeyValuePair<HorizontalAlignment, string>(HorizontalAlignment.Fill, "left"),
            new KeyValuePair<HorizontalAlignment, string>(HorizontalAlignment.Justify, "left"),
            new KeyValuePair<HorizontalAlignment, string>(HorizontalAlignment.CenterSelection, "center"),
            new KeyValuePair<HorizontalAlignment, string>(HorizontalAlignment.General, "left")
        };

        private List<KeyValuePair<VerticalAlignment, string>> VALIGN = new List<KeyValuePair<VerticalAlignment, string>>() {
            new KeyValuePair<VerticalAlignment, string>(VerticalAlignment.Bottom, "bottom"),
            new KeyValuePair<VerticalAlignment, string>(VerticalAlignment.Center, "middle"),
            new KeyValuePair<VerticalAlignment, string>(VerticalAlignment.Top, "top")
        };

        private List<KeyValuePair<BorderStyle, string>> BORDER = new List<KeyValuePair<BorderStyle, string>>() {
            new KeyValuePair<BorderStyle, string>(BorderStyle.DashDot, "dashed 1pt"),
            new KeyValuePair<BorderStyle, string>(BorderStyle.DashDotDot, "dashed 1pt"),
            new KeyValuePair<BorderStyle, string>(BorderStyle.Dashed, "dashed 1pt"),
            new KeyValuePair<BorderStyle, string>(BorderStyle.Dotted, "dotted 1pt"),
            new KeyValuePair<BorderStyle, string>(BorderStyle.Double, "double 3pt"),
            new KeyValuePair<BorderStyle, string>(BorderStyle.Hair, "dashed 1px"),
            new KeyValuePair<BorderStyle, string>(BorderStyle.Medium, "solid 2pt"),
            new KeyValuePair<BorderStyle, string>(BorderStyle.MediumDashDot, "dashed 2pt"),
            new KeyValuePair<BorderStyle, string>(BorderStyle.MediumDashDotDot, "dashed 2pt"),
            new KeyValuePair<BorderStyle, string>(BorderStyle.MediumDashed, "dashed 2pt"),
            new KeyValuePair<BorderStyle, string>(BorderStyle.None, "none"),
            new KeyValuePair<BorderStyle, string>(BorderStyle.SlantedDashDot, "dashed 2pt"),
            new KeyValuePair<BorderStyle, string>(BorderStyle.Thick, "solid 3pt"),
            new KeyValuePair<BorderStyle, string>(BorderStyle.Thin, "solid 1pt")
        };

        public ExcelToHTML(IWorkbook wb)
        {
            this.wb = wb;
        }

        public ExcelToHTML(string path)
        {
            using (var inputfs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                this.wb = WorkbookFactory.Create((Stream)inputfs);
            }
        }

        public string ToHtml(int sheetIndex = 0, bool completeHtmls = true, bool needTitle = true)
        {
            return ToHtml(wb.GetSheetName(sheetIndex), completeHtmls, needTitle);
        }

        public string ToHtml(string sheetName, bool completeHtmls = true, bool needTitle = true)
        {
            StringBuilder sbRet = new StringBuilder();

            if (completeHtmls)
            {
                sbRet.Append("<?xml version=\"1.0\" encoding=\"iso-8859-1\" ?>\n");
                sbRet.Append("<html>\n");
                sbRet.Append("<head>\n");
            }
            sbRet.Append(GetInlineStyle());
            if (completeHtmls)
            {
                sbRet.Append("</head>\n");
                sbRet.Append("<body>\n");
            }
            sbRet.Append(GetSheets(sheetName, needTitle));
            if (completeHtmls)
            {
                sbRet.Append("</body>\n");
                sbRet.Append("</html>\n");
            }

            return sbRet.ToString();
        }

        private string GetSheets(string sheetName, bool needTitle)
        {
            StringBuilder sbRet = new StringBuilder();

            ISheet sheet = wb.GetSheet(sheetName);
            sbRet.Append(GetSheet(sheet, needTitle));

            return sbRet.ToString();
        }

        private string GetSheet(ISheet sheet, bool needTitle)
        {
            StringBuilder sbRet = new StringBuilder();

            List<KeyValuePair<int, int>> widths = computeWidths(sheet);
            int tableWidth = widths.Where(o => o.Key == IDX_TABLE_WIDTH).First().Value;
            sbRet.Append(string.Format("<table class={0} cellspacing=\"0\" cellpadding=\"0\" style=\"width:{1}px;\">\n", DEFAULTS_CLASS, tableWidth));
            sbRet.Append(GetCols(widths, needTitle));
            sbRet.Append(GetSheetContent(sheet, needTitle));
            sbRet.Append("</table>\n");

            return sbRet.ToString();
        }

        private string GetColumnHeads()
        {
            StringBuilder sbRet = new StringBuilder();

            sbRet.Append(string.Format("<thead>\n"));
            sbRet.Append(string.Format("  <tr class={0}>\n", COL_HEAD_CLASS));
            sbRet.Append(string.Format("    <th class={0}>◊</th>\n", COL_HEAD_CLASS));
            //noinspection UnusedDeclaration
            for (int i = firstColumn; i < endColumn; i++)
            {
                StringBuilder colName = new StringBuilder();

                int cnum = i;
                do
                {
                    colName.Insert(0, (char)('A' + cnum % 26));
                    cnum /= 26;
                } while (cnum > 0);

                sbRet.Append(string.Format("    <th class={0}>{1}</th>\n", COL_HEAD_CLASS, colName));
            }
            sbRet.Append("  </tr>\n");
            sbRet.Append("</thead>\n");

            return sbRet.ToString();
        }

        private string GetSheetContent(ISheet sheet, bool needTitle)
        {
            StringBuilder sbRet = new StringBuilder();

            if (needTitle)
            {
                sbRet.Append(GetColumnHeads());
            }

            sbRet.Append(string.Format("<tbody>\n"));
            IEnumerator rows = sheet.GetRowEnumerator();
            while (rows.MoveNext())
            {
                IRow row = (IRow)rows.Current;

                sbRet.Append(string.Format("  <tr>\n"));
                if (needTitle)
                {
                    sbRet.Append(string.Format("    <td class={0}>{1}</td>\n", ROW_HEAD_CLASS, row.RowNum + 1));
                }

                StringBuilder sbTemp = new StringBuilder();
                int mergeCnt = 0;
                ICell preCell = null;
                ICell cell = null;

                for (int i = firstColumn; i < endColumn; i++)
                {
                    String content = " ";
                    String attrs = "";
                    ICellStyle style = null;
                    bool isMerge = false;

                    if (i >= row.FirstCellNum && i < row.LastCellNum)
                    {
                        cell = row.GetCell(i);
                        if (cell != null)
                        {
                            isMerge = cell.IsMergedCell;
                            style = cell.CellStyle;
                            attrs = tagStyle(cell, style);
                            //Set the value that is rendered for the cell
                            //also applies the format
                            string format = style.GetDataFormatString();
                            if (format.IsPresent())
                            {
                                MyCellFormat cf = MyCellFormat.GetInstance(format);
                                CellFormatResult result = cf.Apply(cell);
                                content = result.Text; //never null
                            }
                            if (string.IsNullOrEmpty(content))
                            {
                                content = " ";
                            }
                        }
                    }

                    if (isMerge == true && content == " ")
                    {
                        /*
                         * 因为 NPOI 返回的 cell 没有 mergeCnt 属性，只有一个 IsMergedCell 属性
                         * 如果有5个单元格，后面四个单元格合并成一个大单元格
                         * 它返回的其实还是5个单元格，IsMergedCell 分别是： false,true,true,true,true
                         * 上头这种情况还算好，我们好歹还能猜到后面四个单元格是合并单元格
                         *
                         * 但是如果第一个单独，后面四个每两个合并呢？
                         * TMD返回的还是5个单元格，IsMergedCell 仍然是： false,true,true,true,true
                         * 所以这里是有问题的，我没法知道后面的四个单元格是四个合并成一个呢，还是两个两个的分别合并
                         * 这个是没办法的，除非从NPOI的源代码里头去解决这个问题，介于上班呢，要求的是出结果，所以公司是
                         * 不太会允许我去干这种投入产出比较差的事情的，所以这个问题我采用了一个成本比较低的办法来绕开
                         *
                         * 办法就是我们在定义模板的时候，可以通过为每一个合并单元格添加内容来避免。
                         * 比如说 cell1（内容）, cell2,cell3（内容）, cell4,cell5（内容）
                         * 这样的话我就能知道 cell1 IsMergedCell = false 是一个独立的单元格
                         * cell2, cell3, cell4, cell5 的 IsMergedCell 虽然都是 true， 但是因为 cell4 这个位置有内容了，
                         * 那我就晓得 cell2 和 cell3 是合并的， cell4 和 cell5 也是合并的。
                         *
                         * 当然这里还会有个小小的问题，如果 cell4, cell5 里头是一个会被替换掉的内容，也即 $[字段] 这样的东西
                         * 如果实际的内容为 null 那么 cell4, cell5 合并单元格的内容也就是 null 了，这又回到了之前的问题了，
                         * 所以此处要求定义模板的时候 $[内容] 后面加一个空格，这样在生成 html 的时候，其实是不影响打印效果的。
                         * 也即 “$[] ”注意双引号里头的 “]”后头有个空格
                         */
                        if (mergeCnt == 1 && preCell != null && preCell.IsMergedCell == false)
                        {
                            sbTemp.Append(string.Format("    <td class={0} {1}{3}>{2}</td>\n", styleName(style), attrs, content, (isMerge) ? " colspan=\"1\"" : ""));
                        }
                        else
                        {
                            mergeCnt++;
                        }
                    }
                    else
                    {
                        sbTemp.Replace("colspan=\"1\"", string.Format("colspan=\"{0}\"", mergeCnt));
                        mergeCnt = 1;
                        sbTemp.Append(string.Format("    <td class={0} {1}{3}>{2}</td>\n", styleName(style), attrs, content, (isMerge) ? " colspan=\"1\"" : ""));
                    }
                    preCell = cell;
                }
                sbRet.Append(sbTemp.Replace("colspan=\"1\"", string.Format("colspan=\"{0}\"", mergeCnt)).ToString());


                sbRet.Append(string.Format("  </tr>\n"));
            }
            sbRet.Append(string.Format("</tbody>\n"));


            return sbRet.ToString();
        }

        private String tagStyle(ICell cell, ICellStyle style)
        {
            if (style.Alignment == HorizontalAlignment.General)
            {
                switch (ultimateCellType(cell))
                {
                    case CellType.String:
                        return "style=\"text-align: left;\"";
                    case CellType.Boolean:
                    case CellType.Error:
                        return "style=\"text-align: center;\"";
                    case CellType.Numeric:
                    default:
                        // "right" is the default
                        break;
                }
            }
            return "";
        }

        private static CellType ultimateCellType(ICell c)
        {
            CellType type = c.CellType;
            if (type == CellType.Formula)
            {
                type = c.CachedFormulaResultType;
            }
            return type;
        }

        private string GetCols(List<KeyValuePair<int, int>> widths, bool needTitle)
        {
            StringBuilder sbRet = new StringBuilder();

            if (needTitle)
            {
                int headerColWidth = widths.Where(o => o.Key == IDX_HEADER_COL_WIDTH).First().Value;
                sbRet.Append(string.Format("<col style=\"width:{0}px\"/>\n", headerColWidth));
            }
            for (int i = firstColumn; i < endColumn; i++)
            {
                int colWidth = widths.Where(o => o.Key == i).First().Value;
                sbRet.Append(string.Format("<col style=\"width:{0}px;\"/>\n", colWidth));
            }

            return sbRet.ToString();
        }

        private List<KeyValuePair<int, int>> computeWidths(ISheet sheet)
        {
            List<KeyValuePair<int, int>> ret = new List<KeyValuePair<int, int>>();
            int tableWidth = 0;

            ensureColumnBounds(sheet);

            // compute width of the header column
            int lastRowNum = sheet.LastRowNum;
            int headerCharCount = lastRowNum.ToString().Length;
            int headerColWidth = widthToPixels((headerCharCount + 1) * 256);
            ret.Add(new KeyValuePair<int, int>(IDX_HEADER_COL_WIDTH, headerColWidth));
            tableWidth += headerColWidth;

            for (int i = firstColumn; i < endColumn; i++)
            {
                int colWidth = widthToPixels(sheet.GetColumnWidth(i));
                ret.Add(new KeyValuePair<int, int>(i, colWidth));
                tableWidth += colWidth;
            }

            ret.Add(new KeyValuePair<int, int>(IDX_TABLE_WIDTH, tableWidth));
            return ret;
        }

        private int widthToPixels(double widthUnits)
        {
            return (int)(Math.Round(widthUnits * 9 / 256));
        }

        private void ensureColumnBounds(ISheet sheet)
        {
            if (gotBounds) return;

            IEnumerator iter = sheet.GetRowEnumerator();
            if (iter.MoveNext()) firstColumn = 0;
            else firstColumn = int.MaxValue;

            endColumn = 0;
            iter.Reset();
            while (iter.MoveNext())
            {
                IRow row = (IRow)iter.Current;
                short firstCell = row.FirstCellNum;
                if (firstCell >= 0)
                {
                    firstColumn = Math.Min(firstColumn, firstCell);
                    endColumn = Math.Max(endColumn, row.LastCellNum);
                }
            }
            gotBounds = true;
        }

        private string GetInlineStyle()
        {
            StringBuilder sbRet = new StringBuilder();

            sbRet.Append("<style type=\"text/css\">\n");
            sbRet.Append(GetStyles());
            sbRet.Append("</style>\n");

            return sbRet.ToString();
        }

        private string GetStyles()
        {
            StringBuilder sbRet = new StringBuilder();

            HashSet<ICellStyle> seen = new HashSet<ICellStyle>();
            for (int i = 0; i < wb.NumberOfSheets; i++)
            {
                ISheet sheet = wb.GetSheetAt(i);
                IEnumerator rows = sheet.GetRowEnumerator();
                while (rows.MoveNext())
                {
                    IRow row = (IRow)rows.Current;
                    foreach (ICell cell in row)
                    {
                        ICellStyle style = cell.CellStyle;
                        if (!seen.Contains(style))
                        {
                            sbRet.Append(GetStyle(style));
                            seen.Add(style);
                        }
                    }
                }
            }

            return sbRet.ToString();
        }

        private string GetStyle(ICellStyle style)
        {
            StringBuilder sbRet = new StringBuilder();

            sbRet.Append(string.Format(".{0} .{1} {{\n", DEFAULTS_CLASS, styleName(style)));
            sbRet.Append(styleContents(style));
            sbRet.Append("}\n");

            return sbRet.ToString();
        }

        private string styleContents(ICellStyle style)
        {
            StringBuilder sbRet = new StringBuilder();

            sbRet.Append(styleOut("text-align", style.Alignment));
            sbRet.Append(styleOut("vertical-align", style.VerticalAlignment));
            sbRet.Append(fontStyle(style));
            sbRet.Append(borderStyles(style));
            sbRet.Append(colorStyles(style));

            return sbRet.ToString();
        }

        private string colorStyles(ICellStyle style)
        {
            StringBuilder sbRet = new StringBuilder();

            //sbRet.Append("还未实现！");

            return sbRet.ToString();
        }

        private string borderStyles(ICellStyle style)
        {
            StringBuilder sbRet = new StringBuilder();

            sbRet.Append(styleOut("border-left", style.BorderLeft));
            /*
             * NPOI有BUG，合并单元格的 border-right 永远都是 None
             * 我们可以通过设置合并单元格后边那个单元格的左边框的解决
             * 如果当前合并单元格已经合并到最后一列了，我们就只能再加一列了，为了不影响打印效果
             * 这最后加的这一列在设置好左边框后，需要把宽度设置得很小，比如说0.1这样
             */
            sbRet.Append(styleOut("border-right", style.BorderRight));
            sbRet.Append(styleOut("border-top", style.BorderTop));
            sbRet.Append(styleOut("border-bottom", style.BorderBottom));

            return sbRet.ToString();
        }

        private string fontStyle(ICellStyle style)
        {
            StringBuilder sbRet = new StringBuilder();

            IFont font = style.GetFont(wb);

            if (font.Boldweight == 0)
            {
                sbRet.Append("  font-weight: bold;\n");
            }
            if (font.IsItalic)
            {
                sbRet.Append("  font-style: italic;\n");
            }

            double fontheight = font.FontHeight / 10 - 10;
            if (fontheight == 9)
            {
                //fix for stupid ol Windows
                fontheight = 10;
            }
            sbRet.Append(string.Format("  font-size: {0}pt;\n", fontheight));

            return sbRet.ToString();
        }

        private string styleOut(string k, HorizontalAlignment p)
        {
            return k + ":" + HALIGN.Where(o => o.Key == p).First().Value + ";\n";
        }
        private string styleOut(string k, VerticalAlignment p)
        {
            return k + ":" + VALIGN.Where(o => o.Key == p).First().Value + ";\n";
        }
        private string styleOut(string k, BorderStyle p)
        {
            return k + ":" + BORDER.Where(o => o.Key == p).First().Value + ";\n";
        }

        private string styleName(ICellStyle style)
        {
            if (style == null)
            {
                style = wb.GetCellStyleAt((short)0);
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("style_{0}", style.Index));
            return sb.ToString();
        }
    }
    /// <summary>
    /// 这个东西是为了解决 NPOI CellFormat 的BUG而存在的。
    /// 它在读取 日期格式 的时候有时候会报错。
    /// </summary>
    public class MyCellFormat
    {
        private CellFormat cellformat = null;

        private MyCellFormat(string format)
        {
            this.cellformat = CellFormat.GetInstance(format);
        }

        public static MyCellFormat GetInstance(string format)
        {
            return new MyCellFormat(format);
        }

        public CellFormatResult Apply(ICell cell)
        {
            try
            {
                return cellformat.Apply(cell);
            }
            catch (Exception)
            {
                var formatStr = cell.CellStyle.GetDataFormatString();
                var mc = new Regex(@"(yy|M|d|H|s|ms)").Match(formatStr);
                /*
                 * 目前全部不能正常转换的日期格式都转换成 yyyy - MM - dd 的形式
                 * 比如说：【[$-F800]dddd\,\ mmmm\ dd\,\ yyyy】这个格式
                 * 稍微 google 了下（ https://msdn.microsoft.com/en-us/library/dd318693(VS.85).aspx）
                 * 这个字符串 0x0800 表示 [System default locale language]
                 * 因时间关系，只能干完手头的活之后再慢慢研究了。
                 */
                if (mc.Success)
                {
                    return CellFormat.GetInstance("yyyy-MM-dd").Apply(cell);
                }
                else return cellformat.Apply(cell.ToString() + "<!-- This is the bug of NPOI, Maybe you should modify the file which name is \"MyCellFormat.cs\" -->");
            }
        }

        public CellFormatResult Apply(Object v)
        {
            return cellformat.Apply(v);
        }
    }
}