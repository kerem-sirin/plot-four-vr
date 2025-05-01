using UnityEngine;

namespace PlotFourVR
{
    public class TileMesh : MonoBehaviour
    {
        [SerializeField] private TileMeshType tileMeshType;

        public void Initialize(VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment)
        {
            ToggleSelf(verticalAlignment, horizontalAlignment);
            RotateSelf(verticalAlignment, horizontalAlignment);
        }

        // toggles self based on the node position in the grid
        private void ToggleSelf(VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment)
        {
            // enable/disable the tile mesh based on the node position
            if ((verticalAlignment == VerticalAlignment.Top && horizontalAlignment == HorizontalAlignment.Left) ||
                (verticalAlignment == VerticalAlignment.Bottom && horizontalAlignment == HorizontalAlignment.Left) ||
                (verticalAlignment == VerticalAlignment.Top && horizontalAlignment == HorizontalAlignment.Right) ||
                (verticalAlignment == VerticalAlignment.Bottom && horizontalAlignment == HorizontalAlignment.Right))
            {
                // any corner
                gameObject.SetActive(tileMeshType == TileMeshType.Corner);
            }
            else if ((verticalAlignment == VerticalAlignment.Top && horizontalAlignment == HorizontalAlignment.Center) ||
                   (verticalAlignment == VerticalAlignment.Bottom && horizontalAlignment == HorizontalAlignment.Center) ||
                   (verticalAlignment == VerticalAlignment.Middle && horizontalAlignment == HorizontalAlignment.Left) ||
                   (verticalAlignment == VerticalAlignment.Middle && horizontalAlignment == HorizontalAlignment.Right))
            {
                // any side
                gameObject.SetActive(tileMeshType == TileMeshType.Side);
            }
            else
            {
                gameObject.SetActive(tileMeshType == TileMeshType.Center);
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