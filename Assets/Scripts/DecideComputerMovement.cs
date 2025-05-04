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

            await Task.Delay(100);
            //Block opponent's fork
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
                            Debug.Log("AI: Playeres Fork blocked");
                            Debug.Log("AI: " + node.RowIndex + ":" + node.ColumnIndex);
                            return node;
                        }
                    }
                    nextNode.NodeType = NodeType.Empty;
                }
                // Reset the node type back to empty
                node.NodeType = NodeType.Empty;
            }

            //Take the center for bottom rows
            for (int i = 0; i < availableNodes.Count; i++)
            {
                Node node = availableNodes[i];
                // Try to fake intelligence with cheap, hard coded tricks and magical numbers
                if (node.RowIndex < 4)
                {
                    int centerColumn = Mathf.FloorToInt(availableNodes.Count / 2f);
                    int acceptedDistance = Mathf.CeilToInt(availableNodes.Count / 3f);
                    Debug.Log("AI: acceptedDistance: " + acceptedDistance);
                    // Check if the node is around the center
                    if (i > centerColumn - acceptedDistance  &&
                        i < centerColumn + acceptedDistance)
                    {
                        // Reset the node type back to empty
                        node.NodeType = NodeType.Empty;
                        Debug.Log("AI: Center move played");
                        Debug.Log("AI: " + node.RowIndex + ":" + node.ColumnIndex);
                        return node;
                    }
                }
            }

            // Fallback, if multiple moves tie, pick one at random
            await Task.Delay(100);
            // for testing purposes play a random move
            int randomIndex = Random.Range(0, availableNodes.Count);
            Debug.Log("AI Random move played: " + availableNodes[randomIndex].RowIndex + ":" + availableNodes[randomIndex].ColumnIndex);
            return availableNodes[randomIndex];
        }
    }
}
