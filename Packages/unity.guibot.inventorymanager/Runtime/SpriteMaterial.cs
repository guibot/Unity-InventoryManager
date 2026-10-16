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
                if (material != null) return material;

#if UNITY_EDITOR
                // In the editor use the real built-in asset, so scenes and prefabs keep a valid reference.
                material = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
#endif
                if (material == null)
                {
                    // Players: build it from the shader (Sprites/Default is always included).
                    var shader = Shader.Find("Sprites/Default");
                    if (shader != null) material = new Material(shader) { name = "Sprites-Default (runtime)" };
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
