using UnityEngine;

namespace Unity_InventoryKit
{
    [CreateAssetMenu(menuName = "Inventory/Item", fileName = "NewItem")]
    public class ItemData : ScriptableObject
    {
        public string id;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;
        public Color tint = Color.white;
        [Min(1)] public int maxStack = 99;

        [Tooltip("Gasta uma unidade quando o item e usado (tecla E ou botao direito).")]
        public bool consumeOnUse = true;

        [Tooltip("Opcional: prefab (com Pickup2D/Pickup3D) criado quando o item e largado. Vazio = esfera/quadrado.")]
        public PickupBase worldPrefab;

        // Falls back to the asset name so a forgotten id still works.
        public string Id => string.IsNullOrEmpty(id) ? name : id;

        public static ItemData Create(string id, string displayName, Color tint, int maxStack = 99)
        {
            var item = CreateInstance<ItemData>();
            item.id = id;
            item.name = id;
            item.displayName = displayName;
            item.tint = tint;
            item.maxStack = maxStack;
            return item;
        }
    }
}
