using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vivestudios.UI;

public class PhotoDataManager : SingletonBehaviour<PhotoDataManager>
{
    [SerializeField]
    private bool _isLandscape;
    [SerializeField]
    private int _photoCount = 0;
    private const int _photoCountCartoon = 8;
    private const int _photoCountProfile = 3;
    private const int _photoCountBeauty = 8;
    private const int _photoCountWhatIf = 3;

    [SerializeField]
    private Texture2D _selectedAIProfile;

    [SerializeField]
    private string _imagePath;
    [SerializeField]
    private string _videoPath;
    [SerializeField]
    private List<string> _recordPaths = new List<string>();

    [SerializeField]
    private List<Texture2D> _photoConverted = new List<Texture2D>();
    [SerializeField]
    private List<Texture2D> _photoOrigin = new List<Texture2D>();

    [SerializeField]
    private List<byte[]> _dslrPhotos = new List<byte[]>();

    private Dictionary<Texture2D, PHOTO_TYPE> _selectedPhoto = new Dictionary<Texture2D, PHOTO_TYPE>();
    private Dictionary<int, UC_SelectablePic> _selectedPicDic = new Dictionary<int, UC_SelectablePic>();

    public bool isLandscape => _isLandscape;
    public int photoCount => _photoCount;

    public Texture2D selectedAIProfile => _selectedAIProfile;

    public string imagePath => _imagePath;
    public string videoPath => _videoPath;
    public List<string> recordPaths => _recordPaths;

    public List<Texture2D> photoConverted => _photoConverted;
    public List<Texture2D> photoOrigin => _photoOrigin;
    public List<byte[]> dslrPhotos => _dslrPhotos;
    public Dictionary<Texture2D, PHOTO_TYPE> selectedPhoto => _selectedPhoto;
    public Dictionary<int, UC_SelectablePic> selectedPicDic => _selectedPicDic;



    protected override void Init()
    {
        GameManager.OnGameResetAction += ResetPhotoData;
    }

    public void ResetPhotoData()
    {
        //TODO : RESET
        SetPhotoConverted(new List<Texture2D>());
        SetPhotoOrigin(new List<Texture2D>());
        SetRecordPaths(new List<string>());
        _selectedPhoto = new Dictionary<Texture2D, PHOTO_TYPE>();
        _selectedPicDic = new Dictionary<int, UC_SelectablePic>();
    }

    public void SetLandscape(bool landscape)
    {
        _isLandscape = landscape;
    }

    public void SetPhotoConverted(List<Texture2D> converted)
    {
        _photoConverted = converted;
    }
    public void SetPhotoOrigin(List<Texture2D> origin)
    {
        _photoOrigin = origin;
    }
    public void SetImagePath(string path)
    {
        _imagePath = path;
    }
    public void SetVideoPath(string path)
    {
        _videoPath = path;
    }
    public void SetRecordPaths(List<string> paths)
    {
        _recordPaths = paths;
    }
    public void AddRecordPath(string path)
    {
        _recordPaths.Add(path);
    }
    public void AddPhotoConverted(Texture2D converted)
    {
        _photoConverted.Add(converted);
    }
    public void AddPhotoOrigin(Texture2D origin)
    {
        _photoOrigin.Add(origin);
    }

    public void AddSelectedPhoto(Texture2D texture, PHOTO_TYPE type)
    {
        _selectedPhoto.Add(texture, type);
    }
    public void RemoveSelectedPhoto(Texture2D texture)
    {
        _selectedPhoto.Remove(texture);
    }
    public void SetPhotoCount()
    {
        switch (UserDataManager.Instance.selectedContent)
        {
            case CONTENT_TYPE.AI_CARTOON:
                _photoCount = _photoCountCartoon;
                break;
            case CONTENT_TYPE.AI_PROFILE:
                _photoCount = _photoCountProfile;
                break;
            case CONTENT_TYPE.AI_TIME_MACHINE:
                break;
            case CONTENT_TYPE.AI_BEAUTY:
                _photoCount = _photoCountBeauty;
                break;
            case CONTENT_TYPE.WHAT_IF:
                _photoCount = _photoCountWhatIf;
                break;
            default:
                _photoCount = 0;
                break;
        }
    }

    public void SetSelectedAIProfile(Texture2D selectedProfile)
    {
        _selectedAIProfile = selectedProfile;
    }

    public void SetDslrPhotos(List<byte[]> photos)
    {
        _dslrPhotos = photos;
    }
}
