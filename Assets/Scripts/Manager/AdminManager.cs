using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdminManager : SingletonBehaviour<AdminManager>
{
    protected override void Init()
    {
        GameManager.OnGameResetAction += ResetAdminData;
    }

    public void ResetAdminData()
    {

    }
}
