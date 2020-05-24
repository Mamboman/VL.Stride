﻿using Stride.Particles.Rendering;
using Stride.Rendering;
using Stride.Rendering.Background;
using Stride.Rendering.Compositing;
using Stride.Rendering.Images;
using Stride.Rendering.Lights;
using Stride.Rendering.Materials;
using Stride.Rendering.Shadows;
using Stride.Rendering.Sprites;
using Stride.Rendering.SubsurfaceScattering;
using System;
using System.Collections.Generic;
using VL.Core;

namespace VL.Stride.Rendering.Composition
{
    static class CompositionNodes
    {
        public static IEnumerable<IVLNodeDescription> GetNodeDescriptions(StrideNodeFactory nodeFactory)
        {
            string renderingCategory = "Stride.Rendering";

            string compositionCategory = $"{renderingCategory}.Composition";
            yield return nodeFactory.Create<GraphicsCompositor>(category: compositionCategory)
                .AddInput(nameof(GraphicsCompositor.Game), x => x.Game, (x, v) => x.Game = v)
                .AddListInput(nameof(GraphicsCompositor.RenderStages), x => x.RenderStages)
                .AddListInput(nameof(GraphicsCompositor.RenderFeatures), x => x.RenderFeatures);

            yield return nodeFactory.Create<RenderStage>(category: compositionCategory)
                .AddInput(nameof(RenderStage.Name), x => x.Name, (x, v) => x.Name = v, defaultValue: "RenderStage")
                .AddInput(nameof(RenderStage.EffectSlotName), x => x.EffectSlotName, (x, v) => x.EffectSlotName = v, defaultValue: "Main")
                .AddInput(nameof(RenderStage.SortMode), x => x.SortMode.ToPredefinedSortMode(), (x, v) => x.SortMode = v.ToSortMode());

            // Render stage selectors
            string renderStageSelectorCategory = $"{compositionCategory}.RenderStageSelector";
            yield return nodeFactory.Create<MeshTransparentRenderStageSelector>(category: renderStageSelectorCategory)
                .AddInput(nameof(MeshTransparentRenderStageSelector.EffectName), x => x.EffectName, (x, v) => x.EffectName = v, defaultValue: "StrideForwardShadingEffect")
                .AddInput(nameof(MeshTransparentRenderStageSelector.RenderGroup), x => x.RenderGroup, (x, v) => x.RenderGroup = v, RenderGroupMask.All)
                .AddInput(nameof(MeshTransparentRenderStageSelector.OpaqueRenderStage), x => x.OpaqueRenderStage, (x, v) => x.OpaqueRenderStage = v)
                .AddInput(nameof(MeshTransparentRenderStageSelector.TransparentRenderStage), x => x.TransparentRenderStage, (x, v) => x.TransparentRenderStage = v);

            yield return nodeFactory.Create<ShadowMapRenderStageSelector>(category: renderStageSelectorCategory)
                .AddInput(nameof(ShadowMapRenderStageSelector.EffectName), x => x.EffectName, (x, v) => x.EffectName = v, defaultValue: "StrideForwardShadingEffect.ShadowMapCaster")
                .AddInput(nameof(ShadowMapRenderStageSelector.RenderGroup), x => x.RenderGroup, (x, v) => x.RenderGroup = v, RenderGroupMask.All)
                .AddInput(nameof(ShadowMapRenderStageSelector.ShadowMapRenderStage), x => x.ShadowMapRenderStage, (x, v) => x.ShadowMapRenderStage = v);

            yield return nodeFactory.Create<SimpleGroupToRenderStageSelector>(category: renderStageSelectorCategory)
                .AddInput(nameof(SimpleGroupToRenderStageSelector.EffectName), x => x.EffectName, (x, v) => x.EffectName = v, defaultValue: "Test")
                .AddInput(nameof(SimpleGroupToRenderStageSelector.RenderGroup), x => x.RenderGroup, (x, v) => x.RenderGroup = v, RenderGroupMask.All)
                .AddInput(nameof(SimpleGroupToRenderStageSelector.RenderStage), x => x.RenderStage, (x, v) => x.RenderStage = v);

            yield return nodeFactory.Create<ParticleEmitterTransparentRenderStageSelector>(category: renderStageSelectorCategory)
                .AddInput(nameof(ParticleEmitterTransparentRenderStageSelector.EffectName), x => x.EffectName, (x, v) => x.EffectName = v, defaultValue: "Particles")
                .AddInput(nameof(ParticleEmitterTransparentRenderStageSelector.RenderGroup), x => x.RenderGroup, (x, v) => x.RenderGroup = v, RenderGroupMask.All)
                .AddInput(nameof(ParticleEmitterTransparentRenderStageSelector.OpaqueRenderStage), x => x.OpaqueRenderStage, (x, v) => x.OpaqueRenderStage = v)
                .AddInput(nameof(ParticleEmitterTransparentRenderStageSelector.TransparentRenderStage), x => x.TransparentRenderStage, (x, v) => x.TransparentRenderStage = v);

            yield return nodeFactory.Create<SpriteTransparentRenderStageSelector>(category: renderStageSelectorCategory)
                .AddInput(nameof(SpriteTransparentRenderStageSelector.EffectName), x => x.EffectName, (x, v) => x.EffectName = v, defaultValue: "Test")
                .AddInput(nameof(SpriteTransparentRenderStageSelector.RenderGroup), x => x.RenderGroup, (x, v) => x.RenderGroup = v, RenderGroupMask.All)
                .AddInput(nameof(SpriteTransparentRenderStageSelector.OpaqueRenderStage), x => x.OpaqueRenderStage, (x, v) => x.OpaqueRenderStage = v)
                .AddInput(nameof(SpriteTransparentRenderStageSelector.TransparentRenderStage), x => x.TransparentRenderStage, (x, v) => x.TransparentRenderStage = v);

            // Renderers
            yield return new StrideNodeDesc<ClearRenderer>(nodeFactory, category: compositionCategory) { CopyOnWrite = false };

            yield return nodeFactory.Create<DebugRenderer>(category: compositionCategory, copyOnWrite: false)
                .AddListInput(nameof(DebugRenderer.DebugRenderStages), x => x.DebugRenderStages);

            yield return nodeFactory.Create<ForceAspectRatioSceneRenderer>(category: compositionCategory, copyOnWrite: false)
                .AddInput(nameof(ForceAspectRatioSceneRenderer.FixedAspectRatio), x => x.FixedAspectRatio, (x, v) => x.FixedAspectRatio = v)
                .AddInput(nameof(ForceAspectRatioSceneRenderer.ForceAspectRatio), x => x.ForceAspectRatio, (x, v) => x.ForceAspectRatio = v)
                .AddInput(nameof(ForceAspectRatioSceneRenderer.Child), x => x.Child, (x, v) => x.Child = v);

            yield return nodeFactory.Create<SceneExternalCameraRenderer>(name: "CameraRenderer", category: compositionCategory, copyOnWrite: false)
                .AddInput("Camera", x => x.ExternalCamera, (x, v) => x.ExternalCamera = v)
                .AddInput(nameof(SceneExternalCameraRenderer.Child), x => x.Child, (x, v) => x.Child = v);

            yield return nodeFactory.Create<RenderTextureSceneRenderer>(category: compositionCategory, copyOnWrite: false)
                .AddInput(nameof(RenderTextureSceneRenderer.RenderTexture), x => x.RenderTexture, (x, v) => x.RenderTexture = v)
                .AddInput(nameof(RenderTextureSceneRenderer.Child), x => x.Child, (x, v) => x.Child = v);

            yield return nodeFactory.Create<SceneRendererCollection>(category: compositionCategory, copyOnWrite: false)
                .AddListInput(nameof(SceneRendererCollection.Children), x => x.Children);

            var defaultResolver = new MSAAResolver();
            yield return nodeFactory.Create<ForwardRenderer>(category: compositionCategory, copyOnWrite: false)
                .AddInput(nameof(ForwardRenderer.Clear), x => x.Clear, (x, v) => x.Clear = v)
                .AddInput(nameof(ForwardRenderer.OpaqueRenderStage), x => x.OpaqueRenderStage, (x, v) => x.OpaqueRenderStage = v)
                .AddInput(nameof(ForwardRenderer.TransparentRenderStage), x => x.TransparentRenderStage, (x, v) => x.TransparentRenderStage = v)
                .AddListInput(nameof(ForwardRenderer.ShadowMapRenderStages), x => x.ShadowMapRenderStages)
                .AddInput(nameof(ForwardRenderer.GBufferRenderStage), x => x.GBufferRenderStage, (x, v) => x.GBufferRenderStage = v)
                .AddInput(nameof(ForwardRenderer.PostEffects), x => x.PostEffects, (x, v) => x.PostEffects = v)
                .AddInput(nameof(ForwardRenderer.LightShafts), x => x.LightShafts, (x, v) => x.LightShafts = v)
                .AddInput(nameof(ForwardRenderer.VRSettings), x => x.VRSettings, (x, v) => x.VRSettings = v)
                .AddInput(nameof(ForwardRenderer.SubsurfaceScatteringBlurEffect), x => x.SubsurfaceScatteringBlurEffect, (x, v) => x.SubsurfaceScatteringBlurEffect = v)
                .AddInput(nameof(ForwardRenderer.MSAALevel), x => x.MSAALevel, (x, v) => x.MSAALevel = v)
                .AddInput(nameof(ForwardRenderer.MSAAResolver), x => x.MSAAResolver, (x, v) =>
                {
                    var s = x.MSAAResolver;
                    var y = v ?? defaultResolver;
                    s.FilterType = y.FilterType;
                    s.FilterRadius = y.FilterRadius;
                })
                .AddInput(nameof(ForwardRenderer.BindDepthAsResourceDuringTransparentRendering), x => x.BindDepthAsResourceDuringTransparentRendering, (x, v) => x.BindDepthAsResourceDuringTransparentRendering = v);


            yield return new StrideNodeDesc<SubsurfaceScatteringBlur>(nodeFactory, category: compositionCategory);
            yield return new StrideNodeDesc<VRRendererSettings>(nodeFactory, category: compositionCategory);
            yield return new StrideNodeDesc<LightShafts>(nodeFactory, category: compositionCategory);
            yield return new StrideNodeDesc<MSAAResolver>(nodeFactory, category: compositionCategory);

            // Post processing
            var postFxCategory = $"{renderingCategory}.PostFX";
            yield return CreatePostEffectsNode();

            yield return new StrideNodeDesc<AmbientOcclusion>(nodeFactory, category: postFxCategory);
            yield return new StrideNodeDesc<LocalReflections>(nodeFactory, category: postFxCategory);
            yield return new StrideNodeDesc<DepthOfField>(nodeFactory, category: postFxCategory);
            yield return new StrideNodeDesc<BrightFilter>(nodeFactory, category: postFxCategory);
            yield return new StrideNodeDesc<Bloom>(nodeFactory, category: postFxCategory);
            yield return new StrideNodeDesc<LightStreak>(nodeFactory, category: postFxCategory);
            yield return new StrideNodeDesc<LensFlare>(nodeFactory, category: postFxCategory);
            // AA
            yield return new StrideNodeDesc<FXAAEffect>(nodeFactory, category: postFxCategory);
            yield return new StrideNodeDesc<TemporalAntiAliasEffect>(nodeFactory, category: postFxCategory);

            // Root render features
            yield return nodeFactory.Create<MeshRenderFeature>(category: renderingCategory)
                .AddListInput(nameof(MeshRenderFeature.RenderFeatures), x => x.RenderFeatures)
                .AddListInput(nameof(MeshRenderFeature.RenderStageSelectors), x => x.RenderStageSelectors)
                .AddListInput(nameof(MeshRenderFeature.PipelineProcessors), x => x.PipelineProcessors);

            yield return nodeFactory.Create<BackgroundRenderFeature>(category: renderingCategory)
                .AddListInput(nameof(BackgroundRenderFeature.RenderStageSelectors), x => x.RenderStageSelectors);

            yield return nodeFactory.Create<SpriteRenderFeature>(category: renderingCategory)
                .AddListInput(nameof(SpriteRenderFeature.RenderStageSelectors), x => x.RenderStageSelectors);

            yield return nodeFactory.Create<LayerRenderFeature>(category: renderingCategory)
                .AddListInput(nameof(LayerRenderFeature.RenderStageSelectors), x => x.RenderStageSelectors);

            // Sub render features for mesh render feature
            var renderFeaturesCategory = $"{renderingCategory}.RenderFeatures";
            yield return new StrideNodeDesc<TransformRenderFeature>(nodeFactory, category: renderFeaturesCategory);
            yield return new StrideNodeDesc<SkinningRenderFeature>(nodeFactory, category: renderFeaturesCategory);
            yield return new StrideNodeDesc<MaterialRenderFeature>(nodeFactory, category: renderFeaturesCategory);
            yield return new StrideNodeDesc<ShadowCasterRenderFeature>(nodeFactory, category: renderFeaturesCategory);

            yield return nodeFactory.Create<ForwardLightingRenderFeature>(category: renderFeaturesCategory)
                .AddListInput(nameof(ForwardLightingRenderFeature.LightRenderers), x => x.LightRenderers)
                .AddInput(nameof(ForwardLightingRenderFeature.ShadowMapRenderer), x => x.ShadowMapRenderer, (x, v) => x.ShadowMapRenderer = v);

            var pipelineProcessorsCategory = $"{renderingCategory}.PipelineProcessors";
            yield return nodeFactory.Create<MeshPipelineProcessor>(category: pipelineProcessorsCategory)
                .AddInput(nameof(MeshPipelineProcessor.TransparentRenderStage), x => x.TransparentRenderStage, (x, v) => x.TransparentRenderStage = v);

            yield return nodeFactory.Create<ShadowMeshPipelineProcessor>(category: pipelineProcessorsCategory)
                .AddInput(nameof(ShadowMeshPipelineProcessor.ShadowMapRenderStage), x => x.ShadowMapRenderStage, (x, v) => x.ShadowMapRenderStage = v)
                .AddInput(nameof(ShadowMeshPipelineProcessor.DepthClipping), x => x.DepthClipping, (x, v) => x.DepthClipping = v);

            yield return nodeFactory.Create<WireframePipelineProcessor>(category: pipelineProcessorsCategory)
                .AddInput(nameof(WireframePipelineProcessor.RenderStage), x => x.RenderStage, (x, v) => x.RenderStage = v);

            // Light renderers - make enum
            var lightsCategory = $"{renderingCategory}.Lights";
            yield return new StrideNodeDesc<LightAmbientRenderer>(nodeFactory, category: lightsCategory);
            yield return new StrideNodeDesc<LightSkyboxRenderer>(nodeFactory, category: lightsCategory);
            yield return new StrideNodeDesc<LightDirectionalGroupRenderer>(nodeFactory, category: lightsCategory);
            yield return new StrideNodeDesc<LightPointGroupRenderer>(nodeFactory, category: lightsCategory);
            yield return new StrideNodeDesc<LightSpotGroupRenderer>(nodeFactory, category: lightsCategory);
            yield return new StrideNodeDesc<LightClusteredPointSpotGroupRenderer>(nodeFactory, category: lightsCategory);

            // Shadow map renderers
            var shadowsCategory = $"{renderingCategory}.Shadows";
            yield return nodeFactory.Create<ShadowMapRenderer>(category: shadowsCategory)
                .AddListInput(nameof(ShadowMapRenderer.Renderers), x => x.Renderers);

            yield return new StrideNodeDesc<LightDirectionalShadowMapRenderer>(nodeFactory, category: shadowsCategory);
            yield return new StrideNodeDesc<LightSpotShadowMapRenderer>(nodeFactory, category: shadowsCategory);
            yield return new StrideNodeDesc<LightPointShadowMapRendererParaboloid>(nodeFactory, category: shadowsCategory);
            yield return new StrideNodeDesc<LightPointShadowMapRendererCubeMap>(nodeFactory, category: shadowsCategory);

            IVLNodeDescription CreatePostEffectsNode()
            {
                return nodeFactory.Create<PostProcessingEffects>(category: postFxCategory, copyOnWrite: false)
                    .AddInput(nameof(PostProcessingEffects.AmbientOcclusion), x => x.AmbientOcclusion, (x, v) =>
                    {
                        var s = x.AmbientOcclusion;
                        if (v != null)
                        {
                            s.Enabled = v.Enabled;
                            s.NumberOfSamples = v.NumberOfSamples;
                            s.ParamProjScale = v.ParamProjScale;
                            s.ParamIntensity = v.ParamIntensity;
                            s.ParamBias = v.ParamBias;
                            s.ParamRadius = v.ParamRadius;
                            s.NumberOfBounces = v.NumberOfBounces;
                            s.BlurScale = v.BlurScale;
                            s.EdgeSharpness = v.EdgeSharpness;
                            s.TempSize = v.TempSize;
                        }
                        else
                        {
                            s.Enabled = false;
                        }
                    })
                    .AddInput(nameof(PostProcessingEffects.LocalReflections), x => x.LocalReflections, (x, v) =>
                    {
                        var s = x.LocalReflections;
                        if (v != null)
                        {
                            s.Enabled = v.Enabled;
                            s.DepthResolution = v.DepthResolution;
                            s.RayTracePassResolution = v.RayTracePassResolution;
                            s.MaxSteps = v.MaxSteps;
                            s.BRDFBias = v.BRDFBias;
                            s.GlossinessThreshold = v.GlossinessThreshold;
                            s.WorldAntiSelfOcclusionBias = v.WorldAntiSelfOcclusionBias;
                            s.ResolvePassResolution = v.ResolvePassResolution;
                            s.ResolveSamples = v.ResolveSamples;
                            s.ReduceHighlights = v.ReduceHighlights;
                            s.EdgeFadeFactor = v.EdgeFadeFactor;
                            s.UseColorBufferMips = v.UseColorBufferMips;
                            s.TemporalEffect = v.TemporalEffect;
                            s.TemporalScale = v.TemporalScale;
                            s.TemporalResponse = v.TemporalResponse;
                        }
                        else
                        {
                            s.Enabled = false;
                        }
                    })
                    .AddInput(nameof(PostProcessingEffects.DepthOfField), x => x.DepthOfField, (x, v) =>
                    {
                        var s = x.DepthOfField;
                        if (v != null)
                        {
                            s.Enabled = v.Enabled;
                            s.MaxBokehSize = v.MaxBokehSize;
                            s.DOFAreas = v.DOFAreas;
                            s.QualityPreset = v.QualityPreset;
                            s.Technique = v.Technique;
                            s.AutoFocus = v.AutoFocus;
                        }
                        else
                        {
                            s.Enabled = false;
                        }
                    })
                    .AddInput(nameof(PostProcessingEffects.BrightFilter), x => x.BrightFilter, (x, v) =>
                    {
                        var s = x.BrightFilter;
                        if (v != null)
                        {
                            s.Enabled = v.Enabled;
                            s.Threshold = v.Threshold;
                            s.Steepness = v.Steepness;
                            s.Color = v.Color;
                        }
                        else
                        {
                            s.Enabled = false;
                        }
                    })
                    .AddInput(nameof(PostProcessingEffects.Bloom), x => x.Bloom, (x, v) =>
                    {
                        var s = x.Bloom;
                        if (v != null)
                        {
                            s.Enabled = v.Enabled;
                            s.Radius = v.Radius;
                            s.Amount = v.Amount;
                            s.DownScale = v.DownScale;
                            s.SigmaRatio = v.SigmaRatio;
                            s.Distortion = v.Distortion;
                            s.Afterimage.Enabled = v.Afterimage.Enabled;
                            s.Afterimage.FadeOutSpeed = v.Afterimage.FadeOutSpeed;
                            s.Afterimage.Sensitivity = v.Afterimage.Sensitivity;
                        }
                        else
                        {
                            s.Enabled = false;
                        }
                    })
                    .AddInput(nameof(PostProcessingEffects.LightStreak), x => x.LightStreak, (x, v) =>
                    {
                        var s = x.LightStreak;
                        if (v != null)
                        {
                            s.Enabled = v.Enabled;
                            s.Amount = v.Amount;
                            s.StreakCount = v.StreakCount;
                            s.Attenuation = v.Attenuation;
                            s.Phase = v.Phase;
                            s.ColorAberrationStrength = v.ColorAberrationStrength;
                            s.IsAnamorphic = v.IsAnamorphic;
                        }
                        else
                        {
                            s.Enabled = false;
                        }
                    })
                    .AddInput(nameof(PostProcessingEffects.LensFlare), x => x.LensFlare, (x, v) =>
                    {
                        var s = x.LensFlare;
                        if (v != null)
                        {
                            s.Enabled = v.Enabled;
                            s.Amount = v.Amount;
                            s.ColorAberrationStrength = v.ColorAberrationStrength;
                            s.HaloFactor = v.HaloFactor;
                        }
                        else
                        {
                            s.Enabled = false;
                        }
                    })
                    .AddListInput(nameof(PostProcessingEffects.ColorTransforms), x => x.ColorTransforms.Transforms)
                    .AddInput(nameof(PostProcessingEffects.Antialiasing), x => x.Antialiasing, (x, v) => x.Antialiasing = v);
            }
        }
    }
}
