using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace PlotFourVR.Components
{
    public class TileMesh : MonoBehaviour
    {
        [SerializeField] private TileMeshType tileMeshType;

        [Header("References")]
        [SerializeField] private Material idleMaterial;
        [SerializeField] private Material hoverMaterial;

        private MeshRenderer meshRenderer;
        XRSimpleInteractable xRSimpleInteractable;

        public void Initialize(XRSimpleInteractable xRSimpleInteractable, VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment)
        {
            // determine what kind of mesh this should be
            TileMeshType neededType = DetermineMeshType(verticalAlignment, horizontalAlignment);
            bool isActive = neededType == tileMeshType;
            gameObject.SetActive(isActive);
            if (!isActive) return;

            // cache renderer & set idle material
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.material = idleMaterial;

            // apply correct orientation
            float rotZ = DetermineRotationZ(verticalAlignment, horizontalAlignment);
            transform.localRotation = Quaternion.Euler(0f, 0f, rotZ);
            // subscribe to the node's hover events
            this.xRSimpleInteractable = xRSimpleInteractable;
            this.xRSimpleInteractable.hoverEntered.AddListener(OnHoverEntered);
            this.xRSimpleInteractable.hoverExited.AddListener(OnHoverExited);
        }

        private void OnDestroy()
        {
            // unsubscribe from the hover events
            if (xRSimpleInteractable == null) return;
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

        private TileMeshType DetermineMeshType(VerticalAlignment v, HorizontalAlignment h)
            => (v, h) switch
            {
                // corners
                (VerticalAlignment.Top, HorizontalAlignment.Left) => TileMeshType.Corner,
                (VerticalAlignment.Top, HorizontalAlignment.Right) => TileMeshType.Corner,
                (VerticalAlignment.Bottom, HorizontalAlignment.Left) => TileMeshType.Corner,
                (VerticalAlignment.Bottom, HorizontalAlignment.Right) => TileMeshType.Corner,

                // sides
                (VerticalAlignment.Top, HorizontalAlignment.Center) => TileMeshType.Side,
                (VerticalAlignment.Middle, HorizontalAlignment.Left) => TileMeshType.Side,
                (VerticalAlignment.Middle, HorizontalAlignment.Right) => TileMeshType.Side,
                (VerticalAlignment.Bottom, HorizontalAlignment.Center) => TileMeshType.Side,

                // center of the grid
                _ => TileMeshType.Center,
            };

        private float DetermineRotationZ( VerticalAlignment v, HorizontalAlignment h) 
            => (v, h) switch
            {
                // rotate -90° for top-left & top-center
                (VerticalAlignment.Top, HorizontalAlignment.Left) => -90f,
                (VerticalAlignment.Top, HorizontalAlignment.Center) => -90f,

                // rotate 180° for top-right & middle-right
                (VerticalAlignment.Top, HorizontalAlignment.Right) => 180f,
                (VerticalAlignment.Middle, HorizontalAlignment.Right) => 180f,

                // rotate +90° for bottom-right & bottom-center
                (VerticalAlignment.Bottom, HorizontalAlignment.Right) => 90f,
                (VerticalAlignment.Bottom, HorizontalAlignment.Center) => 90f,

                // no rotation needed for bottom-left & middle-left
                _ => 0f,
            };
    }

    public enum TileMeshType { Center, Side, Corner }
    public enum VerticalAlignment { Top, Middle, Bottom }
    public enum HorizontalAlignment { Left, Center, Right }
}