Shader "Nexweron/Builtin/Lut/Unlit_Lut2D"
{
    
    Properties {
    	_BaseColor("BaseColor", Color) = (1,1,1,1)
        [PerRendererData] _MainTex("MainTex", 2D) = "white" {}
	    [NoScaleOffset]
        _LutTex("LutTex", 2D) = "white" {}
        
        [KeywordEnum(Direct, Inverse)][Tooltip(LutTex Green channel direction)]
        _LutGreenDir("LutGreenDir", Float) = 0
        
        _LutContribution("LutContribution", range(0, 1)) = 1.0

        // ui.mask
        _StencilComp ("Stencil Comparison", Float) = 8.000000
        _Stencil ("Stencil ID", Float) = 0.000000
        _StencilOp ("Stencil Operation", Float) = 0.000000
        _StencilWriteMask ("Stencil Write Mask", Float) = 255.000000
        _StencilReadMask ("Stencil Read Mask", Float) = 255.000000
        _ColorMask ("Color Mask", Float) = 15.000000
        [PerRendererData] _SoftMask ("Mask", 2D) = "white" {}

        // Bilateral Filter
        [Toggle] _Enable("Enable",Float) = 0
        _KernelSize("Kernel Size", int) = 21
        _SigmaS("Sigma S", float) = 50
        _SigmaB("Sigma B", float) = 20
    }
	
	CGINCLUDE
    #include "UnityCG.cginc"
    #include "UnityUI.cginc"
	#include "Lut.hlsl"
    #include "Packages/com.olegknyazev.softmask/Assets/Shaders/Resources/SoftMask.cginc"
	#pragma multi_compile __ UNITY_UI_ALPHACLIP
    #pragma multi_compile __ SOFTMASK_SIMPLE SOFTMASK_SLICED SOFTMASK_TILED

    #define PI 3.141592

	sampler2D _MainTex;
    float4 _MainTex_ST;
    float4 _MainTex_TexelSize;
	float4 _BaseColor;
    sampler2D _LutTex;
    float4 _LutTex_TexelSize;
    float _LutContribution;
    float4 _ClipRect;
    float _Enable;
    int _KernelSize;
    float _SigmaS;
    float _SigmaB;
	
    struct appdata_t
    {
        float4 vertex : POSITION;
        float2 texcoord : TEXCOORD0;
    };

    struct v2f {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;     
        float4 worldPosition : TEXCOORD1;
        SOFTMASK_COORDS(2)   
    };

	v2f vert (appdata_t v) {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
        SOFTMASK_CALCULATE_COORDS(o, v.vertex);
        return o;
    }

    float SpatialGaussian(int i, int j, float sigma_s)
    {
        float r = sqrt(i*i + j*j);

        if (r > _KernelSize/2)
            return 0.0f;

        float spatial_coeff = -0.5 / (sigma_s * sigma_s);
        float spatial_weight = exp(r*r*spatial_coeff);
        return spatial_weight;
    }

    float BrightnessGaussian(float k, float sigma_b)
    {
        float color_coeff = -0.5 / (sigma_b * sigma_b);
        float color_weight = exp(k*k*color_coeff);
        return color_weight;
    }


    float4 BilateralFilter(float2 uv, float sigma_s, float sigma_b)
    {
        float4 output = float4(0.0, 0.0, 0.0, 0.0);
        float2 uvOffset = uv;
        float2 uvCurrent;
        float4 currentColor;
        float4 colorDiff;
        float k;
        float weight_s;
        float weight_b;
        float wSum = 0;
        float dx = _MainTex_TexelSize.x;
        float dy = _MainTex_TexelSize.y;

        for (int i = -_KernelSize; i <= _KernelSize; i++)
            for (int j = -_KernelSize; j <= _KernelSize; j++)
            {
                uvCurrent.x = uvOffset.x + (i * dx); // m, n
                uvCurrent.y = uvOffset.y + (j * dy);

                currentColor = tex2D(_MainTex, uvCurrent) * 255.0f; // 현재 색상

                weight_s = SpatialGaussian(i, j, sigma_s);

                colorDiff = currentColor - tex2D(_MainTex, uvOffset) * 255.0f;
                k = abs(colorDiff.x) + abs(colorDiff.y) + abs(colorDiff.z);
                weight_b = BrightnessGaussian(k, sigma_b);
    
                output += currentColor * weight_s * weight_b;
                wSum += (weight_s * weight_b);
            }

        output *= (1.0f / wSum);
        output /= 255.0f;

        return output;
    }
	
    float4 frag (v2f i) : SV_Target {
        float4 ci;
        if(_Enable == 0) 
        {
            ci = tex2D(_MainTex, i.uv);
        }
        else 
        {
            ci = BilateralFilter(i.uv, _SigmaS, _SigmaB);
        }
        
        float4 c = ci;
    	
        #ifdef _LUTGREENDIR_INVERSE
            c.rgb = ApplyLut2D(c.rgb, _LutTex, _LutTex_TexelSize.w, true);
        #else
            c.rgb = ApplyLut2D(c.rgb, _LutTex, _LutTex_TexelSize.w);
        #endif        

    	c = lerp(ci, c, _LutContribution)*_BaseColor;
    	c.a *= SOFTMASK_GET_MASK(i); // Soft Mask Support
        c.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
        return c;
    }
    ENDCG
    
    SubShader {
		Lighting Off
		AlphaTest Off
		Blend SrcAlpha OneMinusSrcAlpha, One Zero
        Tags { "Queue" = "Transparent" }

		Pass {
			CGPROGRAM
			    #pragma shader_feature _LUTGREENDIR_DIRECT _LUTGREENDIR_INVERSE
			    #pragma vertex vert
				#pragma fragment frag
                
			ENDCG

        Stencil {
        Ref[_Stencil]
        Comp[_StencilComp]
        Pass [_StencilOp]
        ReadMask[_StencilReadMask]
        WriteMask[_StencilWriteMask]
            }
		}
	}
	Fallback Off
}
