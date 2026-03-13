Shader "Custom/SpriteRadialPulse"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _PulseColor ("Pulse Color", Color) = (1,1,1,1)
        _PulseIntensity ("Pulse Intensity", Range(0, 2)) = 1
        _PulseCount ("Pulse Count", Range(0.25, 12)) = 2
        _PulseWidth ("Pulse Width", Range(0.001, 0.5)) = 0.08
        _PulseSoftness ("Pulse Softness", Range(0.001, 0.25)) = 0.03
        _PulseSpeed ("Pulse Speed", Range(0, 6)) = 1.2
        _PhaseOffset ("Phase Offset", Float) = 0
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

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _PulseColor;
            float _PulseIntensity;
            float _PulseCount;
            float _PulseWidth;
            float _PulseSoftness;
            float _PulseSpeed;
            float _PhaseOffset;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 texCol = tex2D(_MainTex, i.uv);

                float2 centered = i.uv - 0.5;
                float radius = saturate(length(centered) * 2.0);

                float phase = frac(radius * _PulseCount - (_Time.y * _PulseSpeed + _PhaseOffset));
                float wrapped = min(phase, 1.0 - phase);
                float ring = 1.0 - smoothstep(_PulseWidth, _PulseWidth + _PulseSoftness, wrapped);

                float baseAlpha = texCol.a * i.color.a;
                float pulseMask = saturate(ring * _PulseIntensity);
                float pulseAlpha = pulseMask * texCol.a;

                // Keep pulse visible even when the sprite base is made transparent.
                float outAlpha = saturate(max(baseAlpha, pulseAlpha));
                float3 baseRgb = (texCol.rgb * i.color.rgb) * baseAlpha;
                float3 pulseRgb = _PulseColor.rgb * pulseAlpha;

                fixed4 col;
                col.rgb = baseRgb + pulseRgb;
                col.a = outAlpha;
                return col;
            }
            ENDCG
        }
    }
}