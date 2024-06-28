//using Newtonsoft.Json;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Vivestudios.UI;
//using System.IO;

//public partial class UC_AiCartoon : UC_BaseComponent
//{

//    void Control_Kawaii(string tagger)
//    {
//        Dictionary<string, object> requestBody = new Dictionary<string, object>();

//        requestBody.Add("width", ConfigData.config.reSizeWidth);
//        requestBody.Add("height", ConfigData.config.reSizeHeight);
//        requestBody.Add("steps", 20);
//        requestBody.Add("sampler_name", "DPM++ 2M Karras");
//        requestBody.Add("cfg_scale", 7.0);
//        requestBody.Add("init_images", new string[1] { _targetEncodeText });
//        requestBody.Add("denoising_strength", ConfigData.config.kawaii_denoising);
//        requestBody.Add("save_images", true);
//        requestBody.Add("clip_skip", 2);

//        Dictionary<string, object> args0 = new Dictionary<string, object>();
//        args0.Add("module", "softedge_pidinet");
//        args0.Add("model", "control_v11p_sd15_softedge");
//        args0.Add("pixel_perfect", true);
//        args0.Add("weight", 1);
//        args0.Add("control_mode", 2);
//        args0.Add("resize_mode", 2);
//        args0.Add("processor_res", 512);
//        args0.Add("threshold_a", 64);
//        args0.Add("threshold_b", 64);

//        Dictionary<string, object> args1 = new Dictionary<string, object>();
//        args1.Add("module", "openpose_hand");
//        args1.Add("model", "control_v11p_sd15_openpose");
//        args1.Add("pixel_perfect", true);
//        args1.Add("weight", 1);
//        args1.Add("control_mode", 2);
//        args1.Add("resize_mode", 2);
//        args1.Add("processor_res", 512);
//        args1.Add("threshold_a", 64);
//        args1.Add("threshold_b", 64);

//        Dictionary<string, object> args2 = new Dictionary<string, object>();
//        args2.Add("module", "tile_resample");
//        args2.Add("model", "control_v11f1e_sd15_tile");
//        args2.Add("pixel_perfect", true);
//        args2.Add("weight", 0.4);
//        args2.Add("control_mode", 2);
//        args2.Add("resize_mode", 2);
//        args2.Add("processor_res", -1);
//        args2.Add("threshold_a", 8);
//        args2.Add("threshold_b", 64);

//        List<Dictionary<string, object>> args = new List<Dictionary<string, object>>();
//        args.Add(args0);
//        args.Add(args1);
//        args.Add(args2);

//        Dictionary<string, object> controlnet = new Dictionary<string, object>();
//        controlnet.Add("args", args);

//        Dictionary<string, object> alwayson = new Dictionary<string, object>();
//        alwayson.Add("ControlNet", controlnet);

//        requestBody.Add("alwayson_scripts", alwayson);
//        requestBody.Add("prompt", tagger);
//        requestBody.Add("negative_prompt", ConfigData.config.negative_prompt);

//        string json = JsonConvert.SerializeObject(requestBody);
//        ApiCall.Instance.Img2Img(json, result => ShowResult(result));

//        //save json
//        requestBody["init_images"] = string.Empty;
//        string tempJson = JsonConvert.SerializeObject(requestBody);
//        //SaveJson("Kawaii", tempJson);
//    }

//    void Control_Manmaru(string tagger)
//    {
//        Dictionary<string, object> requestBody = new Dictionary<string, object>();

//        requestBody.Add("width", ConfigData.config.reSizeWidth);
//        requestBody.Add("height", ConfigData.config.reSizeHeight);
//        requestBody.Add("steps", 20);
//        requestBody.Add("sampler_name", "DPM++ 2M Karras");
//        requestBody.Add("cfg_scale", 7.0);
//        requestBody.Add("init_images", new string[1] { _targetEncodeText });
//        requestBody.Add("denoising_strength", ConfigData.config.manmaru_denoising);
//        requestBody.Add("save_images", true);
//        requestBody.Add("restore_faces", false);
//        requestBody.Add("clip_skip", 2);

//        Dictionary<string, object> args0 = new Dictionary<string, object>();
//        args0.Add("module", "softedge_pidinet");
//        args0.Add("model", "control_v11p_sd15_softedge");
//        args0.Add("pixel_perfect", true);
//        args0.Add("weight", 1);
//        args0.Add("control_mode", 2);
//        args0.Add("resize_mode", 2);
//        args0.Add("processor_res", 512);
//        args0.Add("threshold_a", 64);
//        args0.Add("threshold_b", 64);

