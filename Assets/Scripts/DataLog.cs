using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class DataLog : SingletonBehaviour<DataLog>
{
    string[] pageNames =
    {
        "A2",
        "B1-1",
        "B1-2",
        "B1-4",
        "B2-1",
        "B3-1",
        "B3-3",
        "B4-1",
        "B4-2",
        "B4-3",
        "C1",
        "D1",
        "D1-1",
        "D2"
    };

    public string[] contentNames =
    {
        "Con1",
        "Con2",
        "Con3",
        "Con4"
    };

    public string[] frameNames =
    {
        "Frm1",
        "Frm2"
    };

    string[] feedbackNames =
    {
        "신기해요",
        "자연스러워요",
        "재밌어요",
        "간편해요",
        "또 할래요",
        "뻔해요",
        "어색해요",
        "오래 걸려요",
        "지루해요",
        "체험이 짧아요",
        "불편해요"
    };

    public string home = "Home";
    public string btn = "Btn";
    public string slide = "Sld";
    public string boundary = "Bdr";
    public string inp = "Inp";
    public string tip = "B1-1-2";

    [SerializeField]
    string curLog = string.Empty;

    protected override void Init()
    {

    }

    public void StartLog()
    {
        curLog = string.Empty;
        curLog += System.DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss-fff") + ",";
    }

    public void AddComponent(ePage page, string compName)
    {
        string log = System.DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss-fff") + "." + pageNames[(int)page] + "." + compName;
        curLog += log + ",";
    }
    public void AddComponent(ePage page, string compName, int value)
    {
        string log = System.DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss-fff") + "." + pageNames[(int)page] + "." + compName + "." + value;
        curLog += log + ",";
    }
    public void AddComponent(ePage page, int value, List<string> feedbacks)
    {
        Dictionary<string, object> body = new Dictionary<string, object>();
        body.Add("star", value);
        body.Add("feedback", feedbacks);
        string json = JsonConvert.SerializeObject(body);

        string log = System.DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss-fff") + "." + pageNames[(int)page] + "." + json;
        curLog += log + ",";
    }
    public void AddComponent(string page, string compName)
    {
        string log = System.DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss-fff") + "." + page + "." + compName;
        curLog += log + ",";
    }

    public void SendLog()
    {
        if (curLog == string.Empty)
            return;

        curLog.TrimEnd(',');
        StorageManager.Instance.SendLog(curLog, ResetLog);
    }

    public void ResetLog()
    {
        curLog = string.Empty;
    }

    public enum ePage
    {
        A2,
        B1_1,
        B1_2,
        B1_4,
        B2_1,
        B3_1,
        B3_3,
        B4_1,
        B4_2,
        B4_3,
        C1,
        D1,
        D1_1,
        D2
    } 
}
