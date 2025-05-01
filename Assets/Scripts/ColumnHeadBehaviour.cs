using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PlotFourVR
{
    public class ColumnHeadBehaviour : MonoBehaviour
    {
        private const float HIGHLIGHT_SCALE = 1f; // Scale factor for highlighting
        private const float DEFAULT_SCALE = 0.1f; // Default scale for the column head

        [SerializeField] private Transform diskTransform;

        [SerializeField] private Material redHighlightMaterial;
        [SerializeField] private Material yellowHighlightMaterial;

        private int columnIndex; // The index of the column this head belongs to
        private MeshRenderer meshRenderer;
        private RuntimeController runtimeController;

        private List<NodeDisk> nodeDisks;

        private NodeParent nodeParent;

        public void Initialize(RuntimeController runtimeController, NodeParent nodeParent, int columnIndex, int rowCount)
        {
            this.runtimeController = runtimeController;
            this.nodeParent = nodeParent;
            this.columnIndex = columnIndex;

            meshRenderer = GetComponentInChildren<MeshRenderer>();

            runtimeController.EventBus.InteractionEvents.NodeHoverEntered += OnNodeHoverEntered;
            runtimeController.EventBus.InteractionEvents.NodeHoverExited += OnNodeHoverExited;
            runtimeController.EventBus.InteractionEvents.NodeTypeChanged += OnNodeTypeChanged;

            // create disk pool 
            nodeDisks = new List<NodeDisk>(rowCount);
            for (int i = 0; i < rowCount; i++)
            {
                NodeDisk disk = Instantiate(diskTransform, transform).GetComponent<NodeDisk>();
                disk.Hide();
                nodeDisks.Add(disk);
            }
        }

        private void OnDestroy()
        {
            runtimeController.EventBus.InteractionEvents.NodeHoverEntered -= OnNodeHoverEntered;
            runtimeController.EventBus.InteractionEvents.NodeHoverExited -= OnNodeHoverExited;
            runtimeController.EventBus.InteractionEvents.NodeTypeChanged -= OnNodeTypeChanged;

        }

        private void OnNodeHoverEntered(Node node)
        {
            if (node.ColumnIndex != columnIndex) return;

            // Highlight the column head
            HighlightColumnHead();
        }

        private void OnNodeHoverExited(Node node)
        {
            if (node.ColumnIndex != columnIndex) return;

            // Remove highlight from the column head
            RemoveHighlightFromColumnHead();
        }

        private void OnNodeTypeChanged(Node node)
        {
            if(node.ColumnIndex != columnIndex) return;

            /*
            // Change the node type
            NodeType nodeType = node.NodeType;
            if (nodeType == NodeType.Red)
            {
                meshRenderer.material = redHighlightMaterial;
            }
            else if (nodeType == NodeType.Yellow)
            {
                meshRenderer.material = yellowHighlightMaterial;
            }
            */
            // move the disk to the node position
            Vector3 targetPosition = nodeParent.GetNodeTransform(node).position;
            nodeDisks[0].MoveToSlot(targetPosition);
            nodeDisks.RemoveAt(0);
        }

        private void HighlightColumnHead()
        {
            // if the current state is not of the player turns, do not highlight
            if (runtimeController.CurrentState != StateType.PlayerOneTurn &&
                runtimeController.CurrentState != StateType.PlayerTwoTurn) return;

            // get the first disk from the queue
            nodeDisks[0].Show();

            // Set the highlight material depending on the player turn
            if (runtimeController.CurrentState == StateType.PlayerOneTurn)
            {
                nodeDisks[0].SetMaterial(NodeType.Yellow);
            }
            else if (runtimeController.CurrentState == StateType.PlayerTwoTurn)
            {
                nodeDisks[0].SetMaterial(NodeType.Red);
            }
        }

        private void RemoveHighlightFromColumnHead()
        {
            // defensive check for the last disk, that is not going to be hidden
            if (nodeDisks.Count == 0) return;
            nodeDisks[0].Hide();
        }
    }
}