using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UP_DecoSelectWhatIfPics : UP_DecoSelectPicsBase
{
    public override void OnPageEnable()
    {
        base.OnPageEnable();

        for (int i = 0; i < _contents.Count; i++)
        {
            UC_SelectablePicWhatIf whatif = _contents[i].GetComponent<UC_SelectablePicWhatIf>();
            //UC_SelectablePicWhatIf whatif_as = _contents[i] as UC_SelectablePicWhatIf;

            //whatif_as.SetJobName(ProfileModule.inst.ProfileReorderName[i]);
            (_contents[i] as UC_SelectablePicWhatIf).SetJobName(ProfileModule.inst.caricatureDetail[i+1].Korean_Title);
        }
    }
}
