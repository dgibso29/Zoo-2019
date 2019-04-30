Shader "Custom/AdvancedTerrainShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_TerrainTextures("Terrain Texture Array", 2DArray) = "" {}		
		_HeightMaps("Terrain HeightMap Array", 2DArray) = "" {}
		_HeightmapBlending("Heightmap Blending", Float) = 0.05

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

			#include "heightblend.cginc"


	#pragma require 2darray

		UNITY_DECLARE_TEX2DARRAY(_TerrainTextures);
		UNITY_DECLARE_TEX2DARRAY(_HeightMaps);






			struct Input
			{
				fixed2 uv_TerrainTextures; // Texture Map

				float4 color : COLOR;
				float3 worldPos;
				float4 terrain; // terrain type"

				float2 textureUV : TEXCOORD0;
				float2 uv_MainTex;
			};

			void vert(inout appdata_full v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.terrain = v.texcoord2.xyzw;
			}

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

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
					float c = UNITY_SAMPLE_TEX2DARRAY(_HeightMaps, uvw) + heightShift;
					return c;
				}

				float4 heightBlend(float4 input1, float4 height1, float4 input2, float4 height2, float4 input3, float4 height3, float4 input4, float4 height4)
				{
					float4 height_start1 = max(height1, height2); 
					float4 height_start2 = max(height3, height4); 
					float4 height_start = max(height_start1, height_start2);

					float4 level1 = max(height1 - height_start, 0);
					float4 level2 = max(height2 - height_start, 0);
					float4 level3 = max(height3 - height_start, 0);
					float4 level4 = max(height4 - height_start, 0);

					return((input1 * level1) + (input2 * level2) + (input3 * level3) + (input4 * level4)) / (level1 + level2 + level3 + level4);
				}

				float4 heightBlend(float4 input1, float height1, float4 input2, float height2)
				{
					float4 height_start = max(height1, height2);
					float4 level1 = max(height1 - height_start, 0);
					float4 level2 = max(height2 - height_start, 0);

					return((input1 * level1) + (input2 * level2)) / (level1 + level2);
				}

				float4 heightLerp(float4 input1, float height1, float4 input2, float height2, float t)
				{
					t = clamp(t, 0, 1); 
					return heightBlend(input1, height1 * (1 - t), input2, height2 * t); 
				}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			
			float2 uvWorld = IN.worldPos.xz;

            // Albedo comes from a texture tinted by color
			float4 final; 

						// gets 
			float4 texture1 = Sample2DTextureArray(IN, uvWorld, 2);
			float4 texture2 = Sample2DTextureArray(IN, uvWorld, 3);
			float4 texture3 = Sample2DTextureArray(IN, uvWorld, 0);
			float4 texture4 = Sample2DTextureArray(IN, uvWorld, 1);

			float height1 = SampleHeightMapArray(IN, uvWorld, 2, 0);
			float height2 = SampleHeightMapArray(IN, uvWorld, 3, 0);
			float height3 = SampleHeightMapArray(IN, uvWorld, 0, 0);
			float height4 = SampleHeightMapArray(IN, uvWorld, 1, 0);

			// May need to be uv.y?
			float side1 = heightlerp(texture1, height1, texture2, height2, uvWorld.x);
			// Do all 4 sides

			// Do the same for heights for all 4


			// Do some bullshit to make this work somehow lol?


			final = heightlerp(ab, abh, cd, cdh, uvWorld.xy);

			//final = a + b + c + d;
			//c = texture1 + texture2 + texture3 + texture4;
			//c = heightblend(a, ah, b, bh); //a + b; 
			//c = a + b;
			//float uvy = clamp(uvWorld.y, 0, 1);
			//c = lerp(a, b, uvy);
			//c = texture1;
			//c = height1;
			//fixed4 c = texture1 + texture2 + texture3 + texture4;

            o.Albedo = final;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
			o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
