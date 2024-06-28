using UnityEngine;

namespace Vivestudios.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UC_BaseComponent : MonoBehaviour
    {
        protected UP_BasePage _parentPage;
        protected CanvasGroup _canvasGroup;
        public CanvasGroup canvasGroup { get { return _canvasGroup ? _canvasGroup : GetComponent<CanvasGroup>(); } }

        public abstract void InitComponent ();

        protected internal void SetParent (UP_BasePage parent)
        {
            _parentPage = parent;
        }

        public virtual void SetActivate(bool state)
        {
            gameObject.SetActive(state);
        }
    }

}