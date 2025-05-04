using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

namespace PlotFourVR
{
    public class DecideComputerMovement
    {
        private const int MinPhaseTimeMs = 100;

        private NodeParent nodeParent;

        public DecideComputerMovement(NodeParent nodeParent)
        {
            this.nodeParent = nodeParent;
        }

        public async Task<Node> DecideMoveAsync()
        {
            var sw = Stopwatch.StartNew();

            // Run all decision logic synchronously on a background thread
            Node move = await Task.Run(() =>
            {
                var avail = nodeParent.GetAvailableNodes();
                if (MakeWinningMove(avail) is Node win) return win;
                if (BlockOpponentsWinningMove(avail) is Node blk) return blk;
                if (CreateFork(avail) is Node fork) return fork;
                if (BlockOpponentsFork(avail) is Node bfork) return bfork;
                if (TryToTakeCenter(avail) is Node center) return center;
                return PickRandom(avail);
            });

            sw.Stop();
            // enforce a single total “think time” rather than per-phase
            int remaining = MinPhaseTimeMs - (int)sw.ElapsedMilliseconds;
            if (remaining > 0)
            {
                await Task.Delay(remaining);
            }

            return move;
        }

        private Node PickRandom(List<Node> availableNodes)
        {
            return availableNodes[Random.Range(0, availableNodes.Count)];
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
            bool result = nodeParent.IsBelowNeighbourOccupied(node) && nodeParent.IsWinningMove(node);
            node.NodeType = NodeType.Empty;
            return result;
        }

        private Node CreateFork(List<Node> availableNodes)
        {
            foreach (Node node in availableNodes)
            {
                // Check if placing a disc here would create a fork
                node.NodeType = NodeType.Green;

                // Get the remaining available nodes
                foreach (Node nextNode in availableNodes)
                {
                    nextNode.NodeType = NodeType.Green;
                    if (nodeParent.IsWinningMove(nextNode))
                    {
                        node.NodeType = NodeType.Empty;
                        nextNode.NodeType = NodeType.Empty;
                        if (nodeParent.IsBelowNeighbourOccupied(node))
                        {
                            // Reset the node type back to empty
                            return node;
                        }
                    }
                    nextNode.NodeType = NodeType.Empty;
                }
                // Reset the node type back to empty
                node.NodeType = NodeType.Empty;
            }
            // If no fork is found, return null
            return null;
        }

        private Node BlockOpponentsFork(List<Node> availableNodes)
        {
            foreach (Node node in availableNodes)
            {
                // Check if placing a disc here would create a fork
                node.NodeType = NodeType.Yellow;

                // Get the remaining available nodes
                foreach (Node nextNode in availableNodes)
                {
                    nextNode.NodeType = NodeType.Yellow;
                    if (nodeParent.IsWinningMove(nextNode))
                    {
                        node.NodeType = NodeType.Empty;
                        nextNode.NodeType = NodeType.Empty;
                        if (nodeParent.IsBelowNeighbourOccupied(node))
                        {
                            // Reset the node type back to empty
                            return node;
                        }
                    }
                    nextNode.NodeType = NodeType.Empty;
                }
                // Reset the node type back to empty
                node.NodeType = NodeType.Empty;
            }
            // If no fork is found, return null
            return null;
        }

        private Node TryToTakeCenter(List<Node> availableNodes)
        {
            // Get board width and center
            int width = nodeParent.ColumnCount;
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
    }
}