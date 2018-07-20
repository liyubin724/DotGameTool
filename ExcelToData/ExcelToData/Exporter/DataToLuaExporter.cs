using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game.Core.Tools.ExcelToData
{
    public interface IDataExporter
    {
        void Export();
    }

    public class DataToOptimizeLuaExporter : IDataExporter
    {
        private DataSheetInfo dataSheet;
        private DataExportConfig exportConfig;
        public DataToOptimizeLuaExporter(DataSheetInfo sheet,DataExportConfig config)
        {
            dataSheet = sheet;
            exportConfig = config;

            CreateSummary();
        }

        private void CreateSummary()
        {
            summary.count = dataSheet.GetContentCount();
            summary.subformCount = (int)Math.Ceiling(((float)summary.count) / exportConfig.subform);

            summary.textPath = dataSheet.Name + "Text";
            summary.strPath = dataSheet.Name + "Str";

            for(int i =0;i<dataSheet.GetFieldCount();i++)
            {
                DataFieldInfo dfInfo = dataSheet.GetFieldInfo(i);
                if (dfInfo.type == DataFieldType.Stringt)
                {
                    summary.textFields.Add(dfInfo.name);
                } else if(dfInfo.type == DataFieldType.String || dfInfo.type == DataFieldType.Res)
                {
                    summary.strFields.Add(dfInfo.name);
                }
            }

            int index = 1;
            while(true)
            {
                DataToOptimizeLuaSummary.SubformDataOfSummary subformData = new DataToOptimizeLuaSummary.SubformDataOfSummary();
                subformData.path = string.Format("{0}/{0}_{1}", dataSheet.Name, index);
                subformData.minID = int.Parse((string)dataSheet.GetContentValue((index - 1) * exportConfig.subform, 0));
                int maxIndex = index * exportConfig.subform - 1;
                if(maxIndex>=dataSheet.GetContentCount())
                {
                    maxIndex = dataSheet.GetContentCount() - 1;
                }
                subformData.maxID = int.Parse((string)dataSheet.GetContentValue(maxIndex, 0)); 

                summary.subforms.Add(subformData);

                if (maxIndex == dataSheet.GetContentCount() - 1)
                    break;

                index++;
            }

            for(int i =1;i<dataSheet.GetFieldCount();i++)
            {
                DataFieldInfo dfInfo = dataSheet.GetFieldInfo(i);
                if (dfInfo.type == DataFieldType.Array || dfInfo.type == DataFieldType.Dic)
                    continue;
                Dictionary<string, int> totalCount = new Dictionary<string, int>();
                for (int j = 0; j < dataSheet.GetContentCount(); j++)
                {
                    string data = (string)dataSheet.GetContentValue(j, i, true);
                    if(!string.IsNullOrEmpty(data))
                    {
                        if(dfInfo.type == DataFieldType.Stringt)
                        {
                            data = ""+text.AddText(data);
                        }else if(dfInfo.type == DataFieldType.String || dfInfo.type == DataFieldType.Res)
                        {
                            data = "" + str.AddText(data);
                        }

                        if (totalCount.ContainsKey(data))
                        {
                            totalCount[data]++;
                        }else
                        {
                            totalCount.Add(data, 1);
                        }
                    }
                }
                string maxValue = null;
                int maxCount = 0;
                foreach(KeyValuePair<string,int> kvp in totalCount)
                {
                    if(kvp.Value > maxCount)
                    {
                        maxValue = kvp.Key;
                        maxCount = kvp.Value;
                    }
                }

                if(maxValue!=null && maxCount>1)
                {
                    summary.defaultDic.Add(dfInfo, maxValue);
                }
            }
        }

        private DataToOptimizeLuaSummary summary = new DataToOptimizeLuaSummary();
        private DataToOptimizeLuaString str = new DataToOptimizeLuaString();
        private DataToOptimizeLuaString text = new DataToOptimizeLuaString();

        public void Export()
        {
            string dirPath = exportConfig.targetDir;
            if(!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            summary.Export(dataSheet.Name, dirPath);

            text.Export(dataSheet.Name + "Text_"+ConfigMgr.GetInstance().GetConfig().languageConfig.package, dirPath);
            str.Export(dataSheet.Name + "Str", dirPath);

            string subDirPath = dirPath + "/" + dataSheet.Name;
            if (!Directory.Exists(subDirPath))
            {
                Directory.CreateDirectory(subDirPath);
                //Directory.Delete(subDirPath, true);
            }

            for (int i =1;i<=summary.subformCount;i++)
            {
                int startIndex = (i - 1) * exportConfig.subform;
                int endIndex = i * exportConfig.subform - 1;
                if(endIndex>= dataSheet.GetContentCount() )
                {
                    endIndex = dataSheet.GetContentCount() - 1;
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("local {0}_{1} = {{\n", dataSheet.Name, i);

                for(int k = startIndex;k<=endIndex;k++)
                {
                    int dataID = int.Parse((string)dataSheet.GetContentValue(k, 0));
                    sb.AppendFormat("    [{0}] = {{\n", dataID);
                    for(int j=0;j<dataSheet.GetFieldCount();j++)
                    {
                        DataFieldInfo dfInfo = dataSheet.GetFieldInfo(j);
                        object data = dataSheet.GetContentValue(k, j, true);
                        if(dfInfo.type == DataFieldType.Stringt)
                        {
                            data = "" + text.GetIndex((string)data);
                        }else if(dfInfo.type == DataFieldType.String || dfInfo.type == DataFieldType.Res)
                        {
                            data = "" + str.GetIndex((string)data);
                        }
                        if(summary.IsInDefault(dfInfo,data))
                        {
                            continue;
                        }
                        switch(dfInfo.type)
                        {
                            case DataFieldType.Dic:
                                break;
                            case DataFieldType.Array:
                                break;
                            case DataFieldType.Bool:
                            case DataFieldType.Float:
                            case DataFieldType.Int:
                            case DataFieldType.Long:
                            case DataFieldType.Double:
                            case DataFieldType.Ref:
                                {
                                    string strData = (string)data;
                                    sb.AppendFormat("        {0} = {1},\n", dfInfo.name, strData.ToLower());
                                }
                                break;
                            case DataFieldType.String:
                            case DataFieldType.Stringt:
                            case DataFieldType.Res:
                                {
                                    string strData = (string)data;
                                    sb.AppendFormat("        {0}_index = {1},\n", dfInfo.name, strData.ToLower());
                                }
                                break;
                        }
                    }
                    sb.AppendLine("    },");
                }
                
                sb.AppendLine("}");
                sb.AppendFormat("SetLooseReadonly({0}_{1})\n", dataSheet.Name, i);
                sb.AppendFormat("return {0}_{1}\n", dataSheet.Name, i);

                string fileContent = sb.ToString();
                File.WriteAllText(subDirPath + "/" + dataSheet.Name + "_" + i + ".lua", fileContent);
            }
        }
    }


    public class DataToOptimizeLuaSummary
    {
        public class SubformDataOfSummary
        {
            public string path;
            public int minID;
            public int maxID;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("    {");
                sb.AppendFormat("        path = \"{0}\",\n", path);
                sb.AppendFormat("        minID = {0},\n", minID);
                sb.AppendFormat("        maxID = {0},\n", maxID);
                sb.AppendLine("   },\n");
                return sb.ToString();
            }
        }
        public string textPath = "";
        public DataToOptimizeLuaString textData = null;
        public string strPath = "";
        public DataToOptimizeLuaString strData = null;
        public int count;
        public int subformCount;
        public List<SubformDataOfSummary> subforms = new List<SubformDataOfSummary>();
        public List<string> textFields = new List<string>();
        public List<string> strFields = new List<string>();
        public Dictionary<DataFieldInfo, object> defaultDic = new Dictionary<DataFieldInfo, object>();

        public DataToOptimizeLuaSummary()
        {
        }

        public bool IsInDefault(DataFieldInfo dfInfo,object value)
        {
            object data = null;
            if(defaultDic.TryGetValue(dfInfo,out data))
            {
                if(data.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }

        public void Export(string name,string dirPath)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("require(\"GameDataBase\")");
            sb.AppendLine();
            sb.AppendFormat("{0} = {{}}\n", name);
            sb.AppendLine();
            sb.AppendFormat("{0}.textPath = \"{1}\"\n", name, textPath);
            sb.AppendFormat("{0}.stringPath = \"{1}\"\n", name, strPath);
            sb.AppendFormat("{0}.count = {1}\n", name, count);
            sb.AppendFormat("{0}.subformCount = {1}\n", name, subformCount);
            sb.AppendLine();
            sb.AppendFormat("{0}.subforms = {{\n",name);
            foreach(SubformDataOfSummary subform in subforms)
            {
                sb.Append(subform.ToString());
            }
            sb.AppendLine("}");

            sb.AppendLine();
            sb.AppendFormat("{0}.textFields = {{\n",name);
            foreach(string t in textFields)
            {
                sb.AppendFormat("    \"{0}\",\n", t);
            }
            sb.AppendLine("}");

            sb.AppendLine();
            sb.AppendFormat("{0}.strFields = {{\n", name);
            foreach (string t in strFields)
            {
                sb.AppendFormat("    \"{0}\",\n", t);
            }
            sb.AppendLine("}");

            sb.AppendLine();

            sb.AppendFormat("{0}.defaultValue = {{\n", name);
            foreach(KeyValuePair<DataFieldInfo,object> kvp in defaultDic)
            {
                DataFieldType dfType = kvp.Key.type;
                switch(dfType)
                {
                    case DataFieldType.Dic:
                    case DataFieldType.Array:
                        continue;
                    case DataFieldType.Bool:
                    case DataFieldType.Float:
                    case DataFieldType.Int:
                    case DataFieldType.Long:
                    case DataFieldType.Double:
                    case DataFieldType.Ref:
                        sb.AppendFormat("    {0} = {1},\n", kvp.Key.name, ((string)kvp.Value).ToLower());
                        break;
                    case DataFieldType.String:
                    case DataFieldType.Stringt:
                    case DataFieldType.Res:
                        sb.AppendFormat("    {0}_index = {1},\n", kvp.Key.name, ((string)kvp.Value).ToLower());
                        break;
                }
            }
            sb.AppendLine("}");

            sb.AppendLine();
            sb.AppendFormat("{0}.__index = {0}\n", name);
            sb.AppendFormat("setmetatable({0},GameDataBase.SheetBase)\n",name);

            string fileContent = sb.ToString();
            File.WriteAllText(string.Format("{0}/{1}.lua",dirPath, name),fileContent);
        }
    }

    public class DataToOptimizeLuaString
    {
        protected List<string> texts = new List<string>();

        public int AddText(string t)
        {
            if (t == null)
                return -1;

            int index = texts.IndexOf(t);
            if (index < 0)
            {
                texts.Add(t);
                index = texts.Count - 1;
            }
            return index+1;
        }

        public int GetIndex(string t)
        {
            if (t == null)
                return -1;

            return texts.IndexOf(t) + 1;
        }

        public bool IsValid()
        {
            return texts.Count > 0;
        }

        public virtual void Export(string name,string dirPath)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("local {0} = {{", name));
            for (int i = 0; i < texts.Count; i++)
            {
                sb.AppendLine(string.Format("    [[{0}]],", texts[i]));
            }
            sb.AppendLine("}");
            //sb.AppendLine(string.Format("SetLooseReadonly({0})", name));
            sb.AppendLine(string.Format("return {0}", name));

            string fileContent = sb.ToString();


            File.WriteAllText(string.Format("{0}/{1}.lua", dirPath, name), fileContent);
        }
    }
}
