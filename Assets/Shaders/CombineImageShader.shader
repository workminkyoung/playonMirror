Shader "Custom/CombineImagesShader"
{
    Properties
    {
        _BackgroundTex ("Background Texture", 2D) = "white" {}
        _PersonTex ("Person Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _BackgroundTex;
            sampler2D _PersonTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 bg = tex2D(_BackgroundTex, i.uv);
                fixed4 person = tex2D(_PersonTex, i.uv);
                fixed4 result = lerp(bg, person, person.a);
                return result;
            }
            ENDCG
        }
    }
}
