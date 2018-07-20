using System.Collections.Generic;

namespace Game.Core.Tools.ExcelToData
{
    public static class LogConst
    {
        public static int I_WorkBook_Start = 1;
        public static int I_WorkBook_End = 2;

        public static int I_WorkSheet_Start = 100;
        public static int I_WorkSheet_End = 101;

        public static int I_WorkToData_Start = 200;
        public static int I_WorkToData_End = 201;

        public static int E_WorkBook_NotFound = 1001;
        public static int E_WorkBook_NotExcel = 1002;
        public static int E_WorkBook_SheetNull = 1003;
        public static int E_WorkBook_Empty = 1004;
        public static int E_WorkBook_Null = 1005;
        public static int E_WorkBook_Format = 1006;


        public static int E_WorkSheet_Null = 2001;
        public static int E_WorkSheet_Row_Null = 2002;
        public static int E_WorkSheet_Empty = 2003;
        public static int E_WorkSheet_NameNull = 2004;

        public static int E_DataSheet_NameFormat = 3001;
        public static int E_DataSheet_RowNum = 3002;
        public static int E_DataSheet_ColNum = 3003;

        public static int E_DataField_Empty = 4001;
        public static int E_DataField_TypeNone = 4002;
        public static int E_DataField_ExportNone = 4003;
        public static int E_DataField_DicType = 4004;
        public static int E_DataField_DicKeyRef = 4005;
        public static int E_DataField_DicValueRef = 4006;
        public static int E_DataField_ArrayType = 4007;
        public static int E_DataField_ArrayValueRef = 4008;
        public static int E_DataField_RefRef = 4009;
        public static int E_DataField_ResType = 4010;
        public static int E_DataField_ID = 4011;
        public static int E_DataField_NameFormat = 4012;
        public static int E_DataField_NameRepeat = 4013;

        public static int E_DataContent_DicLen = 5001;
        public static int E_DataContent_DicLarge = 5002;
        public static int E_DataContent_DicKeyRepeat = 5003;
        public static int E_DataContent_ArrayLen = 5101;
        public static int E_DataContent_ArrayLarge = 5102;

        public static int E_DataValidation_Repeat = 6001;
        public static int E_DataValidation_Null = 6002;
        public static int E_DataValidation_StrLen = 6003;



        public static Dictionary<int, string> error_msg = new Dictionary<int, string>
        {
            { I_WorkBook_Start,"开始处理Excel文件->{0}" },
            {I_WorkBook_End,"Excel文件{0}处理完成" },
            { I_WorkSheet_Start,"开始处理Sheet表->{0}" },
            {I_WorkSheet_End,"Sheet表{0}处理完成" },

            {I_WorkToData_Start,"开始转换数据->{0}" },
            {I_WorkToData_End,"数据表转换完成" },

            {E_WorkSheet_Null,"数据表读取失败，请检查数据表！" },
            {E_WorkSheet_Row_Null,"数据表({0})中的第{1}行没有任何数据，请检查数据表！" },
            {E_WorkSheet_Empty,"数据表({0})为空，请检查数据表!" },
            {E_WorkSheet_NameNull,"Excel文件({0})中第{1}个数据表名为空!" },

            {E_WorkBook_NotFound,"Excel文件({0})未找到!" },
            {E_WorkBook_NotExcel,"文件格式有误，只能解析格式为xls与xlsx的Excel文件.文件路径为:{0}" },
            {E_WorkBook_SheetNull,"Excel文件({0})中第{1}个数据表读取失败，请检查数据表！" },
            {E_WorkBook_Empty,"Excel文件({0}不包含任何数据表，请检查数据！" },
            {E_WorkBook_Null,"解析Excel文件({0})出错,请确认文件格式及内容." },
            {E_WorkBook_Format,"解析Excel文件({0})出错,异常信息:{1}" },

            {E_DataSheet_NameFormat,"数据表名称({0})不合规范，名字必须以大写字母开关，并且只能有字母、数字、下划线组成" },
            {E_DataSheet_RowNum, "数据表({0})格式不正确,标准的数据表格式至少要有{1}行数据"},
            {E_DataSheet_ColNum,"数据表({0})格式不正确,标准的数据表格式至少要有{1}列数据" },

            {E_DataField_Empty,"第{0}列字段名称不能为空，只有字典或数组类型的字段，其值才能为空" },
            {E_DataField_TypeNone,"第{0}列导出类型{1}不符合规则, 请查证!" },
            {E_DataField_ExportNone,"第{0}列导出类型不符合规则, 请查证" },
            {E_DataField_DicType,"第{0}列指类型为Dic类型，但是其子类型有误, 请查证" },
            {E_DataField_DicKeyRef,"第{0}列指类型为Dic类型，其Key为引用类型，但是未指定引用的数据表, 请查证" },
            {E_DataField_DicValueRef,"第{0}列指类型为Dic类型，Value为引用类型，但是未指定引用的数据表, 请查证" },
            {E_DataField_ArrayType,"第{0}列指类型为Array类型，但是其子类型有误, 请查证" },
            {E_DataField_ArrayValueRef,"第{0}列指类型为Array类型，其子类型为引用类型，但是未指定引用的数据表, 请查证" },
            {E_DataField_RefRef,"第{0}列指类型为Ref，但是未指定其引用的数据表名, 请查证"},
            {E_DataField_ResType,"第{0}列指类型为Ref，但是未指定其对应资源的后缀名，请查证" },
            { E_DataField_ID ,"数据表格式不正确,第一列字段为必须为ID，同时类型必须为Int"},
            {E_DataField_NameFormat,"第{0}列的字段名({1})不符合规范，必须以大写字母开头，只能包含大小写字母及数字，并且长度为2-15" },
            {E_DataField_NameRepeat, "第{0}列与第{1}列字段名相同，数据中不字段名不能相同"},
            


            {E_DataContent_DicLen,"数据表中第{0}行{1}列指定为Dic类型，需要指定Dic长度" },
            {E_DataContent_DicLarge,"数据表中第{0}行{1}列指定为Dic类型,指定长度大于设定整体数据量，请查证" },
            {E_DataContent_DicKeyRepeat,"数据表中第{0}行{1}列指定为Dic类型,Key必须唯一，不可有重复，请查证" },
            {E_DataContent_ArrayLen,"数据表中第{0}行{1}列指定为Array类型，需要指定Array长度，请查证" },
            {E_DataContent_ArrayLarge,"数据表中第{0}行{1}列指定为Array类型,指定长度大于设定整体数据量，请查证" },

            {E_DataValidation_Repeat,"检测发现数据表中第{0}列的内容有重复->({1})" },
            {E_DataValidation_Null,"检测发现数据表中第{0}列内容为空" },
            {E_DataValidation_StrLen,"检测发现数据中第{0}列中字符串长度超出了限定长度{1},内容->{3}" },

        };

        public static string[] ReserveFieldName = new string[]
        {
            "summary",
        };

        public static string GetLogMsg(int code)
        {
            string result = null;
            if(error_msg.TryGetValue(code,out result))
            {
                return result;
            }
            return ""+ code;
        }
    }


}
