using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UC_SelectablePicWhatIf : UC_SelectablePic
{
    [SerializeField]
    private TextMeshProUGUI _jobName;

    public void SetJobName(string name)
    {
        _jobName.text = "#" + name;
    }
}
