using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Core.Tools.ExcelToData
{
    public enum LogMsgType
    {
        None,
        Info,
        Waring,
        Error,
    }

    public class LogMsgMgr
    {
        private static LogMsgMgr mgr = new LogMsgMgr();

        private class MsgIndentData
        {
            public int indent;
            public LogMsgData data;

            public MsgIndentData(int ind,LogMsgData d)
            {
                indent = ind;
                data = d;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                for(int i =0;i<indent;i++)
                {
                    sb.Append("    ");
                }
                sb.Append(data.Message());
                return sb.ToString() ;
            }
        }

        private LogMsgMgr() { }

        public static LogMsgMgr GetInstance()
        {
            return mgr;
        }

        private List<MsgIndentData> miDatas = new List<MsgIndentData>();
        public int Indent
        { get; set; }

        public void Add(LogMsgData msgData)
        {
            miDatas.Add(new MsgIndentData(Indent, msgData));
        }

        public void Clear()
        {
            miDatas.Clear();
        }

        public void PrintMsg()
        {
            foreach (MsgIndentData d in miDatas)
            {
                if (d.data.LogType == LogMsgType.Error)
                {
                    Logger.LogError(d.ToString());
                }
                else if (d.data.LogType == LogMsgType.Waring)
                {
                    Logger.LogWarning(d.ToString());
                }
                else
                {
                    Logger.Log(d.ToString());
                }
            }
        }
    }

    public class LogMsgData
    {
        protected LogMsgType lType = LogMsgType.None;
        protected int lCode = -1;

        public LogMsgType LogType
        {
            get { return lType; }
        }
        public LogMsgData(LogMsgType t,int code)
        {
            lType = t;
            lCode = code;
        }

        public virtual string Message()
        {
            return Enum.GetName(typeof(LogMsgType), lType) + ":" + LogConst.GetLogMsg(lCode);
        }
    }

    public class EmptyLogData : LogMsgData
    {
        public EmptyLogData():base(LogMsgType.None,-1)
        { }

        public override string Message()
        {
            return "\n";
        }
    }


    public class WarningLogData : LogMsgData
    {
        public WarningLogData(int errCode) :base(LogMsgType.Waring,errCode)
        {

        }
    }

    public class WarningLogData1 : LogMsgData
    {
        private string errorParam1 = "";
        public WarningLogData1(int errCode,string ep) : base(LogMsgType.Waring, errCode)
        {
            errorParam1 = ep;
        }

        public override string Message()
        {
            return Enum.GetName(typeof(LogMsgType), lType) + ":" + string.Format(LogConst.GetLogMsg(lCode),errorParam1);
        }
    }

    public class WarningLogData2 : LogMsgData
    {
        private string errorParam1 = "";
        private string errorParam2 = "";
        public WarningLogData2(int errCode, string ep1,string ep2) : base(LogMsgType.Waring, errCode)
        {
            errorParam1 = ep1;
            errorParam2 = ep2;
        }

        public override string Message()
        {
            return Enum.GetName(typeof(LogMsgType), lType) + ":" + string.Format(LogConst.GetLogMsg(lCode), errorParam1,errorParam2);
        }
    }

    public class ErrorLogData : LogMsgData
    {
        public ErrorLogData(int errCode):base(LogMsgType.Error,errCode)
        {

        }
    }

    public class ErrorLogData1 : LogMsgData
    {
        private string errorParam1 = "";
        public ErrorLogData1(int errCode, string ep) : base(LogMsgType.Error, errCode)
        {
            errorParam1 = ep;
        }

        public override string Message()
        {
            return Enum.GetName(typeof(LogMsgType), lType) + ":" + string.Format(LogConst.GetLogMsg(lCode), errorParam1);
        }
    }

    public class ErrorLogData2 : LogMsgData
    {
        private string errorParam1 = "";
        private string errorParam2 = "";
        public ErrorLogData2(int errCode, string ep1, string ep2) : base(LogMsgType.Error, errCode)
        {
            errorParam1 = ep1;
            errorParam2 = ep2;
        }

        public override string Message()
        {
            return Enum.GetName(typeof(LogMsgType), lType) + ":" + string.Format(LogConst.GetLogMsg(lCode), errorParam1, errorParam2);
        }
    }
    public class ErrorLogData3 : LogMsgData
    {
        private string errorParam1 = "";
        private string errorParam2 = "";
        private string errorParam3 = "";
        public ErrorLogData3(int errCode, string ep1, string ep2,string ep3) : base(LogMsgType.Error, errCode)
        {
            errorParam1 = ep1;
            errorParam2 = ep2;
            errorParam3 = ep3;
        }

        public override string Message()
        {
            return Enum.GetName(typeof(LogMsgType), lType) + ":" + string.Format(LogConst.GetLogMsg(lCode), errorParam1, errorParam2,errorParam3);
        }
    }

    public class InfoLogData : LogMsgData
    {
        public InfoLogData(int errCode) : base(LogMsgType.Info, errCode)
        {

        }
    }

    public class InfoLogData1 : LogMsgData
    {
        private string errorParam1 = "";
        public InfoLogData1(int errCode, string ep) : base(LogMsgType.Info, errCode)
        {
            errorParam1 = ep;
        }

        public override string Message()
        {
            return Enum.GetName(typeof(LogMsgType), lType) + ":" + string.Format(LogConst.GetLogMsg(lCode), errorParam1);
        }
    }

    public class InfoLogData2 : LogMsgData
    {
        private string errorParam1 = "";
        private string errorParam2 = "";
        public InfoLogData2(int errCode, string ep1, string ep2) : base(LogMsgType.Info, errCode)
        {
            errorParam1 = ep1;
            errorParam2 = ep2;
        }

        public override string Message()
        {
            return Enum.GetName(typeof(LogMsgType), lType) + ":" + string.Format(LogConst.GetLogMsg(lCode), errorParam1, errorParam2);
        }
    }
}
