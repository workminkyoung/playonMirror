using FFmpegOut;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UC_ShootState : MonoBehaviour
{
    private UP_BasePage _parentPage;

    [SerializeField]
    private int _duration;
    private int _photoMax;
    private int _photoCurrent;
    private float _time;

    //[SerializeField]
    //private TextMeshProUGUI _textTime, _textPhotoCount;
    [SerializeField]
    List<TextMeshProUGUI> _texts = new List<TextMeshProUGUI>();
    [SerializeField]
    private Image _timerImg;
    public Action _shoot, _endShoot;
    public int photoCurrent => _photoCurrent;
    PC_Main _main;

    Coroutine _timeCoroutine = null;
    Coroutine _recordCoroutine = null;

    public int duration { set { _duration = value; } }

    public PC_Main main { set { _main = value; } }

    public int photoMax { set { _photoMax = value; } }

    public void SetParentPage(UP_BasePage page)
    {
        _parentPage = page;
    }

    public void Setting()
    {
        _texts.AddRange(GetComponentsInChildren<TextMeshProUGUI>());

        //TODO : set value by config data
        //_duration = 10;
        //_photoMax = 4;
    }

    public void ResetShootAction(Action shooting, Action endShoot)
    {
        _shoot = null;
        _endShoot = null;
        _shoot = shooting;
        _endShoot = endShoot;
    }

    // 시작세팅
    public void StartCheckTime()
    {
        //work when page shows
        _photoCurrent = 0;
        _time = 0;

        _texts[(int)eText.Count].text = _photoCurrent.ToString() + '/' + _photoMax;
        _texts[(int)eText.Time].text = _duration.ToString();
    }

    // 촬영수 count up하고 max에 도달했는지 확인
    public bool PhotoCountUp()
    {
        _photoCurrent++;
        _texts[(int)eText.Count].text = _photoCurrent.ToString() + '/' + _photoMax;

        if (_photoCurrent == _photoMax)
        {
            //end of Shooting
            return true;
        }
        else
        {
            return false;
        }
    }

    public void StartCountDown()
    {
        if (!GameManager.inst.isDiffusionSuccess)
        {
            CustomLogger.Log("Diffusion is already Failed");
            return;
        }

        string name = Path.Combine(TextData.storageFolderPath, TextData.recordName + _photoCurrent.ToString() + ".mp4");
        PhotoDataManager.inst.AddRecordPath(name);

        if(_recordCoroutine != null)
        {
            StopCoroutine( _recordCoroutine );
        }
        _recordCoroutine = StartCoroutine(RecordDelay(name));

        if (_timeCoroutine != null)
            StopCoroutine(_timeCoroutine);
        _timeCoroutine = StartCoroutine(CountDown());
    }
    IEnumerator RecordDelay(string name)
    {
        yield return new WaitForSecondsRealtime(1);
        ffmpegManager.Instance.OnRecording(name);
    }

    public void StopCountDown()
    {
        if (_timeCoroutine != null)
            StopCoroutine(_timeCoroutine);
        (_parentPage.pageController as PC_Main).globalPage.CloseToast();
        ffmpegManager.Instance.StopRecording();
    }

    private void OnDisable()
    {
        (_parentPage.pageController as PC_Main).globalPage.CloseToast();
    }

    // 한장의 시간에 대한 카운트다운
    IEnumerator CountDown()
    {
        _time = 0;
        _texts[(int)eText.Time].text = _duration.ToString();
        _texts[(int)eText.Time].color = Color.black;
        if (_timerImg)
        {
            _timerImg.color = Color.black;
        }

        bool alert = false;
        float sec = 0;
        while (_time < _duration)
        {
            sec += Time.deltaTime;
            if (sec >= 1.0f)
            {
                _time++;
                _texts[(int)eText.Time].text = (_duration - (int)_time).ToString();

                if (!alert)
                {
                    if (_duration - (int)_time <= ConfigData.config.shootWarningTime)
                    {
                        SoundManager.Instance.Play(AUDIO.COUNT);
                        (_parentPage.pageController as PC_Main).globalPage.OpenToast("잠시 후 촬영이 시작됩니다.");

                        _texts[(int)eText.Time].color = Color.red;
                        if (_timerImg)
                        {
                            _timerImg.color = Color.red;
                        }

                        alert = true;
                    }
                }
                else
                {
                    if (_duration - (int)_time > 0)
                        SoundManager.Instance.Play(AUDIO.COUNT);
                }
                sec = 0;
            }


            yield return null;
        }

        (_parentPage.pageController as PC_Main).globalPage.CloseToast();
        ffmpegManager.Instance.StopRecording();
        _shoot?.Invoke();
    }

    enum eText
    {
        Time,
        Count
    }

}
