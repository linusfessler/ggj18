Shader "Glitch/ColorShift"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ArtifactNoise ("ArtifactNoise", 2D) = "white"{}
		_Intensity ("Intensity", Range(0,0.2)) = 0
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
			sampler2D _ArtifactNoise;
			float _Intensity;

			fixed4 frag (v2f i) : SV_Target
			{
				//sample noise Texture
				float2 noiseuv = i.uv;
				noiseuv.x = noiseuv.x / 8 - _Time * 0.5 * _Intensity * 50; //adjust flicker here
				noiseuv.y = noiseuv.y / 5 + _Time * 4 * _Intensity * 10; //adjust vertical speed here
				noiseuv % 1;
				float4 stripNoise = tex2D(_ArtifactNoise,noiseuv);
				stripNoise = ((stripNoise * 2) - 1) * _Intensity;
				float2 modifier = float2(1.5,0.8);

				//sample red
				float2 texelSample = (i.uv + stripNoise.rg * modifier) % 1;
				float red = tex2D(_MainTex, texelSample).r;
				//sample green
				texelSample = (i.uv + stripNoise.gb * modifier) % 1;
				float green = tex2D(_MainTex, texelSample).g;
				//sample blue
				texelSample = (i.uv + stripNoise.br * modifier) % 1;
				float blue = tex2D(_MainTex, texelSample).b ;
				
				fixed4 col = tex2D(_MainTex, i.uv);
				// just invert the colors
				col.rgb = 1 - col.rgb;
				//return stripNoise;
				return float4(red,green,blue,1);
			}
			ENDCG
		}
	}
}
