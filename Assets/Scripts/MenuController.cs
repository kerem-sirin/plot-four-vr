using DG.Tweening;
using UnityEngine;

namespace PlotFourVR
{
    public abstract class MenuController : MonoBehaviour
    {
        [Header("Base Menu Properties")]
        [SerializeField] private PanelType menuType;
        [SerializeField] protected GameObject parent;

        protected CanvasGroup parentCanvasGroup;  

        protected RuntimeController runtimeController;

        internal virtual void Initialize(RuntimeController runtimeController, PanelType initialMenuType)
        {
            this.runtimeController = runtimeController;

            RuntimeController.Instance.EventBus.UiEvents.MenuPanelRequested += OnMenuPanelRequested;
            parentCanvasGroup = parent.GetComponent<CanvasGroup>();
            parent.SetActive(menuType == initialMenuType);

            Initialize();
        }

        protected abstract void Initialize();

        protected virtual void OnDestroy()
        {
            RuntimeController.Instance.EventBus.UiEvents.MenuPanelRequested -= OnMenuPanelRequested;
        }

        private void OnMenuPanelRequested(PanelType requestedMenuType)
        {
            if (requestedMenuType == menuType)
            {
                PanelEnabled();
            }
            else
            {
                PanelDisabled();
            }
        }

        protected void PanelDisabled()
        {
            parentCanvasGroup.DOFade(0, 0.5f)
                .OnStart(() =>
                {
                    parentCanvasGroup.interactable = false;
                    parentCanvasGroup.blocksRaycasts = false;
                })
                .OnComplete(() =>
                {
                    parent.SetActive(false);
                });
        }

        protected virtual void PanelEnabled()
        {
            parentCanvasGroup.DOFade(1, 0.5f)
            .OnStart(() =>
            {
                parentCanvasGroup.alpha = 0f;
                parent.SetActive(true);

            })
            .OnComplete(() =>
            {
                parentCanvasGroup.interactable = true;
                parentCanvasGroup.blocksRaycasts = true;
            });
        }
    }
}