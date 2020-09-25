Shader "Cyberon/Glitch4"
{
	Properties
	{
		Vector1_D901398A("Animation Speed", Float) = 10
		Vector1_73D855FA("Glitch Power", Float) = 0.1
		Vector1_65D1031F("Effect Appearance", Float) = 0.4
		[NoScaleOffset]Texture2D_7693CAB0("Texture2D", 2D) = "white" {}
	}
		SubShader
	{
		Tags
		{
			"RenderPipeline" = "UniversalPipeline"
			"RenderType" = "Transparent"
			"Queue" = "Transparent+0"
		}

		Pass
		{
			Name "Pass"
			Tags
			{
		// LightMode: <None>
		}

		// Render State
		Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
		Cull Off
		ZTest[unity_GUIZTestMode]
		ZWrite Off
		Lighting Off

		/*
		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		*/

		// ColorMask: <None>


		HLSLPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		// Debug
		// <None>

		// --------------------------------------------------
		// Pass

		// Pragmas
		#pragma prefer_hlslcc gles
		#pragma exclude_renderers d3d11_9x
		#pragma target 2.0
		#pragma multi_compile_fog
		#pragma multi_compile_instancing

		// Keywords
		#pragma multi_compile _ LIGHTMAP_ON
		#pragma multi_compile _ DIRLIGHTMAP_COMBINED
		#pragma shader_feature _ _SAMPLE_GI
		// GraphKeywords: <None>

		// Defines
		#define _SURFACE_TYPE_TRANSPARENT 1
		#define ATTRIBUTES_NEED_NORMAL
		#define ATTRIBUTES_NEED_TANGENT
		#define ATTRIBUTES_NEED_TEXCOORD0
		#define VARYINGS_NEED_TEXCOORD0
		#define SHADERPASS_UNLIT

		// Includes
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
		#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"

		// --------------------------------------------------
		// Graph

		// Graph Properties
		CBUFFER_START(UnityPerMaterial)
		float Vector1_D901398A;
		float Vector1_73D855FA;
		float Vector1_65D1031F;
		CBUFFER_END
		TEXTURE2D(Texture2D_7693CAB0); SAMPLER(samplerTexture2D_7693CAB0); float4 Texture2D_7693CAB0_TexelSize;
		SAMPLER(_SampleTexture2D_1FA71976_Sampler_3_Linear_Repeat);

		// Graph Functions


		inline float Unity_SimpleNoise_RandomValue_float(float2 uv)
		{
			return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
		}

		inline float Unity_SimpleNnoise_Interpolate_float(float a, float b, float t)
		{
			return (1.0 - t) * a + (t * b);
		}


		inline float Unity_SimpleNoise_ValueNoise_float(float2 uv)
		{
			float2 i = floor(uv);
			float2 f = frac(uv);
			f = f * f * (3.0 - 2.0 * f);

			uv = abs(frac(uv) - 0.5);
			float2 c0 = i + float2(0.0, 0.0);
			float2 c1 = i + float2(1.0, 0.0);
			float2 c2 = i + float2(0.0, 1.0);
			float2 c3 = i + float2(1.0, 1.0);
			float r0 = Unity_SimpleNoise_RandomValue_float(c0);
			float r1 = Unity_SimpleNoise_RandomValue_float(c1);
			float r2 = Unity_SimpleNoise_RandomValue_float(c2);
			float r3 = Unity_SimpleNoise_RandomValue_float(c3);

			float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
			float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
			float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
			return t;
		}
		void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
		{
			float t = 0.0;

			float freq = pow(2.0, float(0));
			float amp = pow(0.5, float(3 - 0));
			t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

			freq = pow(2.0, float(1));
			amp = pow(0.5, float(3 - 1));
			t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

			freq = pow(2.0, float(2));
			amp = pow(0.5, float(3 - 2));
			t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

			Out = t;
		}

		void Unity_Multiply_float(float A, float B, out float Out)
		{
			Out = A * B;
		}

		void Unity_Step_float(float Edge, float In, out float Out)
		{
			Out = step(Edge, In);
		}

		void Unity_Subtract_float(float A, float B, out float Out)
		{
			Out = A - B;
		}

		void Unity_OneMinus_float(float In, out float Out)
		{
			Out = 1 - In;
		}

		void Unity_Add_float(float A, float B, out float Out)
		{
			Out = A + B;
		}

		void Unity_Add_float4(float4 A, float4 B, out float4 Out)
		{
			Out = A + B;
		}

		// Graph Vertex
		// GraphVertex: <None>

		// Graph Pixel
		struct SurfaceDescriptionInputs
		{
			float4 uv0;
			float3 TimeParameters;
		};

		struct SurfaceDescription
		{
			float3 Color;
			float Alpha;
			float AlphaClipThreshold;
		};

		SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
		{
			SurfaceDescription surface = (SurfaceDescription)0;
			float4 _UV_8EDC47F8_Out_0 = IN.uv0;
			float4 _UV_1912F8C9_Out_0 = IN.uv0;
			float _Split_6BB97A7C_R_1 = _UV_1912F8C9_Out_0[0];
			float _Split_6BB97A7C_G_2 = _UV_1912F8C9_Out_0[1];
			float _Split_6BB97A7C_B_3 = _UV_1912F8C9_Out_0[2];
			float _Split_6BB97A7C_A_4 = _UV_1912F8C9_Out_0[3];
			float _SimpleNoise_62580F22_Out_2;
			Unity_SimpleNoise_float((_Split_6BB97A7C_G_2.xx), 500, _SimpleNoise_62580F22_Out_2);
			float _Property_BAD63AF5_Out_0 = Vector1_73D855FA;
			float _Multiply_A208B311_Out_2;
			Unity_Multiply_float(_SimpleNoise_62580F22_Out_2, _Property_BAD63AF5_Out_0, _Multiply_A208B311_Out_2);
			float _Property_82128B0C_Out_0 = Vector1_D901398A;
			float _SimpleNoise_59E35CDB_Out_2;
			Unity_SimpleNoise_float((IN.TimeParameters.x.xx), _Property_82128B0C_Out_0, _SimpleNoise_59E35CDB_Out_2);
			float _Property_4CF32C5F_Out_0 = Vector1_65D1031F;
			float _Step_1E48C4AB_Out_2;
			Unity_Step_float(_SimpleNoise_59E35CDB_Out_2, _Property_4CF32C5F_Out_0, _Step_1E48C4AB_Out_2);
			float _Multiply_3E9B3D6B_Out_2;
			Unity_Multiply_float(_Multiply_A208B311_Out_2, _Step_1E48C4AB_Out_2, _Multiply_3E9B3D6B_Out_2);
			float4 _UV_E7E7025D_Out_0 = IN.uv0;
			float _Split_944C9993_R_1 = _UV_E7E7025D_Out_0[0];
			float _Split_944C9993_G_2 = _UV_E7E7025D_Out_0[1];
			float _Split_944C9993_B_3 = _UV_E7E7025D_Out_0[2];
			float _Split_944C9993_A_4 = _UV_E7E7025D_Out_0[3];
			float _SimpleNoise_C765C62B_Out_2;
			Unity_SimpleNoise_float((_Split_944C9993_G_2.xx), IN.TimeParameters.x, _SimpleNoise_C765C62B_Out_2);
			float _Step_65D51D8E_Out_2;
			Unity_Step_float(_SimpleNoise_C765C62B_Out_2, 0.3, _Step_65D51D8E_Out_2);
			float _Multiply_357E4D89_Out_2;
			Unity_Multiply_float(_Multiply_3E9B3D6B_Out_2, _Step_65D51D8E_Out_2, _Multiply_357E4D89_Out_2);
			float _Multiply_28ADEE93_Out_2;
			Unity_Multiply_float(_Multiply_357E4D89_Out_2, 0.5, _Multiply_28ADEE93_Out_2);
			float _Step_BFCD3574_Out_2;
			Unity_Step_float(_SimpleNoise_C765C62B_Out_2, 0.4, _Step_BFCD3574_Out_2);
			float _Subtract_7947CEA2_Out_2;
			Unity_Subtract_float(_Step_BFCD3574_Out_2, _Step_65D51D8E_Out_2, _Subtract_7947CEA2_Out_2);
			float _Multiply_9BB03E3D_Out_2;
			Unity_Multiply_float(_Multiply_3E9B3D6B_Out_2, _Subtract_7947CEA2_Out_2, _Multiply_9BB03E3D_Out_2);
			float _OneMinus_3189EB26_Out_1;
			Unity_OneMinus_float(_Multiply_9BB03E3D_Out_2, _OneMinus_3189EB26_Out_1);
			float _Multiply_61A530E5_Out_2;
			Unity_Multiply_float(0.5, _OneMinus_3189EB26_Out_1, _Multiply_61A530E5_Out_2);
			float _Add_F6068C2E_Out_2;
			Unity_Add_float(_Multiply_28ADEE93_Out_2, _Multiply_61A530E5_Out_2, _Add_F6068C2E_Out_2);
			float _Subtract_37D564CB_Out_2;
			Unity_Subtract_float(_Add_F6068C2E_Out_2, 0.5, _Subtract_37D564CB_Out_2);
			float4 _Vector4_129B288E_Out_0 = float4(_Subtract_37D564CB_Out_2, 0, 0, 0);
			float4 _Add_6F4857C4_Out_2;
			Unity_Add_float4(_UV_8EDC47F8_Out_0, _Vector4_129B288E_Out_0, _Add_6F4857C4_Out_2);
			float4 _SampleTexture2D_1FA71976_RGBA_0 = SAMPLE_TEXTURE2D(Texture2D_7693CAB0, samplerTexture2D_7693CAB0, (_Add_6F4857C4_Out_2.xy));
			float _SampleTexture2D_1FA71976_R_4 = _SampleTexture2D_1FA71976_RGBA_0.r;
			float _SampleTexture2D_1FA71976_G_5 = _SampleTexture2D_1FA71976_RGBA_0.g;
			float _SampleTexture2D_1FA71976_B_6 = _SampleTexture2D_1FA71976_RGBA_0.b;
			float _SampleTexture2D_1FA71976_A_7 = _SampleTexture2D_1FA71976_RGBA_0.a;
			surface.Color = (_SampleTexture2D_1FA71976_RGBA_0.xyz);
			surface.Alpha = _SampleTexture2D_1FA71976_A_7;
			surface.AlphaClipThreshold = 0;
			return surface;
		}

		// --------------------------------------------------
		// Structs and Packing

		// Generated Type: Attributes
		struct Attributes
		{
			float3 positionOS : POSITION;
			float3 normalOS : NORMAL;
			float4 tangentOS : TANGENT;
			float4 uv0 : TEXCOORD0;
			#if UNITY_ANY_INSTANCING_ENABLED
			uint instanceID : INSTANCEID_SEMANTIC;
			#endif
		};

		// Generated Type: Varyings
		struct Varyings
		{
			float4 positionCS : SV_POSITION;
			float4 texCoord0;
			#if UNITY_ANY_INSTANCING_ENABLED
			uint instanceID : CUSTOM_INSTANCE_ID;
			#endif
			#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
			uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
			#endif
			#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
			uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
			#endif
			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
			#endif
		};

		// Generated Type: PackedVaryings
		struct PackedVaryings
		{
			float4 positionCS : SV_POSITION;
			#if UNITY_ANY_INSTANCING_ENABLED
			uint instanceID : CUSTOM_INSTANCE_ID;
			#endif
			float4 interp00 : TEXCOORD0;
			#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
			uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
			#endif
			#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
			uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
			#endif
			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
			#endif
		};

		// Packed Type: Varyings
		PackedVaryings PackVaryings(Varyings input)
		{
			PackedVaryings output = (PackedVaryings)0;
			output.positionCS = input.positionCS;
			output.interp00.xyzw = input.texCoord0;
			#if UNITY_ANY_INSTANCING_ENABLED
			output.instanceID = input.instanceID;
			#endif
			#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
			output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
			#endif
			#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
			output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
			#endif
			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			output.cullFace = input.cullFace;
			#endif
			return output;
		}

		// Unpacked Type: Varyings
		Varyings UnpackVaryings(PackedVaryings input)
		{
			Varyings output = (Varyings)0;
			output.positionCS = input.positionCS;
			output.texCoord0 = input.interp00.xyzw;
			#if UNITY_ANY_INSTANCING_ENABLED
			output.instanceID = input.instanceID;
			#endif
			#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
			output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
			#endif
			#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
			output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
			#endif
			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			output.cullFace = input.cullFace;
			#endif
			return output;
		}

		// --------------------------------------------------
		// Build Graph Inputs

		SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
		{
			SurfaceDescriptionInputs output;
			ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





			output.uv0 = input.texCoord0;
			output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
		#else
		#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
		#endif
		#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

			return output;
		}


		// --------------------------------------------------
		// Main

		#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

		ENDHLSL
	}

	Pass
	{
		Name "ShadowCaster"
		Tags
		{
			"LightMode" = "ShadowCaster"
		}

			// Render State
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			Cull Back
			ZTest LEqual
			ZWrite On
			// ColorMask: <None>

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// Debug
			// <None>

			// --------------------------------------------------
			// Pass

			// Pragmas
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0
			#pragma multi_compile_instancing

			// Keywords
			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			// GraphKeywords: <None>

			// Defines
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define ATTRIBUTES_NEED_TEXCOORD0
			#define VARYINGS_NEED_TEXCOORD0
			#define SHADERPASS_SHADOWCASTER

			// Includes
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"

			// --------------------------------------------------
			// Graph

			// Graph Properties
			CBUFFER_START(UnityPerMaterial)
			float Vector1_D901398A;
			float Vector1_73D855FA;
			float Vector1_65D1031F;
			CBUFFER_END
			TEXTURE2D(Texture2D_7693CAB0); SAMPLER(samplerTexture2D_7693CAB0); float4 Texture2D_7693CAB0_TexelSize;
			SAMPLER(_SampleTexture2D_1FA71976_Sampler_3_Linear_Repeat);

			// Graph Functions


			inline float Unity_SimpleNoise_RandomValue_float(float2 uv)
			{
				return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
			}

			inline float Unity_SimpleNnoise_Interpolate_float(float a, float b, float t)
			{
				return (1.0 - t) * a + (t * b);
			}


			inline float Unity_SimpleNoise_ValueNoise_float(float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac(uv);
				f = f * f * (3.0 - 2.0 * f);

				uv = abs(frac(uv) - 0.5);
				float2 c0 = i + float2(0.0, 0.0);
				float2 c1 = i + float2(1.0, 0.0);
				float2 c2 = i + float2(0.0, 1.0);
				float2 c3 = i + float2(1.0, 1.0);
				float r0 = Unity_SimpleNoise_RandomValue_float(c0);
				float r1 = Unity_SimpleNoise_RandomValue_float(c1);
				float r2 = Unity_SimpleNoise_RandomValue_float(c2);
				float r3 = Unity_SimpleNoise_RandomValue_float(c3);

				float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
				float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
				float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
				return t;
			}
			void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
			{
				float t = 0.0;

				float freq = pow(2.0, float(0));
				float amp = pow(0.5, float(3 - 0));
				t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3 - 1));
				t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3 - 2));
				t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

				Out = t;
			}

			void Unity_Multiply_float(float A, float B, out float Out)
			{
				Out = A * B;
			}

			void Unity_Step_float(float Edge, float In, out float Out)
			{
				Out = step(Edge, In);
			}

			void Unity_Subtract_float(float A, float B, out float Out)
			{
				Out = A - B;
			}

			void Unity_OneMinus_float(float In, out float Out)
			{
				Out = 1 - In;
			}

			void Unity_Add_float(float A, float B, out float Out)
			{
				Out = A + B;
			}

			void Unity_Add_float4(float4 A, float4 B, out float4 Out)
			{
				Out = A + B;
			}

			// Graph Vertex
			// GraphVertex: <None>

			// Graph Pixel
			struct SurfaceDescriptionInputs
			{
				float4 uv0;
				float3 TimeParameters;
			};

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
			{
				SurfaceDescription surface = (SurfaceDescription)0;
				float4 _UV_8EDC47F8_Out_0 = IN.uv0;
				float4 _UV_1912F8C9_Out_0 = IN.uv0;
				float _Split_6BB97A7C_R_1 = _UV_1912F8C9_Out_0[0];
				float _Split_6BB97A7C_G_2 = _UV_1912F8C9_Out_0[1];
				float _Split_6BB97A7C_B_3 = _UV_1912F8C9_Out_0[2];
				float _Split_6BB97A7C_A_4 = _UV_1912F8C9_Out_0[3];
				float _SimpleNoise_62580F22_Out_2;
				Unity_SimpleNoise_float((_Split_6BB97A7C_G_2.xx), 500, _SimpleNoise_62580F22_Out_2);
				float _Property_BAD63AF5_Out_0 = Vector1_73D855FA;
				float _Multiply_A208B311_Out_2;
				Unity_Multiply_float(_SimpleNoise_62580F22_Out_2, _Property_BAD63AF5_Out_0, _Multiply_A208B311_Out_2);
				float _Property_82128B0C_Out_0 = Vector1_D901398A;
				float _SimpleNoise_59E35CDB_Out_2;
				Unity_SimpleNoise_float((IN.TimeParameters.x.xx), _Property_82128B0C_Out_0, _SimpleNoise_59E35CDB_Out_2);
				float _Property_4CF32C5F_Out_0 = Vector1_65D1031F;
				float _Step_1E48C4AB_Out_2;
				Unity_Step_float(_SimpleNoise_59E35CDB_Out_2, _Property_4CF32C5F_Out_0, _Step_1E48C4AB_Out_2);
				float _Multiply_3E9B3D6B_Out_2;
				Unity_Multiply_float(_Multiply_A208B311_Out_2, _Step_1E48C4AB_Out_2, _Multiply_3E9B3D6B_Out_2);
				float4 _UV_E7E7025D_Out_0 = IN.uv0;
				float _Split_944C9993_R_1 = _UV_E7E7025D_Out_0[0];
				float _Split_944C9993_G_2 = _UV_E7E7025D_Out_0[1];
				float _Split_944C9993_B_3 = _UV_E7E7025D_Out_0[2];
				float _Split_944C9993_A_4 = _UV_E7E7025D_Out_0[3];
				float _SimpleNoise_C765C62B_Out_2;
				Unity_SimpleNoise_float((_Split_944C9993_G_2.xx), IN.TimeParameters.x, _SimpleNoise_C765C62B_Out_2);
				float _Step_65D51D8E_Out_2;
				Unity_Step_float(_SimpleNoise_C765C62B_Out_2, 0.3, _Step_65D51D8E_Out_2);
				float _Multiply_357E4D89_Out_2;
				Unity_Multiply_float(_Multiply_3E9B3D6B_Out_2, _Step_65D51D8E_Out_2, _Multiply_357E4D89_Out_2);
				float _Multiply_28ADEE93_Out_2;
				Unity_Multiply_float(_Multiply_357E4D89_Out_2, 0.5, _Multiply_28ADEE93_Out_2);
				float _Step_BFCD3574_Out_2;
				Unity_Step_float(_SimpleNoise_C765C62B_Out_2, 0.4, _Step_BFCD3574_Out_2);
				float _Subtract_7947CEA2_Out_2;
				Unity_Subtract_float(_Step_BFCD3574_Out_2, _Step_65D51D8E_Out_2, _Subtract_7947CEA2_Out_2);
				float _Multiply_9BB03E3D_Out_2;
				Unity_Multiply_float(_Multiply_3E9B3D6B_Out_2, _Subtract_7947CEA2_Out_2, _Multiply_9BB03E3D_Out_2);
				float _OneMinus_3189EB26_Out_1;
				Unity_OneMinus_float(_Multiply_9BB03E3D_Out_2, _OneMinus_3189EB26_Out_1);
				float _Multiply_61A530E5_Out_2;
				Unity_Multiply_float(0.5, _OneMinus_3189EB26_Out_1, _Multiply_61A530E5_Out_2);
				float _Add_F6068C2E_Out_2;
				Unity_Add_float(_Multiply_28ADEE93_Out_2, _Multiply_61A530E5_Out_2, _Add_F6068C2E_Out_2);
				float _Subtract_37D564CB_Out_2;
				Unity_Subtract_float(_Add_F6068C2E_Out_2, 0.5, _Subtract_37D564CB_Out_2);
				float4 _Vector4_129B288E_Out_0 = float4(_Subtract_37D564CB_Out_2, 0, 0, 0);
				float4 _Add_6F4857C4_Out_2;
				Unity_Add_float4(_UV_8EDC47F8_Out_0, _Vector4_129B288E_Out_0, _Add_6F4857C4_Out_2);
				float4 _SampleTexture2D_1FA71976_RGBA_0 = SAMPLE_TEXTURE2D(Texture2D_7693CAB0, samplerTexture2D_7693CAB0, (_Add_6F4857C4_Out_2.xy));
				float _SampleTexture2D_1FA71976_R_4 = _SampleTexture2D_1FA71976_RGBA_0.r;
				float _SampleTexture2D_1FA71976_G_5 = _SampleTexture2D_1FA71976_RGBA_0.g;
				float _SampleTexture2D_1FA71976_B_6 = _SampleTexture2D_1FA71976_RGBA_0.b;
				float _SampleTexture2D_1FA71976_A_7 = _SampleTexture2D_1FA71976_RGBA_0.a;
				surface.Alpha = _SampleTexture2D_1FA71976_A_7;
				surface.AlphaClipThreshold = 0;
				return surface;
			}

			// --------------------------------------------------
			// Structs and Packing

			// Generated Type: Attributes
			struct Attributes
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 uv0 : TEXCOORD0;
				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : INSTANCEID_SEMANTIC;
				#endif
			};

			// Generated Type: Varyings
			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float4 texCoord0;
				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};

			// Generated Type: PackedVaryings
			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
				float4 interp00 : TEXCOORD0;
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};

			// Packed Type: Varyings
			PackedVaryings PackVaryings(Varyings input)
			{
				PackedVaryings output = (PackedVaryings)0;
				output.positionCS = input.positionCS;
				output.interp00.xyzw = input.texCoord0;
				#if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = input.instanceID;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				output.cullFace = input.cullFace;
				#endif
				return output;
			}

			// Unpacked Type: Varyings
			Varyings UnpackVaryings(PackedVaryings input)
			{
				Varyings output = (Varyings)0;
				output.positionCS = input.positionCS;
				output.texCoord0 = input.interp00.xyzw;
				#if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = input.instanceID;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				output.cullFace = input.cullFace;
				#endif
				return output;
			}

			// --------------------------------------------------
			// Build Graph Inputs

			SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
			{
				SurfaceDescriptionInputs output;
				ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





				output.uv0 = input.texCoord0;
				output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
			#else
			#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
			#endif
			#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

				return output;
			}


			// --------------------------------------------------
			// Main

			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

			ENDHLSL
		}

		Pass
		{
			Name "DepthOnly"
			Tags
			{
				"LightMode" = "DepthOnly"
			}

				// Render State
				Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
				Cull Back
				ZTest LEqual
				ZWrite On
				ColorMask 0


				HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				// Debug
				// <None>

				// --------------------------------------------------
				// Pass

				// Pragmas
				#pragma prefer_hlslcc gles
				#pragma exclude_renderers d3d11_9x
				#pragma target 2.0
				#pragma multi_compile_instancing

				// Keywords
				// PassKeywords: <None>
				// GraphKeywords: <None>

				// Defines
				#define _SURFACE_TYPE_TRANSPARENT 1
				#define ATTRIBUTES_NEED_NORMAL
				#define ATTRIBUTES_NEED_TANGENT
				#define ATTRIBUTES_NEED_TEXCOORD0
				#define VARYINGS_NEED_TEXCOORD0
				#define SHADERPASS_DEPTHONLY

				// Includes
				#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
				#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"

				// --------------------------------------------------
				// Graph

				// Graph Properties
				CBUFFER_START(UnityPerMaterial)
				float Vector1_D901398A;
				float Vector1_73D855FA;
				float Vector1_65D1031F;
				CBUFFER_END
				TEXTURE2D(Texture2D_7693CAB0); SAMPLER(samplerTexture2D_7693CAB0); float4 Texture2D_7693CAB0_TexelSize;
				SAMPLER(_SampleTexture2D_1FA71976_Sampler_3_Linear_Repeat);

				// Graph Functions


				inline float Unity_SimpleNoise_RandomValue_float(float2 uv)
				{
					return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
				}

				inline float Unity_SimpleNnoise_Interpolate_float(float a, float b, float t)
				{
					return (1.0 - t) * a + (t * b);
				}


				inline float Unity_SimpleNoise_ValueNoise_float(float2 uv)
				{
					float2 i = floor(uv);
					float2 f = frac(uv);
					f = f * f * (3.0 - 2.0 * f);

					uv = abs(frac(uv) - 0.5);
					float2 c0 = i + float2(0.0, 0.0);
					float2 c1 = i + float2(1.0, 0.0);
					float2 c2 = i + float2(0.0, 1.0);
					float2 c3 = i + float2(1.0, 1.0);
					float r0 = Unity_SimpleNoise_RandomValue_float(c0);
					float r1 = Unity_SimpleNoise_RandomValue_float(c1);
					float r2 = Unity_SimpleNoise_RandomValue_float(c2);
					float r3 = Unity_SimpleNoise_RandomValue_float(c3);

					float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
					float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
					float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
					return t;
				}
				void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
				{
					float t = 0.0;

					float freq = pow(2.0, float(0));
					float amp = pow(0.5, float(3 - 0));
					t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

					freq = pow(2.0, float(1));
					amp = pow(0.5, float(3 - 1));
					t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

					freq = pow(2.0, float(2));
					amp = pow(0.5, float(3 - 2));
					t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

					Out = t;
				}

				void Unity_Multiply_float(float A, float B, out float Out)
				{
					Out = A * B;
				}

				void Unity_Step_float(float Edge, float In, out float Out)
				{
					Out = step(Edge, In);
				}

				void Unity_Subtract_float(float A, float B, out float Out)
				{
					Out = A - B;
				}

				void Unity_OneMinus_float(float In, out float Out)
				{
					Out = 1 - In;
				}

				void Unity_Add_float(float A, float B, out float Out)
				{
					Out = A + B;
				}

				void Unity_Add_float4(float4 A, float4 B, out float4 Out)
				{
					Out = A + B;
				}

				// Graph Vertex
				// GraphVertex: <None>

				// Graph Pixel
				struct SurfaceDescriptionInputs
				{
					float4 uv0;
					float3 TimeParameters;
				};

				struct SurfaceDescription
				{
					float Alpha;
					float AlphaClipThreshold;
				};

				SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
				{
					SurfaceDescription surface = (SurfaceDescription)0;
					float4 _UV_8EDC47F8_Out_0 = IN.uv0;
					float4 _UV_1912F8C9_Out_0 = IN.uv0;
					float _Split_6BB97A7C_R_1 = _UV_1912F8C9_Out_0[0];
					float _Split_6BB97A7C_G_2 = _UV_1912F8C9_Out_0[1];
					float _Split_6BB97A7C_B_3 = _UV_1912F8C9_Out_0[2];
					float _Split_6BB97A7C_A_4 = _UV_1912F8C9_Out_0[3];
					float _SimpleNoise_62580F22_Out_2;
					Unity_SimpleNoise_float((_Split_6BB97A7C_G_2.xx), 500, _SimpleNoise_62580F22_Out_2);
					float _Property_BAD63AF5_Out_0 = Vector1_73D855FA;
					float _Multiply_A208B311_Out_2;
					Unity_Multiply_float(_SimpleNoise_62580F22_Out_2, _Property_BAD63AF5_Out_0, _Multiply_A208B311_Out_2);
					float _Property_82128B0C_Out_0 = Vector1_D901398A;
					float _SimpleNoise_59E35CDB_Out_2;
					Unity_SimpleNoise_float((IN.TimeParameters.x.xx), _Property_82128B0C_Out_0, _SimpleNoise_59E35CDB_Out_2);
					float _Property_4CF32C5F_Out_0 = Vector1_65D1031F;
					float _Step_1E48C4AB_Out_2;
					Unity_Step_float(_SimpleNoise_59E35CDB_Out_2, _Property_4CF32C5F_Out_0, _Step_1E48C4AB_Out_2);
					float _Multiply_3E9B3D6B_Out_2;
					Unity_Multiply_float(_Multiply_A208B311_Out_2, _Step_1E48C4AB_Out_2, _Multiply_3E9B3D6B_Out_2);
					float4 _UV_E7E7025D_Out_0 = IN.uv0;
					float _Split_944C9993_R_1 = _UV_E7E7025D_Out_0[0];
					float _Split_944C9993_G_2 = _UV_E7E7025D_Out_0[1];
					float _Split_944C9993_B_3 = _UV_E7E7025D_Out_0[2];
					float _Split_944C9993_A_4 = _UV_E7E7025D_Out_0[3];
					float _SimpleNoise_C765C62B_Out_2;
					Unity_SimpleNoise_float((_Split_944C9993_G_2.xx), IN.TimeParameters.x, _SimpleNoise_C765C62B_Out_2);
					float _Step_65D51D8E_Out_2;
					Unity_Step_float(_SimpleNoise_C765C62B_Out_2, 0.3, _Step_65D51D8E_Out_2);
					float _Multiply_357E4D89_Out_2;
					Unity_Multiply_float(_Multiply_3E9B3D6B_Out_2, _Step_65D51D8E_Out_2, _Multiply_357E4D89_Out_2);
					float _Multiply_28ADEE93_Out_2;
					Unity_Multiply_float(_Multiply_357E4D89_Out_2, 0.5, _Multiply_28ADEE93_Out_2);
					float _Step_BFCD3574_Out_2;
					Unity_Step_float(_SimpleNoise_C765C62B_Out_2, 0.4, _Step_BFCD3574_Out_2);
					float _Subtract_7947CEA2_Out_2;
					Unity_Subtract_float(_Step_BFCD3574_Out_2, _Step_65D51D8E_Out_2, _Subtract_7947CEA2_Out_2);
					float _Multiply_9BB03E3D_Out_2;
					Unity_Multiply_float(_Multiply_3E9B3D6B_Out_2, _Subtract_7947CEA2_Out_2, _Multiply_9BB03E3D_Out_2);
					float _OneMinus_3189EB26_Out_1;
					Unity_OneMinus_float(_Multiply_9BB03E3D_Out_2, _OneMinus_3189EB26_Out_1);
					float _Multiply_61A530E5_Out_2;
					Unity_Multiply_float(0.5, _OneMinus_3189EB26_Out_1, _Multiply_61A530E5_Out_2);
					float _Add_F6068C2E_Out_2;
					Unity_Add_float(_Multiply_28ADEE93_Out_2, _Multiply_61A530E5_Out_2, _Add_F6068C2E_Out_2);
					float _Subtract_37D564CB_Out_2;
					Unity_Subtract_float(_Add_F6068C2E_Out_2, 0.5, _Subtract_37D564CB_Out_2);
					float4 _Vector4_129B288E_Out_0 = float4(_Subtract_37D564CB_Out_2, 0, 0, 0);
					float4 _Add_6F4857C4_Out_2;
					Unity_Add_float4(_UV_8EDC47F8_Out_0, _Vector4_129B288E_Out_0, _Add_6F4857C4_Out_2);
					float4 _SampleTexture2D_1FA71976_RGBA_0 = SAMPLE_TEXTURE2D(Texture2D_7693CAB0, samplerTexture2D_7693CAB0, (_Add_6F4857C4_Out_2.xy));
					float _SampleTexture2D_1FA71976_R_4 = _SampleTexture2D_1FA71976_RGBA_0.r;
					float _SampleTexture2D_1FA71976_G_5 = _SampleTexture2D_1FA71976_RGBA_0.g;
					float _SampleTexture2D_1FA71976_B_6 = _SampleTexture2D_1FA71976_RGBA_0.b;
					float _SampleTexture2D_1FA71976_A_7 = _SampleTexture2D_1FA71976_RGBA_0.a;
					surface.Alpha = _SampleTexture2D_1FA71976_A_7;
					surface.AlphaClipThreshold = 0;
					return surface;
				}

				// --------------------------------------------------
				// Structs and Packing

				// Generated Type: Attributes
				struct Attributes
				{
					float3 positionOS : POSITION;
					float3 normalOS : NORMAL;
					float4 tangentOS : TANGENT;
					float4 uv0 : TEXCOORD0;
					#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : INSTANCEID_SEMANTIC;
					#endif
				};

				// Generated Type: Varyings
				struct Varyings
				{
					float4 positionCS : SV_POSITION;
					float4 texCoord0;
					#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : CUSTOM_INSTANCE_ID;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
					#endif
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
					#endif
				};

				// Generated Type: PackedVaryings
				struct PackedVaryings
				{
					float4 positionCS : SV_POSITION;
					#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : CUSTOM_INSTANCE_ID;
					#endif
					float4 interp00 : TEXCOORD0;
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
					#endif
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
					#endif
				};

				// Packed Type: Varyings
				PackedVaryings PackVaryings(Varyings input)
				{
					PackedVaryings output = (PackedVaryings)0;
					output.positionCS = input.positionCS;
					output.interp00.xyzw = input.texCoord0;
					#if UNITY_ANY_INSTANCING_ENABLED
					output.instanceID = input.instanceID;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
					#endif
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					output.cullFace = input.cullFace;
					#endif
					return output;
				}

				// Unpacked Type: Varyings
				Varyings UnpackVaryings(PackedVaryings input)
				{
					Varyings output = (Varyings)0;
					output.positionCS = input.positionCS;
					output.texCoord0 = input.interp00.xyzw;
					#if UNITY_ANY_INSTANCING_ENABLED
					output.instanceID = input.instanceID;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
					#endif
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					output.cullFace = input.cullFace;
					#endif
					return output;
				}

				// --------------------------------------------------
				// Build Graph Inputs

				SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
				{
					SurfaceDescriptionInputs output;
					ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





					output.uv0 = input.texCoord0;
					output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
				#else
				#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
				#endif
				#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

					return output;
				}


				// --------------------------------------------------
				// Main

				#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

				ENDHLSL
			}

	}
		FallBack "Hidden/Shader Graph/FallbackError"
}
