using UnityEngine;
using UnityEngine.UI;

namespace Nexweron.LutShader
{
    public class LutViewRenderer : LutView
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Text _nameTF;
        
        protected override void UpdateMaterial() {
            _renderer.material = lutMaterial;
            _nameTF.text = lutTexture.name;
        }
    }
}