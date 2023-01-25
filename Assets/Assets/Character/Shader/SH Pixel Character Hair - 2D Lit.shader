Shader "Cainos/Customizable Pixel Character/Hair - 2D Lit"
{
    Properties
    {
        _MainTex("Diffuse", 2D) = "white" {}

		_RampTex("Ramp Texture", 2D) = "white" {}
		_RampPower("Ramp Power" , Float) = 1.0

		_SkinShadeTex("Skin Shade Texture",2D) = "black"{}
		_SkinShadeColor("Skin Shade Color" ,Color) = (0.68, 0.52,0.40,1.0)

        _MaskTex("Mask", 2D) = "white" {}

        // Legacy properties. They're here so that materials using this shader can gracefully fallback to the legacy sprite shader.
        [HideInInspector] _Color("Tint", Color) = (1,1,1,1)
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _AlphaTex("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha("Enable External Alpha", Float) = 0
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    ENDHLSL

    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "PreviewType" = "Plane" }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite On

        Pass
        {
            Tags { "LightMode" = "Universal2D" }
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma vertex CombinedShapeLightVertex
            #pragma fragment CombinedShapeLightFragment
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2  uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4  positionCS  : SV_POSITION;
                float4  color       : COLOR;
                float2	uv          : TEXCOORD0;
                float2	lightingUV  : TEXCOORD1;
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);
			TEXTURE2D(_RampTex);
			SAMPLER(sampler_RampTex);
			TEXTURE2D(_SkinShadeTex);
			SAMPLER(sampler_SkinShadeTex);

            half4 _MainTex_ST;
            half4 _NormalMap_ST;
			float _RampPower;
			float4 _SkinShadeColor;

            #if USE_SHAPE_LIGHT_TYPE_0
            SHAPE_LIGHT(0)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_1
            SHAPE_LIGHT(1)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_2
            SHAPE_LIGHT(2)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_3
            SHAPE_LIGHT(3)
            #endif

            Varyings CombinedShapeLightVertex(Attributes v)
            {
                Varyings o = (Varyings)0;

                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                float4 clipVertex = o.positionCS / o.positionCS.w;
                o.lightingUV = ComputeScreenPos(clipVertex).xy;
                o.color = v.color;
                return o;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

            half4 CombinedShapeLightFragment(Varyings i) : SV_Target
            {
				float4 f4Color = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
				clip(f4Color.a - 0.01f);

				if (abs(f4Color.r - f4Color.g) < 0.001 && abs(f4Color.r - f4Color.b) < 0.001)
				{
					float fGrayscale = dot(f4Color.rgb, float3(0.299, 0.587, 0.114));

					fGrayscale = pow(abs(fGrayscale), _RampPower);
					f4Color.rgb = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, fGrayscale).rgb;

					float fSkinShade = SAMPLE_TEXTURE2D(_SkinShadeTex, sampler_SkinShadeTex, i.uv).r;
					f4Color.rgb = lerp(f4Color.rgb, _SkinShadeColor.rgb, fSkinShade * _SkinShadeColor.a);
				}


                half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);

                return CombinedShapeLightShared(f4Color, mask, i.lightingUV);
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" "Queue"="Transparent" "RenderType"="Transparent"}

            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color		: COLOR;
                float2 uv			: TEXCOORD0;
            };

            struct Varyings
            {
                float4  positionCS		: SV_POSITION;
                float4  color			: COLOR;
                float2	uv				: TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
			TEXTURE2D(_SkinMaskTex);
			SAMPLER(sampler_SkinMaskTex);
			TEXTURE2D(_RampTex);
			SAMPLER(sampler_RampTex);
			TEXTURE2D(_SkinShadeTex);
			SAMPLER(sampler_SkinShadeTex);

			float4 _MainTex_ST;
			float4 _SkinMaskTex_ST;
			float4 _SkinTint;
			float _RampPower;
			float4 _SkinShadeColor;


            Varyings UnlitVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;

                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                o.uv = TRANSFORM_TEX(attributes.uv, _MainTex);
                o.uv = attributes.uv;
                o.color = attributes.color;
                return o;
            }

            float4 UnlitFragment(Varyings i) : SV_Target
            {
				float4 f4Color = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
				clip(f4Color.a - 0.01f);

				if (abs(f4Color.r - f4Color.g) < 0.001 && abs(f4Color.r - f4Color.b) < 0.001)
				{
					float fGrayscale = dot(f4Color.rgb, float3(0.299, 0.587, 0.114));

					fGrayscale = pow(abs(fGrayscale), _RampPower);
					f4Color.rgb = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, fGrayscale).rgb;

					float fSkinShade = SAMPLE_TEXTURE2D(_SkinShadeTex, sampler_SkinShadeTex, i.uv).r;
					f4Color.rgb = lerp(f4Color.rgb, _SkinShadeColor.rgb, fSkinShade * _SkinShadeColor.a);
				}

				return f4Color;
            }
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}
