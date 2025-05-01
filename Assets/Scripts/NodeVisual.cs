using System;
using UnityEngine;

namespace PlotFourVR
{
    public class NodeVisual : MonoBehaviour
    {
        public Node Node => node;
        [SerializeField] private Node node;

        private MeshRenderer meshRenderer;
        private NodeColliderWrapper nodeColliderWrapper;

        private void Awake()
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            nodeColliderWrapper = GetComponentInChildren<NodeColliderWrapper>();
            nodeColliderWrapper.ColliderInteracted += OnColliderInteracted;
        }

        private void OnColliderInteracted()
        {
            // Handle the interaction with the node
            node.Interact();
        }

        public void Initialize(Node node)
        {
            this.node = node;
            node.NodeTypeChanged += OnNodeTypeChanged;
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