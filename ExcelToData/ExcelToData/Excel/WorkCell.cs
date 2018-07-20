namespace Game.Core.Tools.ExcelToData
{
    public class WorkCell
    {
        public int row;
        public int col;
        public string value;

        public WorkCell(int r,int c,string v)
        {
            row = r;
            col = c;
            value = v;
        }
    }
}
