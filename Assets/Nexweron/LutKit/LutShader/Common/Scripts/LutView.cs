using UnityEngine;

namespace Nexweron.LutShader
{ 
    public class LutView : MonoBehaviour
    {
        [SerializeField] Material _materialLut2D;
        [SerializeField] Material _materialLut3D;
           
        private Material _lutMaterial;
        protected Material lutMaterial => _lutMaterial;
        
        private Texture _lutTexture;
        protected Texture lutTexture => _lutTexture;
        
        private float _contribution = 1;
        protected float contribution => _contribution;
        
        private readonly int _LutTexID = Shader.PropertyToID("_LutTex");
        private readonly int _LutContributionID = Shader.PropertyToID("_LutContribution");
        
        public void SetLutTexture(Texture value) {
            if (_lutTexture != value) {
                _lutTexture = value;
                UpdateLutTexture();
            }
        }

        private void UpdateLutTexture() {
            if (_lutTexture) {
                var materialPrefab = _materialLut2D;
                if (_lutTexture is Texture3D) {
                    materialPrefab = _materialLut3D;
                }
                
                if (!_lutMaterial) {
                    _lutMaterial = new Material(materialPrefab);
                    _lutMaterial.name += "(Instance)";
                } else
                if (_lutMaterial.shader != materialPrefab.shader) {
                    _lutMaterial.SetTexture(_LutTexID, null);
                    _lutMaterial.shader = materialPrefab.shader;
                    _lutMaterial.name = $"{materialPrefab.name}(Instance)";
                }
                
                _lutMaterial.SetFloat(_LutContributionID, _contribution);
                _lutMaterial.SetTexture(_LutTexID, _lutTexture);
                
                UpdateMaterial();
            }
        }

        protected virtual void UpdateMaterial() { }
        
        public void SetContribution(float value) {
            if (_contribution != value) {
                _contribution = value;
                if (_lutMaterial) {
                    _lutMaterial.SetFloat(_LutContributionID, _contribution);
                }
            }
        }

        void OnDestroy() {
            if (_lutMaterial) {
                DestroyImmediate(_lutMaterial);
            }
        }
    }
}