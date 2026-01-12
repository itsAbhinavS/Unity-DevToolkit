using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    [Header("Slot ID")]
    [SerializeField] private int slotID;


    public void OnPointerEnter(PointerEventData eventData) 
    {
        ItemManager.Instance.ItemSlotHovering(true);
    }

    public void OnPointerExit(PointerEventData eventData) 
    {
        ItemManager.Instance.ItemSlotHovering(false);
    }

    public void OnDrop(PointerEventData eventData) 
    {
        if(eventData.pointerDrag.GetComponent<Item>() == null) return;

        ItemManager.Instance.RegisterItemSlot(slotID, eventData.pointerDrag.GetComponent<Item>().GetID(), () =>
        {
            // hold item in item slot
            eventData.pointerDrag.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
        });
    }
}