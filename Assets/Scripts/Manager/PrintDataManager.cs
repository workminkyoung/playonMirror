using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintDataManager : SingletonBehaviour<PrintDataManager>
{
    [SerializeField]
    private List<string> _recordPaths = new List<string>();
    [SerializeField]
    private List<int> _selectedOrder = new List<int>();

    public List<string> recordPaths => _recordPaths;
    [SerializeField]


    protected override void Init()
    {
    }
}
