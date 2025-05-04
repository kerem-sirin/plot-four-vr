using DG.Tweening;
using UnityEngine;

namespace PlotFourVR
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(AudioSource))]
    public class Disk : MonoBehaviour
    {
        [Header("Disk Materials")]
        [SerializeField] private Material yellowDiskMaterial;
        [SerializeField] private Material redDiskMaterial;
        [SerializeField] private Material greenDiskMaterial;

        [Header("Trail Materials")]
        [SerializeField] private Material yellowTrailMaterial;
        [SerializeField] private Material redTrailMaterial;
        [SerializeField] private Material greenTrailMaterial;

        private MeshRenderer meshRenderer;
        private AudioSource audioSource;
        private TrailRenderer trailRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            audioSource = GetComponent<AudioSource>();
            trailRenderer = GetComponentInChildren<TrailRenderer>();

            RandomizeRotation();
        }

        private void RandomizeRotation()
        {
            float randomZ = Random.Range(0f, 360f);
            transform.localRotation = Quaternion.Euler(0f, 0f, randomZ);
        }

        public void SetPosition(Vector3 position) => transform.position = position;
        public void Hide() => gameObject.SetActive(false);
        public void Show() => gameObject.SetActive(true);

        public void SetMaterial(NodeType nodeType)
        {
            (meshRenderer.material, trailRenderer.material) = nodeType switch
            {
                NodeType.Red => (redDiskMaterial, redTrailMaterial),
                NodeType.Yellow => (yellowDiskMaterial, yellowTrailMaterial),
                NodeType.Green => (greenDiskMaterial, greenTrailMaterial),
                _ => (meshRenderer.material, trailRenderer.material)
            };
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