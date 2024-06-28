using UnityEngine;
using UnityEngine.UI;

namespace Nexweron.LutShader
{
    public class LutGalleryItem : MonoBehaviour
    {
        [SerializeField] LutView _lutView;
        [SerializeField] Graphic _graphic;
        [SerializeField] Toggle _toggle;
        
        [SerializeField] Material _materialLut2D;
        [SerializeField] Material _materialLut3D;
        
        private readonly int _LutTexID = Shader.PropertyToID("_LutTex");
        
        private Texture _lutTexture;
        private Material _material;
        private bool _isSelected = false;
        
        public void SetLutTexture(Texture lutTex) {
            _lutTexture = lutTex;
            if (_lutTexture) {
                var materialPrefab = _materialLut2D;
                if (_lutTexture is Texture3D) {
                    materialPrefab = _materialLut3D;
                }
                
                if (!_material) {
                    _material = new Material(materialPrefab);
                    _material.name += "(Instance)";
                } else
                if (_material.shader != materialPrefab.shader) {
                    _material.SetTexture(_LutTexID, null);
                    _material.shader = materialPrefab.shader;
                    _material.name = $"{materialPrefab.name}(Instance)";
                }
                _material.SetTexture("_LutTex", _lutTexture);
            }
            _graphic.material = _material;
        }
        
        public void SetSelected(bool value) {
            if (_toggle.isOn != value) {
                _toggle.isOn = value;
                UpdateSelected();
            }
        }
        
        public void UpdateSelected() {
            if (_isSelected != _toggle.isOn) {
                _isSelected = _toggle.isOn;

                if (_isSelected) {
                    _lutView.SetLutTexture(_lutTexture);
                }
            }
        }
        
        void OnDestroy() {
            if (_material) {
                DestroyImmediate(_material);
            }
        }
    }
}
