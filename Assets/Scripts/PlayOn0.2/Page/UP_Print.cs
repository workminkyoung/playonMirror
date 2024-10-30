using FFmpegOut;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;
using ZXing.QrCode;
using ZXing;
using sdimage = System.Drawing.Imaging;
using Image = UnityEngine.UI.Image;
using UnityEngine.Video;
using DG.Tweening;

public class UP_Print : UP_BasePage
{
    [SerializeField]
    private RenderTexture _videoRT;
    [SerializeField]
    private RawImage _rawimageDefault;
    [SerializeField]
    private RawImage _rawimagePromotion;
    [SerializeField]
    private RawImage _rawImageQR;
    [SerializeField]
    private GameObject _defaultObj;
    [SerializeField] 
    private GameObject _promotionObj;
    [SerializeField]
    private Image _progressBar;
    [SerializeField]
    private UC_SynchFrame _synchFrame;
    [SerializeField]
    private VideoPlayer _promotionPlayer;
    [SerializeField]
    private float _speed = 10;

    private bool _isPromotion = false;
    private LOAD_TYPE _loadType;
    private RenderTexture _promotionRT = null;

    public override void InitPage()
    {
        _synchFrame.mainController = _pageController as PC_Main;
        _promotionRT = new RenderTexture(
            (int)_rawimagePromotion.rectTransform.sizeDelta.x, (int)_rawimagePromotion.rectTransform.sizeDelta.y, 16);
        _promotionRT.Create();
        _promotionPlayer.targetTexture = _promotionRT;
    }

    public override void BindDelegates()
    {
    }

    private void FrameUpdate()
    {
        _synchFrame.OnEndSaveImage = null;
        _synchFrame.OnEndSaveImage = LoadVideo;
        _synchFrame.SetPrintImageActive(false);
        _synchFrame.UpdateFrame();
    }

    private void OpenDefaultLoading()
    {
        _defaultObj.SetActive(true);
        _promotionObj.SetActive(false);

        FrameUpdate();
    }

    private void OpenEventVideoLoading()
    {
        _defaultObj.SetActive(false);
        _promotionObj.SetActive(true);

        _promotionPlayer.source = VideoSource.Url;
        _promotionPlayer.url = AdminManager.Instance.BasicSetting.Config.PromotionVideo_path;
        _promotionPlayer.Play();
        _rawimagePromotion.texture = _promotionRT;
        FrameUpdate();
    }

    private void OpenEventImageLoading()
    {
        _defaultObj.SetActive(false);
        _promotionObj.SetActive(true);

        _rawimagePromotion.texture = AdminManager.Instance.BasicSetting.Config.PromotionImage_data;
        FrameUpdate();
    }

    public void LoadPrintResultImage()
    {
        byte[] bytes = File.ReadAllBytes(TextData.printPath);
        Texture2D texture = new Texture2D(0, 0);
        texture.LoadImage(bytes);

        _rawimageDefault.texture = texture;
    }

    private void LoadVideo()
    {
        if(_loadType == LOAD_TYPE.DEFAULT)
        {
            _rawimageDefault.texture = _videoRT;
        }
        _synchFrame.PlayVideo();
        StartCoroutine(WaitVideoLoad());
    }

