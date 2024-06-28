using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Klak.Ndi;

public class NdiList : MonoBehaviour
{
    public Dropdown dropdown;
    public NdiReceiver receiver;
    [SerializeField]
    List<string> ndiList = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        dropdown.onValueChanged.AddListener((index) =>
        {
            receiver.ndiName = ndiList[index];
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (ndiList != NdiFinder.sourceNames)
        {
            ndiList = new List<string>();
            dropdown.options.Clear();
            foreach (var item in NdiFinder.sourceNames)
            {
                ndiList.Add(item);
                Dropdown.OptionData option = new Dropdown.OptionData();
                option.text = item;
                dropdown.options.Add(option);
            }
        }
    }
}
