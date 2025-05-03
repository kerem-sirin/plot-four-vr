using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace PlotFourVR
{
    public class DecideComputerMovement
    {
        private NodeParent nodeParent;
        private Dictionary<Node, Transform> nodeDictionary;

        private Node computersNode;

        public DecideComputerMovement(NodeParent nodeParent, Dictionary<Node, Transform> nodeDictionary)
        {
            this.nodeParent = nodeParent;
            this.nodeDictionary = nodeDictionary;
        }

        public async Task<Node> DecideMove()
        {
            List<Node> availableNodes = nodeParent.GetAvailableNodes();
            await Task.Delay(100);
            //Win if you can
            foreach (Node node in availableNodes)
            {
                // Check if placing a disc here would result in a win
                node.NodeType = NodeType.Green;
                if (nodeParent.IsWinningMove(node))
                {
                    node.NodeType = NodeType.Empty;
                    if(nodeParent.IsBelowNeighbourOccupied(node))
                    {
                        // check if the below neighbour is occupied
                        Debug.Log("AI: Winning move played");
                        Debug.Log("AI: " + node.RowIndex + ":" + node.ColumnIndex);
                        return node;
                    }
                    // skip the node, it'll fall below
                }
                // Reset the node type back to empty
                node.NodeType = NodeType.Empty;
            }
            await Task.Delay(100);
            //Block if you can't win
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
                        Debug.Log("AI: Players winning move blocked");
                        Debug.Log("AI: " + node.RowIndex + ":" + node.ColumnIndex);
                        return node;
                    }
                }
                // Reset the node type back to empty
                node.NodeType = NodeType.Empty;

                // Diagonal moves are not fully covered
                // There are cases the diagonal move is on the air
            }
            await Task.Delay(100);
            //Create a fork
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
                            Debug.Log("AI: Fork created");
                            Debug.Log("AI: " + node.RowIndex + ":" + node.ColumnIndex);
                            return node;
                        }
                    }
                    nextNode.NodeType = NodeType.Empty;
                }
                // Reset the node type back to empty
                node.NodeType = NodeType.Empty;
            }

            //Block opponent's fork

            //Take the center

            //Play next to your discs

            //Play in an edge if available

            // Fallback, if multiple moves tie, pick one at random

            await Task.Delay(100);
            // for testing purposes play a random move
            int randomIndex = Random.Range(0, availableNodes.Count);
            Debug.Log("AI Random move played: " + availableNodes[randomIndex].RowIndex + ":" + availableNodes[randomIndex].ColumnIndex);
            return availableNodes[randomIndex];
        }
    }
}
