using System;
using UnityEngine;

namespace PlotFourVR
{
    public class RuntimeController : MonoBehaviour
    {
        public const int DefaultGridWidth = 9;
        public const int DefaultGridHeight = 7;
        public const int DefaultWinLength = 4;

        public static RuntimeController Instance { get; private set; }

        public event Action<StateType> GameStateChanged;
        // Player NodeTypes
        public NodeType PlayerOneNodeType => NodeType.Yellow;
        public NodeType PlayerTwoNodeType => NodeType.Red;

        public StateType CurrentState => currentState;
        [SerializeField] private StateType currentState;

        public EventBus EventBus { get; private set; }
        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }
        public int WinLength { get; private set; }

        public ResultType GameResult { get; set; } = ResultType.None;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            EventBus = new EventBus();

            EventBus.SettingEvents.GridWidthChanged += OnGridWidthChanged;
            EventBus.SettingEvents.GridHeightChanged += OnGridHeightChanged;
            EventBus.SettingEvents.WinLengthChanged += OnWinLengthChanged;

            // Set default values
            RowCount = DefaultGridHeight;
            ColumnCount = DefaultGridWidth;
            WinLength = DefaultWinLength;
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

        private void Start()
        {
            // Set initial menu
            EventBus.UiEvents.RequestMenuPanel(PanelType.MainMenu);
        }

        public void SetCurrentState(StateType stateType)
        {
            currentState = stateType;
            GameStateChanged?.Invoke(currentState);
        }
    }

    public enum  StateType
    {
        None,
        GameStarting,
        PlayerOneTurn,
        PlayerTwoTurn,
        GameOver,
    }

    public enum ResultType
    {
        None,
        PlayerOneWin,
        PlayerTwoWin,
        Draw
    }
}