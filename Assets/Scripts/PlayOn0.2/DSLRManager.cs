using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using Vivestudios.UI;
using System.Security;

public class DSLRManager : SingletonBehaviour<DSLRManager>
{
    private EosLoader _eosLoader;
    public Action<Texture2D> OnLoadTexture;
    //TODO : preview 필요 페이지들에서 action 등록 필요
    public Action<Texture2D> OnLoadPreview;
    public Action OnEndLoadAllTexture = null;


    [SerializeField]
    private Dictionary<string, Texture2D> _loadPhotos = new Dictionary<string, Texture2D>();
    private List<byte[]> _listLoadPhotos = new List<byte[]>();
    public int _loadCountMax = 0;
    public int _curLoadCount = 0;
    Coroutine _loadCoroutine = null;
    Rect _cropRect;

    [SerializeField]
    int wbType = 0;

    public Rect cropRect
    {
        get { return _cropRect; }
        set { _cropRect = value; }
    }
    public int LoadCountMax { set { _loadCountMax = value; } }

    protected override void Init()
    {
        //throw new System.NotImplementedException();
        if (_eosLoader == null)
            _eosLoader = gameObject.AddComponent<EosLoader>();
        GameManager.OnGameResetAction += ResetData;
        GameManager.OnGameLateInitAction += SDKLoad;
    }

    public void SDKLoad()
    {
        _eosLoader?.Initialize();
    }

    public void StartEVF()
    {
        _eosLoader?.CameraEVFOn();
    }

    public void EndEVF()
    {
        _eosLoader?.CameraEVFOff();
    }

    public void CameraShoot()
    {
        _eosLoader?.CameraShoot();
    }

    public void CameraAutoFocusON()
    {
        _eosLoader?.CameraAutoFocusON();
    }
    public void CameraManualFocusON()
    {
        _eosLoader?.CameraManualON();
    }

    public void CameraEVFAutoFocusOn()
    {
        _eosLoader?.CameraEVFAutoFocusOn();
    }

    public void CameraEVFAFOFF()
    {
        _eosLoader?.CameraEVFAutoFocusOff();
    }

    public void CameraForceShoot(Action OnEndShoot = null)
    {
        _eosLoader?.CameraForceShoot(OnEndShoot);
    }

    public void CameraSetting()
    {
        _eosLoader?.CameraSetting();
    }

    public void ReleaseCamera()
    {
        _eosLoader?.FreeSDK();
    }

    public void OpenSession()
    {
        _eosLoader.OpenSession();
    }

    public void CloseSession()
    {
        _eosLoader.CloseSession();
    }

    public void ResetData()
    {
        //TODO : ResetGame활성화되면 페이지에서 reset해주는거 삭제하기
        RemovePhoto();
        OnEndLoadAllTexture = null;
        OnLoadTexture = null;
        _loadPhotos.Clear();
        _loadPhotos = new Dictionary<string, Texture2D>();
        _listLoadPhotos.Clear();
        _listLoadPhotos = new List<byte[]>();
        _curLoadCount = 0;
        _loadCountMax = 0;
        if (_loadCoroutine != null)
        {
            StopCoroutine(_loadCoroutine);
            _loadCoroutine = null;
        }
    }

    public void ErrorOnCamera()
    {
        _eosLoader?.ErrorOnCamera();
    }

    public void RemovePhoto()
    {
        string[] files = Directory.EnumerateFiles(TextData.dslrPhotoPath, "*.jpg").ToArray();

        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);

            // 파일 삭제
            File.Delete(file);
            CustomLogger.Log(fileName + " deleted.");
        }
    }

    public void StartLoadPhoto()
    {
        if (_loadCoroutine != null)
        {
            StopCoroutine(_loadCoroutine);
            _loadCoroutine = null;
        }

        _loadCoroutine = StartCoroutine(LoadPhoto());
    }

    public void StopLoadPhoto()
    {
        if (_loadCoroutine != null)
        {
            StopCoroutine(_loadCoroutine);
            _loadCoroutine = null;
        }
    }

    IEnumerator LoadPhoto()
    {
        while (_loadPhotos.Count < _loadCountMax)
        {
            //완전히 생성되기 전까지 기다리기
            yield return new WaitForSecondsRealtime(UserDataManager.Instance.curShootTime + 3);
            string[] files = Directory.EnumerateFiles(TextData.dslrPhotoPath, "*.jpg").ToArray();

            if (files.Count() > _curLoadCount)
            {
                //new loaded
                foreach (string file in files)
                {
                    string name = Path.GetFileName(file);
                    if (!_loadPhotos.ContainsKey(name))
                    {
                        //add new loaded
                        byte[] data = File.ReadAllBytes(file);
                        Texture2D texture2D = new Texture2D(0, 0);
                        texture2D.LoadImage(data);
                        _loadPhotos.Add(name, texture2D);
                        _listLoadPhotos.Add(data);

                        CustomLogger.Log("[ cartoon ]" + file + " is loaded");
                        OnLoadTexture?.Invoke(texture2D);
                    }
                }
            }
        }
        
        CustomLogger.Log("loaded all");
        PhotoDataManager.Instance.SetDslrPhotos(_listLoadPhotos);
        OnEndLoadAllTexture?.Invoke();
    }

    public void LoadPhotoAll()
    {
        foreach (string file in Directory.EnumerateFiles(TextData.dslrPhotoPath, "*.jpg"))
        {
            byte[] data = File.ReadAllBytes(file);
            Texture2D texture2D = new Texture2D(0, 0);
            texture2D.LoadImage(data);
            _listLoadPhotos.Add(data);

            OnLoadTexture?.Invoke(texture2D);
            CustomLogger.Log("[ profile] " + file + " is loaded");
        }
        PhotoDataManager.Instance.SetDslrPhotos(_listLoadPhotos);
        OnEndLoadAllTexture?.Invoke();
    }

    public int LoadPhotoCounts()
    {
        string[] files = Directory.EnumerateFiles(TextData.dslrPhotoPath, "*.jpg").ToArray();
        return files.Length;
    }

    public bool CheckConnected()
    {
        if(_eosLoader)
            return _eosLoader.IsConnected;
        else
            return false;
    }
}
