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
			float _WorldYPos;
			sampler2D _CompareTex;
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
				o.worldPos =  saturate(_WorldYPos - mul(unity_ObjectToWorld, v.vertex).y);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.normal = v.normal;
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float height = (tex2D(_CompareTex, i.screenPos)).r;
				clip(i.worldPos - height);
				return float4(-i.normal * 0.5 + 0.5, 1);
			}
			ENDCG
		}
	}
}
