using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace Unity_InventoryKit
{
    // New Input System. This whole assembly is skipped by Unity (and its reference to the
    // Input System package is never resolved) unless the project has the new input enabled.
    class InputSystemBackend : IInventoryInputBackend
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register() => InventoryInput.Backend = new InputSystemBackend();

        public bool Digit(int index)
        {
            var k = Keyboard.current;
            return k != null && k[Key.Digit1 + index].wasPressedThisFrame;
        }

        public bool Pressed(InventoryKey key)
        {
            var k = Keyboard.current;
            if (k == null) return false;
            switch (key)
            {
                case InventoryKey.Toggle: return k.iKey.wasPressedThisFrame;
                case InventoryKey.Use: return k.eKey.wasPressedThisFrame;
                case InventoryKey.Drop: return k.gKey.wasPressedThisFrame;
                case InventoryKey.Save: return k.kKey.wasPressedThisFrame;
                default: return k.lKey.wasPressedThisFrame;
            }
        }

        public Vector2 Move()
        {
            var k = Keyboard.current;
            if (k == null) return Vector2.zero;
            float x = (k.dKey.isPressed || k.rightArrowKey.isPressed ? 1 : 0)
                    - (k.aKey.isPressed || k.leftArrowKey.isPressed ? 1 : 0);
            float y = (k.wKey.isPressed || k.upArrowKey.isPressed ? 1 : 0)
                    - (k.sKey.isPressed || k.downArrowKey.isPressed ? 1 : 0);
            return new Vector2(x, y).normalized;
        }

        public Vector2 PointerPosition() =>
            Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;

        public void ConfigureEventSystem(EventSystem eventSystem)
        {
            if (eventSystem.GetComponent<InputSystemUIInputModule>() != null) return;

            // The old module reads UnityEngine.Input, which throws errors with the new Input System only.
            var old = eventSystem.GetComponent<StandaloneInputModule>();
            if (old != null) Object.Destroy(old);
            eventSystem.gameObject.AddComponent<InputSystemUIInputModule>().AssignDefaultActions();
        }
    }
}
