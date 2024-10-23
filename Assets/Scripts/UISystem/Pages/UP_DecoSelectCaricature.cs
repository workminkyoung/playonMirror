using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UP_DecoSelectCaricature : UP_DecoSelectPicsBase
{
    public override void OnPageEnable()
    {
        base.OnPageEnable();

        for (int i = 0; i < ProfileModule.inst.caricatureDetail.Count; i++)
        {
            UC_SelectablePicWhatIf whatif = _contents[i+1].GetComponent<UC_SelectablePicWhatIf>();
            (_contents[i] as UC_SelectablePicWhatIf).SetJobName(ProfileModule.inst.caricatureDetail[i+1].Korean_Title);
        }
    }
}
