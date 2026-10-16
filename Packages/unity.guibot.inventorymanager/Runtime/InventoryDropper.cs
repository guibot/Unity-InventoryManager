using UnityEngine;

namespace Unity_InventoryKit
{
    // Drops items from the inventory into the world (G key, handled by InventoryUI).
    // Works in 2D and 3D: it is 2D if the player has a Rigidbody2D or Collider2D.
    // The item lands in front of the player, in the direction of the last movement.
    [RequireComponent(typeof(InventoryHolder))]
    public class InventoryDropper : MonoBehaviour
    {
        [Min(0f)] public float dropDistance = 1.5f;

        [Tooltip("Segundos ate o item largado poder ser apanhado outra vez.")]
        public float collectDelay = 1.5f;

        [Tooltip("Larga a pilha inteira em vez de uma unidade.")]
        public bool dropWholeStack;

        InventoryHolder holder;
        bool is2D;
        Vector2 lastDirection = new Vector2(0f, -1f);
        Vector3 lastPosition;

        void Awake()
        {
            holder = GetComponent<InventoryHolder>();
            is2D = GetComponent<Rigidbody2D>() != null || GetComponent<Collider2D>() != null;
            lastPosition = transform.position;
        }

        void Update()
        {
            // Direction comes from how the player actually moved, so it works with any movement script.
            var delta = transform.position - lastPosition;
            lastPosition = transform.position;
            var flat = is2D ? new Vector2(delta.x, delta.y) : new Vector2(delta.x, delta.z);
            if (flat.sqrMagnitude > 1e-6f) lastDirection = flat.normalized;
        }

        public bool Drop(int slotIndex)
        {
            var inventory = holder.Inventory;
            if (slotIndex < 0 || slotIndex >= inventory.Size || inventory[slotIndex].IsEmpty) return false;

            var item = inventory[slotIndex].Item;
            int amount = inventory.RemoveAt(slotIndex, dropWholeStack ? inventory[slotIndex].Count : 1);
            if (amount <= 0) return false;

            PickupFactory.Spawn(item, amount, DropPosition(), is2D, collectDelay);
            return true;
        }

        Vector3 DropPosition()
        {
            var origin = transform.position;
            if (is2D)
                return origin + new Vector3(lastDirection.x, lastDirection.y, 0f) * dropDistance;

            var position = origin + new Vector3(lastDirection.x, 0f, lastDirection.y) * dropDistance;
            // Rest on the ground if there is one below.
            if (Physics.Raycast(position + Vector3.up * 3f, Vector3.down, out var hit, 10f, ~0, QueryTriggerInteraction.Ignore))
                position.y = hit.point.y + 0.5f;
            return position;
        }
    }
}
