using System.Collections.Generic;
using UnityEngine;

namespace Unity_InventoryKit
{
    // Resolves saved ids back to ItemData.
    [CreateAssetMenu(menuName = "Inventory/Item Database", fileName = "ItemDatabase")]
    public class ItemDatabase : ScriptableObject
    {
        public ItemData[] items;

        Dictionary<string, ItemData> lookup;

        public static ItemDatabase FromItems(params ItemData[] items)
        {
            var db = CreateInstance<ItemDatabase>();
            db.items = items;
            return db;
        }

        public ItemData Find(string id)
        {
            if (lookup == null)
            {
                lookup = new Dictionary<string, ItemData>();
                if (items != null)
                    foreach (var item in items)
                        if (item != null) lookup[item.Id] = item;
            }
            return lookup.TryGetValue(id, out var found) ? found : null;
        }
    }
}
