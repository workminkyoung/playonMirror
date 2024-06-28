using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vivestudios.UI;

public partial class UC_AiCartoon : UC_BaseComponent
{
    //public Action<Texture2D> SendResult;
    Cartoon _cartoon;
    string _sourceEncodeText;// reference
    string _targetEncodeText;// photo

    bool _checkRequestDone = false;
    List<Texture2D> _readyRequestTexures = new List<Texture2D>();
    [SerializeField]
    int _requestCount = 0;
    [SerializeField]
    int _curRequestCount = 0;

    //public int requestCount { set { _requestCount = value; } }

    public void SetCartoon(Cartoon cartoon)
    {
        _cartoon = cartoon;
        if (cartoon.reference == null)
            return;
        byte[] sourceBytes = cartoon.reference.EncodeToPNG();
        _sourceEncodeText = Convert.ToBase64String(sourceBytes);
    }

    public override void InitComponent()
    {
        //throw new NotImplementedException();
    }

    //public void ResetComp(int count)
    //{
    //    _readyRequestTexures = new List<Texture2D>();
    //    _requestCount = count; 
    //    _curRequestCount = 0;

    //}

    public void RequestDiffusion(Texture2D photo)
    {
        //_readyRequestTexures.Add(photo);
    }

    //public void StartCheckRequest()
    //{
    //    StartCoroutine(CheckAndRequest());
    //}

    //IEnumerator CheckAndRequest()
    //{
    //    while(_curRequestCount < _requestCount)
    //    {
    //        yield return new WaitUntil(() => _readyRequestTexures.Count-1 >= _curRequestCount);

    //        byte[] targetBytes = _readyRequestTexures[_curRequestCount].EncodeToPNG();
    //        _targetEncodeText = Convert.ToBase64String(targetBytes);
    //        ApiCall.Instance._sourceEncodeText = _sourceEncodeText;
    //        ApiCall.Instance._targetEncodeText = _targetEncodeText;
    //        ApiCall.Instance.SendResult = (tex) =>
    //        {
    //            SendResult(tex);
    //            _curRequestCount++;
    //            _checkRequestDone = true;
    //        }; 
    //        ApiCall.Instance.FindTagger(_cartoon);
    //        _checkRequestDone = false;

    //        yield return new WaitUntil(() => _checkRequestDone);

    //        Debug.Log("check done");
    //    }

    //    Debug.Log("End of request");
    //}

}
