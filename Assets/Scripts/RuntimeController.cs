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

        public static RuntimeController Instance { get; private set; }

        public event Action<StateType> GameStateChanged;
        // Player NodeTypes
        public NodeType PlayerOneNodeType => NodeType.Yellow;
        public NodeType PlayerTwoNodeType => NodeType.Red;

        public StateType CurrentState => currentState;
        [SerializeField] private StateType currentState;

        [SerializeField] private Transform nodeParentTransform;
        public NodeParent NodeParent => nodeParent;
        private NodeParent nodeParent;

        [SerializeField] private Transform uiMainTransform;
        private UiMainController uiMainController;

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
            RowCount = DEFAULT_GRID_HEIGHT;
            ColumnCount = DEFAULT_GRID_WIDTH;
            WinLength = DEFAULT_WIN_LENGTH;
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
            SetCurrentState(StateType.Initializing);
        }

        public async void SetCurrentState(StateType stateType)
        {
            await RefreshBeforePublishingNewState();

            currentState = stateType;
                
            if(currentState == StateType.Initializing)
            {
                // instantiate uiMainTransform
                if (uiMainController != null)
                {
                    Destroy(uiMainController.gameObject);
                }
                uiMainController = Instantiate(uiMainTransform).GetComponent<UiMainController>();
                uiMainController.Initialize(this);
                SetCurrentState(StateType.Idle);
            }

            if (currentState == StateType.Idle)
            {
                EventBus.UiEvents.RequestMenuPanel(PanelType.MainMenu);
            }

            if (currentState == StateType.GameStarting)
            {
                if (nodeParent != null)
                {
                    nodeParent.Destroy();
                }

                // instantiate nodeParentTransform
                nodeParent = Instantiate(nodeParentTransform).GetComponent<NodeParent>();
                nodeParent.Initialize(this);
            }

            if (currentState == StateType.EndingCurrentGame)
            {
                if (nodeParent != null)
                {
                    nodeParent.Destroy();
                    SetCurrentState(StateType.Idle);

                }
            }
            GameStateChanged?.Invoke(currentState);
        }

        public async Task RefreshBeforePublishingNewState()
        {
            EventBus.UiEvents.RequestMenuPanel(PanelType.None);
            GameStateChanged?.Invoke(StateType.None);
            await Task.Delay(500);
        }
    }

    public enum  StateType
    {
        None,
        GameStarting,
        PlayerOneTurn,
        PlayerTwoTurn,
        GameOver,
        EndingCurrentGame,
        Initializing,
        Idle,
    }

    public enum ResultType
    {
        None,
        PlayerOneWin,
        PlayerTwoWin,
        Draw
    }
}