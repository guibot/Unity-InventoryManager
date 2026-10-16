#if ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity_InventoryKit
{
    // Old Input Manager (UnityEngine.Input).
    class LegacyInputBackend : IInventoryInputBackend
    {
        public bool Digit(int index) => Input.GetKeyDown(KeyCode.Alpha1 + index);

        public bool Pressed(InventoryKey key)
        {
            switch (key)
            {
                case InventoryKey.Toggle: return Input.GetKeyDown(KeyCode.I);
                case InventoryKey.Use: return Input.GetKeyDown(KeyCode.E);
                case InventoryKey.Drop: return Input.GetKeyDown(KeyCode.G);
                case InventoryKey.Save: return Input.GetKeyDown(KeyCode.K);
                default: return Input.GetKeyDown(KeyCode.L);
            }
        }

        public Vector2 Move() =>
            new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        public Vector2 PointerPosition() => Input.mousePosition;

        public void ConfigureEventSystem(EventSystem eventSystem)
        {
            if (eventSystem.GetComponent<BaseInputModule>() == null)
                eventSystem.gameObject.AddComponent<StandaloneInputModule>();
        }
    }
}
#endif
