using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    [PostProcess(typeof(DistortionRenderer), PostProcessEvent.AfterStack, "Custom/Distortion")]
    public sealed class Distortion : PostProcessEffectSettings
    {
        [Tooltip("Texture")]
        public TextureParameter distortion = new TextureParameter();

        [Range(0f, 1f), Tooltip("Intensity")]
        public FloatParameter intensity = new FloatParameter { value = 0.1f };
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class DistortionRenderer : PostProcessEffectRenderer<Distortion>
    {
        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Distortion"));
            if(settings.distortion.value != null)
                sheet.properties.SetTexture("_DistortionTex", settings.distortion);
            sheet.properties.SetFloat("_Intensity", settings.intensity);
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}
