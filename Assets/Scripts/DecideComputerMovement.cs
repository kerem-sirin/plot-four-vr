using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;
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
            // Run all the logic off the UI thread
            Node result = await Task.Run(async () =>
            {
                var phases = new (string phaseName, Func<Node> step)[]
                {
                ("Checking for Win",       () => MakeWinningMove(nodeParent.GetAvailableNodes())),
                ("Blocking Opponent Win",  () => BlockOpponentsWinningMove(nodeParent.GetAvailableNodes())),
                ("Looking for Fork",       () => CreateFork(nodeParent.GetAvailableNodes())),
                ("Blocking Opponent Fork", () => BlockOpponentsFork(nodeParent.GetAvailableNodes())),
                ("Taking Center",          () => TryToTakeCenter(nodeParent.GetAvailableNodes())),
                ("Picking Random",         () => PickRandom(nodeParent.GetAvailableNodes()))
                };

                foreach (var (phaseName, step) in phases)
                {
                    var sw = Stopwatch.StartNew();
                    Node move = step();
                    sw.Stop();

                    // enforce minimum delay
                    int delay = MinPhaseTimeMs - (int)sw.ElapsedMilliseconds;
                    if (delay > 0)
                    {
                        await Task.Delay(delay);
                    }

                    // if this phase found a move, return it immediately
                    if (move != null)
                    {
                        return move;
                    }
                }
                // should never hit here, but just in case
                Debug.LogError($"No move found in any phase. Returning null.");
                return null;
            });
            return result;
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
            int width = availableNodes.Count;
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