using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

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
        _videoPlayer.clip = clip;
        _videoPlayer.isLooping = true;
        _videoPlayer.playOnAwake = true;

        _videoImage.gameObject.SetActive(true);
        _thumbnailImg.gameObject.SetActive(false);

        RenderTexture videoRT = new RenderTexture((int)clip.width, (int)clip.height, 24);
        _videoImage.texture = videoRT;
        _videoPlayer.targetTexture = videoRT;
    }

    public override void SetThumbnail(Sprite thumbnail)
    {
        base.SetThumbnail(thumbnail);

        _videoImage?.gameObject.SetActive(false);
        _thumbnailImg.gameObject.SetActive(true);
    }
}
