using System.Collections.Generic;
using UnityEngine;

namespace PlotFourVR
{
    public class ColumnHeadBehaviour : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform diskPrefab;

        [Header("Materials")]
        [SerializeField] private Material redHighlightMaterial;
        [SerializeField] private Material yellowHighlightMaterial;
        [SerializeField] private Material greenDiskMaterial;

        [Header("Sfx")]
        [SerializeField] private AudioClip hoverSfx;
        [SerializeField] private AudioClip selectSfx;

        private int columnIndex; // The index of the column this head belongs to
        private int rowCount; // The number of rows in the grid

        private List<Disk> disks;

        private RuntimeController runtimeController;
        private Grid grid;
        private AudioSource audioSource;

        public void Initialize(RuntimeController runtimeController, Grid grid, int columnIndex, int rowCount)
        {
            this.runtimeController = runtimeController;
            this.grid = grid;
            this.columnIndex = columnIndex;
            this.rowCount = rowCount;

            runtimeController.EventBus.InteractionEvents.NodeHoverEntered += OnNodeHoverEntered;
            runtimeController.EventBus.InteractionEvents.NodeHoverExited += OnNodeHoverExited;
            runtimeController.EventBus.InteractionEvents.NodeTypeChanged += OnNodeTypeChanged;

            audioSource = GetComponent<AudioSource>();

            // create disk pool 
            disks = new List<Disk>(this.rowCount);
            for (int i = 0; i < this.rowCount; i++)
            {
                Disk disk = Instantiate(diskPrefab, transform).GetComponent<Disk>();
                disk.Hide();
                disks.Add(disk);
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

            // get the first disk from the queue
            if (disks.Count == 0)
            {
                Debug.LogWarning($"No disks available to highlight for pos: r{node.RowIndex}-c{node.ColumnIndex}");
                return;
            }
            ShowDiskAndSetMaterial(disks[0]);
            PlaySfx(hoverSfx);
        }

        private void OnNodeHoverExited(Node node)
        {
            if (node.ColumnIndex != columnIndex) return;

            // defensive check for the last disk, that is not going to be hidden
            if (disks.Count == 0) return;
            disks[0].Hide();
        }

        private void OnNodeTypeChanged(Node node)
        {
            if (node.ColumnIndex != columnIndex) return;

            // move the disk to the node position
            Vector3 targetPosition = grid.GetNodeTransform(node).position;

            if (disks.Count == 0)
            {
                Debug.LogWarning($"No disks available to move for pos: r{node.RowIndex}-c{node.ColumnIndex}");
                return;
            }
            ShowDiskAndSetMaterial(disks[0]);
            // normalize the distance as row index
            float normalizedIndexDistance = ((float)(rowCount - node.RowIndex) / rowCount);
            disks[0].MoveToSlot(targetPosition, normalizedIndexDistance);
            disks.RemoveAt(0);
        }

        private void ShowDiskAndSetMaterial(Disk disk)
        {
            disk.Show();
            // Set the highlight material depending on the player turn
            if (runtimeController.CurrentState == StateType.PlayerOneTurn)
            {
                disk.SetMaterial(NodeType.Yellow);
            }
            else if (runtimeController.CurrentState == StateType.PlayerTwoTurn)
            {
                disk.SetMaterial(NodeType.Red);
            }
            else if (runtimeController.CurrentState == StateType.PlayerThreeTurn)
            {
                disk.SetMaterial(NodeType.Green);
            }
        }

        private void PlaySfx(AudioClip audioClip)
        {
            audioSource.pitch = Random.Range(0.9f, 1.3f);
            audioSource.volume = Random.Range(0.8f, 1.0f);

            audioSource.PlayOneShot(audioClip);
        }
    }
}