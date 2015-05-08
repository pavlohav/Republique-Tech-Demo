using UnityEngine;
using System.Collections.Generic;

public class RendererUtil
{
    public static List<Material> GetUniqueMaterialsFromRenderers(IEnumerable<Renderer> renderers)
    {
        List<Material> mats = new List<Material>();
        if (renderers != null)
        {
            IEnumerator<Renderer> enumerator = renderers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Renderer currentRenderer = enumerator.Current;
                if (currentRenderer != null)
                {
                    Material[] materials = currentRenderer.sharedMaterials;
                    for (int i = 0; i < materials.Length; ++i)
                    {
                        Material currentMat = materials[i];
                        if (currentMat != null && !mats.Contains(currentMat))
                        {
                            mats.Add(currentMat);
                        }
                    }
                }
            }
        }
        return mats;
    }
}
