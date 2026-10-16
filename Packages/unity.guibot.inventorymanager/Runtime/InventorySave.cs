using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Unity_InventoryKit
{
    public static class InventorySave
    {
        [Serializable] class Entry { public string id; public int count; }
        [Serializable] class Data { public List<Entry> slots = new List<Entry>(); }

        public static string PathFor(string file) => Path.Combine(Application.persistentDataPath, file);

        public static void Save(Inventory inventory, string file = "inventory.json")
        {
            var data = new Data();
            for (int i = 0; i < inventory.Size; i++)
            {
                var s = inventory[i];
                data.slots.Add(new Entry { id = s.IsEmpty ? "" : s.Item.Id, count = s.IsEmpty ? 0 : s.Count });
            }
            File.WriteAllText(PathFor(file), JsonUtility.ToJson(data, true));
        }

        public static bool Load(Inventory inventory, ItemDatabase database, string file = "inventory.json")
        {
            string path = PathFor(file);
            if (!File.Exists(path)) return false;

            var data = JsonUtility.FromJson<Data>(File.ReadAllText(path));
            inventory.Clear();
            int n = Math.Min(inventory.Size, data.slots.Count);
            for (int i = 0; i < n; i++)
            {
                var e = data.slots[i];
                var item = string.IsNullOrEmpty(e.id) ? null : database.Find(e.id);
                inventory.Set(i, item, e.count);
            }
            return true;
        }
    }
}
