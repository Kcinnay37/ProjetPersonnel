Shader "Cainos/Customizable Pixel Character/Hair"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}

		_RampTex ("Ramp Texture", 2D) = "white" {}
		_RampPower ("Ramp Power" , Float) = 1.0

		_SkinShadeTex("Skin Shade Texture",2D) = "black"{}
		_SkinShadeColor("Skin Shade Color" ,Color) = ( 0.68, 0.52,0.40,1.0)
	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType" = "Plane"
		}

		Cull Off
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
				#pragma vertex Vert
				#pragma fragment Frag
				#pragma target 2.0
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				struct appdata_t
		{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float2 texcoord : TEXCOORD0;
					UNITY_VERTEX_OUTPUT_STEREO
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				sampler2D _RampTex;
				float _RampPower;

				sampler2D _SkinShadeTex;
				float4 _SkinShadeColor;

				float4 _Tint;

				v2f Vert (appdata_t v)
				{
					v2f OUT;

					UNITY_SETUP_INSTANCE_ID(OUT);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

					OUT.vertex = UnityObjectToClipPos(v.vertex);
					OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

					return OUT;
				}

				fixed4 Frag (v2f i) : SV_Target
				{
					float4 f4Color = tex2D(_MainTex, i.texcoord);
					clip(f4Color.a - 0.01f);

					if ( abs(f4Color.r - f4Color.g) < 0.001 && abs(f4Color.r - f4Color.b) < 0.001 )
					{
						//float fGrayscale = dot(f4Color.rgb, float3(0.299, 0.587, 0.114));
						float fGrayscale = Luminance(f4Color.rgb);

						fGrayscale = pow(fGrayscale, _RampPower);
						f4Color.rgb = tex2D(_RampTex, fGrayscale);

						float fSkinShade = tex2D(_SkinShadeTex, i.texcoord).r;
						f4Color.rgb = lerp( f4Color.rgb, _SkinShadeColor.rgb , fSkinShade * _SkinShadeColor.a);
					}

					return f4Color;
				}
			ENDCG
		}
	}

}
