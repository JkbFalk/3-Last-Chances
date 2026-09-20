Shader "UI/GradientBarShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _FillAmount ("Fill Amount", Range(0, 1)) = 0.5
        _FadeWidth ("Fade Width", Range(0, 1)) = 0.1

        // Wave Properties
        _WaveTex ("Wave Texture", 2D) = "white" {}
        _WaveSpeed ("Wave Speed", Float) = 0.5
        _WaveScale ("Wave Scale", Float) = 0.5 // NEW: Controls color width (0.5 = twice as wide)
        _Brightness ("Brightness", Float) = 1.5
        _Pulse ("Pulse", Float) = 0.0

        // Required UI Properties
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _WaveTex;
            fixed4 _Color;
            float _FillAmount;
            float _FadeWidth;
            float _WaveSpeed;
            float _WaveScale;
            float _Brightness;
            float _Pulse;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 baseColor = tex2D(_MainTex, IN.texcoord);
                
                // MULTIPLY X BY _WaveScale to stretch the colors
                float2 waveUV = float2(IN.texcoord.x * _WaveScale - _Time.y * _WaveSpeed, 0.5);
                fixed4 waveColor = tex2D(_WaveTex, waveUV);
                
                fixed3 finalRGB = baseColor.rgb * waveColor.rgb * _Brightness;
                finalRGB += waveColor.rgb * _Pulse;
                
                fixed4 color = fixed4(finalRGB, baseColor.a) * IN.color;
                
                float fade = saturate((_FillAmount - IN.texcoord.x) / max(_FadeWidth, 0.0001));
                color.a *= fade;
                
                return color;
            }
            ENDCG
        }
    }
}