Shader "Blendy/Fisheye" {
	Properties {
		_Horizon 		("Horizon", Range (1.0,360.0)) = 180.0
		_Grid	 		("Grid", Range (0.0,1.0)) = 0.0
		[MaterialToggle] _Transparent	("Transparent", Float ) = 0
		_MainTex 		("Grid (RGB)", 2D) 		= "white" {}
		_LeftTex 		("Left (RGB)", 2D) 		= "white" {}
		_FrontTex 		("Front (RGB)", 2D) 	= "white" {}
		_RightTex 		("Right (RGB)", 2D) 	= "white" {}
		_BackTex 		("Back (RGB)", 2D) 		= "white" {}
		_TopTex 		("Top (RGB)", 2D) 		= "white" {}
		_BottomTex 		("Bottom (RGB)", 2D)	= "white" {}
	}
	SubShader {
		Pass {
			Blend SrcAlpha Zero

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "ShLib.cginc"
			
			#define USE_CUBEMAPINDEX

			uniform float _Horizon;
			uniform float _Grid;
			uniform float _Transparent;
			uniform sampler2D _MainTex;
			uniform sampler2D _FrontTex;
			uniform sampler2D _RightTex;
			uniform sampler2D _BackTex;
			uniform sampler2D _LeftTex;
			uniform sampler2D _TopTex;
			uniform sampler2D _BottomTex;

			float4 frag(v2f_img i) : COLOR {
				// texture coordinate
				float2 st = i.uv;
				st.y = 1.0 - st.y;

				// final color
				float4 c = float4(0,0,0,1);
				
				// Intersect into cube
				float4 sample = float4(0,0,0,0);
				//if ( length( texelToUnit2(st) ) <= 1.0 )
				{
					float3 m = makeCubemapIndex( st, _Horizon );		// cubemap index
					st = float2( m.x, m.y );
					int face = int( m.z );
					if (face == FACE_FRONT)
						sample = tex2D( _FrontTex, st );
					else if (face == FACE_RIGHT)
						sample = tex2D( _RightTex, st );
					else if (face == FACE_BACK)
						sample = tex2D( _BackTex, st );
					else if (face == FACE_LEFT)
						sample = tex2D( _LeftTex, st );
					else if (face == FACE_TOP)
						sample = tex2D( _TopTex, st );
					else if (face == FACE_BOTTOM)
						sample = tex2D( _BottomTex, st );
				}

				c.rgb = sample.rgb;
				if (_Transparent)
					c.a = sample.a;

				// Fade In Grid
				if (_Grid > 0.0)
				{
					float4 g = tex2D(_MainTex, i.uv);
					c = c * (1.0 - _Grid) + g * _Grid;
				}

				return c;
			}
			ENDCG
		}
	}
}
