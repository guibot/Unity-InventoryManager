using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity_InventoryKit
{
    // Mouse interaction for one slot: hover (tooltip), click to select, drag and drop, right click to use.
    // Events from the slot's child graphics bubble up to this component.
    public class SlotHandler : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler,
        IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler,
        IPointerClickHandler
    {
        InventoryUI ui;
        int index;

        public void Init(InventoryUI ui, int index)
        {
            this.ui = ui;
            this.index = index;
        }

        public void OnPointerEnter(PointerEventData e) => ui.SetHover(index);
        public void OnPointerExit(PointerEventData e) => ui.ClearHover(index);
        public void OnBeginDrag(PointerEventData e) => ui.BeginDrag(index, e);
        public void OnDrag(PointerEventData e) => ui.Drag(e);
        public void OnEndDrag(PointerEventData e) => ui.EndDrag();
        public void OnDrop(PointerEventData e) => ui.DropOn(index);

        public void OnPointerClick(PointerEventData e)
        {
            // Left click selects, like the number keys; right click uses the item.
            if (e.button == PointerEventData.InputButton.Left) ui.Select(index);
            else if (e.button == PointerEventData.InputButton.Right) ui.UseSlot(index);
        }
    }
}
