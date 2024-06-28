using UnityEngine;
using UnityEngine.UI;

namespace Nexweron.LutShader
{
    public class LutViewGraphic : LutView
    {
        [SerializeField] private Graphic _graphic;
        [SerializeField] private Text _nameTF;
        
        protected override void UpdateMaterial() {
            _graphic.material = lutMaterial;
            _nameTF.text = lutTexture.name;
        }
    }
}