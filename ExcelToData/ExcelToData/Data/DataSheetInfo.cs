using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Game.Core.Tools.ExcelToData
{
    public class DataSheetInfo
    {
        private string name = null;
        public string Name
        {
            get { return name; }
        }
        private List<DataFieldInfo> fields = new List<DataFieldInfo>();
        private List<DataLineInfo> contents = new List<DataLineInfo>();

        private static int MIN_ROW_COUNT = 7;
        private static int MIN_COLUMN_COUNT = 2;
        public DataSheetInfo()
        {

        }

        public DataFieldInfo GetFieldInfo(int index)
        {
            if(index>=0 && index<fields.Count)
            {
                return fields[index];
            }
            return null;
        }

        public int GetContentCount()
        {
            return contents.Count;
        }

        public int GetFieldCount()
        {
            return fields.Count;
        }

        public object GetContentValue(int row,int col,bool useDefault = false)
        {
            if(row >=0 && row <contents.Count && col >= 0 && col < fields.Count)
            {
                object result = contents[row].GetCellData(col).value;
                if(result == null)
                {
                    if(useDefault)
                    {
                        
                        DataFieldInfo dfInfo = fields[col];
                        if( dfInfo.type!= DataFieldType.Dic && dfInfo.type != DataFieldType.Array && !string.IsNullOrEmpty(dfInfo.defaultContent))
                        {
                            result = dfInfo.defaultContent;
                        }
                    }
                }

                return result;
            }
            return null;
        }

        private LogMsgMgr logMsg
        {
            get { return LogMsgMgr.GetInstance(); }
        }

        private int SortDataLineByID(DataLineInfo dli1,DataLineInfo dli2)
        {
            DataCellInfo dci1 = dli1.GetCellData(0);
            DataCellInfo dci2 = dli2.GetCellData(0);

            int id1 = int.Parse(dci1.GetValue<string>());
            int id2 = int.Parse(dci2.GetValue<string>());

            if (id1 == id2)
                return 0;
            else if (id1 > id2)
                return 1;
            else
                return -1;
        }

        public bool ReadFromWorkSheet(WorkSheet wsheet)
        {
            if(wsheet == null)
            {
                return false;
            }
            name = wsheet.Name;

            string nameReg = @"^[A-Z]\w+$";
            if (string.IsNullOrEmpty(name) || !Regex.IsMatch(name, nameReg))
            {
                logMsg.Add(new ErrorLogData1(LogConst.E_DataSheet_NameFormat, name));
                return false;
            }
            int rowCount = wsheet.RowCount();
            int columnCount = wsheet.ColumnCount();

            if(rowCount < MIN_ROW_COUNT)
            {
                logMsg.Add(new ErrorLogData2(LogConst.E_DataSheet_RowNum, name, "" + MIN_ROW_COUNT));
                return false;
            }
            if(columnCount<MIN_COLUMN_COUNT)
            {
                logMsg.Add(new ErrorLogData2(LogConst.E_DataSheet_ColNum, name, "" + MIN_COLUMN_COUNT));
                return false;
            }

            if(ParseToField(wsheet))
            {
                if(fields.Count>0)
                {
                    ParseToContent(wsheet);
                }
            }
            return true;
        }

        private bool ParseToField(WorkSheet wsheet)
        {
            int rowCount = wsheet.RowCount();
            int colCount = wsheet.ColumnCount();

            DataFieldType preFieldType = DataFieldType.None;
            for (int i = 1; i < colCount; i++)
            {
                string fName = wsheet.GetValue(0, i);
                string fTypeStr = wsheet.GetValue(1, i);
                string fExportStr = wsheet.GetValue(2, i);
                string fDesc = wsheet.GetValue(3, i);
                string fDefault = wsheet.GetValue(4, i);
                string fValidation = wsheet.GetValue(5, i);
                string fValidValue = wsheet.GetValue(6, i);

                if (string.IsNullOrEmpty(fName))
                {
                    if (preFieldType == DataFieldType.Dic || preFieldType == DataFieldType.Array)
                    {
                        continue;
                    }
                    else
                    {
                        logMsg.Add(new ErrorLogData1(LogConst.E_DataField_Empty, "" + i));
                        continue;
                    }
                }

                DataFieldType fType = DataHelper.GetFieldType(fTypeStr);
                if (fType == DataFieldType.None)
                {
                    logMsg.Add(new ErrorLogData2(LogConst.E_DataField_TypeNone, "" + i, string.IsNullOrEmpty(fTypeStr) ? "" : fTypeStr));
                    continue;
                }
                DataFieldExport fExport = DataHelper.GetFieldExport(fExportStr);
                if (fExport == DataFieldExport.None)
                {
                    logMsg.Add(new ErrorLogData1(LogConst.E_DataField_ExportNone, "" + i));
                    continue;
                }

                DataFieldInfo fInfo = new DataFieldInfo
                {
                    columnIndex = i,
                    name = fName,
                    type = fType,
                    export = fExport,
                    desc = fDesc,
                    validation = DataHelper.GetFieldValidationType(fValidation),
                    validationValue = fValidValue,
                    defaultContent = fDefault
                };

                if (fType == DataFieldType.Dic)
                {
                    DataHelper.GetFieldDicKeyInfo(fTypeStr, out fInfo.keyField, out fInfo.valueField);
                    if (fInfo.keyField == null || fInfo.valueField == null)
                    {
                        logMsg.Add(new ErrorLogData1(LogConst.E_DataField_DicType, "" + i));
                        continue;
                    }
                    if (fInfo.keyField.type == DataFieldType.Ref)
                    {
                        if (string.IsNullOrEmpty(fInfo.keyField.refName))
                        {
                            logMsg.Add(new ErrorLogData1(LogConst.E_DataField_DicKeyRef, "" + i));
                            continue;
                        }
                    }
                    if (fInfo.valueField.type == DataFieldType.Ref)
                    {
                        if (string.IsNullOrEmpty(fInfo.valueField.refName))
                        {
                            logMsg.Add(new ErrorLogData1(LogConst.E_DataField_DicValueRef, "" + i));
                            continue;
                        }
                    }
                }
                else if (fType == DataFieldType.Array)
                {
                    DataHelper.GetArrayFieldInfo(fTypeStr, out fInfo.valueField);
                    if (fInfo.valueField == null)
                    {
                        logMsg.Add(new ErrorLogData1(LogConst.E_DataField_ArrayType, "" + i));
                        continue;
                    }
                    else if (fInfo.valueField.type == DataFieldType.Ref)
                    {
                        if (string.IsNullOrEmpty(fInfo.valueField.refName))
                        {
                            logMsg.Add(new ErrorLogData1(LogConst.E_DataField_ArrayValueRef, "" + i));
                            continue;
                        }
                    }
                }
                else if (fType == DataFieldType.Ref)
                {
                    fInfo.refName = DataHelper.GetFieldRefName(fTypeStr);
                    if (string.IsNullOrEmpty(fInfo.refName))
                    {
                        logMsg.Add(new ErrorLogData1(LogConst.E_DataField_RefRef, "" + i));
                        continue;
                    }
                }else if(fType == DataFieldType.Res)
                {
                    fInfo.refName = DataHelper.GetFieldRefName(fTypeStr);
                    if (string.IsNullOrEmpty(fInfo.refName))
                    {
                        logMsg.Add(new ErrorLogData1(LogConst.E_DataField_ResType, "" + i));
                        continue;
                    }
                }
                preFieldType = fType;
                if(!AddField(fInfo))
                {
                    fields.Clear();
                    return false;
                }
            }

            return true;
        }

        private bool AddField(DataFieldInfo fieldInfo)
        {
            string nameReg = @"^[A-Z][A-Za-z0-9]{1,15}$";
            if (!Regex.IsMatch(fieldInfo.name, nameReg))
            {
                logMsg.Add(new ErrorLogData2(LogConst.E_DataField_NameFormat, ""+fieldInfo.columnIndex,fieldInfo.name));
                return false;
            }
            if(fields.Count == 0 && (fieldInfo.name != "ID" || fieldInfo.type != DataFieldType.Int))
            {
                logMsg.Add(new ErrorLogData(LogConst.E_DataField_ID));
                return false;
            }
            foreach (DataFieldInfo dfi in fields)
            {
                if(dfi.name == fieldInfo.name)
                {
                    logMsg.Add(new ErrorLogData2(LogConst.E_DataField_NameRepeat, ""+dfi.columnIndex, ""+fieldInfo.columnIndex));
                    return false;
                }
            }

            fields.Add(fieldInfo);

            return true;
        }

        private bool ParseToContent(WorkSheet wSheet)
        {
            int rowCount = wSheet.RowCount();
            int colCount = wSheet.ColumnCount();
            int startRow = MIN_ROW_COUNT;
            for (int j = startRow; j < rowCount; j++)
            {
                DataLineInfo content = new DataLineInfo(fields.Count);
                for (int i = 0; i < fields.Count; i++)
                {
                    DataFieldInfo fInfo = fields[i];
                    string value = wSheet.GetValue(j, fInfo.columnIndex);
                    if (fInfo.type == DataFieldType.Dic)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            logMsg.Add(new ErrorLogData2(LogConst.E_DataContent_DicLen, "" + j, "" + i));
                            continue;
                        }
                        else
                        {
                            if (int.TryParse(value, out int count))
                            {
                                int maxLen = -1;
                                if (i == fields.Count - 1)
                                {
                                    maxLen = colCount - fInfo.columnIndex - 1;
                                }
                                else
                                {
                                    maxLen = fields[i + 1].columnIndex - fInfo.columnIndex - 1;
                                }
                                if (count * 2 > maxLen)
                                {
                                    logMsg.Add(new ErrorLogData2(LogConst.E_DataContent_DicLarge, "" + j, "" + i));
                                    continue;
                                }
                                else
                                {
                                    Dictionary<string, string> result = new Dictionary<string, string>();
                                    for (int m = 1; m <= count; m++)
                                    {
                                        string k = wSheet.GetValue(j, fInfo.columnIndex + 2 * (m - 1) + 1);
                                        string v = wSheet.GetValue(j, fInfo.columnIndex + 2 * m); 
                                        if (result.ContainsKey(k))
                                        {
                                            logMsg.Add(new ErrorLogData2(LogConst.E_DataContent_DicLarge, "" + j, "" + i));
                                            result.Clear();
                                            break;
                                        }
                                        else
                                        {
                                            result.Add(k, v);
                                        }
                                    }
                                    content.AddCellData(i, j, fInfo.columnIndex, result);
                                }
                            }
                            else
                            {
                                logMsg.Add(new ErrorLogData2(LogConst.E_DataContent_DicLen, "" + j, "" + i));
                                continue;
                            }
                        }
                    }
                    else if (fInfo.type == DataFieldType.Array)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            logMsg.Add(new ErrorLogData2(LogConst.E_DataContent_DicLen, "" + j, "" + i));
                            continue;
                        }
                        else
                        {
                            if (int.TryParse(value, out int count))
                            {
                                int maxLen = -1;
                                if (i == fields.Count - 1)
                                {
                                    maxLen = colCount - fInfo.columnIndex - 1;
                                }
                                else
                                {
                                    maxLen = fields[i + 1].columnIndex - fInfo.columnIndex - 1;
                                }
                                if (count > maxLen)
                                {
                                    logMsg.Add(new ErrorLogData2(LogConst.E_DataContent_ArrayLarge, "" + j, "" + i));
                                    continue;
                                }
                                else
                                {
                                    List<string> result = new List<string>();
                                    for (int m = 1; m <= count; m++)
                                    {
                                        result.Add(wSheet.GetValue(j, fInfo.columnIndex + m));
                                    }
                                    content.AddCellData(i, j, fInfo.columnIndex, result);
                                }
                            }
                            else
                            {
                                logMsg.Add(new ErrorLogData2(LogConst.E_DataContent_DicLen, "" + j, "" + i));
                                continue;
                            }
                        }
                    }
                    else
                    {
                        string v = wSheet.GetValue(j, fInfo.columnIndex);
                        content.AddCellData(i, j, fInfo.columnIndex, v);
                    }
                }
                contents.Add(content);
            }

            contents.Sort(SortDataLineByID);

            return true;
        }
    }
}
