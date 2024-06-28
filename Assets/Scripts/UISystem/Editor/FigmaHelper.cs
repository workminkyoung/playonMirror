using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class FigmaHelper : Editor
{
    [MenuItem("GameObject/Set RectTransform - Figma Code", false, 0)]
    private static void SetRectTransformFromFigmaCode()
    {
        RectTransform rectTransform = Selection.activeTransform.GetComponent<RectTransform>();
        RectTransformFigmaCodeWindow window = CreateInstance<RectTransformFigmaCodeWindow>();
        window.ShowModalUtility();

        if (window.IsConfirmed)
        {
            Undo.RecordObject(rectTransform, "Undo RectTransform");
            rectTransform.localScale = Vector3.one;
            rectTransform.pivot = window.Pivot;
            rectTransform.anchorMin = window.AnchorMin;
            rectTransform.anchorMax = window.AnchorMax;
            rectTransform.anchoredPosition = window.Pos;
            rectTransform.localEulerAngles = window.Rotation;
            rectTransform.sizeDelta = window.Size;
        }
    }

    [MenuItem("GameObject/Set RectTransform - Figma Code", true)]
    private static bool SetRectTransformFromLeftTopValidate()
    {
        if (Selection.activeTransform == null)
        {
            return false;
        }
        else
        {
            if (Selection.activeTransform.GetComponent<RectTransform>() == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public class RectTransformFigmaCodeWindow : EditorWindow
    {
        private string code;
        private Vector2 pos;
        private Vector2 size;
        private Vector2 anchorMin;
        private Vector2 anchorMax;
        private Vector2 pivot;
        private Vector3 rotation;

        private string[] split;

        private bool isConfirmed = false;

        public string Code => code;
        public Vector2 Pos => pos;
        public Vector2 Size => size;
        public Vector2 AnchorMin => anchorMin;
        public Vector2 AnchorMax => anchorMax;
        public Vector2 Pivot => pivot;
        public Vector3 Rotation => rotation;
        public bool IsConfirmed => isConfirmed;

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUI.SetNextControlName("TextField");
            code = EditorGUILayout.TextArea(code, GUILayout.Height(150));
            GUI.FocusControl("TextField");

            GUILayout.Space(10);

            if (Event.current.control)
            {
                if (Event.current.keyCode == KeyCode.S)
                {
                    isConfirmed = true;
                    Close();
                }
            }

            if (GUILayout.Button("Save - 저장"))
            {
                isConfirmed = true;
                Close();
            }

            if (GUILayout.Button("Clear - 초기화"))
            {
                code = string.Empty;
                GUI.FocusControl("");
            }

            if (GUILayout.Button("Cancel - 취소"))
            {
                Close();
            }

            if (!string.IsNullOrEmpty(code))
            {
                split = code.Split("\n");

                Regex pattern = new Regex(@"-?[0-9]*\.?[0-9]+");

                if (split.Length > 1)
                {
                    string[] posSplit = split[1].Split(":");
                    if (posSplit.Length > 1)
                        float.TryParse(pattern.Match(posSplit[1]).Value, out pos.x);
                    if (posSplit.Length > 2)
                        float.TryParse(pattern.Match(posSplit[2]).Value, out pos.y);
                }
                if (split.Length > 2)
                {
                    string[] sizeSplit = split[2].Split(":");
                    if (sizeSplit.Length > 1)
                        float.TryParse(pattern.Match(sizeSplit[1]).Value, out size.x);
                    if (sizeSplit.Length > 2)
                        float.TryParse(pattern.Match(sizeSplit[2]).Value, out size.y);

                }
                if (split.Length > 4)
                {
                    string[] minSplit = split[4].Split(":");
                    if (minSplit.Length > 2)
                        float.TryParse(pattern.Match(minSplit[2]).Value, out anchorMin.x);
                    if (minSplit.Length > 3)
                        float.TryParse(pattern.Match(minSplit[3]).Value, out anchorMin.y);

                }
                if (split.Length > 5)
                {
                    string[] maxSplit = split[5].Split(":");
                    if (maxSplit.Length > 2)
                        float.TryParse(pattern.Match(maxSplit[2]).Value, out anchorMax.x);
                    if (maxSplit.Length > 3)
                        float.TryParse(pattern.Match(maxSplit[3]).Value, out anchorMax.y);
                }
                if (split.Length > 6)
                {
                    string[] pivotSplit = split[6].Split(":");

                    if (pivotSplit.Length > 2)
                        float.TryParse(pattern.Match(pivotSplit[2]).Value, out pivot.x);
                    if (pivotSplit.Length > 3)
                        float.TryParse(pattern.Match(pivotSplit[3]).Value, out pivot.y);
                }
                if (split.Length > 7)
                {
                    string[] rotationSplit = split[7].Split(":");
                    if (rotationSplit.Length > 2)
                        float.TryParse(pattern.Match(rotationSplit[2]).Value, out rotation.x);
                    if (rotationSplit.Length > 3)
                        float.TryParse(pattern.Match(rotationSplit[3]).Value, out rotation.y);
                    if (rotationSplit.Length > 4)
                        float.TryParse(pattern.Match(rotationSplit[4]).Value, out rotation.z);
                }
            }

            GUILayout.Label("[RESULT]");
            GUILayout.BeginVertical("HelpBox");
            GUILayout.Label("[Position]");
            GUILayout.Label($"   X : {pos.x} , Y : {pos.y}");
            GUILayout.Space(10);
            GUILayout.Label("[Size]");
            GUILayout.Label($"   Width : {size.x} , Height : {size.y}");
            GUILayout.Space(10);
            GUILayout.Label("[Anchors]");
            GUILayout.Label($"   Min : X : {anchorMin.x} , Y : {anchorMin.y}");
            GUILayout.Label($"   Max : X : {anchorMax.x} , Y : {anchorMax.y}");
            GUILayout.Space(10);
            GUILayout.Label("[Pivot]");
            GUILayout.Label($"   X : {pivot.x} , Y : {pivot.y}");
            GUILayout.Space(10);
            GUILayout.Label("[Rotation]");
            GUILayout.Label($"   X : {rotation.x} , Y : {rotation.y}, Z : {rotation.z}");
            GUILayout.EndVertical();
        }
    }
}
