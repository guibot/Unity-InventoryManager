using UnityEngine;

namespace Unity_InventoryKit
{
    // Creates a pickup in the world from code (used when an item is dropped).
    // Uses the item's World Prefab if it has one; otherwise a simple square (2D) or sphere (3D).
    // If the item has an Icon, PickupBase draws it in place of the placeholder.
    public static class PickupFactory
    {
        static Sprite square;

        public static PickupBase Spawn(ItemData item, int amount, Vector3 position, bool is2D, float collectDelay)
        {
            PickupBase pickup;

            if (item.worldPrefab != null)
            {
                pickup = Object.Instantiate(item.worldPrefab, position, Quaternion.identity);
            }
            else if (is2D)
            {
                var go = new GameObject("Pickup_" + item.Id);
                go.transform.position = position;
                go.transform.localScale = Vector3.one * 0.5f;
                var r = go.AddComponent<SpriteRenderer>();
                SpriteMaterial.Apply(r);
                r.sprite = Square();
                r.color = item.tint;
                var col = go.AddComponent<CircleCollider2D>();
                col.isTrigger = true;
                col.radius = 0.5f;
                pickup = go.AddComponent<Pickup2D>();
            }
            else
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.name = "Pickup_" + item.Id;
                go.transform.position = position;
                go.transform.localScale = Vector3.one * 0.6f;
                go.GetComponent<Renderer>().material.color = item.tint;
                go.GetComponent<Collider>().isTrigger = true;
                pickup = go.AddComponent<Pickup3D>();
            }

            pickup.item = item;
            pickup.amount = amount;
            pickup.SetCollectDelay(collectDelay);
            return pickup;
        }

        static Sprite Square()
        {
            if (square != null) return square;
            var tex = new Texture2D(4, 4);
            var pixels = new Color[16];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
            tex.SetPixels(pixels);
            tex.Apply();
            square = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
            return square;
        }
    }
}
