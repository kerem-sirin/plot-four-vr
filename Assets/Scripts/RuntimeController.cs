using PlotFourVR.Events;
using PlotFourVR.UI;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace PlotFourVR
{
    public class RuntimeController : MonoBehaviour
    {
        public const int DEFAULT_GRID_WIDTH = 9;
        public const int DEFAULT_GRID_HEIGHT = 7;
        public const int DEFAULT_WIN_LENGTH = 4;

        public event Action<StateType> GameStateChanged;

        // Player NodeTypes
        public NodeType PlayerOneNodeType => NodeType.Yellow;
        public NodeType PlayerTwoNodeType => NodeType.Red;
        public NodeType PlayerThreeNodeType => NodeType.Green;

        public StateType CurrentState => currentState;
        private StateType currentState;

        [SerializeField] private Transform nodeParentTransform;
        public NodeParent NodeParent => nodeParent;
        private NodeParent nodeParent;

        [SerializeField] private Transform uiMainTransform;
        private UiMainController uiMainController;

        public EventBus EventBus { get; private set; }
        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }
        public int WinLength { get; private set; }
        public OpponentType OpponentType { get; private set; }

        public ResultType GameResult { get; set; } = ResultType.None;

        private void Awake()
        {
            EventBus = new EventBus();

            EventBus.SettingEvents.GridWidthChanged += OnGridWidthChanged;
            EventBus.SettingEvents.GridHeightChanged += OnGridHeightChanged;
            EventBus.SettingEvents.WinLengthChanged += OnWinLengthChanged;
            EventBus.SettingEvents.OpponentTypeChanged += OnOpponentTypeChanged;

            // Set default values
            RowCount = DEFAULT_GRID_HEIGHT;
            ColumnCount = DEFAULT_GRID_WIDTH;
            WinLength = DEFAULT_WIN_LENGTH;
            OpponentType = OpponentType.HomoSapiens;
        }

        private void Start()
        {
            SetCurrentState(StateType.Initializing);
        }

        private void OnWinLengthChanged(int newValue)
        {
            // Update the win length
            WinLength = newValue;
        }

        private void OnGridHeightChanged(int newValue)
        {
            // Update the grid height
            RowCount = newValue;
        }

        private void OnGridWidthChanged(int newValue)
        {
            // Update the grid width
            ColumnCount = newValue;
        }

        private void OnOpponentTypeChanged(OpponentType opponentType)
        {
            // Update the opponent type
            OpponentType = opponentType;
        }

        public bool IsGridSizeValid()
        {
            // Check if the grid size is valid
            return RowCount >= WinLength || ColumnCount >= WinLength;
        }

        public async void SetCurrentState(StateType stateType)
        {
            await RefreshBeforePublishingNewState();

            currentState = stateType;
                
            if(currentState == StateType.Initializing)
            {
                // instantiate UI_Main.prefab
                if (uiMainController != null)
                {
                    Destroy(uiMainController.gameObject);
                }
                uiMainController = Instantiate(uiMainTransform).GetComponent<UiMainController>();
                uiMainController.Initialize(this);
                SetCurrentState(StateType.Idle);
            }
            else if (currentState == StateType.Idle)
            {
                // User is interacting with main menu and settings in this state
                EventBus.UiEvents.RequestMenuPanel(PanelType.MainMenu);
            }
            else if (currentState == StateType.GameStarting)
            {
                // Settings are already set, create the grid in this state
                if (nodeParent != null)
                {
                    nodeParent.Destroy();
                }

                // instantiate nodeParentTransform
                nodeParent = Instantiate(nodeParentTransform).GetComponent<NodeParent>();
                nodeParent.Initialize(this);
            }
            else if (currentState == StateType.EndingCurrentGame)
            {
                // Destroy the grid and go back to the main menu& settings
                if (nodeParent == null) return;

                nodeParent.Destroy();
                SetCurrentState(StateType.Idle);
            }
            GameStateChanged?.Invoke(currentState);
        }

        public async Task RefreshBeforePublishingNewState()
        {
            EventBus.UiEvents.RequestMenuPanel(PanelType.None);
            GameStateChanged?.Invoke(StateType.None);
            await Task.Delay(600);
        }
    }

    public enum  StateType
    {
        None, // transition state between other states
        GameStarting, // settings are set, creatring the grid
        PlayerOneTurn, // Main player, yellow
        PlayerTwoTurn, // human player, red
        GameOver,
        EndingCurrentGame, // destroying the grid and going back to the main menu
        Initializing, // initial state main menu is instantiated
        Idle, // main menu active, waiting for user input
        PlayerThreeTurn, // AI player, green
    }

    public enum ResultType
    {
        None,
        PlayerOneWin,
        PlayerTwoWin,
        PlayerThreeWin,
        Draw
    }

    public enum  OpponentType
    {
        HomoSapiens,
        Hal9000,
    }
}