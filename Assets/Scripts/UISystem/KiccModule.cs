using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class KiccModule : PaymentModule
{
    private const string URL = "http://127.0.0.1:8090/";
    private const string REQUEST_FORMAT = "D1^^{0}^00^^^^^^^^20^A^^^^^^^^^^^UTF-8^^^^^^^^^^^^^";
    private const string CALL_BACK = "callback=jsonp12345678983543344";
    private const string REQUEST_KEY = "REQ=";

    private const string SUCCESS_CHECK_KEY = "SUC";
    private const string MSG_KEY = "MSG";
    private const string SUCCEED_VALUE = "00";
    private const string APPROVE_CHECK_KEY = "RS04";
    private const string APPROVE_SUCCEED_VALUE = "0000";
    private const string APPROVE_FAIL_KEY = "RS16";

    protected override void SetErrorCodePath()
    {

    }

    protected override string SetPriceForm(int price)
    {
        return price.ToString();
    }

    protected override void SetRequestForm()
    {
        _requestForm = URL + "?" + CALL_BACK + "&" + REQUEST_KEY + UnityWebRequest.EscapeURL(string.Format(REQUEST_FORMAT, _priceForm));
    }

    protected override void SetRequestMethod()
    {
        _requestMethod = "POST";
    }

    protected override void SetResponseEncoding()
    {
        _responseEncoding = Encoding.UTF8;
    }

    protected override void SuccessCheck(Action<bool, string, bool> OnResponse)
    {
        if (string.IsNullOrEmpty(_response))
        {
            return;
        }

        if (_response.Contains("Error: ConnectFailure"))
        {
            //Debug.Log("프로그램 없음");
            OnResponse?.Invoke(false, "프로그램 미실행", true);
            return;
        }

        Regex reg = new Regex(@"\{.*\}");
        Match match = reg.Match(_response);

        Debug.Log(match);

        JObject root = new JObject();
        try
        {
            root = JObject.Parse(match.Value);

            JToken token = root.GetValue(SUCCESS_CHECK_KEY);

            if (token.ToString() == SUCCEED_VALUE)
            {
                //Debug.Log("결제성공 -> 승인확인");

                JToken approveToken = root.GetValue(APPROVE_CHECK_KEY);
                if (approveToken.ToString() == APPROVE_SUCCEED_VALUE)
                {
                    //Debug.Log("승인성공");
                    OnResponse?.Invoke(true, null, false);
                }
                else
                {
                    JToken approveFailToken = root.GetValue(APPROVE_FAIL_KEY);
                    //Debug.Log("승인실패 : " + approveFailToken.ToString());
                    OnResponse?.Invoke(false, approveFailToken.ToString(), false);
                }
            }
            else
            {
                JToken failToken = root.GetValue(MSG_KEY);
                //Debug.Log("결제실패 : " + failToken.ToString());
                OnResponse?.Invoke(false, failToken.ToString(), false);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            OnResponse?.Invoke(false, "통신 실패", true);
            return;
        }

        _response = string.Empty;
    }
}
