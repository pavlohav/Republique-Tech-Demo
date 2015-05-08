using UnityEngine;

public static class TextureUtil 
{
    /// <summary>
    /// Creates a new texture by rendering a quad with the given dimensions with
    /// the given material.  It is the client's responsibility to eventually call
    /// Destroy/DestroyImmediate on the returned texture.
    /// </summary>
    /// <param name="renderMaterial">Material to render the quad with.  May not be null.</param>
    /// <param name="width">Width of the output texture in pixels.</param>
    /// <param name="height">Height of the output texture in pixels.</param>
    /// <param name="linear">True to make output texture in linear space.</param>
    /// <returns>The texture created or null if the renderMaterial is null.</returns>
    public static Texture2D CreateFromMaterial(Material renderMaterial, int width, int height, bool linear)
    {
        Texture2D newTexture;

        if (renderMaterial != null)
        {
            RenderTexture tempRenderTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
            RenderTexture previousActiveRenderTexture = RenderTexture.active;  //remember previous since blit changes it

            //passing in null to preserve texture currently set as _MainTex
            Graphics.Blit(null, tempRenderTexture, renderMaterial);
            newTexture = new Texture2D(width, height, TextureFormat.ARGB32, false, linear);
            newTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);

            RenderTexture.active = previousActiveRenderTexture;
            RenderTexture.ReleaseTemporary(tempRenderTexture);
        }
        else  //no material, don't know what to render on new texture
        {
            newTexture = null;
        }

        return newTexture;
    }
    
    public static void SaveAsPNG(Texture2D texture, string path)
    {
        byte[] pngBytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, pngBytes);
    }
}
