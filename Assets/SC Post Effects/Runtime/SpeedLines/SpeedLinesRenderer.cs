#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
#if URP
    public class SpeedLinesRenderer : ScriptableRendererFeature
    {
        class SpeedLinesRenderPass : PostEffectRenderer<SpeedLines>
        {
            public SpeedLinesRenderPass()
            {
                shaderName = ShaderNames.SpeedLines;
                ProfilerTag = this.ToString();
            }

            public void Setup(RenderTargetIdentifier cameraColorTarget)
            {
                this.cameraColorTarget = cameraColorTarget;
                volumeSettings = VolumeManager.instance.stack.GetComponent<SpeedLines>();
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (!volumeSettings) return;

                base.Configure(cmd, cameraTextureDescriptor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (!volumeSettings) return;
                if (volumeSettings.IsActive() == false) return;

                var cmd = CommandBufferPool.Get(ProfilerTag);

                CopyTargets(cmd);

                float falloff = 2f + (volumeSettings.falloff.value - 0.0f) * (16.0f - 2f) / (1.0f - 0.0f);
                Material.SetVector("_Params", new Vector4(volumeSettings.intensity.value, falloff, volumeSettings.size.value * 2, 0));
                if (volumeSettings.noiseTex.value) Material.SetTexture("_NoiseTex", volumeSettings.noiseTex.value);

                FinalBlit(this, context, cmd, mainTexID.id, cameraColorTarget, Material, 0);
            }
        }

        SpeedLinesRenderPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new SpeedLinesRenderPass();

            // Configures where the render pass should be injected.
            m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.Setup(renderer.cameraColorTarget);
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }
#endif
}