using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Water_Volume : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        public RenderTargetIdentifier source;

        private readonly Material _material;
        private readonly int _tempColorId;

        public CustomRenderPass(Material mat)
        {
            _material = mat;
            _tempColorId = Shader.PropertyToID("_TemporaryColourTexture");
        }

        // Optional: configure targets/clears here if needed
        [System.Obsolete]
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
        }

        [System.Obsolete]
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Reflection)
                return;

            var cmd = CommandBufferPool.Get("Water_Volume");

            try
            {
                var descriptor = renderingData.cameraData.cameraTargetDescriptor;
                // If needed, adjust descriptor settings here (e.g. msaaSamples)

                cmd.GetTemporaryRT(_tempColorId, descriptor, FilterMode.Bilinear);
                var tempRT = new RenderTargetIdentifier(_tempColorId);

                // Use CommandBuffer.Blit instead of the obsolete ScriptableRenderPass.Blit helper
                cmd.Blit(source, tempRT, _material);
                cmd.Blit(tempRT, source);

                cmd.ReleaseTemporaryRT(_tempColorId);

                context.ExecuteCommandBuffer(cmd);
            }
            finally
            {
                CommandBufferPool.Release(cmd);
            }
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            // temporary RT released in Execute, nothing to do here
        }
    }

    [System.Serializable]
    public class _Settings
    {
        public Material material = null;
        public RenderPassEvent renderPass = RenderPassEvent.AfterRenderingSkybox;
    }

    public _Settings settings = new _Settings();

    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        if (settings.material == null)
        {
            settings.material = (Material)Resources.Load("Water_Volume");
        }

        m_ScriptablePass = new CustomRenderPass(settings.material);
        m_ScriptablePass.renderPassEvent = settings.renderPass;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Avoid using possibly-obsolete renderer.cameraColorTarget property.
        // Use the active camera target identifier instead.
        m_ScriptablePass.source = new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}