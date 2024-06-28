using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klak.Ndi;
using UnityEngine.UI;

public class NDIManager : SingletonBehaviour<NDIManager> 
{
    [SerializeField]
    NdiReceiver ndiReceiver;
    [SerializeField]
    Dropdown dropdown;
    CanvasGroup dropGroup;
    bool ndiMatched = false;
    static List<string> ndiList = new List<string>();

    public RenderTexture ndiTexture { get { return ndiReceiver.targetTexture; } }

    protected override void Init()
    {
        //hrow new System.NotImplementedException();
        ndiReceiver = GetComponentInChildren<NdiReceiver>();
        dropGroup = dropdown.GetComponent<CanvasGroup>();
    }

    public void ResetNdi()
    {
        // find ndi list
        foreach (var item in ndiList)
        {
            if (item.Contains("FLIP5"))
            {
                ndiReceiver.ndiName = item;
                return;
            }
        }
        foreach (var item in ndiList)
        {
            if (item.Contains("HX Camera"))
            {
                ndiReceiver.ndiName = item;
                return;
            }
        }
        ndiReceiver.ndiName = "GALAXY-Z-FLIP5 (NDI HX Camera)";
    }

    private void Start()
    {
        StartCoroutine(CheckDropdown());
        dropdown.onValueChanged.AddListener((index) =>
        {
            ndiReceiver.ndiName = ndiList[index];
        });

        StartCoroutine(CheckDropdown());
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.N))
    //        dropGroup.alpha = (dropGroup.alpha == 1) ? 0 : 1;
    //    if (Input.GetKeyDown(KeyCode.Q))
    //        Application.Quit();
    //}
    IEnumerator CheckDropdown()
    {
        //update dropdown ndi list
        while (true)
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

            yield return new WaitForSeconds(5);
        }
    }
}
