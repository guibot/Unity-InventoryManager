using UnityEngine;

namespace Unity_InventoryKit.Demos
{
    // MODO MANUAL: a cena e montada por ti no editor (jogador, itens, pickups).
    // Este script so liga o resto: a UI (grelha + hotbar) e o movimento opcional.
    // Serve em 2D e em 3D: usa o Rigidbody2D ou o Rigidbody do jogador, conforme o que tiver.
    // O inventario nasce vazio e perde-se quando o Play termina (nao ha guardar/carregar).
    //
    // Teclas: I grelha, 1-9 ou clique selecionam um slot (moldura amarela), E usa o item
    // selecionado, G larga-o. Arrastar com o rato troca slots, botao direito usa o item.
    public class InventoryManualSetup : MonoBehaviour
    {
        [Tooltip("O jogador, com o componente InventoryHolder.")]
        public InventoryHolder holder;

        [Header("UI")]
        public UIAnchor hotbarAnchor = UIAnchor.BottomCenter;
        public UIAnchor gridAnchor = UIAnchor.Center;
        [Range(0, 9)] public int hotbarSize = 4;
        public float slotSize = 64f;
        public float margin = 20f;

        [Header("Movimento")]
        [Tooltip("0 = nao move o jogador (usa o teu proprio movimento).")]
        public float moveSpeed = 5f;

        Rigidbody2D rb2d;
        Rigidbody rb3d;

        void Start()
        {
            if (holder == null)
            {
                Debug.LogError("InventoryManualSetup: arrasta o jogador (com InventoryHolder) para o campo Holder.", this);
                enabled = false;
                return;
            }

            rb2d = holder.GetComponent<Rigidbody2D>();
            rb3d = holder.GetComponent<Rigidbody>();

            // The UI reads these in its own Start, which runs after this call returns.
            var ui = InventoryUI.CreateDefault(holder);
            ui.hotbarAnchor = hotbarAnchor;
            ui.gridAnchor = gridAnchor;
            ui.hotbarSize = hotbarSize;
            ui.slotSize = slotSize;
            ui.margin = margin;
        }

        void FixedUpdate()
        {
            if (moveSpeed <= 0f) return;
            var m = InventoryInput.Move();
            if (rb2d != null)
                rb2d.linearVelocity = m * moveSpeed;
            else if (rb3d != null)
                rb3d.linearVelocity = new Vector3(m.x * moveSpeed, rb3d.linearVelocity.y, m.y * moveSpeed);
        }
    }
}
