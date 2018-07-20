using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Game.Core.Tools.ExcelToData
{
    public class ConfigMgr
    {
        private static ConfigMgr mgr = new ConfigMgr();
        private Config config = null;
        private ConfigMgr() { }

        public static ConfigMgr GetInstance()
        {
            return mgr;
        }

        public Config GetConfig()
        {
            return config;
        }

        public void LoadConfig(string path)
        {
            config = (Config)XmlSerializer.LoadFromXml(path, typeof(Config));
        }
    }

    [XmlRoot("config")]
    public class Config
    {
        [XmlArray("exports"),XmlArrayItem("export")]
        public List<DataExportConfig> exportConfigs;
        [XmlElement("language")]
        public DataLanguageConfig languageConfig;
        [XmlElement("project")]
        public DataProjectConfig projectConfig;
    }
    [XmlRoot("export")]
    public class DataExportConfig
    {
        [XmlAttribute("format")]
        public string format = null;
        [XmlAttribute("is_optimize")]
        public bool isOptimize = false;
        [XmlAttribute("subform")]
        public int subform = 30;
        [XmlAttribute("target_dir")]
        public string targetDir;
    }
    [XmlRoot("language")]
    public class DataLanguageConfig
    {
        [XmlAttribute("package")]
        public string package = "zh";
    }
    [XmlRoot("project")]
    public class DataProjectConfig
    {
        [XmlAttribute("path")]
        public string path = null;
        [XmlAttribute("is_valid")]
        public bool isValid = false;
    }

}
