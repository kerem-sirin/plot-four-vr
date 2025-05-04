using PlotFourVR.Events;
using PlotFourVR.UI;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace PlotFourVR
{
    /// <summary>
    /// Manages high-level game states, grid setup, and UI transitions.
    /// </summary>
    public class GameLifescycleController : MonoBehaviour
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

        [SerializeField] private Transform gridPrefab;
        public GridController GridController => gridController;
        private GridController gridController;

        [SerializeField] private Transform uiMainPrefab;
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
            ApplyDefaultSettings();
        }

        private void Start()
        {
            _ = SetGameStateAsync(StateType.Initializing);
        }

        private void ApplyDefaultSettings()
        {
            RowCount = DEFAULT_GRID_HEIGHT;
            ColumnCount = DEFAULT_GRID_WIDTH;
            WinLength = DEFAULT_WIN_LENGTH;
            OpponentType = OpponentType.HomoSapiens;
        }

        private void OnGridWidthChanged(int width) => ColumnCount = width;
        private void OnGridHeightChanged(int height) => RowCount = height;
        private void OnWinLengthChanged(int length) => WinLength = length;
        private void OnOpponentTypeChanged(OpponentType opponent) => OpponentType = opponent;
        public bool IsGridSizeValid() => RowCount >= WinLength || ColumnCount >= WinLength;

        public async Task SetGameStateAsync(StateType stateType)
        {
            await TransitionDelayAsync();

            currentState = stateType;
                
            if(currentState == StateType.Initializing)
            {
                // instantiate UI_Main.prefab
                await InitializeUIAsync();
            }
            else if (currentState == StateType.Idle)
            {
                // User is interacting with main menu and settings in this state
                ShowMainMenu();
            }
            else if (currentState == StateType.GameStarting)
            {
                // Settings are already set, create the grid in this state
                StartNewGame();
            }
            else if (currentState == StateType.EndingCurrentGame)
            {
                // Destroy the grid and go back to the main menu& settings
                EndCurrentGame();
            }
            GameStateChanged?.Invoke(currentState);
        }

        private async Task TransitionDelayAsync()
        {
            EventBus.UiEvents.RequestMenuPanel(PanelType.None);
            GameStateChanged?.Invoke(StateType.None);
            await Task.Delay(500);
        }

        private async Task InitializeUIAsync()
        {
            if (uiMainController != null)
            {
                Destroy(uiMainController.gameObject);
            }

            uiMainController = Instantiate(uiMainPrefab).GetComponent<UiMainController>();
            uiMainController.Initialize(this);

            await SetGameStateAsync(StateType.Idle);
        }

        private void ShowMainMenu()
        {
            EventBus.UiEvents.RequestMenuPanel(PanelType.MainMenu);
        }

        private void StartNewGame()
        {
            if (gridController != null)
            {
                gridController.Destroy();
            }

            gridController = Instantiate(gridPrefab).GetComponent<GridController>();
            gridController.Initialize(this);
        }

        private void EndCurrentGame()
        {
            if (gridController == null)return;

            gridController.Destroy();
            _ = SetGameStateAsync(StateType.Idle);
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