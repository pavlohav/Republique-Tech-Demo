Shader "Dust" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_DustTex ("Particle Texture", 2D) = "black" {}
	_NoiseTex ("Noise Texture", 2D) = "white" {}
	_MaskTex ("Mask Texture", 2D) = "white" {}
	_DustTileX ("Dust Tiling X", float) = 4
	_DustTileY ("Dust Tiling Y", float) = 12
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	//AlphaTest Greater .01
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	// ---- Fragment program cards
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_particles

			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float4 normal : NORMAL;
			};

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			float4 _DustTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				
				o.color = v.color;
				float3 viewDir = normalize(ObjSpaceViewDir ( v.vertex ));
				float3 normalDir = normalize(v.normal);
				float dotProduct = pow( abs ( dot ( normalDir, viewDir ) ), 8);
				o.color.a *= dotProduct;
				
				o.texcoord = TRANSFORM_TEX(v.texcoord,_DustTex);
				return o;
			}

			sampler2D _DustTex;
			sampler2D _NoiseTex;
			sampler2D _MaskTex;
			fixed4 _TintColor;
			half _DustTileX;
			half _DustTileY;
			
			fixed4 frag (v2f i) : COLOR
			{
				half u1 = i.texcoord.x * _DustTileX + _Time;
				half v1 = i.texcoord.y * _DustTileY + _Time * -1.5;
				half2 dustCoord = half2(u1,v1);
				half4 dustTex = tex2D(_DustTex, dustCoord) * 10;
				
				half u2 = i.texcoord.x * 3;
				half v2 = i.texcoord.y + _Time * 4;
				half2 noiseCoord = half2(u2, v2);
				half4 noiseTex = tex2D(_NoiseTex, noiseCoord) * 0.25;
				
				half4 mask = tex2D(_MaskTex, i.texcoord);
				
				half4 baseTex = (dustTex + noiseTex) * mask * i.color * _TintColor * 3;
				
				return baseTex;
			}
			ENDCG 
		}
	}
}
}
