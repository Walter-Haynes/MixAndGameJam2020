using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SkyboxTextureRenderFeature : ScriptableRendererFeature
{
    class SkyboxTextureRenderPass : ScriptableRenderPass
    {
        //public RenderTargetHandle skyboxTexHandle;
        public RenderTexture skyboxTex;
        RenderTargetIdentifier skyboxTexID;
        private RenderTargetIdentifier source;

        public string ProfilerTag = "Skybox to texture";

        public void Setup(RenderTargetIdentifier cameraColorTarget)
        {
            this.source = cameraColorTarget;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            RenderTextureDescriptor dsc = cameraTextureDescriptor;
            dsc.width /= 2;
            dsc.height /= 2;

            skyboxTex = RenderTexture.GetTemporary(dsc.width, dsc.height, 0);
            skyboxTex.filterMode = FilterMode.Trilinear;
            skyboxTex.useMipMap = true;

            skyboxTexID = new RenderTargetIdentifier(skyboxTex);

            ConfigureTarget(skyboxTexID);
            ConfigureClear(ClearFlag.All, Color.black);
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get(ProfilerTag);

            context.DrawSkybox(renderingData.cameraData.camera);

            //Doesn't work in this scope?
            //cmd.SetGlobalTexture("_SkyboxTex", source);

            //Works
            //SCPE.FogRenderer.FogRenderPass.FogMaterial.SetTexture("_SkyboxTex", skyboxTex);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        /// Cleanup any allocated resources that were created during the execution of this render pass.
        public override void FrameCleanup(CommandBuffer cmd)
        {
            RenderTexture.ReleaseTemporary(skyboxTex);
        }
    }

    SkyboxTextureRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new SkyboxTextureRenderPass();

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_ScriptablePass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


