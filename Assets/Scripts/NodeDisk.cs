using DG.Tweening;
using UnityEngine;

namespace PlotFourVR
{
    [RequireComponent(typeof(MeshRenderer))]
    public class NodeDisk : MonoBehaviour
    {
        [SerializeField] private Material yellowMaterial;
        [SerializeField] private Material redMaterial;

        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void SetMaterial(NodeType nodeType)
        {
            // Set the material based on the node type
            if (nodeType == NodeType.Red)
            {
                meshRenderer.material = redMaterial;
            }
            else if (nodeType == NodeType.Yellow)
            {
                meshRenderer.material = yellowMaterial;
            }
        }

        public void MoveToSlot(Vector3 position)
        {
            transform.DOMove(position, 0.4f).SetEase(Ease.OutBounce);
        }
    }
}