using UnityEngine;

namespace SequenceDemo
{
    public static class TextureHelper
    {
        /// <summary>
        /// Converts <paramref name="tex"/> to Texture2D
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Texture2D ConvertToTexture2D(this Texture tex, int width, int height)
        {
            Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);

            RenderTexture currentRT = RenderTexture.active;

            RenderTexture renderTexture = new RenderTexture(width, height, 32);
            Graphics.Blit(tex, renderTexture);

            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();

            RenderTexture.active = currentRT;

            return texture2D;
        }
    }
}
