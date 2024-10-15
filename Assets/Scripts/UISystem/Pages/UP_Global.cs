using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using Vivestudios.UI;

public class UP_Global : UP_BasePage
{
    [SerializeField]
    private RectTransform _dimImg;
    [SerializeField]
    private UC_Toast _toast;
    [SerializeField]
    private UC_ConfirmPopup _confirmPopup;
    [SerializeField]
    private UC_ConfirmPopup _confirmPopupWide;
    [SerializeField]
    private UC_ConfirmPopup _privacyPopup;
    [SerializeField]
    private UC_ConfirmPopup _alertPopup;
    [SerializeField]
    private UC_PolicyPopup _PolicyPopup;
    [SerializeField]
    private UC_ResetPhotoPaperPopup _resetPhotoPaperPopup;
    [SerializeField]
    private UC_AIProfileAlertPopup _aiAlertPopup;
    [SerializeField]
    private RectTransform _paymentOnText;
    [SerializeField]
    private TextMeshProUGUI _cameraOnText;
    [SerializeField]
    private RectTransform _emptyPhotoPaperAlert;
    [SerializeField]
    private RectTransform _serviceErrorPage;
    [SerializeField]
    private RectTransform _serviceTempErrorPage;
    [SerializeField]
    private RectTransform _versionTextArea;
    [SerializeField]
    private TextMeshProUGUI _versionText;
    [SerializeField]
    private UC_ChromaKeySetting _chromaKeySetting;
    [SerializeField]
    private UC_DownloadLoading _downloadLoading;
    [SerializeField]
    private UC_Keyboard _keyboard;

    private Coroutine _timerToastCoroutine = null;
    private Coroutine _showToastCoroutine = null;

    public Action ErrorOpenAction;
    public Action ErrorClossAction;

    public Action TempErrorOpenAction;
    public Action TempErrorClossAction;


    public bool isToastOn { get { return _toast.isOn; } }

    private const string TOAST_MSG = "{0}초 뒤 첫 화면으로 이동합니다. 화면을 터치해주세요.";

    public override void InitPage()
    {
        GameManager.inst.SetGlobalPage(this);
        _confirmPopup.gameObject.SetActive(false);
        _confirmPopupWide.gameObject.SetActive(false);
        _dimImg.gameObject.SetActive(false);
        _alertPopup.gameObject.SetActive(false);
        _serviceErrorPage.gameObject.SetActive(false);
        _PolicyPopup.gameObject.SetActive(false);
        _resetPhotoPaperPopup.gameObject.SetActive(false);
        _versionTextArea.gameObject.SetActive(false);
        _aiAlertPopup.gameObject.SetActive(false);
        _chromaKeySetting.gameObject.SetActive(false);
        _keyboard.gameObject.SetActive(false);
    }

    public override void BindDelegates()
    {
        //_confirmPopup.OnConfirmAction += CloseConfirmPopup;
        _PolicyPopup.OnConfirmAction += ClosePolicyPopup;

        _resetPhotoPaperPopup.OnConfirmAction += () =>
        {
            ResetPhotopaperPopupOn(false);
            PhotoPaperCheckModule.SetRemainPhotoPaper(_resetPhotoPaperPopup.selectedPhotopaperNum);
            OpenAlertPopup("인화지 수량 초기화", "인화지 수량이 초기화 되었습니다.\n수량에 따라 알림이 발송됩니다.", () => EmptyPhotoPaperAlertOn(false));
            MailingModule.inst.SendMail(MAIL_TYPE.RESET_PAPER);
        };
        _resetPhotoPaperPopup.OnCancelAction += () =>
        {
            ResetPhotopaperPopupOn(false);
        };
    }

    public void OpenConfirmPopup(string title, string description, Sprite sprite, bool isWide = false)
    {
        UC_ConfirmPopup popup = _confirmPopup;
        if (isWide)
        {
            popup = _confirmPopupWide;
        }
        popup.gameObject.SetActive(true);
        popup.SetTitle(title);
        popup.SetDescription(description);
        popup.SetImage(sprite);
        popup.OpenPopup(true);
    }

    public void OpenPolicyPopup(POLICY_TYPE type)
    {
        _PolicyPopup.SetContent(type);
        _dimImg.gameObject.SetActive(true);
        _PolicyPopup.gameObject.SetActive(true);
    }

    public void ClosePolicyPopup()
    {
        _PolicyPopup.gameObject.SetActive(false);
        _dimImg.gameObject.SetActive(false);
    }

    public void OpenPrivacyPopup()
    {
        _privacyPopup.gameObject.SetActive(true);
        _privacyPopup.OpenPopup(true);
    }

    public void ClosePrivacyPopup()
    {
        _privacyPopup.gameObject.SetActive(false);
        _privacyPopup.OpenPopup(false);
    }

    public void CloseConfirmPopup(bool isWide = false)
    {
        if (isWide)
        {
            _confirmPopupWide.OpenPopup(false);
        }
        else
        {
            _confirmPopup.OpenPopup(false);
        }
    }

    public void OpenTimerToast(int num)
    {
        if (_timerToastCoroutine != null)
            StopCoroutine(_timerToastCoroutine);
        _timerToastCoroutine = StartCoroutine(TimerToastRoutine(num));
    }

