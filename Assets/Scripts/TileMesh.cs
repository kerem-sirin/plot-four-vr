using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace PlotFourVR
{
    public class TileMesh : MonoBehaviour
    {
        [SerializeField] private TileMeshType tileMeshType;

        [Header("References")]
        [SerializeField] private Material idleMaterial;
        [SerializeField] private Material hoverMaterial;

        private MeshRenderer meshRenderer;

        public void Initialize(XRSimpleInteractable xRSimpleInteractable, VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment)
        {
            bool isRightMesh = IsRightMesh(verticalAlignment, horizontalAlignment);
            gameObject.SetActive(isRightMesh);
            if (!isRightMesh) return;

            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.material = idleMaterial;
            RotateSelf(verticalAlignment, horizontalAlignment);
            // subscribe to the node's hover events
            xRSimpleInteractable.hoverEntered.AddListener(OnHoverEntered);
            xRSimpleInteractable.hoverExited.AddListener(OnHoverExited);

        }

        private void OnHoverEntered(HoverEnterEventArgs arg0)
        {
            // change the material to the hover material
            meshRenderer.material = hoverMaterial;
        }

        private void OnHoverExited(HoverExitEventArgs arg0)
        {
            // change the material back to the idle material
            meshRenderer.material = idleMaterial;
        }

        // toggles self based on the node position in the grid
        private bool IsRightMesh(VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment)
        {
            // enable/disable the tile mesh based on the node position
            if ((verticalAlignment == VerticalAlignment.Top && horizontalAlignment == HorizontalAlignment.Left) ||
                (verticalAlignment == VerticalAlignment.Bottom && horizontalAlignment == HorizontalAlignment.Left) ||
                (verticalAlignment == VerticalAlignment.Top && horizontalAlignment == HorizontalAlignment.Right) ||
                (verticalAlignment == VerticalAlignment.Bottom && horizontalAlignment == HorizontalAlignment.Right))
            {
                // any corner
                return tileMeshType == TileMeshType.Corner;
            }
            else if ((verticalAlignment == VerticalAlignment.Top && horizontalAlignment == HorizontalAlignment.Center) ||
                   (verticalAlignment == VerticalAlignment.Bottom && horizontalAlignment == HorizontalAlignment.Center) ||
                   (verticalAlignment == VerticalAlignment.Middle && horizontalAlignment == HorizontalAlignment.Left) ||
                   (verticalAlignment == VerticalAlignment.Middle && horizontalAlignment == HorizontalAlignment.Right))
            {
                // any side
                return tileMeshType == TileMeshType.Side;
            }
            else
            {
                return tileMeshType == TileMeshType.Center;
            }
        }

        // rotates self based on the node position in the grid
        private void RotateSelf(VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment)
        {
            float rotationZ = 0f;
            // rotate the tile mesh based on the node position
            if ((verticalAlignment == VerticalAlignment.Top && horizontalAlignment == HorizontalAlignment.Left) ||
                (verticalAlignment == VerticalAlignment.Top && horizontalAlignment == HorizontalAlignment.Center))
            {
                rotationZ = -90f;
            }
            else if ((verticalAlignment == VerticalAlignment.Top && horizontalAlignment == HorizontalAlignment.Right) ||
                    (verticalAlignment == VerticalAlignment.Middle && horizontalAlignment == HorizontalAlignment.Right))
            {
                rotationZ = 180f;
            }
            else if ((verticalAlignment == VerticalAlignment.Bottom && horizontalAlignment == HorizontalAlignment.Right) ||
                    (verticalAlignment == VerticalAlignment.Bottom && horizontalAlignment == HorizontalAlignment.Center))
            {
                rotationZ = 90f;
            }
            transform.transform.localRotation = Quaternion.Euler(0f, 0f, rotationZ);
        }
    }

    public enum TileMeshType
    {
        None,
        Center,
        Side,
        Corner,
    }

    public enum VerticalAlignment
    {
        Top,
        Middle,
        Bottom,
    }

    public enum HorizontalAlignment
    {
        Left,
        Center,
        Right,
    }
}