using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using PlotFourVR.UI;

namespace PlotFourVR
{
    public class NodeParent : MonoBehaviour
    {
        private const float NODE_SPACING = 0.2f; // Space between nodes

        [SerializeField] private Transform nodePrefab;
        [SerializeField] private Transform columnHead;

        private int rowCount;
        public int ColumnCount => columnCount;
        private int columnCount;
        private int winLength;

        private DecideComputerMovement decideComputerMovement;
        private Dictionary<Node, Transform> nodeDictionary = new();
        private Dictionary<int, Transform> columnHeads = new();

        private StateType currentStateType;
        private RuntimeController runtimeController;
        private bool canPlayTile;

        public int PlayedTileCount => playedTileCount; // Number of tiles played by the player  
        private int playedTileCount = 0; // Number of tiles played
        private int totalTileCount = 0; // Total number of tiles in the grid
        public float PlayTime => playTime; // Time spent playing the game
        private float playTime = 0; // Time spent playing the game

        // Directions: horizontal, vertical, diag down-right, diag down-left
        // Are used to check for winning moves
        (int dr, int dc)[] directions = {
            (0, 1),
            (1, 0),
            (1, 1),
            (1, -1)
        };

        public void Initialize(RuntimeController runtimeController)
        {
            this.runtimeController = runtimeController;

            decideComputerMovement = new DecideComputerMovement(this);

            // Subscribe to events
            runtimeController.GameStateChanged += OnGameStateChanged;
            runtimeController.EventBus.InteractionEvents.NodeInteracted += OnNodeInteracted;

            // Draw the grid
            DrawGrid();
        }

        private void Update()
        {
            // Check if one of the players is playing
            if (currentStateType == StateType.PlayerOneTurn || currentStateType == StateType.PlayerTwoTurn
                || currentStateType == StateType.None)
            {
                // Update play time
                playTime += Time.deltaTime;
            }
        }

        private void OnDestroy()
        {
            runtimeController.GameStateChanged -= OnGameStateChanged;
            runtimeController.EventBus.InteractionEvents.NodeInteracted -= OnNodeInteracted;
        }

        private async void OnGameStateChanged(StateType stateType)
        {
            currentStateType = stateType;
            if(stateType is StateType.PlayerOneTurn or StateType.PlayerTwoTurn)
            {
                canPlayTile = true;
            }
            else if(stateType == StateType.PlayerThreeTurn)
            {
                canPlayTile = true;
                Node node = await decideComputerMovement.DecideMoveAsync();
                runtimeController.EventBus.InteractionEvents.InvokeNodeHoverEntered(node);
                // wait for a short delay to simulate thinking time
                await Task.Delay(400);
                OnNodeInteracted(node);
            }
            else if (stateType == StateType.GameOver)
            {
                // Handle game over state
                runtimeController.EventBus.UiEvents.RequestMenuPanel(PanelType.GameOverMenu);
            }
        }

