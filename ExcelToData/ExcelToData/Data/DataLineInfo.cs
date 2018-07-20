using System.Collections.Generic;

namespace Game.Core.Tools.ExcelToData
{
    public class DataLineInfo
    {
        public DataLineInfo(int count)
        {
            datas = new DataCellInfo[count];
        }

        public void AddCellData(int index,int r,int c,object v)
        {
            datas[index] = new DataCellInfo(r, c, v);
        }

        public DataCellInfo GetCellData(int index)
        {
            if(index>=0&&index<datas.Length)
            {
                return datas[index];
            }
            return null;
        }

        private DataCellInfo[] datas = null;
    }

    public class DataCellInfo
    {
        public int row;
        public int col;
        public object value;

        public DataCellInfo(int r,int c,object v)
        {
            row = r;
            col = c;
            value = v;
        }

        public T GetValue<T>()
        {
            return (T)value;
        }

        public override string ToString()
        {
            return "row:"+row+",col:"+col+",v:"+value;
        }
    }
}
