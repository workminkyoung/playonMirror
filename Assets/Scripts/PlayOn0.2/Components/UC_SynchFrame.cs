using FFmpegOut;
using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Vivestudios.UI;
using System.Linq;

public class UC_SynchFrame : MonoBehaviour //SingletonBehaviour<UC_SynchFrame>
{
    public Action OnEndSaveImage;
    public Action OnEndSaveVideo;

    [SerializeField]
    private FrameAreaDicBase _frameAreaDic;
    [SerializeField]
    private FrameAreaDicBase _frameAreaDic_video;

    [SerializeField]
    private Camera _captureCam;
    [SerializeField]
    private Camera _captureCam_video;
    [SerializeField]
    private RectTransform _canvas;
    [SerializeField]
    private List<VideoPlayer> _videoPlayers = new List<VideoPlayer>();
    [SerializeField]
    private RenderTexture _videoRT;
    //[SerializeField]
    //private RawImage _RawPhoto;// = new List<RawImage>();
    [SerializeField]
    private List<RawImage> _RawQRs = new List<RawImage>();
    [SerializeField]
    private Texture2D _saveTexture;
    [SerializeField]
    private GameObject photoCaptureArea, videoCaptureArea;
    [SerializeField]
    private GameObject photoStickerContainer, videoStickerContainer;

    private PC_Main _mainController;

    public PC_Main mainController { set { _mainController = value; } }


    private void Awake()
    {
        _mainController.StickerUpdateAction += UpdateSticker;
    }

    private void UpdateSticker()
    {
        if(photoStickerContainer != null)
        {
            Destroy(photoStickerContainer);
        }

        if(videoStickerContainer != null)
        {
            Destroy(videoStickerContainer);
        }

        photoStickerContainer = GameObject.Instantiate(_mainController.stickerContainerPrefab, photoCaptureArea.transform);
        photoStickerContainer.transform.localScale = new Vector3(4, 4, 1);
        videoStickerContainer = GameObject.Instantiate(_mainController.stickerContainerPrefab, videoCaptureArea.transform);
        videoStickerContainer.transform.localScale = new Vector3(2, 2, 1);

        foreach(var elem in photoStickerContainer.GetComponentsInChildren<UC_StickerController>())
        {
            elem.HideController();
            Destroy(elem);
        }

        foreach(var elem in videoStickerContainer.GetComponentsInChildren<UC_StickerController>())
        {
            elem.HideController();
            Destroy(elem);
        }
    }

    public void ResetPage()
    {
        foreach (var elem in _frameAreaDic.Values)
        {
            elem.SetPics(new List<Texture2D>());
            elem.SetRenderTexture(new List<RenderTexture>());
            elem.SetFrameColor(UserDataManager.Instance.defaultFrameColor);//FRAME_COLOR_TYPE.FRAME_WHITE);
            elem.SetLutEffect(string.Empty);
            elem.SetFilterOn(false);
            //elem.UpdateFrame();
        }
        foreach (var elem in _frameAreaDic_video.Values)
        {
            elem.SetPics(new List<Texture2D>());
            elem.SetRenderTexture(new List<RenderTexture>());
            elem.SetFrameColor(UserDataManager.Instance.defaultFrameColor);//FRAME_COLOR_TYPE.FRAME_WHITE);
            elem.SetLutEffect(string.Empty);
            elem.SetFilterOn(false);
            //elem.UpdateFrame();
        }

        for (int i = 0; i < _videoPlayers.Count; i++)
        {
            _videoPlayers[i].source = VideoSource.Url;
            _videoPlayers[i].url = string.Empty;
            _videoPlayers[i].targetTexture = null;
        }

        for (int i = 0; i < _RawQRs.Count; i++)
        {
            _RawQRs[i].gameObject.SetActive(false);
        }

        if(photoStickerContainer != null)
        {
            Destroy(photoStickerContainer);
        }

        if(videoStickerContainer != null)
        {
            Destroy(videoStickerContainer);
        }
    }

