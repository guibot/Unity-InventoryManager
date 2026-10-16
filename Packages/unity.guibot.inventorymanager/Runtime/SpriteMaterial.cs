using UnityEngine;

namespace Unity_InventoryKit
{
    // A SpriteRenderer made from code gets a default material chosen by the project's render
    // pipeline, and in some projects (for example a built-in or 3D-renderer project) that material
    // draws nothing, so the sprite is invisible. Sprites-Default draws in every pipeline.
    public static class SpriteMaterial
    {
        static Material material;

        public static Material Default
        {
            get
            {
                if (material == null) material = Resources.GetBuiltinResource<Material>("Sprites-Default.mat");
                if (material == null)
                {
                    var shader = Shader.Find("Sprites/Default");
                    if (shader != null) material = new Material(shader);
                }
                return material;
            }
        }

        public static void Apply(SpriteRenderer renderer)
        {
            var m = Default;
            if (m != null) renderer.sharedMaterial = m;
        }
    }
}
