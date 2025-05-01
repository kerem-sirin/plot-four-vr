using DG.Tweening;
using UnityEngine;

namespace PlotFourVR
{
    public abstract class MenuController : MonoBehaviour
    {
        [Header("Base Menu Properties")]
        [SerializeField] private MenuType menuType;
        [SerializeField] protected GameObject parent;

        private MenuType initialMenuType = MenuType.MainMenu;
        protected CanvasGroup parentCanvasGroup;  

        protected virtual void Awake()
        {
            RuntimeController.Instance.EventBus.UiEvents.MenuPanelRequested += OnMenuPanelRequested;
            parentCanvasGroup = parent.GetComponent<CanvasGroup>();
            parent.SetActive(menuType == initialMenuType);
        }

        protected virtual void OnDestroy()
        {
            RuntimeController.Instance.EventBus.UiEvents.MenuPanelRequested -= OnMenuPanelRequested;
        }

        private void OnMenuPanelRequested(MenuType requestedMenuType)
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