    public void UpdateFrame()
    {
        foreach (var elem in _frameAreaDic.Keys)
        {
            _frameAreaDic[elem].gameObject.SetActive(UserDataManager.inst.selectedFrameType == elem);
            _frameAreaDic_video[elem].gameObject.SetActive(UserDataManager.inst.selectedFrameType == elem);
        }

        List<Texture2D> texs = new List<Texture2D>();

        for (int i = 0; i < PhotoDataManager.inst.selectedPicDic.Count; i++)
        {
            if (PhotoDataManager.inst.selectedPicDic[i] == null)
            {
                texs.Add(null);
            }
            else
            {
                texs.Add(PhotoDataManager.inst.selectedPicDic[i].thumbnailTex);
            }
        }

        List<PHOTO_TYPE> types = new List<PHOTO_TYPE>();

        int index = 0;
        for (int i = 0; i < texs.Count; i++)
        {
            if (texs[i] == null)
            {
                types.Add(PHOTO_TYPE.NONE);
                continue;
            }
            types.Add(PhotoDataManager.inst.selectedPhoto[PhotoDataManager.inst.selectedPhoto.Keys.ToList()[index]]);
            index++;
        }

        _frameAreaDic[UserDataManager.inst.selectedFrameType].SetSkinFilterOn(_mainController.isSkinFilterOn);
        _frameAreaDic[UserDataManager.inst.selectedFrameType].SetRatioType(UserDataManager.inst.frameRatioType);
        _frameAreaDic[UserDataManager.inst.selectedFrameType].SetFrameColor(UserDataManager.inst.selectedFrameColor);
        _frameAreaDic[UserDataManager.inst.selectedFrameType].SetPics(texs, types);
        _frameAreaDic[UserDataManager.inst.selectedFrameType].SetLutEffect(UserDataManager.inst.selectedLutKey);
        _frameAreaDic[UserDataManager.inst.selectedFrameType].UpdateFrame();

        _frameAreaDic_video[UserDataManager.inst.selectedFrameType].SetSkinFilterOn(_mainController.isSkinFilterOn);
        _frameAreaDic_video[UserDataManager.inst.selectedFrameType].SetRatioType(UserDataManager.inst.frameRatioType);
        _frameAreaDic_video[UserDataManager.inst.selectedFrameType].SetFrameColor(UserDataManager.inst.selectedFrameColor);
        _frameAreaDic_video[UserDataManager.inst.selectedFrameType].SetPics(texs, types);
        _frameAreaDic_video[UserDataManager.inst.selectedFrameType].UpdateFrame();

        ScreenShoot();
    }

    public void ScreenShoot(bool isSavePrint = false)
    {
        StartCoroutine(Shoot(isSavePrint));
    }

    // 프린트 이미지 저장하기
    IEnumerator Shoot(bool isSavePrint = false)
    {
        yield return new WaitForEndOfFrame();

        Texture2D screenShoot = null;

        RenderTexture renderTexture = new RenderTexture((int)_canvas.sizeDelta.x, (int)_canvas.sizeDelta.y, 24);
        _captureCam.targetTexture = renderTexture;
        _captureCam.Render();

        RenderTexture.active = renderTexture;
        screenShoot = new Texture2D(renderTexture.width, renderTexture.height);
        screenShoot.ReadPixels(new Rect(0, 0, screenShoot.width, screenShoot.height), 0, 0);
        screenShoot.Apply();
        _saveTexture = screenShoot;

        _captureCam.targetTexture = null;

        byte[] bytes = screenShoot.EncodeToJPG();
        if (isSavePrint)
        {
            System.IO.File.WriteAllBytes(TextData.printPath, bytes);
            CustomLogger.Log("Saved to " + TextData.printPath);
        }
        else
        {
            System.IO.File.WriteAllBytes(TextData.imagePath, bytes);
            CustomLogger.Log("Saved to " + TextData.imagePath);
            PhotoDataManager.inst.SetImagePath(TextData.imagePath);
        }

        OnEndSaveImage?.Invoke();
    }

