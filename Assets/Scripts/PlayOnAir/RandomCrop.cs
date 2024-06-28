using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RandomCrop : MonoBehaviour
{
    public float moveTime = 0.25f;
    
    const float ranSizeMax = 100;
    const float ranSizeMin = 30;
    const float widRate = 5;
    const float heiRate = 6;
    const float duration = 7;

    private List<int> randomTimeParts = new List<int>();
    private List<Image> listCropFrame = new List<Image>();

    Vector3[] cornerPoint;

    //start is top left, end is bottom right
    Vector2 curCornerStart, curCornerEnd;
    //int moveNum = 0;
    Vector2 moveSize;
    Vector2 movePos;
    List<RawImage> listRaw = new List<RawImage>();
    [SerializeField] RectTransform rectFrame;
    //[SerializeField] RectTransform[] testPointDebugger;
    Texture2D screenShoot;
    private int curShoot = 0;

    // Start is called before the first frame update
    void Start()
    {
        listRaw.AddRange(GetComponentsInChildren<RawImage>());
        listCropFrame.AddRange(UtilityExtensions.GetComponentsOnlyInChildrenWithTag_Recursive<Image>(transform, "cropFrame"));

        cornerPoint = new Vector3[4];
        //it starts from bottom left and ends up at bottom right, clockwise
        listRaw[(int)eRaw.Cam].rectTransform.GetLocalCorners(cornerPoint);
        //for (int i = 0; i < 4; i++)
        //    testPointDebugger[i].anchoredPosition = cornerPoint[i];

        StartRandomMove();
    }

    void StartRandomMove()
    {
        //moveNum = Random.Range(0, ranPositionMax);
        if (curShoot < 4)
        {
            curShoot++;
            listRaw[(int)eRaw.Result].gameObject.SetActive(false);
            SetRandomSize();
        }
    }

    void SetRandomSize()
    {
        float ranSize = Random.Range(ranSizeMin, ranSizeMax);
        moveSize = new Vector2(widRate * ranSize, heiRate * ranSize);
        curCornerStart = new Vector2(cornerPoint[1].x + moveSize.x / 2,
            cornerPoint[1].y - moveSize.y / 2);
        curCornerEnd = new Vector2(cornerPoint[3].x - moveSize.x / 2,
            cornerPoint[3].y + moveSize.y / 2);
        StartCoroutine(SetRandomMovePosition());
    }
    IEnumerator SetRandomMovePosition()
    {
        movePos.x = Random.Range(curCornerStart.x, curCornerEnd.x);
        movePos.y = Random.Range(curCornerEnd.y, curCornerStart.y);
    
        randomTimeParts = DivideIntoFourParts((int)duration);
        // move frame along x, than y (could be random) 2 2 2 1,  2 3 1 1
    
        //yield return new WaitForSeconds(2);
        
        rectFrame.DOAnchorPosX(movePos.x, moveTime).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(moveTime);
        rectFrame.DOAnchorPosY(movePos.y, moveTime).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(moveTime);
        rectFrame.DOSizeDelta(new Vector2(moveSize.x, rectFrame.sizeDelta.y), moveTime).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(moveTime);
        rectFrame.DOSizeDelta(new Vector2(rectFrame.sizeDelta.x ,moveSize.y), moveTime).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(moveTime + 0.5f);
        
        //StartRandomMove();
        StartCoroutine(TakeShoot());
    }
    
    IEnumerator TakeShoot()
    {
        for (int i = 0; i < listCropFrame.Count; i++)
            listCropFrame[i].enabled = false;
        
        yield return new WaitForEndOfFrame();
        //Vector2 readArea = listRaw[(int)eRaw.Cam].rectTransform.sizeDelta;
        Vector2 readStartPoint = new Vector2(Screen.width / 2 - moveSize.x / 2 + movePos.x,
            Screen.height / 2 - moveSize.y / 2 + movePos.y);

        screenShoot = new Texture2D((int)moveSize.x, (int)moveSize.y);
        screenShoot.ReadPixels(new Rect(readStartPoint.x, readStartPoint.y,
            screenShoot.width, screenShoot.height), 0, 0);
        screenShoot.Apply();

        listRaw[(int)eRaw.Result].gameObject.SetActive(true);
        listRaw[(int)eRaw.Result].texture = screenShoot;
        
        for (int i = 0; i < listCropFrame.Count; i++)
            listCropFrame[i].enabled = true;
        yield return new WaitForSeconds(2);
        StartRandomMove();
    }

    System.Random random = new System.Random();
    
    public List<int> DivideIntoFourParts(int total)
    {
        List<int> parts = new List<int>();

        for (int i = 0; i < 3; i++)
        {
            int maxPart = System.Math.Min(total - 3, 3);
            int part = random.Next(1, maxPart + 1);
            parts.Add(part);
            total -= part;
        }

        parts.Add(total); // 마지막 구간은 남은 값을 추가

        return parts;
    }

    enum eRaw
    {
        Cam = 0,
        Result
    }
}
