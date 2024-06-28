using System.Collections.Generic;
using UnityEngine;

namespace Nexweron.LutShader
{
    public class LutGallery : MonoBehaviour
    {
        [SerializeField] Transform _itemContainer;
        [SerializeField] LutGalleryItem _itemPrefab;
        [SerializeField] List<Texture> _lutTextures = new List<Texture>();
        
        void Awake() {
            _itemPrefab.gameObject.SetActive(false);
        }
        
        void Start() {
            LutGalleryItem firstItem = null;
            foreach (var lutTexture in _lutTextures) {
                var item = Instantiate(_itemPrefab, _itemContainer, false);
                item.SetLutTexture(lutTexture);
                item.gameObject.SetActive(true);
                if (!firstItem) firstItem = item;
            }
            
            if (firstItem) firstItem.SetSelected(true);
        }
    }
}
