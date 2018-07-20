using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace Game.Core.Tools.ExcelToData
{
    public class Workbook
    {
        private string filePath = null;
        private Dictionary<string, WorkSheet> sheets = new Dictionary<string, WorkSheet>();

        public Workbook(string filePath)
        {
            this.filePath = filePath;
        }

        private LogMsgMgr logMsgMgr
        {
            get { return LogMsgMgr.GetInstance(); }
        }

        public bool ReadWorkbook()
        {
            bool result = true;
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                logMsgMgr.Add(new ErrorLogData1(LogConst.E_WorkBook_NotFound,filePath == null?"":filePath));
                result = false;
            }else
            {
                string ext = Path.GetExtension(filePath);
                if (ext != ".xlsx" && ext != ".xls")
                {
                    logMsgMgr.Add(new ErrorLogData1(LogConst.E_WorkBook_NotExcel, filePath));
                    result = false;
                }else
                {
                    logMsgMgr.Add(new InfoLogData1(LogConst.I_WorkBook_Start, filePath));
                    logMsgMgr.Indent++;

                    FileStream fs = null;
                    try
                    {
                        fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        IWorkbook workbook = null;
                        if (ext == ".xlsx")
                        {
                            workbook = new XSSFWorkbook(fs);
                        }
                        else
                        {
                            workbook = new HSSFWorkbook(fs);
                        }
                        if (workbook != null)
                        {
                            int sheetCount = workbook.NumberOfSheets;
                            if (sheetCount > 0)
                            {
                                for (int i = 0; i < sheetCount; i++)
                                {
                                    ISheet sheet = workbook.GetSheetAt(i);
                                    if (sheet == null)
                                    {
                                        logMsgMgr.Add(new ErrorLogData2(LogConst.E_WorkBook_SheetNull, filePath, "" + i));
                                    }
                                    else if (string.IsNullOrEmpty(sheet.SheetName))
                                    {
                                        logMsgMgr.Add(new ErrorLogData2(LogConst.E_WorkSheet_NameNull, filePath, "" + i));
                                    }
                                    else
                                    {
                                        WorkSheet wSheet = new WorkSheet();
                                        if (wSheet.ReadSheet(sheet))
                                        {
                                            sheets.Add(wSheet.Name, wSheet);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                logMsgMgr.Add(new WarningLogData1(LogConst.E_WorkBook_Empty, filePath));
                            }
                        }
                        else
                        {
                            logMsgMgr.Add(new ErrorLogData1(LogConst.E_WorkBook_Null, filePath));
                            result = false;
                        }
                    }
                    catch (Exception e)
                    {
                        logMsgMgr.Add(new ErrorLogData2(LogConst.E_WorkBook_Format, filePath, e.Message));
                        if (fs != null)
                        {
                            fs.Close();
                        }

                        result = false;
                    }

                    logMsgMgr.Indent--;
                    logMsgMgr.Add(new InfoLogData1(LogConst.I_WorkBook_End, filePath));
                    logMsgMgr.Add(new EmptyLogData());
                }
            }
            return result;
        }

        public List<DataSheetInfo> ConvertToDataSheet()
        {
            List<DataSheetInfo> result = new List<DataSheetInfo>();
            foreach(KeyValuePair<string,WorkSheet> kvp in sheets)
            {
                logMsgMgr.Add(new InfoLogData1(LogConst.I_WorkToData_Start, kvp.Value.Name));
                logMsgMgr.Indent++;
                DataSheetInfo dSheet = new DataSheetInfo();
                if(dSheet.ReadFromWorkSheet(kvp.Value))
                {
                    result.Add(dSheet);
                }
                logMsgMgr.Indent--;
                logMsgMgr.Add(new InfoLogData(LogConst.I_WorkToData_End));
                logMsgMgr.Add(new EmptyLogData());
            }
            return result;
        }
    }
}
