﻿#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
#if URP
    public class ScanlinesRenderer : ScriptableRendererFeature
    {
        class ScanlinesRenderPass : PostEffectRenderer<Scanlines>
        {
            public ScanlinesRenderPass()
            {
                shaderName = ShaderNames.Scanlines;
                ProfilerTag = this.ToString();
            }

            public void Setup(RenderTargetIdentifier cameraColorTarget)
            {
                this.cameraColorTarget = cameraColorTarget;
                volumeSettings = VolumeManager.instance.stack.GetComponent<Scanlines>();
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

                Material.SetVector("_Params", new Vector4(volumeSettings.amount.value, volumeSettings.intensity.value / 1000, volumeSettings.speed.value * 8f, 0f));

                FinalBlit(this, context, cmd, mainTexID.id, cameraColorTarget, Material, 0);
            }
        }

        ScanlinesRenderPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new ScanlinesRenderPass();

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
