using PlotFourVR.Components;
using PlotFourVR.Controllers;
using PlotFourVR.Models;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace PlotFourVR.Views
{
    /// <summary>
    /// Responsible for displaying the node visual in the game,
    /// toggling interaction layers, handling hover/selection events,
    /// depending on the game state and node state.
    /// </summary>
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class NodeVisual : MonoBehaviour
    {
        private Node node;
        private XRSimpleInteractable xRSimpleInteractable;
        private GameLifecycleController lifecycle;
        private ParticleSystem winningParticleSystem;

        public void Initialize(GameLifecycleController lifecycle,Node node)
        {
            this.lifecycle = lifecycle;

            this.node = node;

            this.lifecycle.EventBus.InteractionEvents.NodeTypeChanged += OnNodeTypeChanged;
            this.lifecycle.EventBus.InteractionEvents.WinningNodeDetected += OnWinningNodeDetected;
            this.lifecycle.GameStateChanged += OnGameStateChanged;

            xRSimpleInteractable = GetComponent<XRSimpleInteractable>();

            xRSimpleInteractable.hoverEntered.AddListener(OnHoverEntered);
            xRSimpleInteractable.hoverExited.AddListener(OnHoverExited);
            xRSimpleInteractable.selectEntered.AddListener(OnSelectEntered);

            winningParticleSystem = GetComponentInChildren<ParticleSystem>();

            // Toggle child tile mesh renderers based on node position
            TileMesh[] tileMeshes = GetComponentsInChildren<TileMesh>();

            UpdateNodeRotationInChildMeshes();
        }

        private void OnDestroy()
        {
            lifecycle.EventBus.InteractionEvents.NodeTypeChanged -= OnNodeTypeChanged;
            lifecycle.GameStateChanged -= OnGameStateChanged;

            xRSimpleInteractable.hoverEntered.RemoveListener(OnHoverEntered);
            xRSimpleInteractable.hoverExited.RemoveListener(OnHoverExited);
            xRSimpleInteractable.selectEntered.RemoveListener(OnSelectEntered);
        }

        private void OnGameStateChanged(StateType stateType)
        {
            // Disable XR interaction when the game is not in a player's turn
            xRSimpleInteractable.interactionLayers = 0;

            if (stateType == StateType.PlayerOneTurn || stateType == StateType.PlayerTwoTurn)
            {
                if (node.NodeType == NodeType.Empty)
                {
                    // Enable XR interaction
                    xRSimpleInteractable.interactionLayers = 1;
                }
            }
        }

        private void OnWinningNodeDetected(Node node)
        {
            if (this.node != node) return;
            winningParticleSystem.Play();
            ParticleSystem.MainModule particleSystemMain = winningParticleSystem.main;
            if (node.NodeType == NodeType.Yellow)
            {
                particleSystemMain.startColor = Color.yellow;
            }
            else if (node.NodeType == NodeType.Red)
            {
                particleSystemMain.startColor = Color.red;
            }
            else if (node.NodeType == NodeType.Green)
            {
                particleSystemMain.startColor = Color.green;
            }
        }

        private void OnNodeTypeChanged(Node node)
        {
            if (this.node != node) return;

            //disable xr interaction
            xRSimpleInteractable.interactionLayers = 0;
        }

        private void OnHoverEntered(HoverEnterEventArgs arg0)
        {
            // Show the node disk at the column head position
            lifecycle.EventBus.InteractionEvents.InvokeNodeHoverEntered(node);
        }

        private void OnHoverExited(HoverExitEventArgs arg0)
        {
            // Hide the node disk
            lifecycle.EventBus.InteractionEvents.InvokeNodeHoverExited(node);
        }

        private void OnSelectEntered(SelectEnterEventArgs arg0)
        {
            lifecycle.EventBus.InteractionEvents.InvokeNodeInteracted(node);
        }

        private void UpdateNodeRotationInChildMeshes()
        {
            VerticalAlignment verticalAlignment = VerticalAlignment.Middle;
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center;

            // Determine the vertical alignment based on the node's position
            if (node.RowIndex == 0)
            {
                // bottom row
                verticalAlignment = VerticalAlignment.Bottom;
            }
            else if (node.RowIndex == lifecycle.RowCount - 1)
            {
                // top row
                verticalAlignment = VerticalAlignment.Top;
            }
            else
            {
                // middle row
                verticalAlignment = VerticalAlignment.Middle;
            }

            // Determine the horizontal alignment based on the node's position
            if (node.ColumnIndex == 0)
            {
                // leftmost column
                horizontalAlignment = HorizontalAlignment.Left;
            }
            else if (node.ColumnIndex == lifecycle.ColumnCount - 1)
            {
                // rightmost column
                horizontalAlignment = HorizontalAlignment.Right;
            }
            else
            {
                // center column
                horizontalAlignment = HorizontalAlignment.Center;
            }

            TileMesh[] tileMeshes = GetComponentsInChildren<TileMesh>();
            foreach (TileMesh tileMesh in tileMeshes)
            {
                tileMesh.Initialize(xRSimpleInteractable, verticalAlignment, horizontalAlignment);
            }
        }
    }
}

namespace PlotFourVR
{
    public enum NodeType
    {
        Empty,
        Yellow,
        Red,
        Green
    }
}