Shader "Custom/SpriteEdgeWobble"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Amplitude ("Edge Wobble Amplitude", Range(0, 0.2)) = 0.04
        _Frequency ("Edge Wobble Frequency", Range(0, 30)) = 6
        _Speed ("Edge Wobble Speed", Range(0, 5)) = 1.0
        _Softness ("Edge Softness", Range(0.001, 0.05)) = 0.006
        _TopEdgeFrequencyMultiplier ("Top Edge Frequency Multiplier", Range(0, 3)) = 1
        _BottomEdgeFrequencyMultiplier ("Bottom Edge Frequency Multiplier", Range(0, 3)) = 1
        _LeftEdgeFrequencyMultiplier ("Left Edge Frequency Multiplier", Range(0, 3)) = 1
        _RightEdgeFrequencyMultiplier ("Right Edge Frequency Multiplier", Range(0, 3)) = 1
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
            float _Amplitude;
            float _Frequency;
            float _Speed;
            float _Softness;
            float _TopEdgeFrequencyMultiplier;
            float _BottomEdgeFrequencyMultiplier;
            float _LeftEdgeFrequencyMultiplier;
            float _RightEdgeFrequencyMultiplier;
            float _PhaseOffset;

            // Layered harmonics: each edge gets multiple overlapping sine waves
            // at different speeds/frequencies so the result looks organic, not patterned.
            float sketchEdge(float pos, float t, float freq, float amp, float seed)
            {
                float w  = sin(pos * freq          + t * 1.00 + seed)        * 0.45;
                w       += sin(pos * freq * 2.13   + t * 1.63 + seed * 1.7)  * 0.28;
                w       += sin(pos * freq * 3.79   + t * 0.81 + seed * 3.1)  * 0.16;
                w       += sin(pos * freq * 0.53   + t * 0.44 + seed * 0.5)  * 0.22;
                w       += sin(pos * freq * 5.31   + t * 2.17 + seed * 2.6)  * 0.09;
                return w * amp;
            }

            v2f vert(appdata v)
            {
                    // Expand the quad so there are actual pixels outside the original
                    // sprite boundary for the wave to push into.
                    v.vertex.xy *= (1.0 + 3.0 * _Amplitude);
                    v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float t    = _Time.y * _Speed + _PhaseOffset;
                float freq = _Frequency;
                float amp  = _Amplitude;

                // Compensate for non-uniform object scale so all four edges remain visible
                // even when a square sprite is stretched into a long platform.
                float scaleX = length(float3(unity_ObjectToWorld._m00, unity_ObjectToWorld._m10, unity_ObjectToWorld._m20));
                float scaleY = length(float3(unity_ObjectToWorld._m01, unity_ObjectToWorld._m11, unity_ObjectToWorld._m21));
                float horizontalEdgeAmp = amp * clamp(scaleX / max(scaleY, 0.0001), 1.0, 8.0);
                float verticalEdgeAmp = amp * clamp(scaleY / max(scaleX, 0.0001), 1.0, 8.0);
                float maxAmp = max(horizontalEdgeAmp, verticalEdgeAmp);
                float topFreq = freq * _TopEdgeFrequencyMultiplier;
                float bottomFreq = freq * _BottomEdgeFrequencyMultiplier;
                float leftFreq = freq * _LeftEdgeFrequencyMultiplier;
                float rightFreq = freq * _RightEdgeFrequencyMultiplier;

                 // Remap UV [0,1] to [-1.5*amp, 1+1.5*amp] to cover the expanded quad.
                 // edgeUV=0 is the original sprite's left/bottom edge, 1 is right/top.
                float2 edgeUV = i.uv * (1.0 + 3.0 * maxAmp) - (1.5 * maxAmp);

                 // Each edge has a unique seed so they never move in sync.
                float waveBottom = sketchEdge(edgeUV.x, t, bottomFreq, horizontalEdgeAmp, 0.00);
                float waveTop    = sketchEdge(edgeUV.x, t, topFreq, horizontalEdgeAmp, 7.39);
                float waveLeft   = sketchEdge(edgeUV.y, t, leftFreq, verticalEdgeAmp, 3.71);
                float waveRight  = sketchEdge(edgeUV.y, t, rightFreq, verticalEdgeAmp, 11.53);

                 // Signed distance from each wobbly edge.
                 // Positive wave = cuts in, negative wave = bulges out.
                 float dBottom = edgeUV.y         - waveBottom;
                 float dTop    = (1.0 - edgeUV.y) - waveTop;
                 float dLeft   = edgeUV.x         - waveLeft;
                 float dRight  = (1.0 - edgeUV.x) - waveRight;

                 float edgeDist  = min(min(dBottom, dTop), min(dLeft, dRight));
                 float edgeAlpha = smoothstep(-_Softness, _Softness, edgeDist);

                 // Clamp so outward pixels sample the sprite's edge color.
                 float2 sampleUV = clamp(edgeUV, 0.0, 1.0);
                 fixed4 col = tex2D(_MainTex, sampleUV) * i.color;
                 col.a   *= edgeAlpha;
                 col.rgb *= col.a;
                 return col;
            }
            ENDCG
        }
    }
}
