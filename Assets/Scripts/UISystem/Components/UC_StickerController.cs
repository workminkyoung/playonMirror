using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vivestudios.UI;

[RequireComponent(typeof(UC_StickerThumbnail))]
public class UC_StickerController : UC_BaseComponent, IPointerDownHandler, IBeginDragHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField]
    private UC_StickerThumbnail _thumbnail;
    [SerializeField]
    private RectTransform _border;
    [SerializeField]
    private Button _deleteBtn;
    [SerializeField]
    private RectTransform _rotateBtn;
    [SerializeField]
    private RectTransform _scaleBtn;
    [SerializeField]
    private RectTransform _controlTransform;

    [SerializeField]
    private bool _isDragging = false;
    [SerializeField]
    private bool _isRotating = false;
    [SerializeField]
    private bool _isScaling = false;

    private float _movingTime = 3.0f;
    private float _offsetRotate;
    private Coroutine _hideBorderRoutine;

    [SerializeField]
    private float _offsetScale;

    public RectTransform rectTransform => transform as RectTransform;
    public Action OnPointerDownAction;

    public static UC_StickerController curControl = null;

    public override void InitComponent ()
    {

    }

    private void Start ()
    {
        _thumbnail = GetComponent<UC_StickerThumbnail>();
        float scale = 0f;
        scale = AdjectScle(_thumbnail.scale);
        SetScale(scale);
        _offsetScale = (ScreenPoint(_scaleBtn.position) - ScreenPoint(rectTransform.position)).magnitude / scale;

        _border.gameObject.SetActive(false);

        _deleteBtn.onClick.AddListener(() =>
        {
            if(_hideBorderRoutine != null)
            {
                StopCoroutine(_hideBorderRoutine);
            }
            Destroy(this.gameObject);
        });

        _thumbnail.scaler.localScale = Vector3.one;
    }
    public void OnPointerDown (PointerEventData eventData)
    {
        if(IsInScreenPoint(_rotateBtn, eventData.pressPosition))
        {
            _isRotating = true;
        }
        else if(IsInScreenPoint(_scaleBtn, eventData.pressPosition))
        {
            _isScaling = true;
        }
        else
        {
            _isDragging = true;
        }

        _border.gameObject.SetActive(true);

        OnPointerDownAction?.Invoke();

        if(curControl != null)
        {
            if(curControl != this)
            {
                curControl.HideController();
            }
        }
        curControl = this;

        if(_hideBorderRoutine != null)
        {
            StopCoroutine(_hideBorderRoutine);
        }
        _hideBorderRoutine = StartCoroutine(HideBorderRoutine());
        rectTransform.SetAsLastSibling();
    }

    public void OnBeginDrag (PointerEventData eventData)
    {
        if(_isRotating)
        {
            ResetAndGetRotateOffset();
        }
    }

    public void OnDrag (PointerEventData eventData)
    {
        if(_isDragging)
        {
            rectTransform.anchoredPosition += eventData.delta;
            rectTransform.anchoredPosition = AdjustPosition();
        }

        if(_isRotating)
        {
            Vector3 dir = eventData.position - ScreenPoint(rectTransform.position);

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - _offsetRotate;
            rectTransform.rotation = Quaternion.Euler(0, 0, angle);
            rectTransform.anchoredPosition = AdjustPosition();
        }

        if(_isScaling)
        {
            float scaleF = (eventData.position - ScreenPoint(rectTransform.position)).magnitude;
            var ratio = (scaleF / _offsetScale);

            SetScale(ratio);
            rectTransform.anchoredPosition = AdjustPosition();
        }
    }

    public void OnPointerUp (PointerEventData eventData)
    {
        _isDragging = false;
        _isRotating = false;
        _isScaling = false;
    }

    public void HideController ()
    {
        _border.gameObject.SetActive(false);
        _isDragging = false;
        _isRotating = false;
        _isScaling = false;

        if(_hideBorderRoutine != null)
        {
            StopCoroutine(_hideBorderRoutine);
        }
    }

    private bool IsInScreenPoint (RectTransform rect, Vector2 screenPoint)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rect, screenPoint, Camera.main);
    }
    private Vector2 ScreenPoint (Vector3 position)
    {
        return RectTransformUtility.WorldToScreenPoint(Camera.main, position);
    }
    private void ResetAndGetRotateOffset ()
    {
        rectTransform.rotation = Quaternion.Euler(0, 0, 0);

        var sreenPosition = ScreenPoint(rectTransform.position);
        var sreenPositionRotate = ScreenPoint(_rotateBtn.position);
        Vector3 dir = sreenPositionRotate - sreenPosition;

        _offsetRotate = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    private void SetScale (float ratio)
    {
        ratio = AdjectScle(ratio);

        rectTransform.localScale = Vector3.one * ratio;
        _controlTransform.localScale = Vector3.one / ratio;
        _controlTransform.sizeDelta = new Vector2(_thumbnail.thumbnail.rectTransform.sizeDelta.x + _thumbnail.margin, _thumbnail.thumbnail.rectTransform.sizeDelta.y + _thumbnail.margin) * ratio;
    }
    private float AdjectScle (float ratio)
    {
        var wratio = (_thumbnail.thumbnail.rectTransform.sizeDelta.x + _thumbnail.margin) / (_thumbnail.thumbnail.rectTransform.sizeDelta.y + _thumbnail.margin);

        var minScaleX = 55.0f / (_thumbnail.thumbnail.rectTransform.sizeDelta.x + _thumbnail.margin);
        var minScaleY = 55.0f / (_thumbnail.thumbnail.rectTransform.sizeDelta.y + _thumbnail.margin);

        // 정사각형에 가까울때는 더 크게 잡아준다.
        if(wratio < 1.2f)
        {
            minScaleX = 70.0f / (_thumbnail.thumbnail.rectTransform.sizeDelta.x + _thumbnail.margin);
            minScaleY = 70.0f / (_thumbnail.thumbnail.rectTransform.sizeDelta.y + _thumbnail.margin);
        }


        if(ratio < minScaleX)
            return minScaleX;
        if(ratio < minScaleY)
            return minScaleY;

        var parent = transform.parent as RectTransform;
        var rectImage = _thumbnail.thumbnail.rectTransform;

        if(ratio * rectImage.rect.width > (parent.rect.width * 1.0f))
            return (parent.rect.width * 1.0f) / rectImage.rect.width;

        if(ratio * rectImage.rect.height > (parent.rect.height * 1.0f))
            return (parent.rect.height * 1.0f) / rectImage.rect.height;

        //if (ratio > parent.rect.height)
        //    return parent.rect.height;

        return ratio;
    }
    public Vector2 AdjustPosition ()
    {
        Vector3[] corners = new Vector3[4];
        _thumbnail.thumbnail.rectTransform.GetWorldCorners(corners);
        var parent = transform.parent as RectTransform;

        float minX = float.PositiveInfinity;
        float maxX = float.NegativeInfinity;
        float minY = float.PositiveInfinity;
        float maxY = float.NegativeInfinity;

        foreach(Vector3 corner in corners)
        {
            Vector3 localPos = parent.InverseTransformPoint(corner);
            minX = Mathf.Min(minX, localPos.x);
            maxX = Mathf.Max(maxX, localPos.x);
            minY = Mathf.Min(minY, localPos.y);
            maxY = Mathf.Max(maxY, localPos.y);
        }

        float hWidth = (maxX - minX) * 0.5f;
        float hHightH = (maxY - minY) * 0.5f;

        float x = minX + hWidth;
        float y = minY + hHightH;

        float clampedX = Mathf.Clamp(x, parent.rect.min.x + (hWidth * 0.5f), parent.rect.max.x - (hWidth * 0.5f));
        float clampedY = Mathf.Clamp(y, parent.rect.min.y + (hHightH * 0.5f), parent.rect.max.y - (hHightH * 0.5f));
        return new Vector2(clampedX, clampedY);
    }

    private IEnumerator HideBorderRoutine ()
    {
        yield return new WaitForSeconds(_movingTime);
        _border.gameObject.SetActive(false);
    }

    private void OnDisable ()
    {
        HideController();
    }
}
