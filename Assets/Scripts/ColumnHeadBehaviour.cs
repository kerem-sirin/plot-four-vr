using System.Collections.Generic;
using UnityEngine;

namespace PlotFourVR
{
    public class ColumnHeadBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform diskPrefab;

        [SerializeField] private Material redHighlightMaterial;
        [SerializeField] private Material yellowHighlightMaterial;

        private int columnIndex; // The index of the column this head belongs to

        private List<NodeDisk> nodeDisks;

        private RuntimeController runtimeController;
        private NodeParent nodeParent;

        public void Initialize(RuntimeController runtimeController, NodeParent nodeParent, int columnIndex, int rowCount)
        {
            this.runtimeController = runtimeController;
            this.nodeParent = nodeParent;
            this.columnIndex = columnIndex;

            runtimeController.EventBus.InteractionEvents.NodeHoverEntered += OnNodeHoverEntered;
            runtimeController.EventBus.InteractionEvents.NodeHoverExited += OnNodeHoverExited;
            runtimeController.EventBus.InteractionEvents.NodeTypeChanged += OnNodeTypeChanged;

            // create disk pool 
            nodeDisks = new List<NodeDisk>(rowCount);
            for (int i = 0; i < rowCount; i++)
            {
                NodeDisk disk = Instantiate(diskPrefab, transform).GetComponent<NodeDisk>();
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
            // if the current state is not of the player turns, do not highlight
            if (runtimeController.CurrentState != StateType.PlayerOneTurn &&
                runtimeController.CurrentState != StateType.PlayerTwoTurn) return;

            // get the first disk from the queue
            if (nodeDisks.Count == 0)
            {
                Debug.LogWarning($"No disks available to highlight for pos: r{node.RowIndex}-c{node.ColumnIndex}");
                return;
            }
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

        private void OnNodeHoverExited(Node node)
        {
            if (node.ColumnIndex != columnIndex) return;

            // defensive check for the last disk, that is not going to be hidden
            if (nodeDisks.Count == 0) return;
            nodeDisks[0].Hide();
        }

        private void OnNodeTypeChanged(Node node)
        {
            if(node.ColumnIndex != columnIndex) return;

            // move the disk to the node position
            Vector3 targetPosition = nodeParent.GetNodeTransform(node).position;

            if (nodeDisks.Count == 0)
            {
                Debug.LogWarning($"No disks available to move for pos: r{node.RowIndex}-c{node.ColumnIndex}");
                return;
            }
            nodeDisks[0].MoveToSlot(targetPosition);
            nodeDisks.RemoveAt(0);
        }
    }
}