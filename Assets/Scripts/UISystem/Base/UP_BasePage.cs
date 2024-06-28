using UnityEngine;

namespace Vivestudios.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UP_BasePage : MonoBehaviour
    {
        private UC_BaseComponent[] _childCompos = null;
        protected PC_BasePageController _pageController = null;
        public PC_BasePageController pageController { get { return _pageController; } }
        protected CanvasGroup _canvasGroup = null;
        public CanvasGroup canvasGroup => _canvasGroup;
        [SerializeField]
        protected PAGE_TYPE _pageType;
        public PAGE_TYPE pageType { get { return _pageType; } }

        public virtual void AwakePage()
        {
            _childCompos = GetComponentsInChildren<UC_BaseComponent>(true);
            _canvasGroup = GetComponent<CanvasGroup>();

            SetPageType(_pageType);
            InitPage();

            foreach (var compo in _childCompos)
            {
                compo.SetParent(this);
                compo.InitComponent();
            }

            BindDelegates();
            GameManager.OnGameResetAction += OnPageReset;
        }
        protected void SetPageType(PAGE_TYPE type)
        {
            _pageType = type;
            _pageController.AddPage(this);
        }

        public abstract void InitPage();
        public abstract void BindDelegates();
        public virtual void EnablePage(bool isEnable)
        {
            if (!isEnable && this.gameObject.activeInHierarchy)
            {
                OnPageDisable();
            }

            gameObject.SetActive(isEnable);

            if (isEnable)
            {
                OnPageEnable();
            }
        }

        public abstract void OnPageEnable();
        public abstract void OnPageDisable();
        protected abstract void OnPageReset();
        public void SetPageController(PC_BasePageController pageController)
        {
            _pageController = pageController;
        }
    }
}
