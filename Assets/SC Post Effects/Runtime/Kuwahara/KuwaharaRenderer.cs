﻿#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
#if URP
    public class KuwaharaRenderer : ScriptableRendererFeature
    {
        class KuwaharaRenderPass : PostEffectRenderer<Kuwahara>
        {
            public KuwaharaRenderPass()
            {
                shaderName = ShaderNames.Kuwahara;
                ProfilerTag = this.ToString();
            }

            public void Setup(RenderTargetIdentifier cameraColorTarget, RenderTargetIdentifier cameraDepthTarget)
            {
                this.cameraColorTarget = cameraColorTarget;
                this.cameraDepthTarget = cameraDepthTarget;
                
                volumeSettings = VolumeManager.instance.stack.GetComponent<Kuwahara>();
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (!volumeSettings) return;

                requiresDepth = volumeSettings.mode == Kuwahara.KuwaharaMode.DepthFade;

                base.Configure(cmd, cameraTextureDescriptor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (!volumeSettings) return;
                if (volumeSettings.IsActive() == false) return;

                var cmd = CommandBufferPool.Get(ProfilerTag);

                CopyTargets(cmd);

                Material.SetFloat("_Radius", (float)volumeSettings.radius);
                Material.SetFloat("_FadeDistance", volumeSettings.fadeDistance.value);
                Material.SetVector("_DistanceParams", new Vector4(volumeSettings.fadeDistance.value, (volumeSettings.invertFadeDistance.value) ? 1 : 0, 0, 0));

                FinalBlit(this, context, cmd, mainTexID.id, cameraColorTarget, Material, (int)volumeSettings.mode.value);
            }
        }

        KuwaharaRenderPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new KuwaharaRenderPass();

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