using UnityEngine;

namespace PlotFourVR
{
    public class UiMainController : MonoBehaviour
    {
        [SerializeField] private MenuController[] menuPanels;
        public void Initialize(RuntimeController runtimeController)
        {
            // Initialize UI elements and event listeners here
            menuPanels = GetComponentsInChildren<MenuController>(true);

            foreach (var panel in menuPanels)
            {
                panel.Initialize(runtimeController);
            }
        }
    }
}
