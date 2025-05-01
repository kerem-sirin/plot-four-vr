using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlotFourVR
{
    public class NodeParent : MonoBehaviour
    {
        [SerializeField] private Transform nodePrefab;
        [SerializeField] private float nodeSpacing; // Space between nodes
        [SerializeField] private Transform columnHead;

        private int rowCount;
        private int columnCount;
        private int winLength;

        private Vector2 mousePosition;

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
        (int dr, int dc)[] directions = {
            (0, 1),
            (1, 0),
            (1, 1),
            (1, -1)
        };

        public void Initialize(RuntimeController runtimeController)
        {
            this.runtimeController = runtimeController;

            // Subscribe to events
            runtimeController.GameStateChanged += OnGameStateChanged;
            runtimeController.EventBus.InteractionEvents.NodeInteracted += OnNodeInteracted;

            // Draw the grid
            DrawGrid();
        }

        private void Update()
        {
            // Check if one of the players is playing
            if (currentStateType == StateType.PlayerOneTurn || currentStateType == StateType.PlayerTwoTurn)
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

        private void OnGameStateChanged(StateType stateType)
        {
            currentStateType = stateType;
            if(stateType is StateType.PlayerOneTurn or StateType.PlayerTwoTurn)
            {
            canPlayTile = true;
            }
            else if (stateType == StateType.GameOver)
            {
                // Handle game over state
                runtimeController.EventBus.UiEvents.RequestMenuPanel(PanelType.GameOverMenu);
            }
        }

        private void DrawGrid()
        {
            rowCount = runtimeController.RowCount;
            columnCount = runtimeController.ColumnCount;
            winLength = runtimeController.WinLength;
            runtimeController.GameResult = ResultType.None;

            nodeDictionary.Clear();
            columnHeads.Clear();
            // Clear existing nodes
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            // Start play time
            playTime = 0f;
            // Set total tile count
            totalTileCount = rowCount * columnCount;
            // reset played tile count
            playedTileCount = 0;

            // position parent transform at the center of the grid
            float xOffset = columnCount  * 0.03f;
            transform.position = new Vector3(-(Mathf.RoundToInt(columnCount/2) * nodeSpacing) + xOffset, transform.position.y, transform.position.z);

            // Initialize the column heads
            for (int column = 0; column < columnCount; column++)
            {
                // Instantiate the column head prefab
                Transform columnHeadTransform = Instantiate(columnHead, transform);

                // Name column head game object according to its position
                columnHeadTransform.name = $"ColumnHead_{column}";

                // Set the position of the column head
                columnHeadTransform.localPosition = new Vector3(column * nodeSpacing, rowCount * nodeSpacing, 0f);

                // Pass the column index to the column head component
                ColumnHeadBehaviour columnHeadComponent = columnHeadTransform.GetComponent<ColumnHeadBehaviour>();
                columnHeadComponent.Initialize(runtimeController, this, column, rowCount);

                // Add the column head to the dictionary
                columnHeads.Add(column, columnHeadTransform);

            }

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
                    nodeTransform.localPosition = new Vector3(column * nodeSpacing, row * nodeSpacing, 0f);

                    // Add the Node component to the node
                    NodeVisual nodeVisual = nodeTransform.GetComponent<NodeVisual>();

                    // Initialize the node with its row and column index
                    nodeVisual.Initialize(runtimeController, node, columnHeads[column].position);
                }
            }

            runtimeController.SetCurrentState(StateType.PlayerOneTurn);
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

            // Set the node type to the active player's node type
            if (currentStateType == StateType.PlayerOneTurn)
            {
                firstAvailableNode.NodeType = runtimeController.PlayerOneNodeType;
            }
            else if (currentStateType == StateType.PlayerTwoTurn)
            {
                firstAvailableNode.NodeType = runtimeController.PlayerTwoNodeType;
            }
            runtimeController.EventBus.InteractionEvents.InvokeNodeTypeChanged(firstAvailableNode);

            playedTileCount++;
            // check if it is the winning move
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
                runtimeController.SetCurrentState(StateType.GameOver);
            }
            // Check if the game is a draw
            else if (playedTileCount >= totalTileCount)
            {
                runtimeController.GameResult = ResultType.Draw;
                runtimeController.SetCurrentState(StateType.GameOver);
            }
            else
            {
                // Switch to the other player's turn
                if (currentStateType == StateType.PlayerOneTurn)
                {
                    runtimeController.SetCurrentState(StateType.PlayerTwoTurn);
                }
                else if (currentStateType == StateType.PlayerTwoTurn)
                {
                    runtimeController.SetCurrentState(StateType.PlayerOneTurn);
                }
            }
        }

        // Check if placing at (row,col) for `player` makes four in a row
        public bool IsWinningMove(Node node)
        {
            int row = node.RowIndex;
            int col = node.ColumnIndex;
            NodeType activePlayersNodeType = node.NodeType;

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
            Debug.LogWarning($"No available node found in column {columnIndex}");
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
    }

    [Serializable]
    public class Node
    {
        public Node(int rowIndex, int columnIndex, NodeType nodeType)
        {
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
            this.nodeType = nodeType;
        }
        public int RowIndex => rowIndex;
        [SerializeField] private int rowIndex;
        public int ColumnIndex => columnIndex;
        [SerializeField] private int columnIndex;
        public NodeType NodeType { get => nodeType; set => nodeType = value;}
        [SerializeField] private NodeType nodeType;
    }
}