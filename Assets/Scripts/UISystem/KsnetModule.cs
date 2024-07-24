using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class KsnetModule : PaymentModule
{
    private const string URL = "http://127.0.0.1:27098/";
    private const string REQUEST_FORMAT = "AP0452%02IC010200N{0}++++000000000000+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++%1C0000{1}000000000000000000000091000000000913000000000000+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++X%03";
    private const string REQUEST_KEY = "REQ=";
    private const string CALL_BACK = "callback=jsonp1234567898123123";

    private const string SUCCESS_CHECK_KEY = "RES";
    private const string MSG_KEY = "MSG";
    private const string SUCCEED_VALUE = "0000";
    private const string APPROVE_CHECK_KEY = "STATUS";
    private const string APPROVE_SUCCEED_VALUE = "O";
    private const string APPROVE_FAIL_KEY = "MESSAGE1";
    private const string APPROVE_FAIL_KEY2 = "MESSAGE2";

    protected override void SetErrorCodePath()
    {
        _errorCodeFileName = "KSNET_ErrorCode.json";
    }

    protected override void SetRequestForm()
    {
        _requestForm = URL + "?" + REQUEST_KEY + string.Format(REQUEST_FORMAT, ConfigData.config.KsnetCatID, _priceForm) + "&" + CALL_BACK;
    }

    protected override void SetRequestMethod()
    {
        _requestMethod = "GET";
    }

    protected override string SetPriceForm(int price)
    {
        return price.ToString("D10");
    }

    protected override void SuccessCheck(Action<bool, string, bool> OnResponse)
    {
        if(string.IsNullOrEmpty(_response))
        {
            return;
        }

        if (_response.Contains("Error: ConnectFailure"))
        {
            //CustomLogger.Log("프로그램 없음");
            OnResponse?.Invoke(false, "프로그램 미실행\n서비스 시작 확인", true);
            return;
        }

        Regex reg = new Regex(@"\{.*\}");
        Match match = reg.Match(_response);

        CustomLogger.Log(match);

        JObject root = new JObject();
        try
        {
            root = JObject.Parse(match.Value);

            JToken token = root.GetValue(SUCCESS_CHECK_KEY);

            if (token.ToString() == SUCCEED_VALUE)
            {
                //CustomLogger.Log("결제성공 -> 승인확인");

                JToken approveToken = root.GetValue(APPROVE_CHECK_KEY);
                if (approveToken.ToString() == APPROVE_SUCCEED_VALUE)
                {
                    //CustomLogger.Log("승인성공");
                    OnResponse?.Invoke(true, null, false);
                }
                else
                {
                    JToken approveFailToken = root.GetValue(APPROVE_FAIL_KEY);
                    JToken approveFailToken2 = root.GetValue(APPROVE_FAIL_KEY2);

                    string failMsg = approveFailToken.ToString().Trim();
                    if (!string.IsNullOrEmpty(approveFailToken2.ToString().Trim()))
                    {
                        failMsg += "\n" + approveFailToken2.ToString().Trim();
                    }

                    //CustomLogger.Log("승인실패 : " + failMsg);
                    OnResponse?.Invoke(false, failMsg, false);
                }
            }
            else
            {
                //Send Response
                JToken failToken = root.GetValue(MSG_KEY);
                //CustomLogger.Log("결제실패 : " + failToken.ToString().Replace("오류", "").Trim());
                OnResponse?.Invoke(false, failToken.ToString().Replace("오류", "").Trim(), false);

                //Send Error Code Mail
                JToken errorToken = root.GetValue(SUCCESS_CHECK_KEY);
                if (_errorCodeDict.ContainsKey(errorToken.ToString()))
                {
                    JObject errorJobject = JObject.Parse(_errorCodeDict[errorToken.ToString()].ToString());
                    JToken errorContent = errorJobject.GetValue("content");
                    _errorContent = errorContent.ToString();
                    MailingModule.inst.SendMail(MAIL_TYPE.PAYMENT_ERROR);
                    //CustomLogger.Log("SendMail : " + errorContent.ToString());
                }
            }
        }
        catch (Exception e)
        {
            CustomLogger.Log(e);
            OnResponse?.Invoke(false, "통신 실패", true);
            return;
        }
    }

    protected override void SetResponseEncoding()
    {
        _responseEncoding = Encoding.GetEncoding("euc-kr");
    }
}