        private void DrawGrid()
        {
            ResetVariables();

            // position parent transform at the center of the grid
            transform.position = new Vector3(-(Mathf.RoundToInt(columnCount / 2) * NODE_SPACING), transform.position.y, transform.position.z);

            InstantiateColumnHeadGameObjects();

            InstantiateNodes();

            transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                // Set the node parent transform to the center of the grid
                runtimeController.SetCurrentState(StateType.PlayerOneTurn);

                // get the top left node, broadcast the position for menu repositioning
                Node topLeftNode = GetNode(0, 0);
                Vector3 topLeftNodePostion = GetNodeTransform(topLeftNode).position;
                runtimeController.EventBus.UiEvents.RequestRepositionGridRelatedMenuPositioning(topLeftNodePostion);
            });
        }

        private void ResetVariables()
        {
            // Set grid dimensions
            rowCount = runtimeController.RowCount;
            columnCount = runtimeController.ColumnCount;
            winLength = runtimeController.WinLength;

            // Reset the game state
            runtimeController.GameResult = ResultType.None;

            // Clear dictionaries
            nodeDictionary.Clear();
            columnHeads.Clear();

            // Clear existing game objects
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            // Reset statistics
            // Start play time
            playTime = 0f;
            // Set total tile count
            totalTileCount = rowCount * columnCount;
            // reset played tile count
            playedTileCount = 0;
        }

        private void InstantiateColumnHeadGameObjects()
        {
            float columnHeadOffset = NODE_SPACING / 5f;
            // Initialize the column heads
            for (int column = 0; column < columnCount; column++)
            {
                // Instantiate the column head prefab
                Transform columnHeadTransform = Instantiate(columnHead, transform);

                // Name column head game object according to its position
                columnHeadTransform.name = $"ColumnHead_{column}";

                // Set the position of the column head.

                columnHeadTransform.localPosition = new Vector3(column * NODE_SPACING, rowCount * NODE_SPACING + columnHeadOffset, 0f);

                // Pass the column index to the column head component
                ColumnHeadBehaviour columnHeadComponent = columnHeadTransform.GetComponent<ColumnHeadBehaviour>();
                columnHeadComponent.Initialize(runtimeController, this, column, rowCount);

                // Add the column head to the dictionary
                columnHeads.Add(column, columnHeadTransform);
            }
        }

        private void InstantiateNodes()
        {
            // Initialize the grid of nodes
            for (int row = 0; row < rowCount; row++)
            {
                for (int column = 0; column < columnCount; column++)
                {
                    Node node = new Node(row, column, NodeType.Empty);

                    // Instantiate the node prefab
                    Transform nodeTransform = Instantiate(nodePrefab, new Vector3(column, 0, row), Quaternion.identity, transform);
                    nodeDictionary.Add(node, nodeTransform);

                    // Name node game object according to its position
                    nodeTransform.name = $"Node_{row}_{column}";

                    // Set the position of the node
                    nodeTransform.localPosition = new Vector3(column * NODE_SPACING, row * NODE_SPACING, 0f);

                    // Add the Node component to the node
                    NodeVisual nodeVisual = nodeTransform.GetComponent<NodeVisual>();

                    // Initialize the node with its row and column index
                    nodeVisual.Initialize(runtimeController, node);
                }
            }
        }

        private void OnNodeInteracted(Node node)
        {
            // Check if game state is not GameStarting or GameOver
            if (currentStateType is StateType.GameStarting or StateType.GameOver) return;
            if (!canPlayTile) return;

            canPlayTile = false;

            // Get the first available node in the column
            Node firstAvailableNode = GetFirstAvailableNodeInColumn(node.ColumnIndex);
            if (firstAvailableNode == null) return;
            MarkSelectedNode(firstAvailableNode);

            playedTileCount++;
            TurnEndChecks(firstAvailableNode);
        }

        private void MarkSelectedNode(Node firstAvailableNode)
        {
            // Set the node type to the active player's node type
            if (currentStateType == StateType.PlayerOneTurn)
            {
                firstAvailableNode.NodeType = runtimeController.PlayerOneNodeType;
            }
            else if (currentStateType == StateType.PlayerTwoTurn)
            {
                firstAvailableNode.NodeType = runtimeController.PlayerTwoNodeType;
            }
            else if(currentStateType == StateType.PlayerThreeTurn)
            {
                firstAvailableNode.NodeType = runtimeController.PlayerThreeNodeType;
            }
            runtimeController.EventBus.InteractionEvents.InvokeNodeTypeChanged(firstAvailableNode);
        }

        private void TurnEndChecks(Node firstAvailableNode)
        {
            // Check if it is the winning move
            if (IsWinningMove(firstAvailableNode))
            {
                if (currentStateType == StateType.PlayerOneTurn)
                {
                    runtimeController.GameResult = ResultType.PlayerOneWin;
                }
                else if (currentStateType == StateType.PlayerTwoTurn)
                {
                    runtimeController.GameResult = ResultType.PlayerTwoWin;
                }
                else if (currentStateType == StateType.PlayerThreeTurn)
                {
                    runtimeController.GameResult = ResultType.PlayerThreeWin;
                }
                runtimeController.SetCurrentState(StateType.GameOver);

                // Enable Vfx for winning Nodes
                foreach (Node winningNode in GetWinningTiles(firstAvailableNode))
                {
                    runtimeController.EventBus.InteractionEvents.InvokeWinningNodeDetected(winningNode);
                }
            }
            // Check if the game is a draw
            else if (playedTileCount >= totalTileCount)
            {
                runtimeController.GameResult = ResultType.Draw;
                runtimeController.SetCurrentState(StateType.GameOver);
            }
            // Noone won, there are still moves left
            else
            {
                // Check who played, switch to the other player's turn
                if (currentStateType == StateType.PlayerOneTurn)
                {
                    if (runtimeController.OpponentType == OpponentType.HomoSapiens)
                    {
                        runtimeController.SetCurrentState(StateType.PlayerTwoTurn);
                    }
                    else if (runtimeController.OpponentType == OpponentType.Hal9000)
                    {
                        runtimeController.SetCurrentState(StateType.PlayerThreeTurn);
                    }
                }
                else if (currentStateType == StateType.PlayerTwoTurn)
                {
                    runtimeController.SetCurrentState(StateType.PlayerOneTurn);
                }
                else if (currentStateType == StateType.PlayerThreeTurn)
                {
                    runtimeController.SetCurrentState(StateType.PlayerOneTurn);
                }
            }
        }

        public bool IsWinningMove(Node node)
        {
            int row = node.RowIndex;
            int col = node.ColumnIndex;
            NodeType activePlayersNodeType = node.NodeType;

            foreach (var (dr, dc) in directions)
            {
                int count = 0;
                // Slide a window from -(winLength - 1) to (winLength - 1) around the new piece
                int slideDistance = winLength - 1;
                for (int i = -slideDistance; i <= slideDistance; i++)
                {
                    int r = row + i * dr;
                    int c = col + i * dc;

                    // Check if the position is within grid bounds
                    if (r >= 0 && r < rowCount && c >= 0 && c < columnCount)
                    {
                        // Check if the node is matches the player's type
                        if (GetNode(r, c).NodeType == activePlayersNodeType)
                        {
                            count++;
                            if (count >= winLength)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            count = 0;
                        }
                    }
                }
            }
            return false;
        }

        private List<Node> GetWinningTiles(Node node)
        {
            int row = node.RowIndex;
            int col = node.ColumnIndex;
            NodeType activePlayersNodeType = node.NodeType;

            List<Node> winningNodes = new List<Node>();
            foreach (var (dr, dc) in directions)
            {
                int count = 0;
                // Slide a window from -(winLength - 1) to (winLength - 1) around the new piece
                int slideDistance = winLength - 1; // The distance to slide the window
                for (int i = -slideDistance; i <= slideDistance; i++)
                {
                    int r = row + i * dr;
                    int c = col + i * dc;

                    // Check if the position is within bounds
                    if (r >= 0 && r < rowCount && c >= 0 && c < columnCount)
                    {
                        // Check if the node is matches the player's type
                        Node nodeCompared = GetNode(r, c);
                        if (nodeCompared.NodeType == activePlayersNodeType)
                        {
                            winningNodes.Add(nodeCompared);
                            count++;
                            if (count >= winLength)
                            {
                                return winningNodes;
                            }
                        }
                        else
                        {
                            // if the node is not the same type, reset the count
                            // and clear the winning nodes list
                            winningNodes.Clear();
                            count = 0;
                        }
                    }
                }
            }
            return null;
        }

        private Node GetFirstAvailableNodeInColumn(int columnIndex)
        {
            Node firstAvailableNode = null;
            // Check nodes in the specified column from bottom to top
            for (int row = 0; row < rowCount; row++)
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

        public Node GetNode(int rowIndex, int columnIndex)
        {
            // Get the node from the dictionary using the row and column index
            foreach (KeyValuePair<Node, Transform> kvp in nodeDictionary)
            {
                if (kvp.Key.RowIndex == rowIndex && kvp.Key.ColumnIndex == columnIndex)
                {
                    return kvp.Key;
                }
            }

            Debug.LogWarning($"Node at ({rowIndex}, {columnIndex}) not found.");
            return default;
        }

        public Transform GetNodeTransform(Node node)
        {
            // Get the transform of the node from the dictionary
            if (nodeDictionary.TryGetValue(node, out Transform nodeTransform))
            {
                return nodeTransform;
            }
            Debug.LogWarning($"Node transform for {node} not found.");
            return null;
        }

        public List<Node> GetAvailableNodes()
        {
            List<Node> availableNodes = new List<Node>();
            // for eacg column, get the first available node on the row
            for (int column = 0; column < columnCount; column++)
            {
                Node firstAvailableNode = GetFirstAvailableNodeInColumn(column);
                if (firstAvailableNode == null) continue;
                availableNodes.Add(firstAvailableNode);
            }
            return availableNodes;
        }

        internal void Destroy()
        {
            transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutCirc).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }

        internal bool IsBelowNeighbourOccupied(Node node)
        {
            // Check if the node below is not empty
            int row = node.RowIndex;
            int col = node.ColumnIndex;

            // Check if the node is at the bottom row, no need to check below
            if (row == 0) return true;

            Node belowNode = GetNode(row - 1, col);
            if (belowNode != null && belowNode.NodeType != NodeType.Empty )
            {
                return true;
            }
            return false;
        }
    }
}