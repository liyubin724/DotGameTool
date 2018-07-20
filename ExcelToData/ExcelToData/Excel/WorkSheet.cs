using NPOI.SS.UserModel;
using System.Collections.Generic;

namespace Game.Core.Tools.ExcelToData
{
    public class WorkSheet
    {
        private string name = null;
        public string Name
        {
            get { return name; }
        }

        private List<List<WorkCell>> cells = new List<List<WorkCell>>();

        public WorkSheet()
        {
        }

        public int RowCount()
        {
            return cells.Count;
        }

        public int ColumnCount()
        {
            if(cells.Count>0)
            {
                return cells[0].Count;
            }
            return 0;
        }

        public string GetValue(int row,int col)
        {
            if(row >=0 && row < cells.Count)
            {
                List<WorkCell> rowValues = cells[row];
                if(col>=0 && col<rowValues.Count)
                {
                    return rowValues[col].value;
                }
            }
            return null;
        }

        private LogMsgMgr logMsgMgr
        {
            get { return LogMsgMgr.GetInstance(); }
        }

        public bool ReadSheet(ISheet sheet)
        {
            bool result = true;
            logMsgMgr.Indent++;
            if (sheet == null)
            {
                logMsgMgr.Add(new InfoLogData(LogConst.E_WorkSheet_Null));
                result = false;
            }
            else
            {
                name = sheet.SheetName;
                logMsgMgr.Indent++;
                logMsgMgr.Add(new InfoLogData1(LogConst.I_WorkSheet_Start, name));

                int rowCount = sheet.LastRowNum - sheet.FirstRowNum + 1;
                if (rowCount > 0)
                {
                    logMsgMgr.Indent++;
                    int firstCellIndex = 99999;
                    int lastCellIndex = -1;
                    for (int i = 0; i < rowCount; i++)
                    {
                        IRow row = sheet.GetRow(sheet.FirstRowNum + i);
                        if (row != null && row.FirstCellNum >= 0 && row.LastCellNum >= 0)
                        {
                            if (firstCellIndex > row.FirstCellNum)
                            {
                                firstCellIndex = row.FirstCellNum;
                            }
                            if (lastCellIndex < row.LastCellNum)
                            {
                                lastCellIndex = row.LastCellNum;
                            }
                        }
                    }

                    if (firstCellIndex < 0 || lastCellIndex < 0)
                    {
                        logMsgMgr.Add(new ErrorLogData1(LogConst.E_WorkSheet_Empty, name));
                        result = false;
                    }
                    else
                    {
                        for (int j = 0; j < rowCount; j++)
                        {
                            IRow row = sheet.GetRow(sheet.FirstRowNum + j);
                            if (row != null)
                            {
                                List<WorkCell> contents = new List<WorkCell>();
                                for (int i = firstCellIndex; i < lastCellIndex; i++)
                                {
                                    if (row.FirstCellNum > i)
                                    {
                                        contents.Add(null);
                                    }
                                    else if (row.LastCellNum < i)
                                    {
                                        contents.Add(null);
                                    }
                                    else
                                    {
                                        ICell cell = row.GetCell(i);
                                        string v = GetCellStringValue(cell);
                                        contents.Add(new WorkCell(sheet.FirstRowNum +j,i,v));
                                    }
                                }
                                cells.Add(contents);
                            }else
                            {
                                logMsgMgr.Add(new WarningLogData2(LogConst.E_WorkSheet_Row_Null, name, "" + j));
                            }
                        }
                    }
                    logMsgMgr.Indent--;
                    logMsgMgr.Add(new InfoLogData1(LogConst.I_WorkSheet_End, name));
                    logMsgMgr.Add(new EmptyLogData());
                }
                else
                {
                    logMsgMgr.Add(new InfoLogData(LogConst.E_WorkSheet_Row_Null));
                    result = false;
                }
                logMsgMgr.Indent--;
            }
            logMsgMgr.Indent--;
            return result;
        }

        private string GetCellStringValue(ICell cell)
        {
            if (cell == null)
                return null;
            CellType cType = cell.CellType;
            if (cType == CellType.String)
            {
                return cell.StringCellValue;
            }
            else if (cType == CellType.Numeric)
            {
                return cell.NumericCellValue.ToString();
            }
            else if (cType == CellType.Boolean)
            {
                return cell.BooleanCellValue.ToString();
            }
            else if (cType == CellType.Formula)
            {
                CellType fCellType = cell.CachedFormulaResultType;
                if (fCellType == CellType.Numeric)
                {
                    return cell.NumericCellValue.ToString();
                }
                else if (fCellType == CellType.String)
                {
                    return cell.StringCellValue;
                }
            }
            return null;
        }
    }
}
