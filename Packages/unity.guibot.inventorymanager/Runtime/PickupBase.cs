using UnityEngine;

namespace Unity_InventoryKit
{
    // Shared by Pickup2D and Pickup3D: only the physics callback differs.
    //
    // Visual: with no Icon on the item, the placeholder (sphere, square...) stays as is.
    // With an Icon, the placeholder is hidden and the icon is drawn in its place, also in
    // the editor. Nothing is written to the scene: the icon object is hidden + not saved,
    // and the placeholder is hidden with Renderer.forceRenderingOff (not serialized).
    [ExecuteAlways]
    public abstract class PickupBase : MonoBehaviour
    {
        const string IconName = "~Icon";

        public ItemData item;
        [Min(1)] public int amount = 1;

        float collectableAt;
        Sprite shownIcon;
        Renderer placeholder;
        Transform iconTransform;
        Transform cam;

        // 3D pickups draw the icon as a billboard facing the camera.
        protected virtual bool FaceCamera => false;

        void OnEnable() => RefreshIcon();
        void OnDisable() => ClearIcon();

        void Update()
        {
            var current = item != null ? item.icon : null;
            if (current != shownIcon) RefreshIcon();
        }

        void LateUpdate()
        {
            if (iconTransform == null || !FaceCamera) return;
            if (cam == null && Camera.main != null) cam = Camera.main.transform;
            if (cam != null) iconTransform.rotation = cam.rotation;
        }

        void RefreshIcon()
        {
            ClearIcon();

            var icon = item != null ? item.icon : null;
            shownIcon = icon;
            if (icon == null) return;

            placeholder = GetComponent<Renderer>();

            // Same on-screen size as the placeholder (a bit larger for 3D billboards).
            float target = 1f;
            if (placeholder != null)
            {
                var b = placeholder.bounds.size;
                float placeholderSize = Mathf.Max(b.x, b.y);
                if (placeholderSize > 0f) target = placeholderSize;
            }
            if (FaceCamera) target *= 1.6f;

            float iconSize = Mathf.Max(icon.bounds.size.x, icon.bounds.size.y);
            if (iconSize <= 0f) return;

            var go = new GameObject(IconName) { hideFlags = HideFlags.HideAndDontSave };
            go.transform.SetParent(transform, false);
            go.transform.localScale = Vector3.one * (target / iconSize / Mathf.Max(0.0001f, transform.lossyScale.x));

            var sr = go.AddComponent<SpriteRenderer>();
            SpriteMaterial.Apply(sr);
            sr.sprite = icon;
            sr.color = item.tint;

            if (placeholder != null) placeholder.forceRenderingOff = true;
            iconTransform = go.transform;
        }

        void ClearIcon()
        {
            if (placeholder != null) placeholder.forceRenderingOff = false;
            placeholder = null;
            iconTransform = null;
            shownIcon = null;

            if (this == null) return;
            var existing = transform.Find(IconName);
            if (existing == null) return;
            if (Application.isPlaying) Destroy(existing.gameObject);
            else DestroyImmediate(existing.gameObject);
        }

        // Dropped items wait a moment so the player does not pick them straight back up.
        public void SetCollectDelay(float seconds) => collectableAt = Time.time + seconds;

        protected void TryCollect(GameObject other)
        {
            if (item == null || Time.time < collectableAt) return;
            var holder = other.GetComponentInParent<InventoryHolder>();
            if (holder == null) return;

            amount = holder.Inventory.Add(item, amount);
            if (amount <= 0) Destroy(gameObject);
        }
    }
}
