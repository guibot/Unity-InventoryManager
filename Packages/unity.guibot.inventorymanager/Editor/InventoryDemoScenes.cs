using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity_InventoryKit.Demos;

namespace Unity_InventoryKit.EditorTools
{
    // Tools > Inventory > Create Demo Scenes: Demo2D/Demo3D (so codigo) e Finetuning2D/3D (montadas a mao).
    public static class InventoryDemoScenes
    {
        const string Root = "Assets/InventoryDemo";

        [MenuItem("Tools/Inventory/Create Demo Scenes")]
        static void CreateAll()
        {
            Directory.CreateDirectory(Root + "/Items");
            Directory.CreateDirectory(Root + "/Prefabs");
            AssetDatabase.Refresh();

            var square = EnsureSquare();
            var items = EnsureItems();

            CodeOnlyScene("Demo2D", typeof(Demo2D));
            CodeOnlyScene("Demo3D", typeof(Demo3D));
            Finetuning2D(square, items);
            Finetuning3D(items);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Inventory: cenas criadas em " + Root);
        }

        // ---------- cenas ----------

        static void CodeOnlyScene(string name, System.Type script)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            new GameObject(name, script);
            EditorSceneManager.SaveScene(scene, $"{Root}/{name}.unity");
        }

        static void Finetuning2D(Sprite square, ItemData[] items)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            var cam = Camera.main;
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.transform.position = new Vector3(0f, 0f, -10f);
            cam.transform.rotation = Quaternion.identity;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.12f, 0.12f, 0.16f);

            var player = new GameObject("Player");
            player.transform.localScale = Vector3.one * 0.8f;
            var playerRenderer = player.AddComponent<SpriteRenderer>();
            playerRenderer.sprite = square;
            SpriteMaterial.Apply(playerRenderer);
            player.AddComponent<BoxCollider2D>();
            var rb = player.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            var holder = player.AddComponent<InventoryHolder>();

            var positions = new[] { new Vector3(-4, 2), new Vector3(0, 3), new Vector3(4, 2), new Vector3(-3, -2), new Vector3(3, -2) };
            for (int i = 0; i < positions.Length; i++)
            {
                var prefab = EnsurePickupPrefab2D(items[i % items.Length], square);
                PlacePickup(prefab, positions[i], i + 1);
            }

            Setup(holder);
            EditorSceneManager.SaveScene(scene, $"{Root}/Finetuning2D.unity");
        }

        static void Finetuning3D(ItemData[] items)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            var cam = Camera.main;
            cam.transform.position = new Vector3(0f, 12f, -9f);
            cam.transform.rotation = Quaternion.Euler(55f, 0f, 0f);

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(2f, 1f, 2f);

            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.transform.position = new Vector3(0f, 1f, 0f);
            player.AddComponent<Rigidbody>().freezeRotation = true;
            var holder = player.AddComponent<InventoryHolder>();

            var positions = new[] { new Vector3(-5, 0.5f, 3), new Vector3(0, 0.5f, 5), new Vector3(5, 0.5f, 3), new Vector3(-4, 0.5f, -3), new Vector3(4, 0.5f, -3) };
            for (int i = 0; i < positions.Length; i++)
            {
                var prefab = EnsurePickupPrefab3D(items[i % items.Length]);
                PlacePickup(prefab, positions[i], i + 1);
            }

            Setup(holder);
            EditorSceneManager.SaveScene(scene, $"{Root}/Finetuning3D.unity");
        }

        static void Setup(InventoryHolder holder)
        {
            var go = new GameObject("InventorySetup");
            var setup = go.AddComponent<InventoryManualSetup>();
            setup.holder = holder;
        }

        static void PlacePickup(GameObject prefab, Vector3 position, int amount)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.position = position;
            var pickup = instance.GetComponent<PickupBase>();
            pickup.amount = amount;
            PrefabUtility.RecordPrefabInstancePropertyModifications(pickup);
        }

        // ---------- assets ----------

        internal static Sprite EnsureSquare()
        {
            Directory.CreateDirectory(Root);
            string path = Root + "/Square.png";
            if (!File.Exists(path))
            {
                var tex = new Texture2D(4, 4);
                var px = new Color[16];
                for (int i = 0; i < px.Length; i++) px[i] = Color.white;
                tex.SetPixels(px);
                tex.Apply();
                File.WriteAllBytes(path, tex.EncodeToPNG());
                Object.DestroyImmediate(tex);
                AssetDatabase.ImportAsset(path);
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 4;
                importer.SaveAndReimport();
            }
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        static ItemData[] EnsureItems()
        {
            var defs = new[]
            {
                (id: "coin", name: "Coin", color: new Color(1f, 0.85f, 0.2f), max: 99),
                (id: "gem", name: "Gem", color: new Color(0.3f, 0.9f, 1f), max: 20),
                (id: "potion", name: "Potion", color: new Color(0.9f, 0.3f, 0.9f), max: 5),
            };

            var list = new List<ItemData>();
            foreach (var d in defs)
            {
                string path = $"{Root}/Items/{d.name}.asset";
                var item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
                if (item == null)
                {
                    item = ItemData.Create(d.id, d.name, d.color, d.max);
                    AssetDatabase.CreateAsset(item, path);
                }
                list.Add(item);
            }
            return list.ToArray();
        }

        static GameObject EnsurePickupPrefab2D(ItemData item, Sprite square)
        {
            string path = $"{Root}/Prefabs/{item.displayName}Pickup2D.prefab";
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null) return existing;

            var go = new GameObject(item.displayName + "Pickup2D");
            go.transform.localScale = Vector3.one * 0.5f;
            var r = go.AddComponent<SpriteRenderer>();
            SpriteMaterial.Apply(r);
            r.sprite = square;
            r.color = item.tint;
            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.5f;
            var pickup = go.AddComponent<Pickup2D>();
            pickup.item = item;

            var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return prefab;
        }

        static GameObject EnsurePickupPrefab3D(ItemData item)
        {
            string path = $"{Root}/Prefabs/{item.displayName}Pickup3D.prefab";
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null) return existing;

            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = item.displayName + "Pickup3D";
            go.transform.localScale = Vector3.one * 0.6f;
            go.GetComponent<Collider>().isTrigger = true;

            // Material proprio guardado como asset, senao o prefab perde a cor.
            var renderer = go.GetComponent<Renderer>();
            var mat = new Material(renderer.sharedMaterial) { color = item.tint };
            AssetDatabase.CreateAsset(mat, $"{Root}/Prefabs/{item.displayName}.mat");
            renderer.sharedMaterial = mat;

            var pickup = go.AddComponent<Pickup3D>();
            pickup.item = item;

            var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return prefab;
        }
    }
}
