using PlotFourVR.Controllers;
using PlotFourVR.Models;
using PlotFourVR.Views;
using System.Collections.Generic;
using UnityEngine;

namespace PlotFourVR.Components
{
    [RequireComponent(typeof(AudioSource))]
    public class ColumnHeadBehaviour : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform diskPrefab;

        [Header("Sfx")]
        [SerializeField] private AudioClip hoverSfx;
        [SerializeField] private AudioClip selectSfx;

        private int columnIndex; // The index of the column this head belongs to
        private int rowCount; // The number of rows in the grid

        private Queue<Disk> diskPool;

        private GameLifecycleController lifecycle;
        private GridView gridView;
        private AudioSource audioSource;

        public void Initialize(GameLifecycleController lifecycle, GridView gridView, int columnIndex, int rowCount)
        {
            this.lifecycle = lifecycle;
            this.gridView = gridView;
            this.columnIndex = columnIndex;
            this.rowCount = rowCount;

            lifecycle.EventBus.InteractionEvents.NodeHoverEntered += OnNodeHoverEntered;
            lifecycle.EventBus.InteractionEvents.NodeHoverExited += OnNodeHoverExited;
            lifecycle.EventBus.InteractionEvents.NodeTypeChanged += OnNodeTypeChanged;

            audioSource = GetComponent<AudioSource>();

            SetupDiskPool();
        }

        private void SetupDiskPool()
        {
            diskPool = new Queue<Disk>(rowCount);
            for (int i = 0; i < rowCount; i++)
            {
                var disk = Instantiate(diskPrefab, transform).GetComponent<Disk>();
                disk.Hide();
                diskPool.Enqueue(disk);
            }
        }

        private void OnDestroy()
        {
            lifecycle.EventBus.InteractionEvents.NodeHoverEntered -= OnNodeHoverEntered;
            lifecycle.EventBus.InteractionEvents.NodeHoverExited -= OnNodeHoverExited;
            lifecycle.EventBus.InteractionEvents.NodeTypeChanged -= OnNodeTypeChanged;
        }

        private void OnNodeHoverEntered(Node node)
        {
            if (node.ColumnIndex != columnIndex || diskPool.Count == 0) return;
            Disk disk = diskPool.Peek();
            ShowAndHighlight(disk);
            PlaySfx(hoverSfx);
        }


        private void OnNodeHoverExited(Node node)
        {
            if (node.ColumnIndex != columnIndex || diskPool.Count == 0) return;
            diskPool.Peek().Hide();
        }

        private void OnNodeTypeChanged(Node node)
        {
            if (node.ColumnIndex != columnIndex || diskPool.Count == 0) return;

            Disk disk = diskPool.Dequeue();
            Vector3 targetPos = gridView.GetTransform(node).position;
            ShowAndHighlight(disk);

            float volume = Mathf.Clamp01((float)(rowCount - node.RowIndex) / rowCount);
            disk.MoveToSlot(targetPos, volume);
            PlaySfx(selectSfx);
        }

        private void ShowAndHighlight(Disk disk)
        {
            disk.Show();
            NodeType type = lifecycle.CurrentState switch
            {
                StateType.PlayerOneTurn => NodeType.Yellow,
                StateType.PlayerTwoTurn => NodeType.Red,
                StateType.PlayerThreeTurn => NodeType.Green,
                _ => NodeType.Green
            };
            disk.SetMaterial(type);
        }

        private void PlaySfx(AudioClip audioClip)
        {
            audioSource.pitch = Random.Range(0.9f, 1.3f);
            audioSource.volume = Random.Range(0.8f, 1.0f);
            audioSource.PlayOneShot(audioClip);
        }
    }
}