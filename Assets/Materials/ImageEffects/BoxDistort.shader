Shader "Glitch/BoxDistort"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BlockNoise ("Block Noise", 2D) = "white" {}
		_Intensity ("Intensity", Range(0,1)) = 0
		_SampleOffset ("Sample Offset", Vector) = (0,0,0,0)
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
			sampler2D _BlockNoise;
			float _Intensity;
			float4 _SampleOffset;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 noiseuv = i.uv + _SampleOffset.xy;
				float2 uvoffset = tex2D(_BlockNoise, noiseuv).xy; //sample noise form blocknoise texture
				uvoffset = (uvoffset * 2)-0.6; //change range fom 0to1 to -1to1
				uvoffset *= _Intensity; //aply intensity
				uvoffset += i.uv; //add to uv coordinats
				float4 col = tex2D(_MainTex, uvoffset); //sample texture form new coordinates
				return col;
			}
			ENDCG
		}
	}
}
