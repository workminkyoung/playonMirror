using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonBehaviour<SoundManager>
{
    [SerializeField]
    private List<AudioClip> _audioClips = new List<AudioClip>();
    [SerializeField]
    private AudioSource _audioSource;

    protected override void Init()
    {
        //throw new System.NotImplementedException();
        _audioSource = GetComponent<AudioSource>();
    }

    public void Play(AUDIO audio, bool isLoop = false)
    {
        _audioSource.clip = _audioClips[(int)audio];
        _audioSource.loop = isLoop;
        _audioSource.Play();
    }

    public void Stop()
    {
        _audioSource.Stop();
    }

    public void Pause()
    {
        _audioSource.Pause();
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Play(AUDIO.TOUCH);
            }
        }
    }
}
