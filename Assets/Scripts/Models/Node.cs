namespace PlotFourVR.Models
{
    public class Node
    {
        public Node(int rowIndex, int columnIndex, NodeType nodeType)
        {
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
            this.nodeType = nodeType;
        }
        public int RowIndex => rowIndex;
        private int rowIndex;
        public int ColumnIndex => columnIndex;
        private int columnIndex;
        public NodeType NodeType { get => nodeType; set => nodeType = value; }
        private NodeType nodeType;
    }
}