    public void PlayVideo()
    {
        //record renderTexture and camera setting
        _captureCam_video.targetTexture = _videoRT;
        ffmpegManager.Instance.Setting(_captureCam_video);
        List<RenderTexture> setPics = new List<RenderTexture>();
        FRAME_RATIO_TYPE ratioType = FRAME_RATIO_TYPE.HORIZONTAL;

        if (UserDataManager.inst.selectedContent == CONTENT_TYPE.AI_PROFILE ||
            UserDataManager.inst.selectedContent == CONTENT_TYPE.WHAT_IF)
        {
            foreach (var elem in _frameAreaDic_video.Keys)
            {
                _frameAreaDic_video[elem].gameObject.SetActive(FRAME_TYPE.FRAME_1 == elem);
            }

            //get activated video path

            _videoPlayers[0].url = PhotoDataManager.inst.recordPaths[UserDataManager.inst.selectedProfilePicNum];

            RenderTexture renderTexture;
            renderTexture = new RenderTexture(PlayOnProperties.crop3x4_width, PlayOnProperties.crop3x4_height, 24);
            ratioType = FRAME_RATIO_TYPE.VERTICAL;

            _videoPlayers[0].targetTexture = renderTexture;
            setPics.Add(renderTexture);

            _frameAreaDic_video[FRAME_TYPE.FRAME_1].SetPics(new List<Texture2D>());
            _frameAreaDic_video[FRAME_TYPE.FRAME_1].SetRatioType(ratioType);
            _frameAreaDic_video[FRAME_TYPE.FRAME_1].SetRenderTexture(setPics);
            _frameAreaDic_video[FRAME_TYPE.FRAME_1].SetFrameColor(UserDataManager.inst.selectedFrameColor);
            _frameAreaDic_video[FRAME_TYPE.FRAME_1].UpdateFrame();
        }
        else
        {
            //get activated video path
            for (int i = 0; i < _mainController.resultIndexs.Count; i++)
            {
                _videoPlayers[i].url = PhotoDataManager.inst.recordPaths[_mainController.resultIndexs[i]];

                RenderTexture renderTexture;
                if (PhotoDataManager.inst.isLandscape)
                {
                    renderTexture = new RenderTexture(PlayOnProperties.crop4x3_width, PlayOnProperties.crop4x3_height, 24);
                    ratioType = FRAME_RATIO_TYPE.HORIZONTAL;
                }
                else
                {
                    renderTexture = new RenderTexture(PlayOnProperties.crop3x4_width, PlayOnProperties.crop3x4_height, 24);
                    ratioType = FRAME_RATIO_TYPE.VERTICAL;
                }

                _videoPlayers[i].targetTexture = renderTexture;
                setPics.Add(renderTexture);
            }

            _frameAreaDic_video[UserDataManager.inst.selectedFrameType].SetPics(new List<Texture2D>());
            _frameAreaDic_video[UserDataManager.inst.selectedFrameType].SetRatioType(ratioType);
            _frameAreaDic_video[UserDataManager.inst.selectedFrameType].SetRenderTexture(setPics);
            _frameAreaDic_video[UserDataManager.inst.selectedFrameType].SetFrameColor(UserDataManager.inst.selectedFrameColor);
            _frameAreaDic_video[UserDataManager.inst.selectedFrameType].UpdatePhotos();
        }

        CustomLogger.Log("Loaded");
    }

    public void SetPrintImage(Texture2D qrImage)
    {
        //_RawPhoto.gameObject.SetActive(true);
        //_RawPhoto.texture = _saveTexture;

        FrameData.DefinitionEntry entry = UserDataManager.Instance.selectedFrameDefinition;

        if(qrImage != null)
        {
            for (int i = 0; i < entry.qrRects.Count; i++)
            {
                _RawQRs[i].gameObject.SetActive(true);
                _RawQRs[i].texture = qrImage;

                _RawQRs[i].rectTransform.pivot = entry.qrRects[i].pivot;
                _RawQRs[i].rectTransform.anchorMin = entry.qrRects[i].anchorMin;
                _RawQRs[i].rectTransform.anchorMax = entry.qrRects[i].anchorMax;
                _RawQRs[i].rectTransform.sizeDelta = entry.qrRects[i].sizeDelta * 4;
                _RawQRs[i].rectTransform.anchoredPosition = entry.qrRects[i].anchoredPosition * 4;
            }
        }
        else
        {
            for (int i = 0; i < entry.qrRects.Count; i++)
            {
                _RawQRs[i].gameObject.SetActive(false);
            }
        }

        OnEndSaveImage += () =>
        {
            //_captureCam.targetTexture = _videoRT;
            SetPrintImageActive(false);
        };
        ScreenShoot(true);
    }

    public void SetPrintImageActive(bool state)
    {
        for (int i = 0; i < _RawQRs.Count; i++)
        {
            _RawQRs[i].gameObject.SetActive(state);
        }
    }

    [Serializable]
    private class FrameAreaDicBase : SerializableDictionaryBase<FRAME_TYPE, UC_FrameArea> { };
}
