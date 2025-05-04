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
                node.NodeType = NodeType.Green;
                if (nodeParent.IsWinningMove(node))
                {
                    node.NodeType = NodeType.Empty;
                    if (nodeParent.IsBelowNeighbourOccupied(node))
                    {
                        // check if the below neighbour is occupied
                        return node;
                    }
                    // skip the node, it'll fall below
                }
                // Reset the node type back to empty
                node.NodeType = NodeType.Empty;
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

        private Node BlockOpponentsWinningMove(List<Node> availableNodes)
        {
            foreach (Node node in availableNodes)
            {
                // Check if placing a disc here would result in a win
                node.NodeType = NodeType.Yellow;
                if (nodeParent.IsWinningMove(node))
                {
                    node.NodeType = NodeType.Empty;
                    if (nodeParent.IsBelowNeighbourOccupied(node))
                    {
                        // check if the below neighbour is occupied
                        return node;
                    }
                }
                // Reset the node type back to empty
                node.NodeType = NodeType.Empty;
            }
            // If no winning move is found, return null
            return null;
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
            int centerColumn = Mathf.FloorToInt(availableNodes.Count / 2f);
            int acceptedDistance = Mathf.CeilToInt(availableNodes.Count / 3f);

            int leftOrRight = Random.Range(0, 2);
            if(leftOrRight == 0)
            {
                for (int i = centerColumn; i < availableNodes.Count; i++)
                {
                    Node node = availableNodes[i];
                    // Try to fake intelligence with cheap, hard coded tricks and magical numbers
                    if (node.RowIndex < 4)
                    {
                        // Check if the node is around the center
                        if (i > centerColumn - acceptedDistance &&
                            i < centerColumn + acceptedDistance)
                        {
                            // Reset the node type back to empty
                            node.NodeType = NodeType.Empty;
                            return node;
                        }
                    }
                }
            }
            else
            {
                for (int i = centerColumn; i >= 0; i--)
                {
                    Node node = availableNodes[i];
                    // Try to fake intelligence with cheap, hard coded tricks and magical numbers
                    if (node.RowIndex < 3)
                    {
                        // Check if the node is around the center
                        if (i > centerColumn - acceptedDistance &&
                            i < centerColumn + acceptedDistance)
                        {
                            // Reset the node type back to empty
                            node.NodeType = NodeType.Empty;
                            return node;
                        }
                    }
                }
            }
            // If no center move is found, return null
            return null;
        }
    }
}