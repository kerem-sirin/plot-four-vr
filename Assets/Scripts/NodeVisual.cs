using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace PlotFourVR
{
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class NodeVisual : MonoBehaviour
    {
        public event Action<VerticalAlignment, HorizontalAlignment> NodePositionSet;

        private Node node;

        private MeshRenderer meshRenderer;
        private ColliderWrapper nodeColliderWrapper;
        private XRSimpleInteractable xRSimpleInteractable;

        private RuntimeController runtimeController;


        public void Initialize(RuntimeController runtimeController,Node node)
        {
            this.runtimeController = runtimeController;

            this.node = node;
            node.NodeTypeChanged += OnNodeTypeChanged;

            meshRenderer = GetComponentInChildren<MeshRenderer>();

            nodeColliderWrapper = GetComponentInChildren<ColliderWrapper>();
            nodeColliderWrapper.ColliderInteracted += OnColliderInteracted;

            xRSimpleInteractable = GetComponent<XRSimpleInteractable>();

            xRSimpleInteractable.hoverEntered.AddListener(OnHoverEntered);
            xRSimpleInteractable.hoverExited.AddListener(OnHoverExited);
            xRSimpleInteractable.selectEntered.AddListener(OnSelectEntered);

            // Toggle child tile mesh renderers based on node position
            TileMesh[] tileMeshes = GetComponentsInChildren<TileMesh>();

            PublishNodePosition();
        }

        private void OnDestroy()
        {
            nodeColliderWrapper.ColliderInteracted -= OnColliderInteracted;

            xRSimpleInteractable.hoverEntered.RemoveListener(OnHoverEntered);
            xRSimpleInteractable.hoverExited.RemoveListener(OnHoverExited);
            xRSimpleInteractable.selectEntered.RemoveListener(OnSelectEntered);
        }

        private void OnHoverEntered(HoverEnterEventArgs arg0)
        {
            runtimeController.EventBus.InteractionEvents.InvokeNodeHoverEntered(node);
        }

        private void OnHoverExited(HoverExitEventArgs arg0)
        {
            runtimeController.EventBus.InteractionEvents.InvokeNodeHoverExited(node);
        }

        private void OnSelectEntered(SelectEnterEventArgs arg0)
        {
            runtimeController   .EventBus.InteractionEvents.InvokeNodeInteracted(node);
        }

        private void OnColliderInteracted()
        {
            // Handle the interaction with the node
            runtimeController.EventBus.InteractionEvents.InvokeNodeInteracted(node);
        }

        private void PublishNodePosition()
        {
            VerticalAlignment verticalAlignment = VerticalAlignment.Middle;
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center;
            if (node.RowIndex == 0)
            {
                // bottom row
                if (node.ColumnIndex == 0)
                {
                    // bottom left
                    verticalAlignment = VerticalAlignment.Bottom;
                    horizontalAlignment = HorizontalAlignment.Left;

                }
                else if (node.ColumnIndex == runtimeController.ColumnCount - 1)
                {
                    // bottom right
                    verticalAlignment = VerticalAlignment.Bottom;
                    horizontalAlignment = HorizontalAlignment.Right;
                }
                else
                {
                    // bottom center
                    verticalAlignment = VerticalAlignment.Bottom;
                    horizontalAlignment = HorizontalAlignment.Center;
                }
            }
            else if (node.RowIndex == runtimeController.RowCount - 1)
            {
                // top row
                if (node.ColumnIndex == 0)
                {
                    // top left
                    verticalAlignment = VerticalAlignment.Top;
                    horizontalAlignment = HorizontalAlignment.Left;
                }
                else if (node.ColumnIndex == runtimeController.ColumnCount - 1)
                {
                    // top right
                    verticalAlignment = VerticalAlignment.Top;
                    horizontalAlignment = HorizontalAlignment.Right;
                }
                else
                {
                    // top center
                    verticalAlignment = VerticalAlignment.Top;
                    horizontalAlignment = HorizontalAlignment.Center;
                }
            }
            else
            {
                // middle row
                if (node.ColumnIndex == 0)
                {
                    // middle left
                    verticalAlignment = VerticalAlignment.Middle;
                    horizontalAlignment = HorizontalAlignment.Left;
                }
                else if (node.ColumnIndex == runtimeController.ColumnCount - 1)
                {
                    // middle right
                    verticalAlignment = VerticalAlignment.Middle;
                    horizontalAlignment = HorizontalAlignment.Right;
                }
                else
                {
                    // middle center
                    verticalAlignment = VerticalAlignment.Middle;
                    horizontalAlignment = HorizontalAlignment.Center;
                }
            }
            NodePositionSet?.Invoke(verticalAlignment, horizontalAlignment);
        }

        private void OnNodeTypeChanged(NodeType nodeType)
        {
            switch (nodeType)
            {
                case NodeType.Empty:
                    meshRenderer.material.color = Color.white;
                    break;
                case NodeType.Yellow:
                    meshRenderer.material.color = Color.yellow;
                    break;
                case NodeType.Red:
                    meshRenderer.material.color = Color.red;
                    break;
                default:
                    throw new NotImplementedException($"{nodeType} is not implemented for material updates");
            }
        }
    }

    public enum NodeType
    {
        Empty,
        Yellow,
        Red
    }
}