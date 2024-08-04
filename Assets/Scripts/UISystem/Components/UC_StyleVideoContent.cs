using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using static Unity.Barracuda.TextureAsTensorData;

public class UC_StyleVideoContent : UC_StyleContent
{
    [SerializeField]
    private RawImage _videoImage;
    [SerializeField]
    private VideoPlayer _videoPlayer;

    public override void InitComponent()
    {
        base.InitComponent();
    }

    public void SetVideo(VideoClip clip)
    {
        if (clip == null)
        {
            CustomLogger.Log("No Thumbnail Video Clip");
            return;
        }

        _videoPlayer.source = VideoSource.VideoClip;
        _videoPlayer.clip = clip;
        _videoPlayer.isLooping = true;
        _videoPlayer.playOnAwake = true;

        _videoImage.gameObject.SetActive(true);
        _thumbnailImg.gameObject.SetActive(false);

        RenderTexture videoRT = new RenderTexture((int)clip.width, (int)clip.height, 24);
        _videoImage.texture = videoRT;
        _videoPlayer.targetTexture = videoRT;
    }

    public void SetVideo(string path)
    {
        if (path == null || path.Length < 1)
        {
            CustomLogger.Log("No Thumbnail Video Path");
            return;
        }

        _videoPlayer.source = VideoSource.Url;
        _videoPlayer.url = path;
        _videoPlayer.isLooping = true;
        _videoPlayer.playOnAwake = true;

        _videoImage.gameObject.SetActive(true);
        _thumbnailImg.gameObject.SetActive(false);

        _videoPlayer.prepareCompleted += (player) =>
        {
            RenderTexture videoRT = new RenderTexture((int)player.width, (int)player.height, 24);
            _videoImage.texture = videoRT;
            _videoPlayer.targetTexture = videoRT;
        };
        _videoPlayer.Prepare();
    }

    public override void SetThumbnail(Sprite thumbnail)
    {
        if(_thumbnailImg == null)
        {
            CustomLogger.Log("No Thumbnail Image");
            return;
        }
        base.SetThumbnail(thumbnail);

        _videoImage?.gameObject.SetActive(false);
        _thumbnailImg.gameObject.SetActive(true);
    }

    public void SetThumbnail(Texture2D thumbnail)
    {
        if (thumbnail == null)
        {
            CustomLogger.Log("No Thumbnail Image");
            return;
        }

        Rect rect = new Rect(0, 0, thumbnail.width, thumbnail.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        Sprite sprite = Sprite.Create(thumbnail, rect, pivot);

        _thumbnailImg.sprite = sprite;
        _videoImage?.gameObject.SetActive(false);
        _thumbnailImg.gameObject.SetActive(true);
    }
}