//        Dictionary<string, object> args1 = new Dictionary<string, object>();
//        args1.Add("module", "openpose_hand");
//        args1.Add("model", "control_v11p_sd15_openpose");
//        args1.Add("pixel_perfect", true);
//        args1.Add("weight", 1);
//        args1.Add("control_mode", 2);
//        args1.Add("resize_mode", 2);
//        args1.Add("processor_res", 512);
//        args1.Add("threshold_a", 64);
//        args1.Add("threshold_b", 64);

//        Dictionary<string, object> args2 = new Dictionary<string, object>();
//        args2.Add("module", "tile_resample");
//        args2.Add("model", "control_v11f1e_sd15_tile");
//        args2.Add("pixel_perfect", true);
//        args2.Add("weight", 0.4);
//        args2.Add("control_mode", 2);
//        args2.Add("resize_mode", 2);
//        args2.Add("processor_res", -1);
//        args2.Add("threshold_a", 8);
//        args2.Add("threshold_b", 64);

//        List<Dictionary<string, object>> args = new List<Dictionary<string, object>>();
//        args.Add(args0);
//        args.Add(args1);
//        args.Add(args2);

//        Dictionary<string, object> controlnet = new Dictionary<string, object>();
//        controlnet.Add("args", args);

//        Dictionary<string, object> alwayson = new Dictionary<string, object>();
//        alwayson.Add("ControlNet", controlnet);

//        requestBody.Add("alwayson_scripts", alwayson);
//        requestBody.Add("prompt", tagger);
//        requestBody.Add("negative_prompt", ConfigData.config.negative_prompt);

//        string json = JsonConvert.SerializeObject(requestBody);
//        ApiCall.Instance.Img2Img(json, result => ShowResult(result));

//        //save json
//        requestBody["init_images"] = string.Empty;
//        string tempJson = JsonConvert.SerializeObject(requestBody);
//        //SaveJson("Manmaru", tempJson);
//    }

//    void Control_Artyou(string tagger)
//    {
//        Dictionary<string, object> requestBody = new Dictionary<string, object>();

//        requestBody.Add("width", ConfigData.config.reSizeWidth);
//        requestBody.Add("height", ConfigData.config.reSizeHeight);
//        requestBody.Add("steps", 20);
//        requestBody.Add("sampler_name", "DPM++ 2M Karras");
//        requestBody.Add("cfg_scale", 7.0);
//        requestBody.Add("init_images", new string[1] { _targetEncodeText });
//        requestBody.Add("denoising_strength", ConfigData.config.artyou_denoising);
//        requestBody.Add("save_images", true);
//        requestBody.Add("restore_faces", false);
//        requestBody.Add("clip_skip", 2);

//        Dictionary<string, object> args0 = new Dictionary<string, object>();
//        args0.Add("module", "softedge_pidinet");
//        args0.Add("model", "control_v11p_sd15_softedge");
//        args0.Add("pixel_perfect", true);
//        args0.Add("weight", 1);
//        args0.Add("control_mode", 2);
//        args0.Add("resize_mode", 2);
//        args0.Add("processor_res", 512);
//        args0.Add("threshold_a", 64);
//        args0.Add("threshold_b", 64);

//        Dictionary<string, object> args1 = new Dictionary<string, object>();
//        args1.Add("module", "openpose_hand");
//        args1.Add("model", "control_v11p_sd15_openpose");
//        args1.Add("pixel_perfect", true);
//        args1.Add("weight", 1);
//        args1.Add("control_mode", 2);
//        args1.Add("resize_mode", 2);
//        args1.Add("processor_res", 512);
//        args1.Add("threshold_a", 64);
//        args1.Add("threshold_b", 64);

//        Dictionary<string, object> args2 = new Dictionary<string, object>();
//        args2.Add("module", "tile_resample");
//        args2.Add("model", "control_v11f1e_sd15_tile");
//        args2.Add("pixel_perfect", true);
//        args2.Add("weight", 0.4);
//        args2.Add("control_mode", 2);
//        args2.Add("resize_mode", 2);
//        args2.Add("processor_res", -1);
//        args2.Add("threshold_a", 8);
//        args2.Add("threshold_b", 64);

