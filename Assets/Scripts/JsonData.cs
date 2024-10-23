using System;
using System.Collections.Generic;

[Serializable]
public class CaricatureRequestData
{
    public string menu_code;
    public string encoded_source_image;
    public int gender_index;
}

[Serializable]
public class CartoonRequestData
{
    public string menu_code;
    public string encoded_source_image;
}

[Serializable]
public class ProfileRequestData
{
    public string menu_code;
    public string encoded_source_image;
    public int image_index;
}

[Serializable]
public class APIResponse
{
    public string images;
}

[Serializable]
public class Img2ImgResponse
{
    public string[] images;
    public string msg;
    public string type;
}

[Serializable]
public class TaggerRequest
{
    public string model = "wd14-vit-v2-git";
    public float threshold = 0.35f;
    public string image;
}

[Serializable]
public class ModelRequest
{
    public string sd_model_checkpoint = "abyssorangemix3AOM3_aom3a1b.safetensors";
}

#region cartoon model Request Body

[Serializable]
public class CartoonRequestBody
{
    public string prompt;
    public string negative_prompt;
    public string[] styles;
    public int seed;
    public int subseed;
    public float subseed_strength;
    public int seed_resize_from_h;
    public int seed_resize_from_w;
    public string sampler_name;
    public int batch_size;
    public int n_iter;
    public int steps;
    public float cfg_scale;
    public int width;
    public int height;
    public bool restore_faces;
    public bool tiling;
    public bool do_not_save_samples;
    public bool do_not_save_grid;
    public int eta;
    public float denoising_strength;
    public int s_min_uncond;
    public int s_churn;
    public int s_tmax;
    public int s_tmin;
    public int s_noise;
    public Dictionary<string, object> override_settings;
    public bool override_settings_restore_afterwards;
    public string refiner_checkpoint;
    public int refiner_switch_at;
    public bool disable_extra_networks;
    public Dictionary<string, object> comments;
    public string[] init_images;
    public int resize_mode;
    public int image_cfg_scale;
    public string mask;
    public int mask_blur_x;
    public int mask_blur_y;
    public int mask_blur;
    public int inpainting_fill;
    public bool inpaint_full_res;
    public int inpaint_full_res_padding;
    public int inpainting_mask_invert;
    public int initial_noise_multiplier;
    public string latent_mask;
    public string sampler_index;
    public bool include_init_images;
    public string script_name;
    public string[] script_args;
    public bool send_images;
    public bool save_images;
    public Dictionary<string, object> alwayson_scripts;
}

#endregion