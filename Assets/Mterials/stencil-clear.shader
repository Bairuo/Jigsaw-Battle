Shader "stencil-clear"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	
	SubShader
	{
		Cull Off ZTest Always
		
		Tags
		{ 
			"RenderType" = "Opaque" 
			"Queue" = "Geometry+1"
		}
		
		Stencil
		{
			Ref 0 // value 1.
			Comp Always // Always pass.
			Pass Replace // replace passed pixel's stencil buffer by 1.
		}
		
		Blend Zero One
		BlendOp Add
		
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

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				if(col.a < 0.01f) discard;
				return float4(0.5f, 0.0f, 0.0f, 0.5f);
			}
			ENDCG
		}
	}
}
