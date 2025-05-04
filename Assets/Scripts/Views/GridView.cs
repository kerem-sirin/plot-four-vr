using PlotFourVR.Components;
using PlotFourVR.Controllers;
using PlotFourVR.Helpers;
using PlotFourVR.Models;
using System.Collections.Generic;
using UnityEngine;

namespace PlotFourVR.Views
{
    /// <summary>
    /// Renders a GridModel into the scene (nodes, column heads) and exposes Transform lookup.
    /// </summary>
    public class GridView
    {
        private readonly Transform parent;
        private readonly Transform nodePrefab;
        private readonly Transform columnHeadPrefab;

        private Dictionary<Node, Transform> nodeTransforms = new();
        private Dictionary<int, ColumnHeadBehaviour> columnHeads = new();

        public GridView(Transform parent, Transform nodePrefab, Transform columnHeadPrefab)
        {
            this.parent = parent;
            this.nodePrefab = nodePrefab;
            this.columnHeadPrefab = columnHeadPrefab;
        }

        public void Build(GridModel model, GameLifecycleController lifecycle)
        {
            float spacing = GridLayoutService.Spacing;
            parent.position = new Vector3(-((model.ColumnCount - 1) * spacing) / 2, 0, 0);
            InstantiateColumnHeads(model, lifecycle, spacing);
            InstantiateNodes(model, lifecycle, spacing);
        }

        private void InstantiateColumnHeads(GridModel model, GameLifecycleController lifecycle, float spacing)
        {
            float headY = model.RowCount * spacing + spacing / 5f;
            for (int c = 0; c < model.ColumnCount; c++)
            {
                Transform go = Object.Instantiate(columnHeadPrefab, parent);
                go.name = $"ColumnHead_{c}";
                go.localPosition = new Vector3(c * spacing, headY, 0);
                ColumnHeadBehaviour comp = go.GetComponent<ColumnHeadBehaviour>();
                comp.Initialize(lifecycle, this, c, model.RowCount);
                columnHeads[c] = comp;
            }
        }

        private void InstantiateNodes(GridModel model, GameLifecycleController lifecycle, float spacing)
        {
            for (int r = 0; r < model.RowCount; r++)
                for (int c = 0; c < model.ColumnCount; c++)
                {
                    var node = model.GetNode(r, c);
                    Transform go = Object.Instantiate(nodePrefab, parent);
                    go.name = $"Node_{r}_{c}";
                    go.localPosition = new Vector3(c * spacing, r * spacing, 0);
                    NodeVisual visual = go.GetComponent<NodeVisual>();
                    visual.Initialize(lifecycle, node);
                    nodeTransforms[node] = go;
                }
        }

        public Transform GetTransform(Node node) => nodeTransforms.TryGetValue(node, out var t) ? t : null;
    }
}