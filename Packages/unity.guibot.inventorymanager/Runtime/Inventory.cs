using System;

namespace Unity_InventoryKit
{
    // Pure data: knows nothing about 2D, 3D or UI.
    public class Inventory
    {
        readonly InventorySlot[] slots;

        public event Action OnChanged;

        public int Size => slots.Length;
        public InventorySlot this[int index] => slots[index];

        public Inventory(int size)
        {
            slots = new InventorySlot[Math.Max(1, size)];
            for (int i = 0; i < slots.Length; i++) slots[i] = new InventorySlot();
        }

        // Returns the amount that did not fit (0 = everything was added).
        public int Add(ItemData item, int amount = 1)
        {
            if (item == null || amount <= 0) return amount;
            int max = Math.Max(1, item.maxStack);
            bool changed = false;

            foreach (var s in slots)
            {
                if (amount == 0) break;
                if (s.IsEmpty || s.Item != item || s.Count >= max) continue;
                int moved = Math.Min(max - s.Count, amount);
                s.Count += moved;
                amount -= moved;
                changed = true;
            }

            foreach (var s in slots)
            {
                if (amount == 0) break;
                if (!s.IsEmpty) continue;
                int moved = Math.Min(max, amount);
                s.Item = item;
                s.Count = moved;
                amount -= moved;
                changed = true;
            }

            if (changed) OnChanged?.Invoke();
            return amount;
        }

        // All or nothing: returns false and changes nothing if there is not enough.
        public bool Remove(ItemData item, int amount = 1)
        {
            if (item == null || amount <= 0 || Count(item) < amount) return false;

            for (int i = slots.Length - 1; i >= 0 && amount > 0; i--)
            {
                var s = slots[i];
                if (s.IsEmpty || s.Item != item) continue;
                int taken = Math.Min(s.Count, amount);
                s.Count -= taken;
                amount -= taken;
                if (s.Count == 0) s.Clear();
            }

            OnChanged?.Invoke();
            return true;
        }

        public int Count(ItemData item)
        {
            int total = 0;
            foreach (var s in slots)
                if (!s.IsEmpty && s.Item == item) total += s.Count;
            return total;
        }

        public void Swap(int a, int b)
        {
            if (a == b) return;
            (slots[a].Item, slots[b].Item) = (slots[b].Item, slots[a].Item);
            (slots[a].Count, slots[b].Count) = (slots[b].Count, slots[a].Count);
            OnChanged?.Invoke();
        }

        // Removes up to `amount` from one specific slot. Returns how many were removed.
        public int RemoveAt(int index, int amount = 1)
        {
            if (index < 0 || index >= slots.Length) return 0;
            var s = slots[index];
            if (s.IsEmpty || amount <= 0) return 0;

            int taken = Math.Min(s.Count, amount);
            s.Count -= taken;
            if (s.Count == 0) s.Clear();
            OnChanged?.Invoke();
            return taken;
        }

        // Drag and drop: moves into an empty slot, merges equal items, otherwise swaps.
        public void Move(int from, int to)
        {
            if (from == to || from < 0 || to < 0 || from >= slots.Length || to >= slots.Length) return;
            var a = slots[from];
            var b = slots[to];
            if (a.IsEmpty) return;

            if (!b.IsEmpty && b.Item == a.Item)
            {
                int max = Math.Max(1, a.Item.maxStack);
                int moved = Math.Min(max - b.Count, a.Count);
                if (moved > 0)
                {
                    b.Count += moved;
                    a.Count -= moved;
                    if (a.Count == 0) a.Clear();
                    OnChanged?.Invoke();
                    return;
                }
            }
            Swap(from, to);
        }

        public void Set(int index, ItemData item, int count)
        {
            var s = slots[index];
            s.Item = item;
            s.Count = item == null ? 0 : count;
            OnChanged?.Invoke();
        }

        public void Clear()
        {
            foreach (var s in slots) s.Clear();
            OnChanged?.Invoke();
        }
    }
}
