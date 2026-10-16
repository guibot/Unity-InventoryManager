using UnityEngine;

namespace Unity_InventoryKit
{
    // Needs a Collider with Is Trigger; one of the two objects needs a Rigidbody.
    public class Pickup3D : PickupBase
    {
        protected override bool FaceCamera => true;

        void OnTriggerEnter(Collider other) => TryCollect(other.gameObject);
    }
}
