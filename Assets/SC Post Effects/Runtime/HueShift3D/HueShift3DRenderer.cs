#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
#if URP
    public class HueShift3DRenderer : ScriptableRendererFeature
    {
        class HueShift3DRenderPass : PostEffectRenderer<HueShift3D>
        {
            public HueShift3DRenderPass()
            {
                shaderName = ShaderNames.HueShift3D;
                ProfilerTag = this.ToString();
            }

            public void Setup(RenderTargetIdentifier cameraColorTarget, RenderTargetIdentifier cameraDepthTarget)
            {
                this.cameraColorTarget = cameraColorTarget;
                this.cameraDepthTarget = cameraDepthTarget;
                
                volumeSettings = VolumeManager.instance.stack.GetComponent<HueShift3D>();
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (!volumeSettings) return;

                requiresDepthNormals = volumeSettings.IsActive() && volumeSettings.geoInfluence.value > 0f;

                base.Configure(cmd, cameraTextureDescriptor);
            }
            
            enum Pass
            {
                ColorSpectrum,
                GradientTexture
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (!volumeSettings) return;
                if (!volumeSettings.IsActive()) return;

                var cmd = CommandBufferPool.Get(ProfilerTag);

                CopyTargets(cmd);

                HueShift3D.isOrtho = renderingData.cameraData.camera.orthographic;

                Material.SetVector("_Params", new Vector4(volumeSettings.speed.value, volumeSettings.size.value, volumeSettings.geoInfluence.value, volumeSettings.intensity.value));
                if(volumeSettings.gradientTex.value) Material.SetTexture("_GradientTex", volumeSettings.gradientTex.value);
                
                FinalBlit(this, context, cmd, mainTexID.id, cameraColorTarget, Material, volumeSettings.colorSource.value == (int)HueShift3D.ColorSource.RGBSpectrum ? (int)Pass.ColorSpectrum : (int)Pass.GradientTexture);
            }
        }

        HueShift3DRenderPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new HueShift3DRenderPass();

            // Configures where the render pass should be injected.
            m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.CheckForStackedRendering(renderer, renderingData.cameraData);
            m_ScriptablePass.Setup(renderer.cameraColorTarget, renderer.cameraDepth);
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }
#endif
}