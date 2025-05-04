using DG.Tweening;
using PlotFourVR.Controllers;
using UnityEngine;

namespace PlotFourVR.UI.MenuControllers
{
    public abstract class MenuController : MonoBehaviour
    {
        [Header("Base Menu Properties")]
        [SerializeField] private PanelType menuType;
        [SerializeField] protected GameObject parent;

        protected CanvasGroup parentCanvasGroup;  

        protected GameLifecycleController lifecycle;

        internal virtual void Initialize(GameLifecycleController lifecycle)
        {
            this.lifecycle = lifecycle;

            this.lifecycle.EventBus.UiEvents.MenuPanelRequested += OnMenuPanelRequested;
            parentCanvasGroup = parent.GetComponent<CanvasGroup>();

            Initialize();
        }

        protected abstract void Initialize();

        protected virtual void OnDestroy()
        {
            lifecycle.EventBus.UiEvents.MenuPanelRequested -= OnMenuPanelRequested;
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