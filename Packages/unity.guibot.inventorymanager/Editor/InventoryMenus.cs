using UnityEditor;
using UnityEngine;
using Unity_InventoryKit.Demos;

namespace Unity_InventoryKit.EditorTools
{
    // One-click setup:
    //  - GameObject > Inventory > Make Selected the Player
    //  - right click an Item asset > Inventory > Create Pickup 2D / 3D in Scene
    public static class InventoryMenus
    {
        // ---------- player ----------

        [MenuItem("GameObject/Inventory/Make Selected the Player", false, 20)]
        static void MakePlayer()
        {
            foreach (var go in Selection.gameObjects) SetUpPlayer(go);
        }

        [MenuItem("GameObject/Inventory/Make Selected the Player", true)]
        static bool MakePlayerValid()
        {
            return Selection.activeGameObject != null && !EditorUtility.IsPersistent(Selection.activeGameObject);
        }

        static void SetUpPlayer(GameObject go)
        {
            bool is2D = go.GetComponent<Collider2D>() != null
                     || go.GetComponent<Rigidbody2D>() != null
                     || go.GetComponent<SpriteRenderer>() != null;

            var holder = go.GetComponent<InventoryHolder>();
            if (holder == null) holder = Undo.AddComponent<InventoryHolder>(go);

            // Pickups are triggers: the player needs a collider and a body for them to fire.
            // Added bodies are kinematic, so the player's own movement is not affected.
            if (is2D)
            {
                if (go.GetComponent<Collider2D>() == null) Undo.AddComponent<BoxCollider2D>(go);
                if (go.GetComponent<Rigidbody2D>() == null)
                    Undo.AddComponent<Rigidbody2D>(go).bodyType = RigidbodyType2D.Kinematic;
            }
            else
            {
                if (go.GetComponent<Collider>() == null) Undo.AddComponent<CapsuleCollider>(go);
                if (go.GetComponent<Rigidbody>() == null && go.GetComponent<CharacterController>() == null)
                    Undo.AddComponent<Rigidbody>(go).isKinematic = true;
            }

            var setup = go.GetComponent<InventoryManualSetup>();
            if (setup == null) setup = Undo.AddComponent<InventoryManualSetup>(go);
            setup.holder = holder;
            setup.moveSpeed = 0f;   // the student's own movement script moves the player
            EditorUtility.SetDirty(setup);
        }

        // ---------- pickups ----------

        [MenuItem("Assets/Inventory/Create Pickup 2D in Scene", false, 2000)]
        static void CreatePickup2D() => CreatePickup((ItemData)Selection.activeObject, true);

        [MenuItem("Assets/Inventory/Create Pickup 3D in Scene", false, 2001)]
        static void CreatePickup3D() => CreatePickup((ItemData)Selection.activeObject, false);

        [MenuItem("Assets/Inventory/Create Pickup 2D in Scene", true)]
        [MenuItem("Assets/Inventory/Create Pickup 3D in Scene", true)]
        static bool CreatePickupValid() => Selection.activeObject is ItemData;

        static void CreatePickup(ItemData item, bool is2D)
        {
            string label = string.IsNullOrEmpty(item.displayName) ? item.name : item.displayName;
            var view = SceneView.lastActiveSceneView;
            var position = view != null ? view.pivot : Vector3.zero;
            GameObject go;

            if (is2D)
            {
                position.z = 0f;
                var sprite = item.icon != null ? item.icon : InventoryDemoScenes.EnsureSquare();

                go = new GameObject(label + " Pickup");
                var renderer = go.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                if (item.icon == null)
                {
                    renderer.color = item.tint;
                    go.transform.localScale = Vector3.one * 0.5f;
                }

                var collider = go.AddComponent<CircleCollider2D>();
                collider.isTrigger = true;
                collider.radius = Mathf.Max(sprite.bounds.size.x, sprite.bounds.size.y) * 0.5f;
                go.AddComponent<Pickup2D>().item = item;
            }
            else
            {
                go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.name = label + " Pickup";
                go.transform.localScale = Vector3.one * 0.6f;
                go.GetComponent<Collider>().isTrigger = true;
                go.AddComponent<Pickup3D>().item = item;
            }

            go.transform.position = position;
            Undo.RegisterCreatedObjectUndo(go, "Create Pickup");
            Selection.activeGameObject = go;
        }
    }
}
