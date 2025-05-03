using DG.Tweening;
using UnityEngine;

namespace PlotFourVR
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(AudioSource))]
    public class NodeDisk : MonoBehaviour
    {
        [Header("Disk Materials")]
        [SerializeField] private Material yellowDiskMaterial;
        [SerializeField] private Material redDiskMaterial;

        [Header("Trail Materials")]
        [SerializeField] private Material yellowTrailMaterial;
        [SerializeField] private Material redTrailMaterial;

        private MeshRenderer meshRenderer;
        private AudioSource audioSource;
        private TrailRenderer trailRenderer;
        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            audioSource = GetComponent<AudioSource>();
            trailRenderer = GetComponentInChildren<TrailRenderer>();

            // set a random z rotation
            float randomZRotation = Random.Range(0f, 360f);
            transform.rotation = Quaternion.Euler(0f, 0f, randomZRotation);
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
                meshRenderer.material = redDiskMaterial;
                trailRenderer.material = redTrailMaterial;

            }
            else if (nodeType == NodeType.Yellow)
            {
                meshRenderer.material = yellowDiskMaterial;
                trailRenderer.material = yellowTrailMaterial;
            }
        }

        public void MoveToSlot(Vector3 position, float normalizedIndexDistance)
        {
            audioSource.volume = normalizedIndexDistance;
            transform.DOMove(position, 0.4f)
                .OnStart(() =>{
                    audioSource.Play();
                    trailRenderer.enabled = true;})
                .SetEase(Ease.OutBounce)
                .OnComplete(() => trailRenderer.enabled = false);
        }
    }
}