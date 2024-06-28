using FFmpegOut;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FFmpegOut
{
    public class ffmpegManager : MonoBehaviour
    {
        #region Public properties

        public static ffmpegManager instance;

        [SerializeField] int _width = 1920;

        public int width
        {
            get { return _width; }
            set { _width = value; }
        }

        [SerializeField] int _height = 1080;

        public int height
        {
            get { return _height; }
            set { _height = value; }
        }

        [SerializeField] FFmpegPreset _preset;

        public FFmpegPreset preset
        {
            get { return _preset; }
            set { _preset = value; }
        }

        [SerializeField] float _frameRate = 60;

        public float frameRate
        {
            get { return _frameRate; }
            set { _frameRate = value; }
        }

        #endregion

        #region Private members

        FFmpegSession _session;
        RenderTexture _tempRT;
        GameObject _blitter;
        bool _recording = false;
        Action _onEndRecord = null;
        Camera _camera;

        RenderTextureFormat GetTargetFormat(Camera camera)
        {
            return camera.allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
        }

        int GetAntiAliasingLevel(Camera camera)
        {
            return camera.allowMSAA ? QualitySettings.antiAliasing : 1;
        }

        #endregion

        #region Time-keeping variables

        int _frameCount;
        float _startTime;
        int _frameDropCount;

        float FrameTime
        {
            get { return _startTime + (_frameCount - 0.5f) / _frameRate; }
        }

        void WarnFrameDrop()
        {
            if (++_frameDropCount != 10) return;

            Debug.LogWarning(
                "Significant frame droppping was detected. This may introduce " +
                "time instability into output video. Decreasing the recording " +
                "frame rate is recommended."
            );
        }

        #endregion

        private void Awake()
        {
            if(instance == null)
            {
                //singleton 확인 필요
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public static ffmpegManager Instance
        {
            get
            {
                if(instance == null)
                {
                    GameObject obj;
                    obj = GameObject.Find(typeof(ffmpegManager).Name);
                    if(obj == null)
                    {
                        obj = new GameObject(typeof(ffmpegManager).Name);
                        instance = obj.AddComponent<ffmpegManager>();
                    }
                    else
                    {
                        instance = obj.GetComponent<ffmpegManager>();
                    }
                }
                return instance;
            }
        }

        public void Setting(Camera camera)
        {
            _camera = camera;
        }

        public void OnRecording(string filename, Action onEndRecord = null)
        {
            _recording = true;
            _onEndRecord = onEndRecord;
            StartCoroutine(SyncFFmpegPipeThread());
            StartCoroutine(RecordRenderTexture(filename));
        }

        public void OnRecording(string filename, float time, Action onEndRecord = null)
        {
            _recording = true;
            _onEndRecord = onEndRecord;

            StartCoroutine(SyncFFmpegPipeThread());
            StartCoroutine(RecordRenderTexture(filename));
            StartCoroutine(CheckRecordTime(time));
        }

        //녹화시 선실행
        IEnumerator SyncFFmpegPipeThread()
        {
            // Sync with FFmpeg pipe thread at the end of every frame.
            if (_recording)
            {
                for (var eof = new WaitForEndOfFrame(); ;)
                {
                    yield return eof;
                    _session?.CompletePushFrames();
                    //Debug.Log("Sync with FFmpeg thread");
                    if (!_recording)
                        break;
                }
            }
        }

        IEnumerator RecordRenderTexture(string name)
        {
            Debug.Log("Start recording...");
            while( _recording)
            {
                //var camera = GetComponent<camera>();

                // Lazy initialization
                if (_session == null)
                {
                    // Give a newly created temporary render texture to the _camera
                    // if it's set to render to a screen. Also create a blitter
                    // object to keep frames presented on the screen.
                    if (_camera.targetTexture == null)
                    {
                        _tempRT = new RenderTexture(_width, _height, 24, GetTargetFormat(_camera));
                        _tempRT.antiAliasing = GetAntiAliasingLevel(_camera);
                        _camera.targetTexture = _tempRT;
                        _blitter = Blitter.CreateInstance(_camera);
                    }

                    // Start an FFmpeg session.
                    _session = FFmpegSession.Create(
                        //gameObject.name,
                        name,
                        _camera.targetTexture.width,
                        _camera.targetTexture.height,
                        _frameRate, preset
                    );

                    _startTime = Time.time;
                    _frameCount = 0;
                    _frameDropCount = 0;
                }

                var gap = Time.time - FrameTime;
                var delta = 1 / _frameRate;

                if (gap < 0)
                {
                    // Update without frame data.
                    _session.PushFrame(null);
                }
                else if (gap < delta)
                {
                    // Single-frame behind from the current time:
                    // Push the current frame to FFmpeg.
                    _session.PushFrame(_camera.targetTexture);
                    _frameCount++;
                }
                else if (gap < delta * 2)
                {
                    // Two-frame behind from the current time:
                    // Push the current frame twice to FFmpeg. Actually this is not
                    // an efficient way to catch up. We should think about
                    // implementing frame duplication in a more proper way. #fixme
                    _session.PushFrame(_camera.targetTexture);
                    _session.PushFrame(_camera.targetTexture);
                    _frameCount += 2;
                }
                else
                {
                    // Show a warning message about the situation.
                    WarnFrameDrop();

                    // Push the current frame to FFmpeg.
                    _session.PushFrame(_camera.targetTexture);

                    // Compensate the time delay.
                    _frameCount += Mathf.FloorToInt(gap * _frameRate);
                }
                yield return null;
            }
        }

        IEnumerator CheckRecordTime(float time)
        {
            yield return new WaitForSeconds(time);
            StopRecording();
        }

        public void StopRecording()
        {
            _recording = false;

            if (_session != null)
            {
                // Close and dispose the FFmpeg session.
                _session.Close();
                _session.Dispose();
                _session = null;
            }

            if (_tempRT != null)
            {
                // Dispose the frame texture.
                _camera.targetTexture = null;
                Destroy(_tempRT);
                _tempRT = null;
            }

            if (_blitter != null)
            {
                // Destroy the blitter game object.
                Destroy(_blitter);
                _blitter = null;
            }
            _onEndRecord?.Invoke();
            _onEndRecord = null;

            Debug.Log("End of recording...");
        }

        void OnDisable()
        {
            if (_session != null)
            {
                // Close and dispose the FFmpeg session.
                _session.Close();
                _session.Dispose();
                _session = null;
            }

            if (_tempRT != null)
            {
                // Dispose the frame texture.
                _camera.targetTexture = null;
                Destroy(_tempRT);
                _tempRT = null;
            }

            if (_blitter != null)
            {
                // Destroy the blitter game object.
                Destroy(_blitter);
                _blitter = null;
            }
        }
    }
}
