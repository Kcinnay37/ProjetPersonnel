Shader "Cainos/Customizable Pixel Character/Body"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_SkinMaskTex("Skin Mask Texture", 2D) = "white" {}
		_SkinTint("Skin Tint", Color) = (1.0,1.0,1.0,1.0)
	}

	SubShader
	{
		Tags
		{
			"Queue"="AlphaTest"
			"IgnoreProjector"="True"
			"RenderType"="TransparentCutout"
			"PreviewType" = "Plane"
		}
		Cull Off
		ZWrite On

		Pass
		{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
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
				sampler2D _SkinMaskTex;

				float4 _MainTex_ST;

				float4 _SkinTint;

				v2f vert (appdata_t v)
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					return o;
				}

				fixed4 frag (v2f i) : SV_Target
				{
					float4 f4Color = tex2D(_MainTex, i.texcoord);
					float fMask = tex2D(_SkinMaskTex, i.texcoord).a;

					clip(f4Color.a - 0.95f);

					f4Color.rgb = lerp(f4Color.rgb, f4Color.rgb * _SkinTint.rgb, fMask);

					return f4Color;
				}
			ENDCG
		}
	}

}
