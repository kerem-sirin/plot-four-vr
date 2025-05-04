using System.Collections.Generic;

namespace PlotFourVR
{
    /// <summary>
    /// Holds grid data and game-logic (nodes, win checks, availability).
    /// </summary>
    public class GridModel
    {
        public int RowCount { get; }
        public int ColumnCount { get; }
        public int WinLength { get; }

        private Node[,] nodes;
        private (int dr, int dc)[] directions = { (0, 1), (1, 0), (1, 1), (1, -1) };

        public GridModel(int rows, int cols, int winLength)
        {
            RowCount = rows;
            ColumnCount = cols;
            WinLength = winLength;
            nodes = new Node[rows, cols];
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    nodes[r, c] = new Node(r, c, NodeType.Empty);
        }

        public Node GetNode(int row, int col)
        {
            if (row < 0 || row >= RowCount || col < 0 || col >= ColumnCount) return null;
            return nodes[row, col];
        }

        public List<Node> GetAvailableNodes()
        {
            List<Node> availableNodes = new List<Node>();
            // for eacg column, get the first available node on the row
            for (int column = 0; column < ColumnCount; column++)
            {
                Node firstAvailableNode = GetFirstAvailableNodeInColumn(column);
                if (firstAvailableNode == null) continue;
                availableNodes.Add(firstAvailableNode);
            }
            return availableNodes;
        }

        public Node GetFirstAvailableNodeInColumn(int columnIndex)
        {
            Node firstAvailableNode = null;
            // Check nodes in the specified column from bottom to top
            for (int row = 0; row < RowCount; row++)
            {
                Node node = GetNode(row, columnIndex);
                if (node != null && node.NodeType == NodeType.Empty)
                {
                    firstAvailableNode = node;
                    break;
                }
            }
            if (firstAvailableNode != null)
            {
                return firstAvailableNode;
            }
            return null;
        }

        public bool IsBelowOccupied(Node node)
        {
            if (node.RowIndex == 0) return true;
            Node below = GetNode(node.RowIndex - 1, node.ColumnIndex);
            return below != null && below.NodeType != NodeType.Empty;
        }

        public bool IsWinningMove(Node node)
        {
            var type = node.NodeType;
            int r0 = node.RowIndex, c0 = node.ColumnIndex;
            int slide = WinLength - 1;
            foreach (var (dr, dc) in directions)
            {
                int count = 0;
                for (int i = -slide; i <= slide; i++)
                {
                    var n = GetNode(r0 + dr * i, c0 + dc * i);
                    if (n != null && n.NodeType == type)
                    {
                        count++;
                        if (count >= WinLength) return true;
                    }
                    else count = 0;
                }
            }
            return false;
        }

        public List<Node> GetWinningTiles(Node node)
        {
            var result = new List<Node>();
            var type = node.NodeType;
            int r0 = node.RowIndex, c0 = node.ColumnIndex;
            int slide = WinLength - 1;
            foreach (var (dr, dc) in directions)
            {
                var line = new List<Node>();
                int count = 0;
                for (int i = -slide; i <= slide; i++)
                {
                    var n = GetNode(r0 + dr * i, c0 + dc * i);
                    if (n != null && n.NodeType == type)
                    {
                        line.Add(n);
                        count++;
                        if (count >= WinLength) return line;
                    }
                    else
                    {
                        line.Clear();
                        count = 0;
                    }
                }
            }
            return null;
        }
    }
}