Shader "Hidden/SnowNormal"
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
			sampler2D _CompareTex;
			sampler2D _PlaneHeightMap;
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float worldPos : TEXCOORD1;
				float3 normal : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).y;
				o.screenPos = ComputeScreenPos(o.vertex);
				o.normal = v.normal;
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float2 screenUV = i.screenPos.xy / i.screenPos.w;
				float worldHeight = saturate(_WorldYPos.z + (tex2D(_PlaneHeightMap, screenUV).r - _WorldYPos.y) * _WorldYPos.x - i.worldPos);
				clip(worldHeight - (tex2D(_CompareTex, screenUV)).r);
				return float4(-i.normal * 0.5 + 0.5, 1);
			}
			ENDCG
		}
	}
}
