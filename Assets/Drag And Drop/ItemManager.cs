using System;
using Unity.VisualScripting;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance = null;

    public static event Action OnAllAnswerCorrect;

    [Header("Item Containers")]
    [SerializeField] private RectTransform[] itemHolder;
    [SerializeField] private int[] itemSlotHoldingID;
    [SerializeField] private bool[] itemSlotCorrect;
    [SerializeField] private bool itemSlotHover;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else 
        {
            Destroy(this);
        }
    }

    public void ItemBeingDragged(RectTransform rect, int itemID)
    {
        for (int i = 0; i < itemSlotHoldingID.Length; i++)
        {
            if (itemSlotHoldingID[i] == itemID)
            {
                itemSlotHoldingID[i] = -1;
                itemSlotCorrect[i] = false;

                return;
            }
        }
    }
    public void DroppedItem(RectTransform rect, int itemID)
    {
        if(itemSlotHover) return; // user trying to drop an item so skip placing the item back to its slot

        for (int i = 0; i < itemSlotHoldingID.Length; i++) 
        {
            if (itemSlotHoldingID[i] == itemID) 
            {
                return;
            }
        }

        rect.localPosition = Vector3.zero;
    }

    public void RegisterItemSlot(int itemSlotID, int itemID, Action action) 
    {
        // item slot is already holding an item
        if (itemSlotHoldingID[itemSlotID - 1] != -1) return;

        itemSlotHoldingID[itemSlotID-1] = itemID;

        if (itemSlotID == itemID) 
        { 
            itemSlotCorrect[itemID - 1] = true; 
        }

        action.Invoke();

        CheckAllAnswers();
    }

    private void CheckAllAnswers()
    {
        for (int i = 0; i < itemSlotCorrect.Length; i++)
        {
            if (itemSlotCorrect[i] == false)
            {
                return;
            }
        }

        Debug.Log("All Answers Correct !");

        OnAllAnswerCorrect?.Invoke();
    }

    public void ItemSlotHovering(bool hovering) 
    {
        itemSlotHover = hovering;
    }
}
