using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Security.Policy;
using RotaryHeart.Lib.SerializableDictionary;
using Newtonsoft.Json;
using System.Collections.Generic;

public abstract class PaymentModule : SingletonBehaviour<PaymentModule>
{
    [SerializeField]
    [TextArea]
    protected string _response = string.Empty;
    protected string _requestForm = string.Empty;
    protected string _requestMethod = string.Empty;
    protected string _priceForm = string.Empty;
    protected string _errorCodeFileName;
    protected string _errorContent;

    protected Encoding _responseEncoding;
    protected ErrorCodeDicBase _errorCodeDict = new ErrorCodeDicBase();

    [Serializable]
    protected class ErrorCodeDicBase : SerializableDictionaryBase<string, object> { }

    public string ErrorContent => _errorContent;

    public virtual IEnumerator PaymentRoutine(int price, Action<bool, string, bool> OnResponse)
    {
        bool isDone = false;
        _response = string.Empty;
        _priceForm = SetPriceForm(price);
        SetRequestForm();

        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_requestForm);
            request.Method = _requestMethod;

            if(_requestMethod == "POST")
            {
                Stream requestStream = request.GetRequestStream();
                requestStream.Close();
            }

            request.BeginGetResponse(new AsyncCallback((IAsyncResult iarres) =>
            {
                try
                {
                    using (HttpWebResponse resp = (HttpWebResponse)request.EndGetResponse(iarres))
                    {
                        using (StreamReader streamReader = new StreamReader(resp.GetResponseStream(), _responseEncoding))
                        {
                            _response = streamReader.ReadToEnd();
                            isDone = true;
                            streamReader.Close();
                        }
                        resp.Close();
                    }
                }
                catch (Exception e)
                {
                    _response = e.Message;
                    isDone = true;
                }
            }), null);
        }
        catch (Exception e)
        {
            _response = e.Message;
            isDone = true;
        }

        while (!isDone)
        {
            yield return null;
        }

        //CustomLogger.Log(_response);

        SuccessCheck(OnResponse);
    }
    protected abstract void SuccessCheck(Action<bool, string, bool> OnResponse);

    protected abstract void SetResponseEncoding();
    protected abstract void SetRequestForm();
    protected abstract void SetRequestMethod();
    protected abstract void SetErrorCodePath();
    protected abstract string SetPriceForm(int price);

    private void GetErrorCodeDict()
    {
        string path = Application.streamingAssetsPath + "/Mailing/" + _errorCodeFileName;

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SerializableDictionaryBase<string, object> dd = JsonConvert.DeserializeObject<SerializableDictionaryBase<string, object>>(json);
            ErrorCodeDicBase rawErrorCode = JsonConvert.DeserializeObject<ErrorCodeDicBase>(json);

            _errorCodeDict = new ErrorCodeDicBase();
            foreach (var pair in rawErrorCode)
            {
                JObject root = JObject.Parse(pair.Value.ToString());
                JToken isSend = root.GetValue("sendMail");
                if (isSend.ToString().ToLower() == "true")
                {
                    _errorCodeDict.Add(pair.Key, pair.Value);
                    //CustomLogger.LogFormat("Key : {0}, Value : {1}", pair.Key, pair.Value);
                }
            }
        }
        else
        {
            CustomLogger.Log("No Error Code File");
        }
    }

    protected override void Init()
    {
        SetResponseEncoding();
        SetRequestMethod();
        SetErrorCodePath();
        GetErrorCodeDict();
    }
}


