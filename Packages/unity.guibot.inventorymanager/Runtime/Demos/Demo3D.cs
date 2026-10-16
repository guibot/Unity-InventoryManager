using UnityEngine;
using Unity_InventoryKit;
namespace Unity_InventoryKit.Demos
{
    // SO CODIGO: poe num GameObject vazio de uma cena vazia e carrega Play.
    // A cena aparece vazia no editor; tudo e criado em runtime.
    // WASD/setas movem, 1-4 hotbar, I grelha.
    public class Demo3D : MonoBehaviour
    {
        Rigidbody rb;
        InventoryHolder holder;

        void Start()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                cam = new GameObject("Main Camera", typeof(Camera)).GetComponent<Camera>();
                cam.tag = "MainCamera";
            }
            cam.orthographic = false;
            cam.transform.position = new Vector3(0f, 12f, -9f);
            cam.transform.rotation = Quaternion.Euler(55f, 0f, 0f);

            if (FindAnyObjectByType<Light>() == null)
            {
                var light = new GameObject("Light", typeof(Light)).GetComponent<Light>();
                light.type = LightType.Directional;
                light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.transform.localScale = new Vector3(2f, 1f, 2f);

            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.transform.position = new Vector3(0f, 1f, 0f);
            rb = player.AddComponent<Rigidbody>();
            rb.freezeRotation = true;
            holder = player.AddComponent<InventoryHolder>();

            var items = new[]
            {
                ItemData.Create("coin", "Coin", new Color(1f, 0.85f, 0.2f), 99),
                ItemData.Create("gem", "Gem", new Color(0.3f, 0.9f, 1f), 20),
                ItemData.Create("potion", "Potion", new Color(0.9f, 0.3f, 0.9f), 5),
            };

            for (int i = 0; i < 10; i++)
            {
                var item = items[Random.Range(0, items.Length)];
                var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.name = item.displayName;
                go.transform.position = new Vector3(Random.Range(-8f, 8f), 0.5f, Random.Range(-8f, 8f));
                go.transform.localScale = Vector3.one * 0.6f;
                go.GetComponent<Renderer>().material.color = item.tint;
                go.GetComponent<Collider>().isTrigger = true;
                var pickup = go.AddComponent<Pickup3D>();
                pickup.item = item;
                pickup.amount = Random.Range(1, 4);
            }

            InventoryUI.CreateDefault(holder);
        }

        void FixedUpdate()
        {
            var m = InventoryInput.Move();
            rb.linearVelocity = new Vector3(m.x * 5f, rb.linearVelocity.y, m.y * 5f);
        }
    }
}
