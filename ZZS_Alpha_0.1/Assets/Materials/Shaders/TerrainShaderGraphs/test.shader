Shader "Custom/test"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_TerrainTextures("Terrain Texture Array", 2DArray) = "" {}
		_HeightMaps("Terrain HeightMap Array", 2DArray) = "" {}

	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200


			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows vertex:vert

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0


			#pragma require 2darray
				
			
			sampler2D _MainTex;
					   			 
			struct Input
			{
				fixed2 uv_TerrainTextures; // Texture Map

				float4 color : COLOR;
				float3 worldPos;
				float4 terrain; // terrain type"

				float2 textureUV : TEXCOORD0;
				float2 uv_MainTex;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

			void vert(inout appdata_full v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.terrain = v.texcoord2.xyzw;
			}


			UNITY_DECLARE_TEX2DARRAY(_TerrainTextures);
			UNITY_DECLARE_TEX2DARRAY(_HeightMaps);
			


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		float4 Sample2DTextureArray(Input IN, float2 uv, int index)
		{
			float3 uvw = float3(uv, IN.terrain[index]);
			float4 c = UNITY_SAMPLE_TEX2DARRAY(_TerrainTextures, uvw);
			return c;
		}

		float SampleHeightMapArray(Input IN, float2 uv, int index, float heightShift)
		{
			float3 uvw = float3(uv, IN.terrain[index]);
			float c = UNITY_SAMPLE_TEX2DARRAY(_HeightMaps, uvw);
			return c;
		}



		 void surf(Input IN, inout SurfaceOutputStandard o)
		 {



			float2 uvWorld = IN.worldPos.xz;

			// Albedo comes from a texture tinted by color
			float4 c;

			// gets 
			float4 texture1 = Sample2DTextureArray(IN, uvWorld, 0);
			float4 texture2 = Sample2DTextureArray(IN, uvWorld, 1);
			float4 texture3 = Sample2DTextureArray(IN, uvWorld, 2);
			float4 texture4 = Sample2DTextureArray(IN, uvWorld, 3);

			float height1 = SampleHeightMapArray(IN, uvWorld, 0, 0);
			float height2 = SampleHeightMapArray(IN, uvWorld, 1, 0);
			float height3 = SampleHeightMapArray(IN, uvWorld, 2, 0);
			float height4 = SampleHeightMapArray(IN, uvWorld, 3, 0);

			float4 color = float4(IN.color.r + height1, IN.color.g + height2, IN.color.b + height3, IN.color.a + height4);

			texture1 *= _Color;
			texture2 *= _Color;
			texture3 *= _Color;
			texture4 *= _Color;

			float t1 = (1 / (1 * pow(2, color.r * 4)) + 1) / 2;
			float t2 = (1 / (1 * pow(2, color.g * 4)) + 1) / 2;
			float t3 = (1 / (1 * pow(2, color.b * 4)) + 1) / 2;
			float t4 = (1 / (1 * pow(2, color.a * 4)) + 1) / 2;

			float tSum = t1 + t2 + t3 + t4;

			t1 /= tSum;
			t2 /= tSum;
			t3 /= tSum;
			t4 /= tSum;

			texture1 *= t1;
			texture2 *= t2;
			texture3 *= t3;
			texture4 *= t4;


			o.Albedo = (texture1 + texture2 + texture3 + texture4);


            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
