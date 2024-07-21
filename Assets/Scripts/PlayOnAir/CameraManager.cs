using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : SingletonBehaviour<CameraManager>
{
    [SerializeField]
    WebCamTexture _webCamTexture = null;
    WebCamDevice[] _devices;

    public WebCamTexture webCamTexture
    {
        get { return _webCamTexture; }
    }
    
    public void FindWebcam()
    {
        _devices = WebCamTexture.devices;
        if (_devices.Length <= 0)
        {
            Debug.Log("There is no connected webcam..");
        }
        else
        {
            Debug.Log("webcam is available");

            for (int i = 0; i < _devices.Length; i++)
            {
                Debug.Log(i + ", " + _devices[i].name + " can available");
            }
            if (_webCamTexture == null)
            {
                //for (int i = 0; i < _devices.Length; i++)
                //{
                //    if (_devices[i].name.Contains("Insta") || _devices[i].name.Contains("insta"))
                //    {
                        WebCamDevice device = _devices[6];
                        _webCamTexture = new WebCamTexture(device.name);
                        _webCamTexture.name = "WebcamTexture";
                        _webCamTexture.requestedFPS = 60f;
                        //break;
                //    }
                //}
                //WebCamDevice device = _devices[0];
                //_webCamTexture = new WebCamTexture(device.name);
                //_webCamTexture.requestedFPS = 60f;
            }

            //_webCamTexture.Play();
        }
    }

    public void StopWebcam()
    {
        if (_webCamTexture != null)
            _webCamTexture.Stop();
    }

    public void PlayWebcam()
    {
        if (_webCamTexture != null)
            _webCamTexture.Play();
    }

    public void PauseWebcam()
    {
        if (_webCamTexture != null)
            _webCamTexture.Pause();
    }
    
    public byte[] CaptureImage()
    {
        if (_webCamTexture == null || !_webCamTexture.isPlaying) return null;

        Texture2D capturedImage = new Texture2D(_webCamTexture.width, _webCamTexture.height);
        capturedImage.SetPixels(_webCamTexture.GetPixels());
        capturedImage.Apply();
        
        byte[] imageBytes = capturedImage.EncodeToPNG();
        return imageBytes;
    }

    protected override void Init()
    {
        FindWebcam();
    }
}
