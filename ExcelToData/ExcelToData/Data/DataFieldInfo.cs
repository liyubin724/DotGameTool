using Game.Core.Tools.ExcelToData;
using System;

namespace Game.Core.Tools.ExcelToData
{
    public enum DataFieldType
    {
        None,
        Int,
        Float,
        Double,
        Long,
        Bool,
        String,
        Stringt,
        Ref,
        Dic,
        Array,
        Res,
        Max,
    }

    [Flags]
    public enum DataFieldExport
    {
        None = 0,
        Unexport = 1,
        Client = 1<<1,
        Server = 1<<2,
    }

    public class DataFieldInfo
    {
        public int columnIndex = -1;
        public string name = null;
        public DataFieldType type = DataFieldType.None;
        public DataFieldExport export = DataFieldExport.Unexport;
        public string desc = null;
        public DataFieldValidationType validation = DataFieldValidationType.None;
        public string validationValue = null;
        public string defaultContent = null;

        public string refName;
        public DataFieldInfo keyField;
        public DataFieldInfo valueField;
    }
}