//        List<Dictionary<string, object>> args = new List<Dictionary<string, object>>();
//        args.Add(args0);
//        args.Add(args1);
//        args.Add(args2);

//        Dictionary<string, object> controlnet = new Dictionary<string, object>();
//        controlnet.Add("args", args);

//        Dictionary<string, object> alwayson = new Dictionary<string, object>();
//        alwayson.Add("ControlNet", controlnet);

//        requestBody.Add("alwayson_scripts", alwayson);
//        requestBody.Add("prompt", tagger);
//        requestBody.Add("negative_prompt", ConfigData.config.negative_prompt);

//        string json = JsonConvert.SerializeObject(requestBody);
//        ApiCall.Instance.Img2Img(json, result => ShowResult(result));

//        //save json
//        requestBody["init_images"] = string.Empty;
//        string tempJson = JsonConvert.SerializeObject(requestBody);
//        //SaveJson("Artyou", tempJson);
//    }

//    void Control_Ref_Aniflat(string tagger)
//    {
//        Dictionary<string, object> requestBody = new Dictionary<string, object>();

//        requestBody.Add("width", ConfigData.config.reSizeWidth);
//        requestBody.Add("height", ConfigData.config.reSizeHeight);
//        requestBody.Add("steps", 20);
//        requestBody.Add("sampler_name", "DPM++ 2M Karras");
//        requestBody.Add("cfg_scale", 7.0);
//        requestBody.Add("init_images", new string[1] { _targetEncodeText });
//        requestBody.Add("denoising_strength", ConfigData.config.aniflat_denoising);
//        requestBody.Add("save_images", true);
//        requestBody.Add("restore_faces", false);
//        requestBody.Add("clip_skip", 2);

//        Dictionary<string, object> args0 = new Dictionary<string, object>();
//        args0.Add("module", "softedge_pidinet");
//        args0.Add("model", "control_v11p_sd15_softedge");
//        args0.Add("pixel_perfect", true);
//        args0.Add("weight", 1);
//        args0.Add("control_mode", 2);
//        args0.Add("resize_mode", 2);
//        args0.Add("processor_res", 512);
//        args0.Add("threshold_a", 64);
//        args0.Add("threshold_b", 64);

//        Dictionary<string, object> args1 = new Dictionary<string, object>();
//        args1.Add("module", "openpose_hand");
//        args1.Add("model", "control_v11p_sd15_openpose");
//        args1.Add("pixel_perfect", true);
//        args1.Add("weight", 1);
//        args1.Add("control_mode", 2);
//        args1.Add("resize_mode", 2);
//        args1.Add("processor_res", 512);
//        args1.Add("threshold_a", 64);
//        args1.Add("threshold_b", 64);

//        Dictionary<string, object> args2 = new Dictionary<string, object>();
//        args2.Add("module", "tile_resample");
//        args2.Add("model", "control_v11f1e_sd15_tile");
//        args2.Add("pixel_perfect", true);
//        args2.Add("weight", 0.4);
//        args2.Add("control_mode", 2);
//        args2.Add("resize_mode", 2);
//        args2.Add("processor_res", -1);
//        args2.Add("threshold_a", 8);
//        args2.Add("threshold_b", 64);

//        Dictionary<string, object> args3 = new Dictionary<string, object>();
//        args3.Add("input_image", _sourceEncodeText);
//        args3.Add("module", "reference_only");
//        args3.Add("pixel_perfect", true);
//        args3.Add("weight", 1);
//        args3.Add("control_mode", 0);
//        args3.Add("resize_mode", 2);
//        args3.Add("processor_res", -1);
//        args3.Add("threshold_a", 0.5);
//        args3.Add("threshold_b", 64);

//        List<Dictionary<string, object>> args = new List<Dictionary<string, object>>();
//        args.Add(args0);
//        args.Add(args1);
//        args.Add(args2);
//        args.Add(args3);

//        Dictionary<string, object> controlnet = new Dictionary<string, object>();
//        controlnet.Add("args", args);

//        Dictionary<string, object> alwayson = new Dictionary<string, object>();
//        alwayson.Add("ControlNet", controlnet);

//        requestBody.Add("alwayson_scripts", alwayson);
//        requestBody.Add("prompt", tagger);
//        requestBody.Add("negative_prompt", ConfigData.config.negative_prompt);

