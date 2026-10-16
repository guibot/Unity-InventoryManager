using UnityEngine;

namespace Unity_InventoryKit
{
    // Needs a Collider2D with Is Trigger; one of the two objects needs a Rigidbody2D.
    public class Pickup2D : PickupBase
    {
        void OnTriggerEnter2D(Collider2D other) => TryCollect(other.gameObject);
    }
}
