using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vivestudios.UI
{
    public class CartoonManager : SingletonBehaviour<CartoonManager>
    {
        public static List<Cartoon> cartoons = new List<Cartoon>();
        protected override void Init()
        {
            //throw new System.NotImplementedException();
        }

        public void SetCartoon(Cartoon cartoon)
        {
            cartoons.Add(cartoon);
        }
    }

    [Serializable]
    public class Cartoon
    {
        public CARTOON_TYPE type;
        public string model;
        public Texture2D reference;
    }
}
