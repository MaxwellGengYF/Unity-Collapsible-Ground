Shader "Hidden/SnowWorldPos"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			float4 _WorldYPos;
			sampler2D _PlaneHeightMap;
			sampler2D _CompareTex;
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD2;
				float worldPos : TEXCOORD3;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).y;
				return o;
			}
			
			float frag (v2f i) : SV_Target
			{
				float2 screenUV = i.screenPos.xy / i.screenPos.w;
				float worldHeight = (_WorldYPos.z - (tex2D(_PlaneHeightMap, float2(1-screenUV.x,screenUV.y)).r - _WorldYPos.y) * _WorldYPos.x - i.worldPos);
				worldHeight = saturate(worldHeight * 0.5);
				clip(worldHeight - tex2D(_CompareTex, screenUV).r);
				return worldHeight;
			}
			ENDCG
		}
	}
}