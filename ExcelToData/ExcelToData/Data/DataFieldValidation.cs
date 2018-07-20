using System;
using System.Collections.Generic;

namespace Game.Core.Tools.ExcelToData
{
    [Flags]
    public enum DataFieldValidationType
    {
        None = 0,
        NeverRepeat = 1 << 0,
        NeverNull = 1 << 1,
        StrLengthMax = 1 << 2,
        NumberRange = 1 << 3,
        Resource = 1 << 4,
    }

    public static class DataFieldValidationFactory
    {
        public static List<IDataFieldValidation> GetFieldValidation(DataFieldValidationType t)
        {
            List<IDataFieldValidation> validations = new List<IDataFieldValidation>();
            if((t&DataFieldValidationType.NeverRepeat)!=0)
            {
                validations.Add(new NeverRepeatFieldValidation());
            }
            if((t&DataFieldValidationType.NeverNull)!=0)
            {
                validations.Add(new NeverNullFieldValidation());
            }
            if((t&DataFieldValidationType.StrLengthMax)!=0)
            {
                validations.Add(new StrLengthMaxFieldValidation());
            }
            if((t&DataFieldValidationType.NumberRange)!=0)
            {
                validations.Add(new NumberRangeFieldValidation());
            }
            if((t&DataFieldValidationType.Resource)!=0)
            {
                validations.Add(new ResourceFieldValidation());
            }
            return validations;
        }
    }

    public interface IDataFieldValidation
    {
        LogMsgData[] IsValid(DataSheetInfo sInfo, int index);
    }

    public class NeverRepeatFieldValidation : IDataFieldValidation
    {
        public LogMsgData[] IsValid(DataSheetInfo sInfo, int index)
        {
            List<LogMsgData> result = new List<LogMsgData>();
            DataFieldInfo dfi = sInfo.GetFieldInfo(index);
            int count = sInfo.GetContentCount();
            Dictionary<string, bool> cacheData = new Dictionary<string, bool>();
            for(int i =0;i<count;i++)
            {
                object data = sInfo.GetContentValue(i, index);
                if(data!=null)
                {
                    string validaStr = null;
                    if(dfi.type == DataFieldType.Dic)
                    {
                        Dictionary<string, string> dicData = (Dictionary<string, string>)data;
                        List<string> tempStrs = new List<string>();
                        foreach(KeyValuePair<string,string> kvp in dicData)
                        {
                            string t = kvp.Key + ":" + (kvp.Value == null ? "null" : kvp.Value);
                            tempStrs.Add(t);
                        }
                        tempStrs.Sort();
                        validaStr = string.Join(",", tempStrs.ToArray());
                    }else if(dfi.type == DataFieldType.Array)
                    {
                        List<string> arrData = (List<string>)data;
                        arrData.Sort();
                        validaStr = string.Join(",", arrData.ToArray());
                    }else
                    {
                        validaStr = (string)data;
                    }
                    if (cacheData.ContainsKey(validaStr))
                    {
                        result.Add(new ErrorLogData2(LogConst.E_DataValidation_Repeat,""+index, validaStr));
                    }
                    else
                    {
                        cacheData.Add(validaStr, true);
                    }
                }
            }
            if (result.Count > 0)
                return result.ToArray();
            else
                return null;
        }
    }

    public class NeverNullFieldValidation : IDataFieldValidation
    {
        public LogMsgData[] IsValid(DataSheetInfo sInfo, int index)
        {
            List<LogMsgData> result = new List<LogMsgData>();
            DataFieldInfo dfi = sInfo.GetFieldInfo(index);
            int count = sInfo.GetContentCount();
            Dictionary<string, bool> cacheData = new Dictionary<string, bool>();
            for (int i = 0; i < count; i++)
            {
                object data = sInfo.GetContentValue(i, index);
                if (data == null)
                {
                    result.Add(new ErrorLogData1(LogConst.E_DataValidation_Null, "" + index));
                }
            }

            if (result.Count > 0)
                return result.ToArray();
            else
                return null;
        }
    }

    public class StrLengthMaxFieldValidation : IDataFieldValidation
    {
        public LogMsgData[] IsValid(DataSheetInfo sInfo, int index)
        {
            List<LogMsgData> result = new List<LogMsgData>();
            DataFieldInfo dfi = sInfo.GetFieldInfo(index);
            int len = 0;
            if(!int.TryParse(dfi.validationValue,out len))
            {
                return null;
            }
            int count = sInfo.GetContentCount();

            for (int i = 0; i < count; i++)
            {
                object data = sInfo.GetContentValue(i, index);
                if (data != null && (dfi.type == DataFieldType.String || dfi.type == DataFieldType.Stringt || dfi.type == DataFieldType.Res))
                {
                    string value = (string)data;
                    if(value.Length > len)
                    {
                        result.Add(new ErrorLogData3(LogConst.E_DataValidation_StrLen, "" + index,""+len,value));
                    }
                }
            }

            if (result.Count > 0)
                return result.ToArray();
            else
                return null;
        }
    }

    public class NumberRangeFieldValidation : IDataFieldValidation
    {
        public LogMsgData[] IsValid(DataSheetInfo sInfo, int index)
        {
            List<LogMsgData> result = new List<LogMsgData>();
            DataFieldInfo dfi = sInfo.GetFieldInfo(index);
            int len = 0;
            if (!int.TryParse(dfi.validationValue, out len))
            {
                return null;
            }
            int count = sInfo.GetContentCount();

            for (int i = 0; i < count; i++)
            {
                object data = sInfo.GetContentValue(i, index);
                if (data != null && (dfi.type == DataFieldType.Int || dfi.type == DataFieldType.Float|| dfi.type == DataFieldType.Double || dfi.type == DataFieldType.Long))
                {
                    
                }
            }

            if (result.Count > 0)
                return result.ToArray();
            else
                return null;
        }
    }

    public class ResourceFieldValidation : IDataFieldValidation
    {
        public LogMsgData[] IsValid(DataSheetInfo sInfo, int index)
        {
            throw new NotImplementedException();
        }
    }
}
