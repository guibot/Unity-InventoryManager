using UnityEngine;
using UnityEngine.UI;

namespace Unity_InventoryKit
{
    // One slot drawn with uGUI: border, background, icon (or tinted square) and stack count.
    // Uses the plain UI Text with the built-in font, so there is nothing to import (no TextMeshPro).
    public class SlotView
    {
        static readonly Color Normal = new Color(0f, 0f, 0f, 0.55f);
        static readonly Color Border = new Color(1f, 0.85f, 0.2f, 1f);
        const float BorderWidth = 3f;

        static Font font;

        Image border;
        Image icon;
        Text count;

        public static Font DefaultFont()
        {
            if (font == null) font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null) font = Font.CreateDynamicFontFromOSFont("Arial", 16);
            return font;
        }

        public static SlotView Create(Transform parent, float size, InventoryUI ui, int index)
        {
            // The root is the border (yellow, only visible when selected); the dark
            // background sits inside it, so a selected slot gets an outline instead of a fill.
            var root = new GameObject("Slot", typeof(RectTransform), typeof(Image));
            root.transform.SetParent(parent, false);
            ((RectTransform)root.transform).sizeDelta = new Vector2(size, size);

            var view = new SlotView { border = root.GetComponent<Image>() };
            view.border.color = Border;
            view.border.enabled = false;
            root.AddComponent<SlotHandler>().Init(ui, index);

            var bgGo = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bgGo.transform.SetParent(root.transform, false);
            Stretch((RectTransform)bgGo.transform, BorderWidth);
            var bg = bgGo.GetComponent<Image>();
            bg.color = Normal;
            bg.raycastTarget = true;   // receives the mouse; events bubble up to the SlotHandler

            var iconGo = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            iconGo.transform.SetParent(root.transform, false);
            Stretch((RectTransform)iconGo.transform, 8f);
            view.icon = iconGo.GetComponent<Image>();
            view.icon.preserveAspect = true;
            view.icon.raycastTarget = false;

            var countGo = new GameObject("Count", typeof(RectTransform), typeof(Text), typeof(Shadow));
            countGo.transform.SetParent(root.transform, false);
            Stretch((RectTransform)countGo.transform, 4f);
            view.count = countGo.GetComponent<Text>();
            view.count.font = DefaultFont();
            view.count.fontSize = Mathf.RoundToInt(size * 0.28f);
            view.count.fontStyle = FontStyle.Bold;
            view.count.alignment = TextAnchor.LowerRight;
            view.count.color = Color.white;
            view.count.horizontalOverflow = HorizontalWrapMode.Overflow;
            view.count.verticalOverflow = VerticalWrapMode.Overflow;
            view.count.raycastTarget = false;
            var shadow = countGo.GetComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.85f);
            shadow.effectDistance = new Vector2(1.5f, -1.5f);

            view.Show(null);
            return view;
        }

        public void Show(InventorySlot slot)
        {
            bool has = slot != null && !slot.IsEmpty;
            icon.enabled = has;
            count.text = has && slot.Count > 1 ? slot.Count.ToString() : "";
            if (!has) return;
            icon.sprite = slot.Item.icon;
            icon.color = slot.Item.tint;
        }

        public void Highlight(bool on) => border.enabled = on;

        static void Stretch(RectTransform rt, float inset)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(inset, inset);
            rt.offsetMax = new Vector2(-inset, -inset);
        }
    }
}
