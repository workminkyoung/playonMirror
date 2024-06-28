using Google.Protobuf.Collections;
using System.Collections;

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class AutoIncreaseBuildVersion : MonoBehaviour
{

    static bool _autoIncrease = true;
    const string _autoIncreaseMenuName = "Build/AutoIncreaseVersion";
    const string _checkVersionMenuName = "Build/CheckVersion";
    const string _versionTypeBeta = "d";
    const string _versionTypeRelease = "r";

    [MenuItem(_autoIncreaseMenuName, false, 1)]
    private static void SetAutoIncrease()
    {
        _autoIncrease = !_autoIncrease;
        EditorPrefs.SetBool(_autoIncreaseMenuName, _autoIncrease);
        Debug.Log("Auto Increase : " + _autoIncrease);
    }

    [MenuItem(_autoIncreaseMenuName, true)]
    private static bool SetAutoIncreaseValidate()
    {
        Menu.SetChecked(_autoIncreaseMenuName, _autoIncrease);
        return true;
    }

    [MenuItem(_checkVersionMenuName, false, 2)]
    private static void CheckCurrentVersion()
    {
        Debug.Log("Build v" + PlayerSettings.bundleVersion +
             " (" + PlayerSettings.Android.bundleVersionCode + ")"); //현재 버전 표시
    }

    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (_autoIncrease) IncreaseBuild();
    }

    static void EditVersion(int majorIncr, int minorIncr, int updateIncr, int buildIncr)
    {
        string[] dotSplit = PlayerSettings.bundleVersion.Split('.');
        string[] underUpdate = SplitStringAndNumber(dotSplit[dotSplit.Length - 1]);
        List<string> splitVersion = new List<string>();
        splitVersion.Add(dotSplit[0]);
        splitVersion.Add(dotSplit[1]);
        splitVersion.AddRange(underUpdate);

        int majorVersion = int.Parse(splitVersion[0]) + majorIncr;
        int minorVersion = int.Parse(splitVersion[1]) + minorIncr;
        int updateVersion = int.Parse(splitVersion[2]) + updateIncr;
        int buildVersion = int.Parse(splitVersion[4]) + buildIncr;

        string releaseType = Debug.isDebugBuild ? _versionTypeBeta : _versionTypeRelease;

        PlayerSettings.bundleVersion = majorVersion.ToString("0") + "." +
                                       minorVersion.ToString("0") + "." +
                                       updateVersion.ToString("0") + releaseType +
                                       buildVersion.ToString("00");

        CheckCurrentVersion();
    }

    //major 1.0.0r0
    [MenuItem("Build/Increase Major Version", false, 51)]
    private static void IncreaseMajor()
    {
        EditVersion(1, 0, 0, 0);
    }

    //minor 0.1.0r0
    [MenuItem("Build/Increase Minor Version", false, 51)]
    private static void IncreaseMinor()
    {
        EditVersion(0, 1, 0, 0);
    }

    //update 0.0.1r0
    [MenuItem("Build/Increase Update Version", false, 51)]
    private static void IncreaseUpdate()
    {
        EditVersion(0, 0, 1, 0);
    }

    //build 0.0.0r1
    [MenuItem("Build/Increase Build Version", false, 51)]
    private static void IncreaseBuild()
    {
        EditVersion(0, 0, 0, 1);
    }

    private static string[] SplitStringAndNumber(string input)
    {
        // 정규표현식을 사용하여 문자열과 숫자를 분리
        Match match = Regex.Match(input, @"(\d+)([a-zA-Z]+)(\d+)");

        // 결과 배열 초기화
        string[] result = new string[3];

        // 매치된 결과 확인
        if (match.Success)
        {
            //숫자
            result[0] = match.Groups[1].Value;

            //문자
            result[1] = match.Groups[2].Value;

            //숫자
            result[2] = match.Groups[3].Value;
            //Debug.Log(result[1]);
        }
        else
        {
            // 매치되지 않으면 전체 문자열을 문자열 부분으로 설정하고 숫자 부분은 없음
            result[0] = input;
            result[1] = "";
            result[2] = "";
        }

        return result;
    }

    enum eVersionType
    {
        Major = 0,
        Minor,
        Update,
        Build,
    }

}
#endif
