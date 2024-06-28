using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressCount : MonoBehaviour
{
    List<Image> listImg = new List<Image>();
    int curCount = -1;
    int fillMax = 3;

    public void Setting()
    {
        listImg.AddRange(GetComponentsInChildren<Image>());
    }

    public void Init()
    {
        for (int i = 0; i < listImg.Count; i++)
        {
            listImg[i].enabled = false;
        }
        curCount = -1;
        listImg[(int)eImg.Circle].fillAmount = 1;
    }

    public void ProgressFill(int count, float value)
    {
        if (!listImg[(int)eImg.Circle].enabled)
        {
            listImg[(int)eImg.Circle].enabled = true;
        }

        if (curCount != count)
        {
            if(curCount != -1)
                listImg[count].enabled = false;
            listImg[count+1].enabled = true;
            curCount = count;
        }

        listImg[(int)eImg.Circle].fillAmount = 1-(count + value) / fillMax;
        if(listImg[(int)eImg.Circle].fillAmount <= 0)
        {
            for (int i = 0; i < listImg.Count; i++)
            {
                listImg[i].enabled = false;
            }
        }
    }
    
    enum eImg
    {
        Circle = 0,
        Count3,
        Count2,
        Count1
    }
}
