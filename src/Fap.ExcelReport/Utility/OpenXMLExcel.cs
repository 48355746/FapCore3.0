using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using System.Text.RegularExpressions;
using System.Data;

namespace Fap.ExcelReport.Utility
{
    /// <summary>
    /// 暂时无用，以后解决excel模板图片问题
    /// </summary>
    public class OpenXMLExcel : IDisposable
    {
        #region Constrution and dispose

        SpreadsheetDocument spreadSheet;
        public WorksheetPart CurrentWorksheetPart { get; set; }
        SharedStringTablePart shareStringPart;
        public OpenXMLExcel(Stream stream, bool bCreate)
        {
            if (bCreate == true)
            {
                spreadSheet = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);
                WorkbookPart workbookPart = spreadSheet.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = InsertWorksheet(spreadSheet.WorkbookPart);

                if (spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
                {
                    shareStringPart = spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
                }
                else
                {
                    shareStringPart = spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart>();
                }

                shareStringPart.SharedStringTable = new SharedStringTable();
                shareStringPart.SharedStringTable.Count = 1;
                shareStringPart.SharedStringTable.UniqueCount = 1;

                CurrentWorksheetPart = worksheetPart;
            }
            else
            {
                spreadSheet = SpreadsheetDocument.Open(stream, true);
                shareStringPart = spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
                CurrentWorksheetPart = spreadSheet.WorkbookPart.WorksheetParts.First();
            }
        }
        public void Dispose()
        {
            spreadSheet.Close();
            spreadSheet.Dispose();
        }

        #endregion
        #region Public interface
        public void WriteData(int x, int y, string strContent)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Column1");
            dt.Rows.Add(strContent);
            WriteDataIntoWorkSheet(x, y, dt);
        }

