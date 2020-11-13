#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
#if URP
    public class RefractionRenderer : ScriptableRendererFeature
    {
        class RefractionRenderPass : PostEffectRenderer<Refraction>
        {
            public RefractionRenderPass()
            {
                shaderName = ShaderNames.Refraction;
                ProfilerTag = this.ToString();
            }

            public void Setup(RenderTargetIdentifier cameraColorTarget)
            {
                this.cameraColorTarget = cameraColorTarget;
                volumeSettings = VolumeManager.instance.stack.GetComponent<Refraction>();
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

                Material.SetFloat("_Amount", volumeSettings.amount.value);
                if (volumeSettings.refractionTex.value) Material.SetTexture("_RefractionTex", volumeSettings.refractionTex.value);

                FinalBlit(this, context, cmd, mainTexID.id, cameraColorTarget, Material, (volumeSettings.convertNormalMap.value) ? 1 : 0);
            }
        }

        RefractionRenderPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new RefractionRenderPass();

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