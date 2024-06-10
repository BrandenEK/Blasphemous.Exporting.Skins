using UnityEngine;

namespace Blasphemous.Exporting.Skins;

internal static class ImageExtensions
{
    public static Texture2D GetSlicedTexture(this Sprite sprite)
    {
        var output = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        Rect r = sprite.textureRect;
        Color[] pixels = sprite.texture.Duplicate().GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);
        output.SetPixels(pixels);
        output.Apply();
        return output;
    }

    public static Texture2D Duplicate(this Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
}
