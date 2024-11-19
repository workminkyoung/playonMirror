using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Vivestudios.UI
{
    public class PlayOnProperties
    {
        //768*576
        public const int crop4x3_width = 1280;
        public const int crop4x3_height = 960;

        public const int crop3x4_width = 720;
        public const int crop3x4_height = 960;

        public const int reSize4 = 768;
        public const int reSize3 = 576;
    }

    public class TextData
    {
        public static string[] resultTitles =
        {
            "사진을 선택해주세요",
            "사진에 적용할 효과를 선택해주세요",
            "프레임 컬러를 선택해주세요"
        };

        public static string[] cartoonModel =
        {
            "kawaiimixNijiV5Cute_v10.safetensors",
            "manmaruMix_v10.safetensors",
            "AnythingV5Ink_ink.safetensors",
            "revAnimated_v122.safetensors",
            "arteyou_alpha1.safetensors"
        };

        public const string beautyModel = "xxmix9realistic_v40.safetensors";

        public static string storageFolderPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Storage");

        public static string imagePath = Path.Combine(storageFolderPath, "image.jpg");
        public static string printPath = Path.Combine(storageFolderPath, "print.png");
        public static string vidpath = Path.Combine(storageFolderPath, "video.mp4");
        public static string[] filePaths = { "SNAPAI.jpg", "SNAPAI.mp4" };
        public static string jsonpath = Directory.GetParent(Application.dataPath) + "/json.json";
        public static string recordName = "recordPart";

        public static string replace_target = "TextReplace_target";
        public static string replace_source = "TextReplace_source";
        public static string replace_tagger = "TextReplace_tagger";
        public static string replace_width = "TextReplace_width";
        public static string replace_height = "TextReplace_height";

        public static string testLog_url = "http://3.35.8.52:1996/logs";
        public static string testLog_fileUrl = "http://3.35.8.52:1996/data";
        public static string releaseLog_url = "http://43.200.46.181:1996/logs";
        public static string releaseLog_fileUrl = "http://43.200.46.181:1996/data";

        public static string dslrPhotoPath = Directory.GetParent(Application.dataPath).FullName;
    }
}
