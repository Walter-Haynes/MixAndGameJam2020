#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
#if URP
    public class DoubleVisionRenderer : ScriptableRendererFeature
    {
        class DoubleVisionRenderPass : PostEffectRenderer<DoubleVision>
        {
            public DoubleVisionRenderPass()
            {
                shaderName = ShaderNames.DoubleVision;
                ProfilerTag = this.ToString();
            }

            public void Setup(RenderTargetIdentifier cameraColorTarget)
            {
                this.cameraColorTarget = cameraColorTarget;
                volumeSettings = VolumeManager.instance.stack.GetComponent<DoubleVision>();
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

                Material.SetFloat("_Amount", volumeSettings.intensity.value / 10);

                FinalBlit(this, context, cmd, mainTexID.id, cameraColorTarget, Material, (int)volumeSettings.mode.value);
            }
        }

        DoubleVisionRenderPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new DoubleVisionRenderPass();

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