    IEnumerator WaitVideoLoad()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        ffmpegManager.Instance.OnRecording(TextData.vidpath, UserDataManager.Instance.curShootTime - 1, () =>
        {
            //StartCoroutine(WaitAfterPrinting());
            _synchFrame.OnEndSaveImage = null;
            _synchFrame.OnEndSaveImage = () => StartCoroutine(WaitAfterPrinting());
            PhotoDataManager.inst.SetVideoPath(TextData.vidpath);
            UploadImage();
        });
    }

    IEnumerator WaitAfterPrinting()
    {
        Print();
        LogDataManager.inst.SendLog();

        yield return new WaitForSeconds(AdminManager.Instance.BasicSetting.Config.Printing_data);

        (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_END);
        _synchFrame.ResetPage();
    }

    public void Print()
    {
        int copy = UserDataManager.inst.curPicAmount;
        bool cut = false;
        string argument = "";

        argument += " --copies " + copy;
        argument += " --file " + TextData.printPath;
        if (UserDataManager.inst.selectedFrameType == FRAME_TYPE.FRAME_8)
            argument += " --cut";

#if UNITY_EDITOR
#else
        Process.Start(Directory.GetParent(Application.dataPath) + "/printer.exe", argument);
#endif
    }

    void UploadImage()
    {
        List<byte[]> datas = new List<byte[]>();

        datas.Add(File.ReadAllBytes(TextData.imagePath));
        datas.Add(File.ReadAllBytes(TextData.vidpath));

        StorageManager.Instance.StartUploadCloud(datas, (path) =>
        {
            MakeQrCode(path);
        });
    }

    public void MakeQrCode(string link)
    {
        _rawImageQR.texture = null;
        _rawImageQR.enabled = true;
        ZXing.BarcodeWriter barcode = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                NoPadding = true,
                Margin = 1,
                Height = 512,
                Width = 512
            }
        };

        Bitmap bitmap = barcode.Write(link);
        Texture2D qrcode = new Texture2D(512, 512);
        qrcode = Convert(bitmap);
        qrcode.Apply();

        _rawImageQR.texture = qrcode;
        if (UserDataManager.Instance.IsQRPrint)
        {
            _synchFrame.SetPrintImage(qrcode);
        }
        else
        {
            _synchFrame.SetPrintImage(null);
        }
    }

    public Texture2D Convert(Bitmap bitmap)
    {
        Texture2D texture = new Texture2D(bitmap.Width, bitmap.Height, TextureFormat.RGBA32, false);

        sdimage.BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                             sdimage.ImageLockMode.ReadOnly, sdimage.PixelFormat.Format32bppArgb);

        int bytes = System.Math.Abs(bmpData.Stride) * bitmap.Height;
        byte[] rgbaValues = new byte[bytes];
        System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, rgbaValues, 0, bytes);

        bitmap.UnlockBits(bmpData);

        texture.LoadRawTextureData(rgbaValues);
        texture.Apply();

        return texture;
    }

    public override void OnPageEnable()
    {
        if (!_isContentCreated)
        {
            CreateContent();
        }
        _rawImageQR.enabled = false;

        if (!_isPromotion)
        {
            OpenDefaultLoading();
        }
        else if (_loadType == LOAD_TYPE.EVENT_VIDEO)
        {
            OpenEventVideoLoading();
        }
        else
        {
            OpenEventImageLoading();
        }

        StartCoroutine(RotateProgress());
    }

    private void CreateContent()
    {
        _isPromotion = bool.Parse(AdminManager.Instance.BasicSetting.Config.PlayShotMovie.ToLower());

        if (!string.IsNullOrEmpty(AdminManager.Instance.BasicSetting.Config.PromotionVideo_path))
        {
            _loadType = LOAD_TYPE.EVENT_VIDEO;
        }
        else if(AdminManager.Instance.BasicSetting.Config.PromotionImage_data != null)
        {
            _loadType = LOAD_TYPE.EVENT_IMAGE;
        }
        else
        {
            _loadType = LOAD_TYPE.DEFAULT;
        }

        _isContentCreated = true;
    }

    private IEnumerator RotateProgress()
    {
        while (gameObject.activeSelf)
        {
            _progressBar.rectTransform.Rotate(0f, 0f, _speed * Time.deltaTime);
            yield return null;
        }
    }

    public override void OnPageDisable()
    {
    }

    protected override void OnPageReset()
    {
    }

    enum LOAD_TYPE
    {
        EVENT_VIDEO = 0,
        EVENT_IMAGE,
        DEFAULT
    }
}
