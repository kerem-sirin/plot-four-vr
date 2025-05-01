using UnityEngine;

namespace PlotFourVR
{
    public class TileMesh : MonoBehaviour
    {
        public TileMeshType TileMeshType => tileMeshType;
        [SerializeField] private TileMeshType tileMeshType;

        private void Awake()
        {
            GetComponentInParent<NodeVisual>().NodePositionSet += OnNodePositionSet;
        }

        private void OnDestroy()
        {
            GetComponentInParent<NodeVisual>().NodePositionSet -= OnNodePositionSet;
        }

        private void OnNodePositionSet(VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment)
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

            float rotationZ = 0f;
            // rotate the tile mesh based on the node position
            if((verticalAlignment == VerticalAlignment.Top && horizontalAlignment == HorizontalAlignment.Left) ||
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