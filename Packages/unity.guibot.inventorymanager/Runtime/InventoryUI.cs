using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unity_InventoryKit
{
    public enum UIAnchor
    {
        BottomCenter, TopCenter,
        TopLeft, TopRight, BottomLeft, BottomRight,
        Center
    }

    // Grid panel (toggle with I) plus a hotbar (keys 1-9) built from the first slots.
    // Works the same in 2D and 3D: it only reads the Inventory.
    //  - The selected slot (yellow border) is picked with keys 1-9 or by clicking a slot.
    //  - E uses the selected item, G drops it, right click uses the clicked item.
    //  - Drag a slot onto another one to move or swap items.
    //  - Hovering a slot shows a tooltip with the item's name and description.
    public class InventoryUI : MonoBehaviour
    {
        public InventoryHolder holder;

        [Header("Layout")]
        public UIAnchor hotbarAnchor = UIAnchor.BottomCenter;
        public UIAnchor gridAnchor = UIAnchor.Center;
        [Tooltip("Distancia ao bordo do ecra.")]
        public float margin = 20f;
        public float slotSize = 64f;
        [Tooltip("Slots da hotbar (primeiros do inventario). 0 = sem hotbar.")]
        [Range(0, 9)] public int hotbarSize = 4;

        // -1 = nothing selected until the player presses a hotbar key or clicks a slot.
        public int SelectedSlot { get; private set; } = -1;

        // Only used for the tooltip.
        public int HoveredSlot { get; private set; } = -1;

        SlotView[] gridViews;
        SlotView[] hotbarViews = new SlotView[0];
        GameObject gridPanel;
        Inventory inventory;

        RectTransform tooltipRect;
        Text tooltipText;
        Image ghost;
        int dragFrom = -1;

        public static InventoryUI CreateDefault(InventoryHolder holder)
        {
            var go = new GameObject("InventoryCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            go.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);

            if (holder.GetComponent<InventoryDropper>() == null)
                holder.gameObject.AddComponent<InventoryDropper>();

            var ui = go.AddComponent<InventoryUI>();
            ui.holder = holder;
            return ui;
        }

        void Start()
        {
            EnsureEventSystem();
            inventory = holder.Inventory;

            gridPanel = MakePanel("Grid", holder.columns, inventory.Size, gridAnchor, out gridViews);
            gridPanel.SetActive(false);

            int hotbar = Mathf.Min(hotbarSize, inventory.Size);
            if (hotbar > 0) MakePanel("Hotbar", hotbar, hotbar, hotbarAnchor, out hotbarViews);

            BuildTooltip();
            BuildGhost();

            inventory.OnChanged += Refresh;
            Refresh();
        }

        void OnDestroy()
        {
            if (inventory != null) inventory.OnChanged -= Refresh;
        }

        void Update()
        {
            if (InventoryInput.Toggle())
            {
                gridPanel.SetActive(!gridPanel.activeSelf);
                if (!gridPanel.activeSelf) HoveredSlot = -1;
            }

            for (int i = 0; i < hotbarViews.Length; i++)
            {
                if (InventoryInput.Digit(i)) Select(i);
            }

            if (InventoryInput.Use()) UseSlot(SelectedSlot);
            if (InventoryInput.Drop()) DropSlot(SelectedSlot);

            UpdateTooltip();
        }

        // ---------- actions ----------

        public void Select(int index)
        {
            SelectedSlot = index;
            Refresh();
        }

        public void UseSlot(int index) => holder.UseSlot(index);

        void DropSlot(int index)
        {
            var dropper = holder.GetComponent<InventoryDropper>();
            if (dropper != null) dropper.Drop(index);
        }

        // ---------- mouse (called by SlotHandler) ----------

        public void SetHover(int index) => HoveredSlot = index;

        public void ClearHover(int index)
        {
            if (HoveredSlot == index) HoveredSlot = -1;
        }

        public void BeginDrag(int index, PointerEventData e)
        {
            if (inventory[index].IsEmpty) return;

            dragFrom = index;
            var item = inventory[index].Item;
            ghost.sprite = item.icon;
            ghost.color = new Color(item.tint.r, item.tint.g, item.tint.b, 0.85f);
            ghost.rectTransform.position = e.position;
            ghost.transform.SetAsLastSibling();
            ghost.gameObject.SetActive(true);
        }

        public void Drag(PointerEventData e)
        {
            if (dragFrom >= 0) ghost.rectTransform.position = e.position;
        }

        public void DropOn(int index)
        {
            if (dragFrom >= 0) inventory.Move(dragFrom, index);
        }

        public void EndDrag()
        {
            dragFrom = -1;
            ghost.gameObject.SetActive(false);
        }

        // ---------- drawing ----------

        void Refresh()
        {
            for (int i = 0; i < gridViews.Length; i++)
            {
                gridViews[i].Show(inventory[i]);
                gridViews[i].Highlight(i == SelectedSlot);
            }
            for (int i = 0; i < hotbarViews.Length; i++)
            {
                hotbarViews[i].Show(inventory[i]);
                hotbarViews[i].Highlight(i == SelectedSlot);
            }
        }

        void UpdateTooltip()
        {
            bool show = dragFrom < 0 && HoveredSlot >= 0 && !inventory[HoveredSlot].IsEmpty;
            if (tooltipRect.gameObject.activeSelf != show) tooltipRect.gameObject.SetActive(show);
            if (!show) return;

            var item = inventory[HoveredSlot].Item;
            string title = string.IsNullOrEmpty(item.displayName) ? item.Id : item.displayName;
            string text = "<b>" + title + "</b>";
            if (!string.IsNullOrEmpty(item.description)) text += "\n" + item.description;
            if (tooltipText.text != text) tooltipText.text = text;

            // Opens away from the screen edges so it never goes off screen.
            var p = InventoryInput.PointerPosition();
            bool right = p.x > Screen.width * 0.5f;
            bool low = p.y < Screen.height * 0.5f;
            tooltipRect.pivot = new Vector2(right ? 1f : 0f, low ? 0f : 1f);
            tooltipRect.position = p + new Vector2(right ? -16f : 16f, low ? 16f : -16f);
        }

        GameObject MakePanel(string panelName, int cols, int count, UIAnchor anchor, out SlotView[] views)
        {
            var go = new GameObject(panelName, typeof(RectTransform), typeof(Image), typeof(GridLayoutGroup), typeof(ContentSizeFitter));
            go.transform.SetParent(transform, false);

            var a = AnchorVector(anchor);
            var rt = (RectTransform)go.transform;
            rt.anchorMin = rt.anchorMax = rt.pivot = a;
            // Pushes the panel in from the edge it is anchored to.
            rt.anchoredPosition = new Vector2(a.x == 0f ? 1f : a.x == 1f ? -1f : 0f,
                                              a.y == 0f ? 1f : a.y == 1f ? -1f : 0f) * margin;

            var image = go.GetComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.35f);
            image.raycastTarget = false;

            var grid = go.GetComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(slotSize, slotSize);
            grid.spacing = new Vector2(6f, 6f);
            grid.padding = new RectOffset(8, 8, 8, 8);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = cols;

            var fitter = go.GetComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            views = new SlotView[count];
            for (int i = 0; i < count; i++) views[i] = SlotView.Create(go.transform, slotSize, this, i);
            return go;
        }

        void BuildTooltip()
        {
            var go = new GameObject("Tooltip", typeof(RectTransform), typeof(Image), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            go.transform.SetParent(transform, false);
            tooltipRect = (RectTransform)go.transform;

            var image = go.GetComponent<Image>();
            image.color = new Color(0.05f, 0.05f, 0.08f, 0.92f);
            image.raycastTarget = false;

            var layout = go.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 8, 8);
            layout.childControlWidth = layout.childControlHeight = true;
            layout.childForceExpandWidth = layout.childForceExpandHeight = false;

            var fitter = go.GetComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var textGo = new GameObject("Text", typeof(RectTransform), typeof(Text), typeof(LayoutElement));
            textGo.transform.SetParent(go.transform, false);
            textGo.GetComponent<LayoutElement>().preferredWidth = 220f;
            tooltipText = textGo.GetComponent<Text>();
            tooltipText.font = SlotView.DefaultFont();
            tooltipText.fontSize = 18;
            tooltipText.color = Color.white;
            tooltipText.supportRichText = true;
            tooltipText.horizontalOverflow = HorizontalWrapMode.Wrap;
            tooltipText.verticalOverflow = VerticalWrapMode.Overflow;
            tooltipText.raycastTarget = false;

            go.SetActive(false);
        }

        void BuildGhost()
        {
            var go = new GameObject("DragGhost", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(transform, false);
            ghost = go.GetComponent<Image>();
            ghost.raycastTarget = false;
            ghost.preserveAspect = true;
            ghost.rectTransform.sizeDelta = new Vector2(slotSize * 0.9f, slotSize * 0.9f);
            go.SetActive(false);
        }

        static Vector2 AnchorVector(UIAnchor anchor)
        {
            switch (anchor)
            {
                case UIAnchor.BottomCenter: return new Vector2(0.5f, 0f);
                case UIAnchor.TopCenter: return new Vector2(0.5f, 1f);
                case UIAnchor.TopLeft: return new Vector2(0f, 1f);
                case UIAnchor.TopRight: return new Vector2(1f, 1f);
                case UIAnchor.BottomLeft: return new Vector2(0f, 0f);
                case UIAnchor.BottomRight: return new Vector2(1f, 0f);
                default: return new Vector2(0.5f, 0.5f);
            }
        }

        // Slots only receive mouse events if the scene has an EventSystem with the right input module.
        static void EnsureEventSystem()
        {
            var es = FindAnyObjectByType<EventSystem>();
            if (es == null) es = new GameObject("EventSystem", typeof(EventSystem)).GetComponent<EventSystem>();
            InventoryInput.ConfigureEventSystem(es);
        }
    }
}
