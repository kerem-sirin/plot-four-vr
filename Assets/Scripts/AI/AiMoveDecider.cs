using PlotFourVR.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

namespace PlotFourVR.AI
{
    /// <summary>
    /// Decides the next move for the AI player
    /// </summary>
    public class AiMoveDecider
    {
        private const int MinPhaseTimeMs = 500;
        private GridModel gridModel;

        public AiMoveDecider(GridModel gridModel)
        {
            this.gridModel = gridModel;
        }

        public async Task<Node> DecideMoveAsync()
        {
            var sw = Stopwatch.StartNew();

            var availableNodes = gridModel.GetAvailableNodes();
            // Run all decision logic synchronously on a background thread
            Node move = await Task.Run(() =>
            {
                if (MakeWinningMove(availableNodes) is Node win) return win;
                if (BlockOpponentsWinningMove(availableNodes) is Node blk) return blk;
                if (CreateFork(availableNodes) is Node fork) return fork;
                if (BlockOpponentsFork(availableNodes) is Node bfork) return bfork;
                if (TryToTakeCenter(availableNodes) is Node center) return center;
                return PickRandom(availableNodes);
            });

            sw.Stop();
            // enforce a single total "think time" rather than per-phase
            int remaining = MinPhaseTimeMs - (int)sw.ElapsedMilliseconds;
            if (remaining > 0)
            {
                await Task.Delay(remaining);
            }
            return move;
        }

        private Node MakeWinningMove(List<Node> availableNodes)
        {
            foreach (Node node in availableNodes)
            {
                // Check if placing a disc here would result in a win
                if(WouldMoveSucceed(node, NodeType.Green))
                {                  
                    // check if the below neighbour is occupied
                    return node;
                }
            }
            // If no winning move is found, return null
            return null;
        }

        private Node BlockOpponentsWinningMove(List<Node> availableNodes)
        {
            foreach (Node node in availableNodes)
            {
                // Check if placing a disc here would result in a win
                if (WouldMoveSucceed(node, NodeType.Yellow))
                {
                    // check if the below neighbour is occupied
                    return node;
                }
            }
            // If no winning move is found, return null
            return null;
        }

        private bool WouldMoveSucceed(Node node, NodeType testType)
        {
            node.NodeType = testType;
            bool result = gridModel.IsBelowOccupied(node) && gridModel.IsWinningMove(node);
            node.NodeType = NodeType.Empty;
            return result;
        }

        private Node CreateFork(List<Node> availableNodes)
        {
            return availableNodes.FirstOrDefault(n => Fork(n, NodeType.Green));
        }

        private Node BlockOpponentsFork(List<Node> availableNodes)
        {
            return availableNodes.FirstOrDefault(n => Fork(n, NodeType.Yellow));
        }

        private bool Fork(Node n, NodeType color)
        {
            if (!gridModel.IsBelowOccupied(n)) return false;
            n.NodeType = color;
            int threats = 0;
            foreach (var next in gridModel.GetAvailableNodes())
            {
                if (!gridModel.IsBelowOccupied(next)) continue;
                next.NodeType = color;
                if (gridModel.IsWinningMove(next)) threats++;
                next.NodeType = NodeType.Empty;
                if (threats >= 2) break;
            }
            n.NodeType = NodeType.Empty;
            return threats >= 2;
        }

        private Node TryToTakeCenter(List<Node> availableNodes)
        {
            // Get board width and center
            int width = gridModel.ColumnCount;
            int centerCol = width / 2;

            // build a dictionary of available nodes by column
            var nodesByCol = new Dictionary<int, List<Node>>();
            foreach (var node in availableNodes)
            {
                if (!nodesByCol.ContainsKey(node.ColumnIndex))
                    nodesByCol[node.ColumnIndex] = new List<Node>();
                nodesByCol[node.ColumnIndex].Add(node);
            }

            // for each column offset 0,1,2,… try center+offset then center-offset
            for (int offset = 0; offset <= centerCol; offset++)
            {
                foreach (int sign in new[] { +1, -1 })
                {
                    // move to the left or right
                    int col = centerCol + offset * sign;
                    if (col < 0 || col >= width) continue;

                    // If we have any available nodes in this column
                    if (!nodesByCol.TryGetValue(col, out List<Node> colNodes)) continue;
                    if (colNodes[0] != null) return colNodes[0];
                }

                // Special case: when offset == 0, we tried center twice (+ and –). Skip duplicate.
                if (offset == 0) continue;
            }

            // Nothing found in or around center
            return null;
        }

        private Node PickRandom(List<Node> availableNodes)
        {
            return availableNodes[Random.Range(0, availableNodes.Count)];
        }
    }
}