using UnityEngine;
using Unity_InventoryKit;
namespace Unity_InventoryKit.Demos
{
    // SO CODIGO: poe num GameObject vazio de uma cena vazia e carrega Play.
    // A cena aparece vazia no editor; tudo e criado em runtime.
    // WASD/setas movem, 1-4 hotbar, I grelha.
    public class Demo2D : MonoBehaviour
    {
        Rigidbody2D rb;
        InventoryHolder holder;

        void Start()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                cam = new GameObject("Main Camera", typeof(Camera)).GetComponent<Camera>();
                cam.tag = "MainCamera";
            }
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.transform.position = new Vector3(0f, 0f, -10f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.12f, 0.12f, 0.16f);

            var square = MakeSquare();

            var player = new GameObject("Player");
            player.transform.localScale = Vector3.one * 0.8f;
            var sr = player.AddComponent<SpriteRenderer>();
            SpriteMaterial.Apply(sr);
            sr.sprite = square;
            player.AddComponent<BoxCollider2D>();
            rb = player.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
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
                var go = new GameObject(item.displayName);
                go.transform.position = new Vector3(Random.Range(-7f, 7f), Random.Range(-4f, 4f), 0f);
                go.transform.localScale = Vector3.one * 0.5f;
                var r = go.AddComponent<SpriteRenderer>();
                SpriteMaterial.Apply(r);
                r.sprite = square;
                r.color = item.tint;
                var col = go.AddComponent<CircleCollider2D>();
                col.isTrigger = true;
                var pickup = go.AddComponent<Pickup2D>();
                pickup.item = item;
                pickup.amount = Random.Range(1, 4);
            }

            InventoryUI.CreateDefault(holder);
        }

        void FixedUpdate()
        {
            rb.linearVelocity = InventoryInput.Move() * 5f;
        }

        static Sprite MakeSquare()
        {
            var tex = new Texture2D(4, 4);
            var pixels = new Color[16];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
        }
    }
}
