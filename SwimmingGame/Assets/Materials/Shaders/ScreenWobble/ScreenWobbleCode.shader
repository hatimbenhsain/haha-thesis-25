Shader "Shader Graphs/ScreenWobble"
{
    Properties
    {
        _Blend("Blend", Range(0, 1)) = 0
        _NoiseScale("NoiseScale", Float) = 10
        _PanSpeed("PanSpeed", Float) = 1
        _1("Amplitude", Float) = 0
        [HideInInspector][Enum(UnityEditor.Rendering.Fullscreen.ShaderGraph.FullscreenBlendMode)]_Fullscreen_BlendMode("Float", Float) = 1
        [HideInInspector][Enum(UnityEngine.Rendering.BlendMode)]_Fullscreen_SrcColorBlend("Float", Float) = 5
        [HideInInspector][Enum(UnityEngine.Rendering.BlendMode)]_Fullscreen_DstColorBlend("Float", Float) = 10
        [HideInInspector][Enum(UnityEngine.Rendering.BlendMode)]_Fullscreen_SrcAlphaBlend("Float", Float) = 1
        [HideInInspector][Enum(UnityEngine.Rendering.BlendMode)]_Fullscreen_DstAlphaBlend("Float", Float) = 10
        [HideInInspector][Enum(UnityEditor.ShaderGraph.BlendOp)]_Fullscreen_ColorBlendOperation("Float", Float) = 0
        [HideInInspector][Enum(UnityEditor.ShaderGraph.BlendOp)]_Fullscreen_AlphaBlendOperation("Float", Float) = 0
        [HideInInspector][ToggleUI]_Fullscreen_DepthWrite("Boolean", Float) = 0
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            // RenderType: <None>
            // Queue: <None>
            // DisableBatching: <None>
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalFullscreenSubTarget"
            "LightingMode"="UseColorTexture"
        }
        Pass
        {
            Name "DrawProcedural"
        
        // Render State
        Cull Off
        Blend [_Fullscreen_SrcColorBlend] [_Fullscreen_DstColorBlend], [_Fullscreen_SrcAlphaBlend] [_Fullscreen_DstAlphaBlend]
        BlendOp [_Fullscreen_ColorBlendOperation], [_Fullscreen_AlphaBlendOperation]
        ZTest Off
        ZWrite [_Fullscreen_DepthWrite]
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 3.0
        #pragma vertex vert
        #pragma fragment frag
        // #pragma enable_d3d11_debug_symbols
        
        /* WARNING: $splice Could not find named fragment 'DotsInstancingOptions' */
        /* WARNING: $splice Could not find named fragment 'HybridV1InjectedBuiltinProperties' */
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        #define FULLSCREEN_SHADERGRAPH
        
        // Defines
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_VERTEXID
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD1
        
        // Force depth texture because we need it for almost every nodes
        // TODO: dependency system that triggers this define from position or view direction usage
        #define REQUIRE_DEPTH_TEXTURE
        #define REQUIRE_NORMAL_TEXTURE
        
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DRAWPROCEDURAL
        #define REQUIRE_OPAQUE_TEXTURE
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/Fullscreen/Includes/FullscreenShaderPass.cs.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
        #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
             uint vertexID : VERTEXID_SEMANTIC;
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float3 TimeParameters;
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
             float4 texCoord1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
        };
        struct VertexDescriptionInputs
        {
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 texCoord1 : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.texCoord1.xyzw = input.texCoord1;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.texCoord1 = input.texCoord1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Blend;
        float _NoiseScale;
        float _PanSpeed;
        float _1;
        CBUFFER_END
        
        
        // Object and Global properties
        float _FlipY;
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
        
        // Graph Functions
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        float Unity_SimpleNoise_ValueNoise_Deterministic_float (float2 uv)
        {
            float2 i = floor(uv);
            float2 f = frac(uv);
            f = f * f * (3.0 - 2.0 * f);
            uv = abs(frac(uv) - 0.5);
            float2 c0 = i + float2(0.0, 0.0);
            float2 c1 = i + float2(1.0, 0.0);
            float2 c2 = i + float2(0.0, 1.0);
            float2 c3 = i + float2(1.0, 1.0);
            float r0; Hash_Tchou_2_1_float(c0, r0);
            float r1; Hash_Tchou_2_1_float(c1, r1);
            float r2; Hash_Tchou_2_1_float(c2, r2);
            float r3; Hash_Tchou_2_1_float(c3, r3);
            float bottomOfGrid = lerp(r0, r1, f.x);
            float topOfGrid = lerp(r2, r3, f.x);
            float t = lerp(bottomOfGrid, topOfGrid, f.y);
            return t;
        }
        
        void Unity_SimpleNoise_Deterministic_float(float2 UV, float Scale, out float Out)
        {
            float freq, amp;
            Out = 0.0f;
            freq = pow(2.0, float(0));
            amp = pow(0.5, float(3-0));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
            freq = pow(2.0, float(1));
            amp = pow(0.5, float(3-1));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
            freq = pow(2.0, float(2));
            amp = pow(0.5, float(3-2));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_SceneColor_float(float4 UV, out float3 Out)
        {
            Out = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV.xy);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        // GraphVertex: <None>
        
        // Custom interpolators, pre surface
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreSurface' */
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _UV_e5c8e2dab9c44c72b2f3aa4adbe9bfd0_Out_0_Vector4 = IN.uv0;
            float _Property_e386e2964f804c79bc75c024a0f6eaad_Out_0_Float = _PanSpeed;
            float _Multiply_12db7a0c31fa41da8064b2aae6e61f61_Out_2_Float;
            Unity_Multiply_float_float(_Property_e386e2964f804c79bc75c024a0f6eaad_Out_0_Float, 0.01, _Multiply_12db7a0c31fa41da8064b2aae6e61f61_Out_2_Float);
            float _Multiply_a169cf2ff670458e85f36344f0cda5e8_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Multiply_12db7a0c31fa41da8064b2aae6e61f61_Out_2_Float, _Multiply_a169cf2ff670458e85f36344f0cda5e8_Out_2_Float);
            float _Sine_27f0287123064b04a7065cce6e1555ff_Out_1_Float;
            Unity_Sine_float(_Multiply_a169cf2ff670458e85f36344f0cda5e8_Out_2_Float, _Sine_27f0287123064b04a7065cce6e1555ff_Out_1_Float);
            float _Property_3829064020d5499aafaf9b61691b7bf1_Out_0_Float = _1;
            float _Multiply_97ae44a5a4024af6886077ed8da626ab_Out_2_Float;
            Unity_Multiply_float_float(_Sine_27f0287123064b04a7065cce6e1555ff_Out_1_Float, _Property_3829064020d5499aafaf9b61691b7bf1_Out_0_Float, _Multiply_97ae44a5a4024af6886077ed8da626ab_Out_2_Float);
            float2 _TilingAndOffset_a2997360d0da43ec93d430941596243a_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), (_Multiply_97ae44a5a4024af6886077ed8da626ab_Out_2_Float.xx), _TilingAndOffset_a2997360d0da43ec93d430941596243a_Out_3_Vector2);
            float _Property_046179f2958a4b52b28d2d5f13d6de96_Out_0_Float = _NoiseScale;
            float _SimpleNoise_f2b49f1e7ee94ab7878b02ec32fe530e_Out_2_Float;
            Unity_SimpleNoise_Deterministic_float(_TilingAndOffset_a2997360d0da43ec93d430941596243a_Out_3_Vector2, _Property_046179f2958a4b52b28d2d5f13d6de96_Out_0_Float, _SimpleNoise_f2b49f1e7ee94ab7878b02ec32fe530e_Out_2_Float);
            float _Property_3b16534708084be8a54824a5f2b36721_Out_0_Float = _Blend;
            float4 _Lerp_dc748febab55442ab744f58c5e788e80_Out_3_Vector4;
            Unity_Lerp_float4(_UV_e5c8e2dab9c44c72b2f3aa4adbe9bfd0_Out_0_Vector4, (_SimpleNoise_f2b49f1e7ee94ab7878b02ec32fe530e_Out_2_Float.xxxx), (_Property_3b16534708084be8a54824a5f2b36721_Out_0_Float.xxxx), _Lerp_dc748febab55442ab744f58c5e788e80_Out_3_Vector4);
            float3 _SceneColor_1e7e20ca650c49579bd2b747c8bc0894_Out_1_Vector3;
            Unity_SceneColor_float(_Lerp_dc748febab55442ab744f58c5e788e80_Out_3_Vector4, _SceneColor_1e7e20ca650c49579bd2b747c8bc0894_Out_1_Vector3);
            surface.BaseColor = _SceneColor_1e7e20ca650c49579bd2b747c8bc0894_Out_1_Vector3;
            surface.Alpha = 1;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            float3 normalWS = SHADERGRAPH_SAMPLE_SCENE_NORMAL(input.texCoord0.xy);
            float4 tangentWS = float4(0, 1, 0, 0); // We can't access the tangent in screen space
        
        
        
        
            float3 viewDirWS = normalize(input.texCoord1.xyz);
            float linearDepth = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(input.texCoord0.xy), _ZBufferParams);
            float3 cameraForward = -UNITY_MATRIX_V[2].xyz;
            float camearDistance = linearDepth / dot(viewDirWS, cameraForward);
            float3 positionWS = viewDirWS * camearDistance + GetCameraPositionWS();
        
        
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
        
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/Fullscreen/Includes/FullscreenCommon.hlsl"
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/Fullscreen/Includes/FullscreenDrawProcedural.hlsl"
        
        ENDHLSL
        }
        Pass
        {
            Name "Blit"
        
        // Render State
        Cull Off
        Blend [_Fullscreen_SrcColorBlend] [_Fullscreen_DstColorBlend], [_Fullscreen_SrcAlphaBlend] [_Fullscreen_DstAlphaBlend]
        BlendOp [_Fullscreen_ColorBlendOperation], [_Fullscreen_AlphaBlendOperation]
        ZTest Off
        ZWrite [_Fullscreen_DepthWrite]
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 3.0
        #pragma vertex vert
        #pragma fragment frag
        // #pragma enable_d3d11_debug_symbols
        
        /* WARNING: $splice Could not find named fragment 'DotsInstancingOptions' */
        /* WARNING: $splice Could not find named fragment 'HybridV1InjectedBuiltinProperties' */
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        #define FULLSCREEN_SHADERGRAPH
        
        // Defines
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_VERTEXID
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD1
        
        // Force depth texture because we need it for almost every nodes
        // TODO: dependency system that triggers this define from position or view direction usage
        #define REQUIRE_DEPTH_TEXTURE
        #define REQUIRE_NORMAL_TEXTURE
        
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_BLIT
        #define REQUIRE_OPAQUE_TEXTURE
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/Fullscreen/Includes/FullscreenShaderPass.cs.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
        #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
             uint vertexID : VERTEXID_SEMANTIC;
             float3 positionOS : POSITION;
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float3 TimeParameters;
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
             float4 texCoord1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
        };
        struct VertexDescriptionInputs
        {
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 texCoord1 : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.texCoord1.xyzw = input.texCoord1;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.texCoord1 = input.texCoord1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Blend;
        float _NoiseScale;
        float _PanSpeed;
        float _1;
        CBUFFER_END
        
        
        // Object and Global properties
        float _FlipY;
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
        
        // Graph Functions
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        float Unity_SimpleNoise_ValueNoise_Deterministic_float (float2 uv)
        {
            float2 i = floor(uv);
            float2 f = frac(uv);
            f = f * f * (3.0 - 2.0 * f);
            uv = abs(frac(uv) - 0.5);
            float2 c0 = i + float2(0.0, 0.0);
            float2 c1 = i + float2(1.0, 0.0);
            float2 c2 = i + float2(0.0, 1.0);
            float2 c3 = i + float2(1.0, 1.0);
            float r0; Hash_Tchou_2_1_float(c0, r0);
            float r1; Hash_Tchou_2_1_float(c1, r1);
            float r2; Hash_Tchou_2_1_float(c2, r2);
            float r3; Hash_Tchou_2_1_float(c3, r3);
            float bottomOfGrid = lerp(r0, r1, f.x);
            float topOfGrid = lerp(r2, r3, f.x);
            float t = lerp(bottomOfGrid, topOfGrid, f.y);
            return t;
        }
        
        void Unity_SimpleNoise_Deterministic_float(float2 UV, float Scale, out float Out)
        {
            float freq, amp;
            Out = 0.0f;
            freq = pow(2.0, float(0));
            amp = pow(0.5, float(3-0));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
            freq = pow(2.0, float(1));
            amp = pow(0.5, float(3-1));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
            freq = pow(2.0, float(2));
            amp = pow(0.5, float(3-2));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_SceneColor_float(float4 UV, out float3 Out)
        {
            Out = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV.xy);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        // GraphVertex: <None>
        
        // Custom interpolators, pre surface
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreSurface' */
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _UV_e5c8e2dab9c44c72b2f3aa4adbe9bfd0_Out_0_Vector4 = IN.uv0;
            float _Property_e386e2964f804c79bc75c024a0f6eaad_Out_0_Float = _PanSpeed;
            float _Multiply_12db7a0c31fa41da8064b2aae6e61f61_Out_2_Float;
            Unity_Multiply_float_float(_Property_e386e2964f804c79bc75c024a0f6eaad_Out_0_Float, 0.01, _Multiply_12db7a0c31fa41da8064b2aae6e61f61_Out_2_Float);
            float _Multiply_a169cf2ff670458e85f36344f0cda5e8_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Multiply_12db7a0c31fa41da8064b2aae6e61f61_Out_2_Float, _Multiply_a169cf2ff670458e85f36344f0cda5e8_Out_2_Float);
            float _Sine_27f0287123064b04a7065cce6e1555ff_Out_1_Float;
            Unity_Sine_float(_Multiply_a169cf2ff670458e85f36344f0cda5e8_Out_2_Float, _Sine_27f0287123064b04a7065cce6e1555ff_Out_1_Float);
            float _Property_3829064020d5499aafaf9b61691b7bf1_Out_0_Float = _1;
            float _Multiply_97ae44a5a4024af6886077ed8da626ab_Out_2_Float;
            Unity_Multiply_float_float(_Sine_27f0287123064b04a7065cce6e1555ff_Out_1_Float, _Property_3829064020d5499aafaf9b61691b7bf1_Out_0_Float, _Multiply_97ae44a5a4024af6886077ed8da626ab_Out_2_Float);
            float2 _TilingAndOffset_a2997360d0da43ec93d430941596243a_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), (_Multiply_97ae44a5a4024af6886077ed8da626ab_Out_2_Float.xx), _TilingAndOffset_a2997360d0da43ec93d430941596243a_Out_3_Vector2);
            float _Property_046179f2958a4b52b28d2d5f13d6de96_Out_0_Float = _NoiseScale;
            float _SimpleNoise_f2b49f1e7ee94ab7878b02ec32fe530e_Out_2_Float;
            Unity_SimpleNoise_Deterministic_float(_TilingAndOffset_a2997360d0da43ec93d430941596243a_Out_3_Vector2, _Property_046179f2958a4b52b28d2d5f13d6de96_Out_0_Float, _SimpleNoise_f2b49f1e7ee94ab7878b02ec32fe530e_Out_2_Float);
            float _Property_3b16534708084be8a54824a5f2b36721_Out_0_Float = _Blend;
            float4 _Lerp_dc748febab55442ab744f58c5e788e80_Out_3_Vector4;
            Unity_Lerp_float4(_UV_e5c8e2dab9c44c72b2f3aa4adbe9bfd0_Out_0_Vector4, (_SimpleNoise_f2b49f1e7ee94ab7878b02ec32fe530e_Out_2_Float.xxxx), (_Property_3b16534708084be8a54824a5f2b36721_Out_0_Float.xxxx), _Lerp_dc748febab55442ab744f58c5e788e80_Out_3_Vector4);
            float3 _SceneColor_1e7e20ca650c49579bd2b747c8bc0894_Out_1_Vector3;
            Unity_SceneColor_float(_Lerp_dc748febab55442ab744f58c5e788e80_Out_3_Vector4, _SceneColor_1e7e20ca650c49579bd2b747c8bc0894_Out_1_Vector3);
            surface.BaseColor = _SceneColor_1e7e20ca650c49579bd2b747c8bc0894_Out_1_Vector3;
            surface.Alpha = 1;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            float3 normalWS = SHADERGRAPH_SAMPLE_SCENE_NORMAL(input.texCoord0.xy);
            float4 tangentWS = float4(0, 1, 0, 0); // We can't access the tangent in screen space
        
        
        
        
            float3 viewDirWS = normalize(input.texCoord1.xyz);
            float linearDepth = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(input.texCoord0.xy), _ZBufferParams);
            float3 cameraForward = -UNITY_MATRIX_V[2].xyz;
            float camearDistance = linearDepth / dot(viewDirWS, cameraForward);
            float3 positionWS = viewDirWS * camearDistance + GetCameraPositionWS();
        
        
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
        
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/Fullscreen/Includes/FullscreenCommon.hlsl"
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/Fullscreen/Includes/FullscreenBlit.hlsl"
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.Rendering.Fullscreen.ShaderGraph.FullscreenShaderGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}