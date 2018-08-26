Shader "Blendy/Normals" {
	Properties {
		_MainTex ("Grid (RGB)", 2D) = "white" {}
	}
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "ShLib.cginc"

			uniform sampler2D _MainTex;
			
			float4 frag(v2f_img i) : COLOR {
				//float4 c = tex2D(_MainTex, i.uv);
				//return c;
				
				float2 st = i.uv;
				float3 dc = texelToDome( st );
				float3 n = unitToTexel3( dc );
			
				float4 c = float4(0,0,0,1);
				if ( length( texelToUnit2(st) ) <= 1.0 )
					c.rgb = n;
				return c;
				
				//return getColorRed();
			}
			
			ENDCG
		}
	}
}
