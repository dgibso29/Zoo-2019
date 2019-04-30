Shader "Custom/TestShader01"
 {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_NoiseTexture("Noise", 2D) = "white" {}
		_Offset("Vertex Offset", Float) = 0
	}

		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 100

			Pass {
				CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					#pragma target 2.0
					#pragma multi_compile_fog

					#include "UnityCG.cginc"

					struct appdata_t {
						float4 vertex : POSITION;
						UNITY_VERTEX_INPUT_INSTANCE_ID
					};

					struct v2f {
						float4 vertex : SV_POSITION;
						UNITY_FOG_COORDS(0)
						UNITY_VERTEX_OUTPUT_STEREO
					};

					fixed4 _Color;
					float _Offset;
					sampler2D _NoiseTexture;

					v2f vert(appdata_t v)
					{
						v2f o;
						UNITY_SETUP_INSTANCE_ID(v);
						UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
						o.vertex = UnityObjectToClipPos(v.vertex);
						UNITY_TRANSFER_FOG(o,o.vertex);
						//o.vertex.y += _NoiseTexture.vertex.y;
						return o;
					}

					fixed4 frag(v2f i) : COLOR
					{
						fixed4 col = _Color;
					col = sin(_Color + _Time[1])
						UNITY_APPLY_FOG(i.fogCoord, col);
						UNITY_OPAQUE_ALPHA(col.a);
						return col;
					}
				ENDCG
			}
	}

}
