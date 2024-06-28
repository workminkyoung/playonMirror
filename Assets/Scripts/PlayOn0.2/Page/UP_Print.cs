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

public class UP_Print : UP_BasePage
{
    [SerializeField]
    RenderTexture _videoRT;
    RawImage _rawimage;

    [SerializeField]
    UC_SynchFrame _synchFrame;

    public override void InitPage()
    {
        _rawimage = GetComponentInChildren<RawImage>();
        _synchFrame.mainController = _pageController as PC_Main;
    }

    public override void BindDelegates()
    {
    }

    public override void EnablePage(bool isEnable)
    {
        base.EnablePage(isEnable);
        if (isEnable)
        {
            _synchFrame.OnEndSaveImage = null;
            _synchFrame.OnEndSaveImage = LoadVideo;
            _synchFrame.SetPrintImageActive(false);
            _synchFrame.UpdateFrame();
        }
    }

    public void LoadImage()
    {
        byte[] bytes = File.ReadAllBytes(TextData.printPath);
        Texture2D texture = new Texture2D(0, 0);
        texture.LoadImage(bytes);

        _rawimage.texture = texture;
    }

    void LoadVideo()
    {
        _rawimage.texture = _videoRT;
        _synchFrame.PlayVideo();
        StartCoroutine(WaitVideoLoad());
    }

    IEnumerator WaitVideoLoad()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        ffmpegManager.Instance.OnRecording(TextData.vidpath, ConfigData.config.photoTime - 1, () =>
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

        yield return new WaitForSeconds(ConfigData.config.waitPrintTime);

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
        if (UserDataManager.inst.selectedFrame == FRAME_TYPE.FRAME_8)
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

        _synchFrame.SetPrintImage(qrcode);
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
    }

    public override void OnPageDisable()
    {
    }

    protected override void OnPageReset()
    {
    }
}
