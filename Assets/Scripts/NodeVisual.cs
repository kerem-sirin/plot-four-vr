using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace PlotFourVR
{
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class NodeVisual : MonoBehaviour
    {
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