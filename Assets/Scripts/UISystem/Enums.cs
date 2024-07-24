
namespace Vivestudios.UI
{
    public enum PAGE_TYPE
    {
        PAGE_AOD,
        PAGE_SELECT_CONTENT,
        PAGE_SELECT_CARTOON_STYLE,
        PAGE_SELECT_AI_PROFILE,
        PAGE_SELECT_FRAME,
        PAGE_PAYMENT,
        PAGE_CAUTION,
        PAGE_GLOBAL,
        //after paying
        PAGE_SHOOT_CARTOON,
        PAGE_SHOOT_PROFILE,
        PAGE_SHOOT_BEAUTY,

        PAGE_SELECT_AI_PROFILE_RESULT,
        PAGE_LOADING,
        PAGE_SELECT_RESULT,
        PAGE_PRINT,
        PAGE_END,

        PAGE_DECO_SELECT_PICS_CARTOON,
        PAGE_DECO_SELECT_PICS_PROFILE,
        PAGE_DECO_SELECT_PICS_BEAUTY,
        PAGE_DECO_SELECT_EFFECT,
        PAGE_DECO_SELECT_FRAME,

        PAGE_SELECT_WHAT_IF,
        PAGE_DECO_SELECT_PICS_WHAT_IF,
        PAGE_SHOOT_WHAT_IF
    }

    public enum CARTOON_TYPE
    {
        CA00001,
        CA00002,
        CA00003,
        CA00004,
        CA00005,

        END
    }

    public enum PROFILE_TYPE
    {
        PR00001,
        PR00002,
        PR00003,
        PR00004,
        PR00005,
        PR00006,
        PR00007,
        PR00008,
        PR00009,
        PR00010,
        PR00011,
        PR00012,
        PR00013,
        PR00014,

        END
    }

    public enum PHOTO_TYPE
    {
        NONE = 2,
        CONVERTED = 0,
        REAL = 1
    }
}

public enum CONTENT_TYPE
{
    //NONE,//??
    AI_CARTOON,
    AI_PROFILE,
    AI_TIME_MACHINE,
    AI_BEAUTY,
    WHAT_IF
}

public enum FRAME_TYPE
{
    FRAME_4,
    FRAME_8,
    FRAME_2,
    FRAME_1,

    FRAME_2_1,
    FRAME_2_2,
}

public enum TCP_STRING_TYPE
{
    TCP_SEND_OPEN_CAMERA,
    TCP_SEND_CLOSE_CAMERA,
}

public enum FRAME_SIDE
{
    LEFT,
    RIGHT
}

public enum FRAME_RATIO_TYPE
{
    HORIZONTAL,
    VERTICAL
}

public enum FRAME_COLOR_TYPE
{
    FRAME_WHITE,
    FRAME_BLACK,
    
    //######################크리스마스 이벤트
    FRAME_GREENNIT,
    FRAME_RED,
    FRAME_SNOW,
    //######################크리스마스 이벤트

    FRAME_INK,
    FRAME_LIMEYELLOW,
    FRAME_SKYBLUE,
    FRAME_GREEN,

    FRAME_JTBC_WH,
    FRAME_JTBC_BL,
    FRAME_JTBC_SI,
}

public enum LUT_EFFECT_TYPE
{
    LUT_DEFAULT,
    LUT_COOL,
    LUT_BRIGHT,
    LUT_GRAYSCALE
}

public enum RECORD_CAMERA_TEXTURE
{
    PORTRAIT = 0,
    LANDSCAPE,
    SPOUT
}

public enum CAMERA_TYPE
{
    WEBCAM = 0,
    NDI,
    DSLR
}

public enum AUDIO
{
    CAMERA = 0,
    COUNT,
    TOUCH
}

public enum MAIL_TYPE
{
    REMAIN_PAPER,
    RESET_PAPER,
    ERROR,
    PAYMENT_ERROR,
    DIFFUSION_ERROR,
    QR_ERROR
}

public enum POLICY_TYPE
{
    PRIVACY_POLICY,
    TERMS_OF_USE
}

public enum GENDER_TYPE
{
    MALE,
    FEMALE,
}

public enum CAMERA_VIEW_TYPE
{
    UI,
    CAPTURE,
    RECORD
}