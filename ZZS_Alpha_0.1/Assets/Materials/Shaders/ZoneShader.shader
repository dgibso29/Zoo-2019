// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ZoneShader"
{
    Properties
    {
        _ZoneTexture ("Texture", 2D) = "white" {}
    } 
    SubShader
	{
		Tags { "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha // alpha blend

		Pass{
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma vertex vert
		#pragma fragment frag

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct vertInput
		{
			float4 pos : POSITION;
			//float2 uv_MainTex;
		};

		struct vertOutput
		{
			float4 pos :POSITION;
			fixed3 worldPos : TEXCOORD1;
		};

		vertOutput vert(vertInput input) {
			vertOutput o;
			o.pos = UnityObjectToClipPos(input.pos);
			o.worldPos = mul(unity_ObjectToWorld, input.pos).xyz;
			return o;
		}


		uniform int _Points_Length = 0;
		uniform float3 _Points[100];
		uniform float2 _Properties[100];
		 
		sampler2D _ZoneTexture;

		half4 frag(vertOutput output) : COLOR
		{

			half h = 0;
			for (int i = 0; i < _Points_Length; i++)
			{
				half di = distance(output.worldPos, _Points[i].xyz);

				half ri = _Properties[i].x;
				half hi = 1 - saturate(di / ri);

				h += hi * _Properties[i].y;
			}

			h = saturate(h);
			half4 color = tex2D(_ZoneTexture, fixed2(h, 0.5));
			return color;
		}
		ENDCG
		}
		}
    FallBack "Diffuse"
}