        public void WriteDataIntoWorkSheet(int startx, int starty, DataTable dt)
        {
            //if (startx < 1)
            //    startx = 1;
            //if (starty < 1)
            //    starty = 1;

            WorksheetPart worksheetPart = CurrentWorksheetPart;
            //starty -= 1;
            int j = 0;
            foreach (DataRow dr in dt.Rows)
            {
                j++;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string name = GetColumnName(i + starty);
                    string text = Convert.IsDBNull(dr[i]) ? "" : dr[i].ToString();
                    int index = InsertSharedStringItem(text, shareStringPart);
                    Cell cell = InsertCellInWorksheet(name, Convert.ToUInt32(j + startx), worksheetPart);

                    cell.CellValue = new CellValue(index.ToString());
                    cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                    worksheetPart.Worksheet.Save();
                }
            }
        }
        public void WriteDataIntoWorkSheet(int startx, int starty, string[][] data)
        {
            WorksheetPart worksheetPart = CurrentWorksheetPart;
            starty -= 1;
            int i = 0;
            foreach (string[] row in data)
            {
                int j = 0;
                foreach (string text in row)
                {
                    string name = GetColumnName(j + starty);
                    int index = InsertSharedStringItem(text, shareStringPart);
                    Cell cell = InsertCellInWorksheet(name, Convert.ToUInt32(i + startx), worksheetPart);

                    cell.CellValue = new CellValue(index.ToString());
                    cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                    worksheetPart.Worksheet.Save();
                    j++;
                }
                i++;
            }
        }
        public void AddNewWorksheet()
        {
            WorkbookPart workbookPart = spreadSheet.WorkbookPart;

            WorksheetPart newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            newWorksheetPart.Worksheet = new Worksheet(new SheetData());
            newWorksheetPart.Worksheet.Save();
            CurrentWorksheetPart = newWorksheetPart;
            //workbookPart.SharedStringTablePart .SharedStringTable.Count = workbookPart.SharedStringTablePart .SharedStringTable.Count + 1;
            //workbookPart.SharedStringTablePart .SharedStringTable.UniqueCount = workbookPart.SharedStringTablePart .SharedStringTable.UniqueCount + 1;
            string relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);
            Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Count() > 0)
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            string sheetName = "Sheet" + sheetId;

            // Append the new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
            sheets.Append(sheet);
            workbookPart.Workbook.Save();
        }

        public void InsertImage(long x, long y, long? width, long? height, string sImagePath)
        {
            try
            {
                WorksheetPart wsp = CurrentWorksheetPart;
                DrawingsPart dp;
                ImagePart imgp;
                WorksheetDrawing wsd;

                ImagePartType ipt;
                switch (sImagePath.Substring(sImagePath.LastIndexOf('.') + 1).ToLower())
                {
                    case "png":
                        ipt = ImagePartType.Png;
                        break;
                    case "jpg":
                    case "jpeg":
                        ipt = ImagePartType.Jpeg;
                        break;
                    case "gif":
                        ipt = ImagePartType.Gif;
                        break;
                    default:
                        return;
                }

                if (wsp.DrawingsPart == null)
                {
                    //----- no drawing part exists, add a new one

                    dp = wsp.AddNewPart<DrawingsPart>();
                    imgp = dp.AddImagePart(ipt, wsp.GetIdOfPart(dp));
                    wsd = new WorksheetDrawing();
                }
                else
                {
                    //----- use existing drawing part

                    dp = wsp.DrawingsPart;
                    imgp = dp.AddImagePart(ipt);
                    dp.CreateRelationshipToPart(imgp);
                    wsd = dp.WorksheetDrawing;
                }

                using (FileStream fs = new FileStream(sImagePath, FileMode.Open))
                {
                    imgp.FeedData(fs);
                }

                int imageNumber = dp.ImageParts.Count<ImagePart>();
                if (imageNumber == 1)
                {
                    Drawing drawing = new Drawing();
                    drawing.Id = dp.GetIdOfPart(imgp);
                    CurrentWorksheetPart.Worksheet.Append(drawing);
                }

                NonVisualDrawingProperties nvdp = new NonVisualDrawingProperties();
                nvdp.Id = new UInt32Value((uint)(1024 + imageNumber));
                nvdp.Name = "Picture " + imageNumber.ToString();
                nvdp.Description = "";
                DocumentFormat.OpenXml.Drawing.PictureLocks picLocks = new DocumentFormat.OpenXml.Drawing.PictureLocks();
                picLocks.NoChangeAspect = true;
                picLocks.NoChangeArrowheads = true;
                NonVisualPictureDrawingProperties nvpdp = new NonVisualPictureDrawingProperties();
                nvpdp.PictureLocks = picLocks;
                NonVisualPictureProperties nvpp = new NonVisualPictureProperties();
                nvpp.NonVisualDrawingProperties = nvdp;
                nvpp.NonVisualPictureDrawingProperties = nvpdp;

                DocumentFormat.OpenXml.Drawing.Stretch stretch = new DocumentFormat.OpenXml.Drawing.Stretch();
                stretch.FillRectangle = new DocumentFormat.OpenXml.Drawing.FillRectangle();

                BlipFill blipFill = new BlipFill();
                DocumentFormat.OpenXml.Drawing.Blip blip = new DocumentFormat.OpenXml.Drawing.Blip();
                blip.Embed = dp.GetIdOfPart(imgp);
                blip.CompressionState = DocumentFormat.OpenXml.Drawing.BlipCompressionValues.Print;
                blipFill.Blip = blip;
                blipFill.SourceRectangle = new DocumentFormat.OpenXml.Drawing.SourceRectangle();
                blipFill.Append(stretch);

                DocumentFormat.OpenXml.Drawing.Transform2D t2d = new DocumentFormat.OpenXml.Drawing.Transform2D();
                DocumentFormat.OpenXml.Drawing.Offset offset = new DocumentFormat.OpenXml.Drawing.Offset();
                offset.X = 0;
                offset.Y = 0;
                t2d.Offset = offset;
                Bitmap bm = new Bitmap(sImagePath);

                DocumentFormat.OpenXml.Drawing.Extents extents = new DocumentFormat.OpenXml.Drawing.Extents();

                if (width == null)
                    extents.Cx = (long)bm.Width * (long)((float)914400 / bm.HorizontalResolution);
                else
                    extents.Cx = width * (long)((float)914400 / bm.HorizontalResolution);

                if (height == null)
                    extents.Cy = (long)bm.Height * (long)((float)914400 / bm.VerticalResolution);
                else
                    extents.Cy = height * (long)((float)914400 / bm.VerticalResolution);

                bm.Dispose();
                t2d.Extents = extents;
                ShapeProperties sp = new ShapeProperties();
                sp.BlackWhiteMode = DocumentFormat.OpenXml.Drawing.BlackWhiteModeValues.Auto;
                sp.Transform2D = t2d;
                DocumentFormat.OpenXml.Drawing.PresetGeometry prstGeom = new DocumentFormat.OpenXml.Drawing.PresetGeometry();
                prstGeom.Preset = DocumentFormat.OpenXml.Drawing.ShapeTypeValues.Rectangle;
                prstGeom.AdjustValueList = new DocumentFormat.OpenXml.Drawing.AdjustValueList();
                sp.Append(prstGeom);
                sp.Append(new DocumentFormat.OpenXml.Drawing.NoFill());

                DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture picture = new DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture();
                picture.NonVisualPictureProperties = nvpp;
                picture.BlipFill = blipFill;
                picture.ShapeProperties = sp;

                Position pos = new Position();
                pos.X = x * 914400 / 72;
                pos.Y = y * 914400 / 72;
                Extent ext = new Extent();
                ext.Cx = extents.Cx;
                ext.Cy = extents.Cy;
                AbsoluteAnchor anchor = new AbsoluteAnchor();
                anchor.Position = pos;
                anchor.Extent = ext;
                anchor.Append(picture);
                anchor.Append(new ClientData());
                wsd.Append(anchor);
                wsd.Save(dp);
            }
            catch (Exception ex)
            {
                throw ex; // or do something more interesting if you want
            }
        }

        public void InsertImage(long x, long y, string sImagePath)
        {
            InsertImage(x, y, null, null, sImagePath);
        }

        #endregion
        #region private static OpenXml methods

        private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
                shareStringPart.SharedStringTable.Count = 1;
                shareStringPart.SharedStringTable.UniqueCount = 1;
            }
            int i = 0;
            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }
                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }

        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                    {
                        refCell = cell;
                        break;
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                worksheet.Save();
                return newCell;
            }
        }

        private static WorksheetPart InsertWorksheet(WorkbookPart workbookPart)
        {
            // Add a new worksheet part to the workbook.
            WorksheetPart newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            newWorksheetPart.Worksheet = new Worksheet(new SheetData());
            newWorksheetPart.Worksheet.Save();

            workbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
            string relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);

            // Get a unique ID for the new sheet.
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Count() > 0)
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            string sheetName = "Sheet" + sheetId;

            // Append the new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
            sheets.Append(sheet);

            workbookPart.Workbook.Save();
            return newWorksheetPart;
        }

        // Given a Worksheet and a cell name, verifies that the specified cell exists.
        // If it does not exist, creates a new cell. 
        private static void CreateSpreadsheetCellIfNotExist(Worksheet worksheet, string cellName)
        {
            string columnName = GetColumnName(cellName);
            uint rowIndex = GetRowIndex(cellName);

            IEnumerable<Row> rows = worksheet.Descendants<Row>().Where(r => r.RowIndex.Value == rowIndex);

            // If the Worksheet does not contain the specified row, create the specified row.
            // Create the specified cell in that row, and insert the row into the Worksheet.
            if (rows.Count() == 0)
            {
                Row row = new Row() { RowIndex = new UInt32Value(rowIndex) };
                Cell cell = new Cell() { CellReference = new StringValue(cellName) };
                row.Append(cell);
                worksheet.Descendants<SheetData>().First().Append(row);
                worksheet.Save();
            }
            else
            {
                Row row = rows.First();

                IEnumerable<Cell> cells = row.Elements<Cell>().Where(c => c.CellReference.Value == cellName);

                // If the row does not contain the specified cell, create the specified cell.
                if (cells.Count() == 0)
                {
                    Cell cell = new Cell() { CellReference = new StringValue(cellName) };
                    row.Append(cell);
                    worksheet.Save();
                }
            }

        }
        // Given a cell name, parses the specified cell to get the column name.
        private static string GetColumnName(string cellName)
        {
            // Create a regular expression to match the column name portion of the cell name.
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellName);

            return match.Value;
        }

        // Given a cell name, parses the specified cell to get the row index.
        private static uint GetRowIndex(string cellName)
        {
            // Create a regular expression to match the row index portion the cell name.
            Regex regex = new Regex(@"\d+");
            Match match = regex.Match(cellName);

            return uint.Parse(match.Value);
        }

        #endregion
        #region Utility methods
        private static string GetColumnName(int index)
        {
            string name = "";
            char[] columnNames = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            int num = index;
            do
            {
                int i = num % 26;
                name = columnNames[i] + name;
                num = num / 26 - 1;
            } while (num > -1);
            if (string.IsNullOrEmpty(name))
                name = "A";
            return name;
        }
        #endregion
    }
}

