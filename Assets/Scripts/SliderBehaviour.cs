using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlotFourVR
{
    public class SliderBehaviour : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI sliderValueText;
        [SerializeField] private Slider slider;
        public Slider Slider => slider;

        private void Awake()
        {
            GetComponentInChildren<Slider>().onValueChanged.AddListener(OnSliderValueChanged);
        }

        private void Start()
        {
            // Set the initial value of the slider
            sliderValueText.text = slider.value.ToString();
        }


        private void OnDestroy()
        {
            GetComponentInChildren<Slider>().onValueChanged.RemoveListener(OnSliderValueChanged);
        }

        private void OnSliderValueChanged(float sliderValue)
        {
            // Update the text with the current slider value
            sliderValueText.text = sliderValue.ToString();
        }
    }
}