using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace PlotFourVR
{
    public class DecideComputerMovement
    {
        private NodeParent nodeParent;

        public DecideComputerMovement(NodeParent nodeParent)
        {
            this.nodeParent = nodeParent;
        }

        public async Task<Node> DecideMove()
        {
            List<Node> availableNodes = nodeParent.GetAvailableNodes();
            await Task.Delay(100);
            //Win if you can
            Node winningMove = MakeWinningMove(availableNodes);
            if (winningMove != null) return winningMove;

            await Task.Delay(100);
            //Block if you can't win
            Node winningMoveBlocker = BlockOpponentsWinningMove(availableNodes);
            if (winningMoveBlocker != null) return winningMoveBlocker;

            await Task.Delay(100);
            //Create a fork if there is a chance
            Node forkMove = CreateFork(availableNodes);
            if (forkMove != null) return forkMove;

            await Task.Delay(100);
            //Block opponent's fork if there is a chance
            Node blockForkMove = BlockOpponentsFork(availableNodes);
            if (blockForkMove != null) return blockForkMove;

            await Task.Delay(100);
            //Take the center for bottom rows
            Node centerNode = TryToTakeCenter(availableNodes);
            if (centerNode != null) return centerNode;

            // take a leap of faith and play a random move
            await Task.Delay(100);
            int randomIndex = Random.Range(0, availableNodes.Count);
            return availableNodes[randomIndex];
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