﻿#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
#if URP
    public class BlurRenderer : ScriptableRendererFeature
    {
        class BlurRenderPass : PostEffectRenderer<Blur>
        {
            int blurredID;
            int blurredID2;

            enum Pass
            {
                Blend,
                Gaussian,
                Box
            }
            public BlurRenderPass()
            {
                shaderName = ShaderNames.Blur;
                ProfilerTag = this.ToString();

                blurredID = Shader.PropertyToID("_Temp1");
                blurredID2 = Shader.PropertyToID("_Temp2");
            }

            public void Setup(RenderTargetIdentifier cameraColorTarget)
            {
                this.cameraColorTarget = cameraColorTarget;
                
                volumeSettings = VolumeManager.instance.stack.GetComponent<Blur>();
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (!volumeSettings) return;
                
                RenderTextureDescriptor opaqueDesc = cameraTextureDescriptor;
                //opaqueDesc.depthBufferBits = 0;

                opaqueDesc.width /= volumeSettings.downscaling.value;
                opaqueDesc.height /= volumeSettings.downscaling.value;
                //opaqueDesc.msaaSamples = 0;

                cmd.GetTemporaryRT(blurredID, opaqueDesc);
                cmd.GetTemporaryRT(blurredID2, opaqueDesc);
                
                base.Configure(cmd, cameraTextureDescriptor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (!volumeSettings) return;
                if (volumeSettings.IsActive() == false) return;

                var cmd = CommandBufferPool.Get(ProfilerTag);

                CopyTargets(cmd);
                Blit(cmd, cameraColorTarget, blurredID);

                int blurPass = (volumeSettings.mode == Blur.BlurMethod.Gaussian) ? (int)Pass.Gaussian : (int)Pass.Box;

                for (int i = 0; i < volumeSettings.iterations.value; i++)
                {
                    //Safeguard for exploding GPUs
                    if (volumeSettings.iterations.value > 12) return;

                    // horizontal blur
                    cmd.SetGlobalVector("_BlurOffsets", new Vector4(volumeSettings.amount.value / renderingData.cameraData.camera.scaledPixelWidth, 0, 0, 0));
                    Blit(this, cmd, blurredID, blurredID2, Material, blurPass);

                    // vertical blur
                    cmd.SetGlobalVector("_BlurOffsets", new Vector4(0, volumeSettings.amount.value / renderingData.cameraData.camera.scaledPixelHeight, 0, 0));
                    Blit(this, cmd, blurredID2, blurredID, Material, blurPass);

                    //Double blur
                    if (volumeSettings.highQuality.value)
                    {
                        // horizontal blur
                        cmd.SetGlobalVector("_BlurOffsets", new Vector4(volumeSettings.amount.value / renderingData.cameraData.camera.scaledPixelWidth, 0, 0, 0));
                        Blit(this, cmd, blurredID, blurredID2, Material, blurPass);

                        // vertical blur
                        cmd.SetGlobalVector("_BlurOffsets", new Vector4(0, volumeSettings.amount.value / renderingData.cameraData.camera.scaledPixelHeight, 0, 0));
                        Blit(this, cmd, blurredID2, blurredID, Material, blurPass);
                    }
                }

                FinalBlit(this, context, cmd, blurredID, cameraColorTarget, Material, (int)Pass.Blend);
            }

            public override void Cleanup(CommandBuffer cmd)
            {
                base.Cleanup(cmd);
                
                cmd.ReleaseTemporaryRT(blurredID);
                cmd.ReleaseTemporaryRT(blurredID2);
            }
        }

        BlurRenderPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new BlurRenderPass();

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