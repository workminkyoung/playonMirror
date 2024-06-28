using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UC_SelectPhotoHorizon : UC_SelectPhoto
{
    public override void SetActivate(bool state)
    {
        base.SetActivate(state);
        if (state)
        {
            InitDictSelect();
            // Cartoon photo 생성
            for (int i = 0; i < _photoConverted.Count; i++)
                CreatePhoto(i, _photoConverted[i]);
        }
    }
}
