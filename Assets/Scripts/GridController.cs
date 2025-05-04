using DG.Tweening;
using NUnit.Framework;
using PlotFourVR.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace PlotFourVR
{
    /// <summary>
    /// Coordinates model, view, input, AI, and game rules.
    /// </summary>
    public class GridController : MonoBehaviour
    {
        [SerializeField] private Transform nodePrefab;
        [SerializeField] private Transform columnHeadPrefab;

        private GridModel model;
        private GridView view;
        private GameLifescycleController lifecycle;
        private DecideComputerMovement ai;

        private bool canPlay;
        public int PlayedCount { get; private set; }
        private int totalCount;
        public float ElapsedTime { get; private set; }

        public void Initialize(GameLifescycleController lifecycle)
        {
            this.lifecycle = lifecycle;
            model = new GridModel(lifecycle.RowCount, lifecycle.ColumnCount, lifecycle.WinLength);
            view = new GridView(transform, nodePrefab, columnHeadPrefab);
            view.Build(model, lifecycle);

            ai = new DecideComputerMovement(model);

            lifecycle.GameStateChanged += OnStateChanged;
            lifecycle.EventBus.InteractionEvents.NodeInteracted += OnNodeInteracted;

            totalCount = lifecycle.RowCount * lifecycle.ColumnCount;

            // position parent transform at the center of the grid
            transform.position = GridLayoutService.ComputeGridOffset(lifecycle.ColumnCount);

            transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                // fire the event to change state
                _ = lifecycle.SetGameStateAsync(StateType.PlayerOneTurn);

                // get the top left node, broadcast the position for menu repositioning
                Node topLeftNode = model.GetNode(0, 0);
                Vector3 topLeftNodePostion = view.GetTransform(topLeftNode).position;
                lifecycle.EventBus.UiEvents.RequestRepositionGridRelatedMenuPositioning(topLeftNodePostion);
            });
        }

        private void Update()
        {
            // Check if one of the players is playing
            if (lifecycle.CurrentState is StateType.PlayerOneTurn or StateType.PlayerTwoTurn
                or StateType.PlayerThreeTurn or StateType.None)
            {
                // Update play time
                ElapsedTime += Time.deltaTime;
            }
        }

        private void OnDestroy()
        {
            lifecycle.GameStateChanged -= OnStateChanged;
            lifecycle.EventBus.InteractionEvents.NodeInteracted -= OnNodeInteracted;
        }

        // we want keep the event handler return type as void, 
        // so we use a Task to handle the async call
        private void OnStateChanged(StateType state)
        {
            _ = HandleStateChangedAsync(state);
        }

        private async Task HandleStateChangedAsync(StateType state)
        {
            switch (state)
            {
                case StateType.PlayerOneTurn:
                    canPlay = true;
                    break;
                case StateType.PlayerTwoTurn:
                    canPlay = true;
                    break;
                case StateType.PlayerThreeTurn:
                    canPlay = true;
                    Node move = await ai.DecideMoveAsync();
                    lifecycle.EventBus.InteractionEvents.InvokeNodeInteracted(move);
                    break;
                case StateType.GameOver:
                    lifecycle.EventBus.UiEvents.RequestMenuPanel(PanelType.GameOverMenu);
                    break;
            }
        }

        private void OnNodeInteracted(Node node)
        {
            if (!canPlay || lifecycle.CurrentState is StateType.GameStarting or StateType.GameOver) return;
            canPlay = false;

            Node firstAvailableNode = model.GetFirstAvailableNodeInColumn(node.ColumnIndex);
            firstAvailableNode.NodeType = lifecycle.CurrentState switch
            {
                StateType.PlayerOneTurn => lifecycle.PlayerOneNodeType,
                StateType.PlayerTwoTurn => lifecycle.PlayerTwoNodeType,
                _ => lifecycle.PlayerThreeNodeType,
            };
            lifecycle.EventBus.InteractionEvents.InvokeNodeTypeChanged(firstAvailableNode);
            PlayedCount++;
            CheckEndConditions(firstAvailableNode);
        }

        private void CheckEndConditions(Node node)
        {
            if (model.IsWinningMove(node))
            {
                lifecycle.GameResult = lifecycle.CurrentState switch
                {
                    StateType.PlayerOneTurn => ResultType.PlayerOneWin,
                    StateType.PlayerTwoTurn => ResultType.PlayerTwoWin,
                    _ => ResultType.PlayerThreeWin,
                };

                // Enable Vfx for winning Nodes
                foreach (Node winningNode in model.GetWinningTiles(node))
                {
                    lifecycle.EventBus.InteractionEvents.InvokeWinningNodeDetected(winningNode);
                }
                _ = lifecycle.SetGameStateAsync(StateType.GameOver);
                return;
            }
            if (PlayedCount >= totalCount)
            {
                lifecycle.GameResult = ResultType.Draw;
                _ = lifecycle.SetGameStateAsync(StateType.GameOver);
                return;
            }
            var next = GetNextState();
            _ = lifecycle.SetGameStateAsync(next);
        }

        private StateType GetNextState()
        {
            switch (lifecycle.CurrentState)
            {
                case StateType.PlayerOneTurn:
                    return lifecycle.OpponentType == OpponentType.HomoSapiens
                        ? StateType.PlayerTwoTurn
                        : StateType.PlayerThreeTurn;
                default:
                    return StateType.PlayerOneTurn;
            }
        }

        internal void Destroy()
        {
            transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutCirc).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
    }
}