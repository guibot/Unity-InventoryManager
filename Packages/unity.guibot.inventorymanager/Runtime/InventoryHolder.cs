using System;
using UnityEngine;
using UnityEngine.Events;

namespace Unity_InventoryKit
{
    // Put this on the player (or a chest). Pickups look for it on the collider's parents.
    // The grid size (columns x rows) is the inventory capacity.
    public class InventoryHolder : MonoBehaviour
    {
        [Min(1)] public int columns = 4;
        [Min(1)] public int rows = 3;

        [Tooltip("Chamado quando o jogador usa um item. Liga aqui o que o item faz (curar, etc.).")]
        public UnityEvent<ItemData> onItemUsed;

        public event Action<ItemData> OnItemUsed;

        public Inventory Inventory { get; private set; }
        public int Size => columns * rows;

        void Awake()
        {
            Inventory = new Inventory(Size);
        }

        // Uses the item in a slot: consumes one (if the item allows it) and raises the events.
        public bool UseSlot(int index)
        {
            if (index < 0 || index >= Inventory.Size) return false;
            var slot = Inventory[index];
            if (slot.IsEmpty) return false;

            var item = slot.Item;
            if (item.consumeOnUse) Inventory.RemoveAt(index, 1);

            OnItemUsed?.Invoke(item);
            onItemUsed?.Invoke(item);
            return true;
        }
    }
}
