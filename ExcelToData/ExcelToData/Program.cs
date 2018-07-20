using System;
using System.Collections.Generic;


namespace Game.Core.Tools.ExcelToData
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigMgr.GetInstance().LoadConfig("./config.xml");
            Workbook workbook = new Workbook("./data.xlsx");
            workbook.ReadWorkbook();

            List<DataSheetInfo> dSheet = workbook.ConvertToDataSheet();
            
            foreach(DataSheetInfo s in dSheet)
            {
                new DataToOptimizeLuaExporter(s, ConfigMgr.GetInstance().GetConfig().exportConfigs[0]).Export();
            }
            LogMsgMgr.GetInstance().PrintMsg();
            Console.ReadKey();
        }
    }
}
