using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Vivestudios.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UC_AIProfileAlertPopup : UC_BaseComponent
    {
        public Action OnAlertClosed;
        [SerializeField]
        private Button _okBtn;
        private Image _bgImage;

        public override void InitComponent()
        {
            _okBtn?.onClick.AddListener(() =>
            {
                OnAlertClosed?.Invoke();
                gameObject.SetActive(false); 
            });

            _bgImage = GetComponent<Image>();
        }

        public void SetBGImage(Sprite sprite)
        {
            _bgImage.sprite = sprite;
        }
    }
}