    public void CloseToast()
    {
        if (_timerToastCoroutine != null)
            StopCoroutine(_timerToastCoroutine);
        if (_showToastCoroutine != null)
            StopCoroutine(_showToastCoroutine);
        _toast.OpenToast(false);
        _toast.SetText("");
    }

    public void OpenAlertPopup(string title, string description, Action onConfirm = null)
    {
        _alertPopup.SetTitle(title);
        _alertPopup.SetDescription(description);
        _alertPopup.gameObject.SetActive(true);
        _alertPopup.OpenPopup(true, onConfirm);
    }

    public void CloseAlertPopup()
    {
        _alertPopup.OpenPopup(false);
    }

    public void OpenDim(bool isEnable)
    {
        _dimImg.gameObject.SetActive(isEnable);
    }

    public void OpenToast(string message)
    {
        _toast.SetText(message);
        _toast.OpenToast(true);
    }

    public void OpenToast(string message, int time)
    {
        if (_showToastCoroutine != null)
            StopCoroutine(_showToastCoroutine);

        _toast.SetText(message);
        _toast.OpenToast(true);
        _showToastCoroutine = StartCoroutine(ShowToastRoutine(time));
    }

    public void PaymentTextOn(bool isOn)
    {
        _paymentOnText.gameObject.SetActive(isOn);
    }

    public void CameraTextOn(bool isOn, bool isCamOn)
    {
        _cameraOnText.text = isCamOn ? "Y" : "N";
        _cameraOnText.gameObject.SetActive(isOn);
    }

    public void EmptyPhotoPaperAlertOn(bool isOn)
    {
        _emptyPhotoPaperAlert.gameObject.SetActive(isOn);
    }

    public void ResetPhotopaperPopupOn(bool isOn)
    {
        _dimImg.gameObject.SetActive(isOn);
        _resetPhotoPaperPopup.gameObject.SetActive(isOn);
    }

    public void OpenServiceErrorPage(bool isOn)
    {
        if (_serviceErrorPage.gameObject.activeInHierarchy == isOn)
        {
            return;
        }

        _serviceErrorPage.gameObject.SetActive(isOn);

        if (isOn)
            ErrorOpenAction?.Invoke();
        else
            ErrorClossAction?.Invoke();
    }

    public void OpenServiceErrorPageTemp(bool isOn)
    {
        if (_serviceTempErrorPage.gameObject.activeInHierarchy == isOn)
        {
            return;
        }

        _serviceTempErrorPage.gameObject.SetActive(isOn);

        if (isOn)
            TempErrorOpenAction?.Invoke();
        else
            TempErrorClossAction?.Invoke();
    }

    public void OpenAIProfileAlert(Sprite image, Action OnAlertClosed = null)
    {
        if(OnAlertClosed != null)
        {
            _aiAlertPopup.SetBGImage(image);
            _aiAlertPopup.OnAlertClosed = OnAlertClosed;
            _aiAlertPopup.gameObject.SetActive(true);
        }
    }

    public void OpenChromaKeySetting()
    {
        _chromaKeySetting.gameObject.SetActive(true);
    }

    public void OpenKeyboard()
    {
        _keyboard.gameObject.SetActive(true);
    }
    public void CloseChromaKeySetting()
    {
        _chromaKeySetting.gameObject.SetActive(false);
    }

    public void OpenDownloadLoading()
    {
        _downloadLoading.SetActivate(true);
    }

    public void CloseDownloadLoading()
    {
        _downloadLoading.SetActivate(false);
        GameManager.OnGoogleDownloadEnd?.Invoke();
    }

    private void Update()
    {
        if (!GameManager.inst.isAdminDownloadSuccess)
        {
            return;
        }

        if (GameManager.inst.isCameraConnected &&
            GameManager.inst.isPaymentReaderConnected && 
            GameManager.inst.isInternetReachable &&
            GameManager.inst.isDiffusionSuccess &&
            GameManager.inst.isQRUploadSuccess)
        {
            OpenServiceErrorPage(false);
        }
        else
        {
            OpenServiceErrorPage(true);
        }

        if (GameManager.inst.isCameraTempConnected)
        {
            OpenServiceErrorPageTemp(false);
        }
        else
        {
            OpenServiceErrorPageTemp(true);
        }
    }

    public void ToggleVersionText()
    {
        if (_versionTextArea.gameObject.activeInHierarchy)
        {
            _versionTextArea.gameObject.SetActive(false);
        }
        else
        {
            _versionText.text = "PLAY ON Ver " + Application.version;
            _versionTextArea.gameObject.SetActive(true);
        }
    }

    private IEnumerator TimerToastRoutine(int num)
    {
        int time = 0;

        while (time < num)
        {
            _toast.SetText(string.Format(TOAST_MSG, num - time));
            _toast.OpenToast(true);
            yield return new WaitForSecondsRealtime(1);
            time++;
        }
        _toast.OpenToast(false);
        _toast.SetText("");
    }

    private IEnumerator ShowToastRoutine(int num)
    {
        int time = 0;

        while (time < num)
        {
            yield return new WaitForSecondsRealtime(1);
            time++;
        }
        _toast.OpenToast(false);
        _toast.SetText("");
    }

    public override void OnPageEnable()
    {
    }

    public override void OnPageDisable()
    {
    }

    protected override void OnPageReset()
    {
    }
}
