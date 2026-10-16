using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity_InventoryKit
{
    public enum InventoryKey { Toggle, Use, Drop, Save, Load }

    // Everything the package needs from the keyboard and mouse.
    public interface IInventoryInputBackend
    {
        bool Digit(int index);                 // number keys 1-9 (index 0-8)
        bool Pressed(InventoryKey key);
        Vector2 Move();
        Vector2 PointerPosition();
        void ConfigureEventSystem(EventSystem eventSystem);
    }

    // The package works with the old Input Manager, the new Input System, or both,
    // and does NOT require the Input System package to be installed:
    //  - the old backend lives here and is only compiled when the old input is enabled;
    //  - the new backend lives in Runtime/InputSystem, a separate assembly that is only
    //    compiled when ENABLE_INPUT_SYSTEM is defined, and registers itself at startup.
    public static class InventoryInput
    {
        public static IInventoryInputBackend Backend { get; set; } = CreateDefault();

        static IInventoryInputBackend CreateDefault()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            return new LegacyInputBackend();
#else
            return null;
#endif
        }

        public static bool Digit(int index) => Backend != null && Backend.Digit(index);

        public static bool Toggle() => Pressed(InventoryKey.Toggle);   // I: open/close the grid
        public static bool Use() => Pressed(InventoryKey.Use);         // E: use the selected item
        public static bool Drop() => Pressed(InventoryKey.Drop);       // G: drop the selected item
        public static bool Save() => Pressed(InventoryKey.Save);       // K
        public static bool Load() => Pressed(InventoryKey.Load);       // L

        public static Vector2 Move() => Backend != null ? Backend.Move() : Vector2.zero;
        public static Vector2 PointerPosition() => Backend != null ? Backend.PointerPosition() : Vector2.zero;

        public static void ConfigureEventSystem(EventSystem eventSystem) => Backend?.ConfigureEventSystem(eventSystem);

        static bool Pressed(InventoryKey key) => Backend != null && Backend.Pressed(key);
    }
}
