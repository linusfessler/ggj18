Shader "Hidden/BarrelDistrortion"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
			
			float2 Distort(float2 p)
			{
				p*=1.01;
				float theta  = atan2(p.y, p.x);
				float radius = length(p);
				radius = pow(radius, 1.3);
				p.x = radius * cos(theta);
				p.y = radius * sin(theta);
				return 0.5f * (p + 1);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uvOffset = i.uv * 2 - 1;
				float2 uv = Distort(uvOffset);
				float black = 1;
				if (uv.x < 0 || uv.x > 1){
					black = 0;
				}
				if (uv.y < 0 || uv.y > 1){
					black = 0;
				}
				fixed4 col = tex2D(_MainTex, uv);
				return col * black;
			}
			ENDCG
		}
	}
}
