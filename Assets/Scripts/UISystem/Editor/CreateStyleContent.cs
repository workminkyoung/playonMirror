using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using MPUIKIT;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor.UIElements;
using Unity.VisualScripting;

public class CreateStyleContent : Editor
{
    [MenuItem("GameObject/UI/Style Content - Big", false, 0)]
    private static void CreateStyleContentBig()
    {
        GameObject styleContent = new GameObject("UC_StyleContent(Big)", typeof(RectTransform), typeof(UC_StyleContent));
        if (Selection.activeTransform != null)
        {
            styleContent.transform.SetParent(Selection.activeTransform);
        }
        else
        {
            styleContent.transform.SetParent(FindAnyObjectByType<Canvas>().transform);
        }

        RectTransform rectTransform = styleContent.transform as RectTransform;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.localScale = Vector3.one;
        rectTransform.sizeDelta = new Vector2(400, 626);

        RectTransform bg = new GameObject("BG", typeof(RectTransform), typeof(MPImage)).GetComponent<RectTransform>();
        bg.SetParent(rectTransform);
        StretchRectTransform(bg);
        MPImage bgImg = bg.GetComponent<MPImage>();
        bgImg.DrawShape = DrawShape.Rectangle;
        bgImg.FalloffDistance = 0;
        Rectangle bgRectangle = new Rectangle();
        bgRectangle.CornerRadius = Vector4.one * 12;
        bgRectangle.Init(bgImg.material, bgImg.material, bg);
        bgImg.Rectangle = bgRectangle;

        RectTransform thumbnail = new GameObject("ThumbnailImg", typeof(RectTransform), typeof(MPImage)).GetComponent<RectTransform>();
        thumbnail.SetParent(rectTransform);
        SetRectTransformFromLeftTop(thumbnail, 200, -230, 400, 460);
        MPImage thumbnailImg = thumbnail.GetComponent<MPImage>();
        thumbnailImg.DrawShape = DrawShape.Rectangle;
        thumbnailImg.FalloffDistance = 0;
        Rectangle thumbnailRect = new Rectangle();
        thumbnailRect.CornerRadius = new Vector4(0, 0, 12, 12);
        thumbnailRect.Init(thumbnailImg.material, thumbnailImg.material, thumbnail);
        thumbnailImg.Rectangle = thumbnailRect;

        RectTransform infoArea = new GameObject("InfoArea", typeof(RectTransform)).GetComponent<RectTransform>();
        infoArea.SetParent(rectTransform);
        SetRectTransformFromLeftTop(infoArea, 200, -525, 400, 102);

        RectTransform titleArea = new GameObject("TitleArea", typeof(RectTransform)).GetComponent<RectTransform>();
        titleArea.SetParent(infoArea);
        SetRectTransformFromLeftTop(titleArea, 200, -20, 320, 40);

        RectTransform titleText = new GameObject("TitleText", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(ContentSizeFitter)).GetComponent<RectTransform>();
        titleText.SetParent(titleArea);
        SetRectTransformFromLeftTop(titleText, 0, -20, 100, 40);
        titleText.pivot = new Vector2(0, 0.5f);
        TextMeshProUGUI titleTextCompo = titleText.GetComponent<TextMeshProUGUI>();
        titleTextCompo.font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("SpoqaHanSansNeo-Bold SDF")[0]));
        titleTextCompo.fontSize = 32;
        titleTextCompo.overrideColorTags = true;
        titleTextCompo.color = new Color(0.1294118f, 0.1294118f, 0.1294118f);
        titleTextCompo.text = "타이틀 텍스트";
        ContentSizeFitter titleTextFitter = titleText.GetComponent<ContentSizeFitter>();
        titleTextFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        RectTransform maxPlayerArea = new GameObject("MaxPlayerArea", typeof(RectTransform), typeof(MPImage)).GetComponent<RectTransform>();
        maxPlayerArea.SetParent(titleArea);
        SetRectTransformFromLeftTop(maxPlayerArea, 270.735f, -20, 98.529f, 31);
        MPImage maxPlayerAreaImg = maxPlayerArea.GetComponent<MPImage>();
        maxPlayerAreaImg.color = new Color(0.8078431f, 0.2f, 0.5372549f);
        maxPlayerAreaImg.DrawShape = DrawShape.Rectangle;
        maxPlayerAreaImg.FalloffDistance = 0;
        Rectangle maxPlayerRect = new Rectangle();
        maxPlayerRect.CornerRadius = Vector4.one * 20;
        maxPlayerRect.Init(maxPlayerAreaImg.material, maxPlayerAreaImg.material, maxPlayerArea);
        maxPlayerAreaImg.Rectangle = maxPlayerRect;

        RectTransform maxPlayerImage = new GameObject("MaxPlayerImg", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
        maxPlayerImage.SetParent(maxPlayerArea);
        maxPlayerImage.anchoredPosition = new Vector2(-25.5f, 0);
        maxPlayerImage.sizeDelta = new Vector2(15.529f, 12);
        maxPlayerImage.localScale = Vector3.one;
        maxPlayerImage.GetComponent<Image>().sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("MaxPlayerImg")[0]));

        RectTransform maxPlayerText = new GameObject("MaxPlayerText", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<RectTransform>();
        maxPlayerText.SetParent(maxPlayerArea);
        SetRectTransformFromLeftTop(maxPlayerText, 61.029f, -15.5f, 43, 15);
        TextMeshProUGUI maxPlayerTextCompo = maxPlayerText.GetComponent<TextMeshProUGUI>();
        maxPlayerTextCompo.font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("SpoqaHanSansNeo-Bold SDF")[0]));
        maxPlayerTextCompo.fontSize = 12;
        maxPlayerTextCompo.overrideColorTags = true;
        maxPlayerTextCompo.color = Color.white;
        maxPlayerTextCompo.text = "최대 0명";

        RectTransform descriptText = new GameObject("DescriptionText", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<RectTransform>();
        descriptText.SetParent(infoArea);
        SetRectTransformFromLeftTop(descriptText, 200, -77, 320, 50);
        TextMeshProUGUI descriptTextCompo = descriptText.GetComponent<TextMeshProUGUI>();
        descriptTextCompo.font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("SpoqaHanSansNeo-Regular SDF")[0]));
        descriptTextCompo.fontSize = 20;
        descriptTextCompo.overrideColorTags = true;
        descriptTextCompo.color = new Color(0.1294118f, 0.1294118f, 0.1294118f);
        descriptTextCompo.text = "설명 텍스트\n두줄까지 확인";

        RectTransform touchFeedback = new GameObject("TouchFeedback", typeof(RectTransform), typeof(MPImage)).GetComponent<RectTransform>();
        touchFeedback.SetParent(rectTransform);
        StretchRectTransform(touchFeedback);
        MPImage touchFeedbackImg = touchFeedback.GetComponent<MPImage>();
        touchFeedbackImg.color = new Color(1, 0, 0, 0.1019608f);
        touchFeedbackImg.DrawShape = DrawShape.Rectangle;
        touchFeedbackImg.FalloffDistance = 0;
        Rectangle touchFeedbackRect = new Rectangle();
        touchFeedbackRect.CornerRadius = Vector4.one * 12;
        touchFeedbackRect.Init(touchFeedbackImg.material, touchFeedbackImg.material, touchFeedback);
        touchFeedbackImg.Rectangle = touchFeedbackRect;

        RectTransform stroke = new GameObject("Stroke", typeof(RectTransform), typeof(MPImage)).GetComponent<RectTransform>();
        stroke.SetParent(rectTransform);
        StretchRectTransform(stroke);
        MPImage strokeImg = stroke.GetComponent<MPImage>();
        strokeImg.color = new Color(0.4588235f, 0.4588235f, 0.4588235f);
        strokeImg.DrawShape = DrawShape.Rectangle;
        strokeImg.FalloffDistance = 0;
        strokeImg.StrokeWidth = 1;
        Rectangle strokeRect = new Rectangle();
        strokeRect.CornerRadius = Vector4.one * 12;
        strokeRect.Init(strokeImg.material, strokeImg.material, stroke);
        strokeImg.Rectangle = strokeRect;

        var contentCompo = Editor.CreateEditor(styleContent.GetComponent<UC_StyleContent>());
        var touchFeedbackProperty = contentCompo.serializedObject.FindProperty("_touchFeedback");
        var strokeProperty = contentCompo.serializedObject.FindProperty("_stroke");
        var thumbnailProperty = contentCompo.serializedObject.FindProperty("_thumbnailImg");
        var titleTextProperty = contentCompo.serializedObject.FindProperty("_titleText");
        var descriptionProperty = contentCompo.serializedObject.FindProperty("_descriptionText");
        var maxPlayerTextProperty = contentCompo.serializedObject.FindProperty("_maxPlayerText");

        touchFeedbackProperty.SetUnderlyingValue(touchFeedbackImg);
        strokeProperty.SetUnderlyingValue(strokeImg);
        thumbnailProperty.SetUnderlyingValue(thumbnailImg);
        titleTextProperty.SetUnderlyingValue(titleTextCompo);
        descriptionProperty.SetUnderlyingValue(descriptTextCompo);
        maxPlayerTextProperty.SetUnderlyingValue(maxPlayerTextCompo);

        Selection.activeTransform = rectTransform;
    }

    [MenuItem("GameObject/UI/Style Content - Small", false, 0)]
    private static void CreateStyleContentSmall()
    {
        GameObject styleContent = new GameObject("UC_StyleContent(Small)", typeof(RectTransform), typeof(UC_StyleContent));
        if (Selection.activeTransform != null)
        {
            styleContent.transform.SetParent(Selection.activeTransform);
        }
        else
        {
            styleContent.transform.SetParent(FindAnyObjectByType<Canvas>().transform);
        }

        RectTransform rectTransform = styleContent.transform as RectTransform;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.localScale = Vector3.one;
        rectTransform.sizeDelta = new Vector2(300, 413);

        RectTransform bg = new GameObject("BG", typeof(RectTransform), typeof(MPImage)).GetComponent<RectTransform>();
        bg.SetParent(rectTransform);
        StretchRectTransform(bg);
        MPImage bgImg = bg.GetComponent<MPImage>();
        bgImg.DrawShape = DrawShape.Rectangle;
        bgImg.FalloffDistance = 0;
        Rectangle bgRectangle = new Rectangle();
        bgRectangle.CornerRadius = Vector4.one * 12;
        bgRectangle.Init(bgImg.material, bgImg.material, bg);
        bgImg.Rectangle = bgRectangle;

        RectTransform thumbnail = new GameObject("ThumbnailImg", typeof(RectTransform), typeof(MPImage)).GetComponent<RectTransform>();
        thumbnail.SetParent(rectTransform);
        SetRectTransformFromLeftTop(thumbnail, 150, -150, 300, 300);
        MPImage thumbnailImg = thumbnail.GetComponent<MPImage>();
        thumbnailImg.DrawShape = DrawShape.Rectangle;
        thumbnailImg.FalloffDistance = 0;
        Rectangle thumbnailRect = new Rectangle();
        thumbnailRect.CornerRadius = new Vector4(0, 0, 12, 12);
        thumbnailRect.Init(thumbnailImg.material, thumbnailImg.material, thumbnail);
        thumbnailImg.Rectangle = thumbnailRect;

        RectTransform infoArea = new GameObject("InfoArea", typeof(RectTransform)).GetComponent<RectTransform>();
        infoArea.SetParent(rectTransform);
        SetRectTransformFromLeftTop(infoArea, 150, -358.5f, 300, 69);

        RectTransform titleArea = new GameObject("TitleArea", typeof(RectTransform)).GetComponent<RectTransform>();
        titleArea.SetParent(infoArea);
        SetRectTransformFromLeftTop(titleArea, 150, -17.5f, 260, 35);

        RectTransform titleText = new GameObject("TitleText", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(ContentSizeFitter)).GetComponent<RectTransform>();
        titleText.SetParent(titleArea);
        SetRectTransformFromLeftTop(titleText, 68, -17.5f, 136, 35);
        titleText.pivot = new Vector2(0, 0.5f);
        titleText.anchoredPosition = new Vector2(0, -17.5f);
        TextMeshProUGUI titleTextCompo = titleText.GetComponent<TextMeshProUGUI>();
        titleTextCompo.font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("SpoqaHanSansNeo-Bold SDF")[0]));
        titleTextCompo.fontSize = 28;
        titleTextCompo.overrideColorTags = true;
        titleTextCompo.color = new Color(0.1294118f, 0.1294118f, 0.1294118f);
        titleTextCompo.text = "타이틀 텍스트";
        ContentSizeFitter titleTextFitter = titleText.GetComponent<ContentSizeFitter>();
        titleTextFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        RectTransform descriptText = new GameObject("DescriptionText", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<RectTransform>();
        descriptText.SetParent(infoArea);
        SetRectTransformFromLeftTop(descriptText, 150, -58, 260, 22);
        TextMeshProUGUI descriptTextCompo = descriptText.GetComponent<TextMeshProUGUI>();
        descriptTextCompo.font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("SpoqaHanSansNeo-Regular SDF")[0]));
        descriptTextCompo.fontSize = 18;
        descriptTextCompo.overrideColorTags = true;
        descriptTextCompo.color = new Color(0.1294118f, 0.1294118f, 0.1294118f);
        descriptTextCompo.text = "설명 텍스트";

        RectTransform touchFeedback = new GameObject("TouchFeedback", typeof(RectTransform), typeof(MPImage)).GetComponent<RectTransform>();
        touchFeedback.SetParent(rectTransform);
        StretchRectTransform(touchFeedback);
        MPImage touchFeedbackImg = touchFeedback.GetComponent<MPImage>();
        touchFeedbackImg.color = new Color(1, 0, 0, 0.1019608f);
        touchFeedbackImg.DrawShape = DrawShape.Rectangle;
        touchFeedbackImg.FalloffDistance = 0;
        Rectangle touchFeedbackRect = new Rectangle();
        touchFeedbackRect.CornerRadius = Vector4.one * 12;
        touchFeedbackRect.Init(touchFeedbackImg.material, touchFeedbackImg.material, touchFeedback);
        touchFeedbackImg.Rectangle = touchFeedbackRect;

        RectTransform stroke = new GameObject("Stroke", typeof(RectTransform), typeof(MPImage)).GetComponent<RectTransform>();
        stroke.SetParent(rectTransform);
        StretchRectTransform(stroke);
        MPImage strokeImg = stroke.GetComponent<MPImage>();
        strokeImg.color = new Color(0.4588235f, 0.4588235f, 0.4588235f);
        strokeImg.DrawShape = DrawShape.Rectangle;
        strokeImg.FalloffDistance = 0;
        strokeImg.StrokeWidth = 1;
        Rectangle strokeRect = new Rectangle();
        strokeRect.CornerRadius = Vector4.one * 12;
        strokeRect.Init(strokeImg.material, strokeImg.material, stroke);
        strokeImg.Rectangle = strokeRect;

        var contentCompo = Editor.CreateEditor(styleContent.GetComponent<UC_StyleContent>());
        var touchFeedbackProperty = contentCompo.serializedObject.FindProperty("_touchFeedback");
        var strokeProperty = contentCompo.serializedObject.FindProperty("_stroke");
        var thumbnailProperty = contentCompo.serializedObject.FindProperty("_thumbnailImg");
        var titleTextProperty = contentCompo.serializedObject.FindProperty("_titleText");
        var descriptionProperty = contentCompo.serializedObject.FindProperty("_descriptionText");

        touchFeedbackProperty.SetUnderlyingValue(touchFeedbackImg);
        strokeProperty.SetUnderlyingValue(strokeImg);
        thumbnailProperty.SetUnderlyingValue(thumbnailImg);
        titleTextProperty.SetUnderlyingValue(titleTextCompo);
        descriptionProperty.SetUnderlyingValue(descriptTextCompo);

        Selection.activeTransform = rectTransform;
    }

    [MenuItem("GameObject/UI/Selectable Content (Frame)", false, 1)]
    private static void CreateSelectableContent()
    {
        GameObject styleContent = new GameObject("UC_SelectableContent", typeof(RectTransform), typeof(UC_SelectableContent));
        if (Selection.activeTransform != null)
        {
            styleContent.transform.SetParent(Selection.activeTransform);
        }
        else
        {
            styleContent.transform.SetParent(FindAnyObjectByType<Canvas>().transform);
        }

        RectTransform rectTransform = styleContent.transform as RectTransform;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.localScale = Vector3.one;
        rectTransform.sizeDelta = new Vector2(300, 413);

        RectTransform bg = new GameObject("BG", typeof(RectTransform), typeof(MPImage)).GetComponent<RectTransform>();
        bg.SetParent(rectTransform);
        StretchRectTransform(bg);
        MPImage bgImg = bg.GetComponent<MPImage>();
        bgImg.DrawShape = DrawShape.Rectangle;
        bgImg.FalloffDistance = 0;
        Rectangle bgRectangle = new Rectangle();
        bgRectangle.CornerRadius = Vector4.one * 12;
        bgRectangle.Init(bgImg.material, bgImg.material, bg);
        bgImg.Rectangle = bgRectangle;

        RectTransform thumbnail = new GameObject("ThumbnailImg", typeof(RectTransform), typeof(MPImage)).GetComponent<RectTransform>();
        thumbnail.SetParent(rectTransform);
        StretchRectTransform(thumbnail);
        MPImage thumbnailImg = thumbnail.GetComponent<MPImage>();
        thumbnailImg.DrawShape = DrawShape.Rectangle;
        thumbnailImg.FalloffDistance = 0;
        Rectangle thumbnailRect = new Rectangle();
        thumbnailRect.CornerRadius = new Vector4(0, 0, 12, 12);
        thumbnailRect.Init(thumbnailImg.material, thumbnailImg.material, thumbnail);
        thumbnailImg.Rectangle = thumbnailRect;

        RectTransform touchFeedback = new GameObject("TouchFeedback", typeof(RectTransform), typeof(MPImage)).GetComponent<RectTransform>();
        touchFeedback.SetParent(rectTransform);
        StretchRectTransform(touchFeedback);
        MPImage touchFeedbackImg = touchFeedback.GetComponent<MPImage>();
        touchFeedbackImg.color = new Color(1, 0, 0, 0.1019608f);
        touchFeedbackImg.DrawShape = DrawShape.Rectangle;
        touchFeedbackImg.FalloffDistance = 0;
        Rectangle touchFeedbackRect = new Rectangle();
        touchFeedbackRect.CornerRadius = Vector4.one * 12;
        touchFeedbackRect.Init(touchFeedbackImg.material, touchFeedbackImg.material, touchFeedback);
        touchFeedbackImg.Rectangle = touchFeedbackRect;

        RectTransform stroke = new GameObject("Stroke", typeof(RectTransform), typeof(MPImage)).GetComponent<RectTransform>();
        stroke.SetParent(rectTransform);
        StretchRectTransform(stroke);
        MPImage strokeImg = stroke.GetComponent<MPImage>();
        strokeImg.color = new Color(0.4588235f, 0.4588235f, 0.4588235f);
        strokeImg.DrawShape = DrawShape.Rectangle;
        strokeImg.FalloffDistance = 0;
        strokeImg.StrokeWidth = 1;
        Rectangle strokeRect = new Rectangle();
        strokeRect.CornerRadius = Vector4.one * 12;
        strokeRect.Init(strokeImg.material, strokeImg.material, stroke);
        strokeImg.Rectangle = strokeRect;

        var contentCompo = Editor.CreateEditor(styleContent.GetComponent<UC_SelectableContent>());
        var touchFeedbackProperty = contentCompo.serializedObject.FindProperty("_touchFeedback");
        var strokeProperty = contentCompo.serializedObject.FindProperty("_stroke");
        var thumbnailProperty = contentCompo.serializedObject.FindProperty("_thumbnailImg");

        touchFeedbackProperty.SetUnderlyingValue(touchFeedbackImg);
        strokeProperty.SetUnderlyingValue(strokeImg);
        thumbnailProperty.SetUnderlyingValue(thumbnailImg);

        Selection.activeTransform = rectTransform;
    }

    [CustomEditor(typeof(UC_SelectableContent))]
    private class UC_SelectableContentEditor : Editor { }

    [CustomEditor(typeof(UC_StyleContent))]
    private class UC_StyleContentEditor : Editor { }

    private static void StretchRectTransform(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.localScale = Vector3.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private static void SetRectTransformFromLeftTop(RectTransform rectTransform, float posX = 0, float posY = 0, float width = 50, float height = 50)
    {
        rectTransform.pivot = Vector2.one * 0.5f;
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.localScale = Vector3.one;
        rectTransform.anchoredPosition = new Vector2(posX, posY);
        rectTransform.sizeDelta = new Vector2(width, height);
    }

    public class RectTransformLeftTopWindow : EditorWindow
    {
        private Vector2 pos;
        private Vector2 size;
        public Vector2 Pos => pos;
        public Vector2 Size => size;

        private bool isConfirmed = false;
        public bool IsConfirmed => isConfirmed;

        private void OnGUI()
        {
            pos = EditorGUILayout.Vector2Field("Position", pos);
            size = EditorGUILayout.Vector2Field("Size", size);
            if (GUILayout.Button("Save"))
            {
                isConfirmed = true;
                Close();
            }
            if (GUILayout.Button("Cancel"))
            {
                isConfirmed = false;
                Close();
            }
        }
    }


}