//        string json = JsonConvert.SerializeObject(requestBody);
//        ApiCall.Instance.Img2Img(json, result => ShowResult(result));

//        //save json
//        args3["input_image"] = string.Empty;
//        requestBody["init_images"] = string.Empty;
//        string tempJson = JsonConvert.SerializeObject(requestBody);
//        //SaveJson("Ref_Aniflat", tempJson);
//    }

//    void Control_Ref_Rev(string tagger)
//    {
//        Dictionary<string, object> requestBody = new Dictionary<string, object>();

//        requestBody.Add("width", ConfigData.config.reSizeWidth);
//        requestBody.Add("height", ConfigData.config.reSizeHeight);
//        requestBody.Add("steps", 20);
//        requestBody.Add("sampler_name", "DPM++ 2M Karras");
//        requestBody.Add("cfg_scale", 7.0);
//        requestBody.Add("init_images", new string[1] { _targetEncodeText });
//        requestBody.Add("denoising_strength", ConfigData.config.rev_denoising);
//        requestBody.Add("save_images", true);
//        requestBody.Add("restore_faces", false);
//        requestBody.Add("clip_skip", 2);

//        Dictionary<string, object> args0 = new Dictionary<string, object>();
//        args0.Add("module", "softedge_pidinet");
//        args0.Add("model", "control_v11p_sd15_softedge");
//        args0.Add("pixel_perfect", true);
//        args0.Add("weight", 1);
//        args0.Add("control_mode", 2);
//        args0.Add("resize_mode", 2);
//        args0.Add("processor_res", 512);
//        args0.Add("threshold_a", 64);
//        args0.Add("threshold_b", 64);

//        Dictionary<string, object> args1 = new Dictionary<string, object>();
//        args1.Add("module", "openpose_hand");
//        args1.Add("model", "control_v11p_sd15_openpose");
//        args1.Add("pixel_perfect", true);
//        args1.Add("weight", 1);
//        args1.Add("control_mode", 2);
//        args1.Add("resize_mode", 2);
//        args1.Add("processor_res", 512);
//        args1.Add("threshold_a", 64);
//        args1.Add("threshold_b", 64);

//        Dictionary<string, object> args2 = new Dictionary<string, object>();
//        args2.Add("module", "tile_resample");
//        args2.Add("model", "control_v11f1e_sd15_tile");
//        args2.Add("pixel_perfect", true);
//        args2.Add("weight", 0.4);
//        args2.Add("control_mode", 2);
//        args2.Add("resize_mode", 2);
//        args2.Add("processor_res", -1);
//        args2.Add("threshold_a", 8);
//        args2.Add("threshold_b", 64);

//        Dictionary<string, object> args3 = new Dictionary<string, object>();
//        args3.Add("input_image", _sourceEncodeText);
//        args3.Add("module", "reference_only");
//        args3.Add("pixel_perfect", true);
//        args3.Add("weight", 1);
//        args3.Add("control_mode", 0);
//        args3.Add("resize_mode", 2);
//        args3.Add("processor_res", -1);
//        args3.Add("threshold_a", 0.5);
//        args3.Add("threshold_b", 64);

//        List<Dictionary<string, object>> args = new List<Dictionary<string, object>>();
//        args.Add(args0);
//        args.Add(args1);
//        args.Add(args2);
//        args.Add(args3);

//        Dictionary<string, object> controlnet = new Dictionary<string, object>();
//        controlnet.Add("args", args);

//        Dictionary<string, object> alwayson = new Dictionary<string, object>();
//        alwayson.Add("ControlNet", controlnet);

//        requestBody.Add("alwayson_scripts", alwayson);
//        requestBody.Add("prompt", tagger);
//        requestBody.Add("negative_prompt", ConfigData.config.negative_prompt);

//        string json = JsonConvert.SerializeObject(requestBody);
//        ApiCall.Instance.Img2Img(json, result => ShowResult(result));

//        //save json
//        args3["input_image"] = string.Empty;
//        requestBody["init_images"] = string.Empty;
//        string tempJson = JsonConvert.SerializeObject(requestBody);
//        //SaveJson("Ref_Rev", tempJson);
//    }

//    void SaveJson(string name, string json)
//    {
//        string path = Path.Combine(Directory.GetParent(Application.dataPath).FullName, name + ".json");
//        File.WriteAllText(path, json);
//    }
//}
