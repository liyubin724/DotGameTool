using System;

namespace Game.Core.Tools.ExcelToData
{
    public static class DataHelper
    {
        public static readonly string FieldTypeDesc = "";
        public static readonly string FieldExportDesc = "";

        public static DataFieldValidationType GetFieldValidationType(string fvt)
        {
            if (string.IsNullOrEmpty(fvt))
                return DataFieldValidationType.None;

            int fvtInt = -1;
            if (int.TryParse(fvt, out fvtInt))
            {
                if (Enum.IsDefined(typeof(DataFieldValidationType), fvtInt))
                {
                    return (DataFieldValidationType)fvtInt;
                }
                return DataFieldValidationType.None;
            }
            return DataFieldValidationType.None;
        }

        public static DataFieldExport GetFieldExport(string fe)
        {
            if (string.IsNullOrEmpty(fe))
                return DataFieldExport.None;
            int feInt = -1;
            if(int.TryParse(fe,out feInt))
            {
                if(Enum.IsDefined(typeof(DataFieldExport),feInt))
                {
                    return (DataFieldExport)feInt;
                }
                return DataFieldExport.None;
            }
            return DataFieldExport.None;
        }

        public static void GetFieldDicKeyInfo(string ft, out DataFieldInfo keyField, out DataFieldInfo valueField)
        {
            keyField = null;
            valueField = null;

            if (string.IsNullOrEmpty(ft))
                return;
            int sIndex = ft.IndexOf('(');
            int lIndex = ft.LastIndexOf(')');
            if (sIndex < 0 || lIndex < 0)
            {
                return;
            }
            string fTypeStr = ft.Substring(sIndex+1, lIndex - sIndex-1);
            if (fTypeStr.IndexOf('/') <= 0)
                return;
            string[] fTypeStrSplit = fTypeStr.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (fTypeStrSplit == null || fTypeStrSplit.Length != 2)
                return;

            DataFieldType keyFT = GetFieldType(fTypeStrSplit[0]);
            DataFieldType valueFT = GetFieldType(fTypeStrSplit[1]);
            if (keyFT == DataFieldType.None || valueFT == DataFieldType.None)
                return;

            keyField = new DataFieldInfo();
            keyField.type = keyFT;
            if(keyFT == DataFieldType.Ref)
            {
                keyField.refName = GetFieldRefName(fTypeStrSplit[0]);
            }
            valueField = new DataFieldInfo();
            valueField.type = valueFT;
            if(valueFT == DataFieldType.Ref)
            {
                valueField.refName = GetFieldRefName(fTypeStrSplit[1]);
            }
        }

        public static void GetArrayFieldInfo(string ft,out DataFieldInfo valueField)
        {
            valueField = null;
            if (string.IsNullOrEmpty(ft))
                return;
            int sIndex = ft.IndexOf('(');
            int lIndex = ft.LastIndexOf(')');
            if (sIndex < 0 || lIndex < 0)
            {
                return;
            }
            string fTypeStr = ft.Substring(sIndex+1, lIndex - sIndex-1);
            DataFieldType fType = GetFieldType(fTypeStr);

            valueField = new DataFieldInfo();
            valueField.type = fType;
            if(fType == DataFieldType.Ref)
            {
                valueField.refName = GetFieldRefName(ft);
            }
        }

        public static string GetFieldRefName(string ft)
        {
            if (string.IsNullOrEmpty(ft))
                return null;
            int sIndex = ft.IndexOf('(');
            int lIndex = ft.IndexOf(')');
            if (sIndex < 0 || lIndex < 0)
            {
                return null;
            }
            return ft.Substring(sIndex+1, lIndex - sIndex-1);
        }

        public static DataFieldType GetFieldType(string ft)
        {
            if (string.IsNullOrEmpty(ft))
                return DataFieldType.None;

            string fType = ft;
            if(ft.IndexOf('(')>=0)
            {
                fType = ft.Substring(0, ft.IndexOf('('));
            }

            if (string.IsNullOrEmpty(ft))
                return DataFieldType.None;

            DataFieldType result = DataFieldType.None;
            if(Enum.TryParse<DataFieldType>(fType,true,out result))
            {
                return result;
            }
            return DataFieldType.None;
        }
    }
}
