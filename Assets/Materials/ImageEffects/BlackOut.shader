﻿Shader "Glitch/BlackOut"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Intensity ("Intensity", Range(0,1)) = 0
		_Factor1 ("Factor 1", float) = 1
        _Factor2 ("Factor 2", float) = 1
        _Factor3 ("Factor 3", float) = 1
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _Intensity;
			float _Factor1 = 1;
            float _Factor2 = 1;
            float _Factor3 = 1;
			float timeOffset = 0;

			float noise(float2 uv)
            {
                return frac(sin(dot(uv, float2(_Factor1, _Factor2))) * (_Factor3 + timeOffset));
            }

			fixed4 frag (v2f i) : SV_Target
			{
				timeOffset = _Time * 20;
				fixed4 col = tex2D(_MainTex, i.uv);
				float4 frameStrength = (noise(float2(3.1415f,2.736f)) + noise(float2(0.2415f,0.636f))) / 2;
				float w = frameStrength > _Intensity ? 0 : 1;
				return lerp(col,float4(0,0,0,1),w);
			}
			ENDCG
		}
	}
}
