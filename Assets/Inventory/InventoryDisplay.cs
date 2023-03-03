using System;
using System.Collections;
using System.Collections.Generic;
using I302.Manu;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryDisplay : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerMoveHandler
{
    public Inventory targetInventory;
    [SerializeField] private GameObject inventoryDisplayListObject;
    [SerializeField] private GameObject inventorySlotObject;
    public BoolVariable isMouseOverUserInterface;
    public bool isLocked;
    public bool removeOnly;
    [SerializeField] private GameEvent onMoveOrAddItem;

    private bool flaggedToUpdate;
    private List<GameObject> existingInventorySlotListObjects = new();
    public static InventoryDisplay mouseOverInventoryDisplay;

    private void Start()
    {
        UpdateInventoryDisplay();
    }

    private void LateUpdate()
    {
        if (flaggedToUpdate && !isLocked)
        {
            flaggedToUpdate = false;
            onMoveOrAddItem.Raise();
        }
    }

    public int AddItem(Item incomingItem, int amount)
    {
        int removedAmount = amount - targetInventory.TryAddItem(incomingItem, amount);
        flaggedToUpdate = true;
        return removedAmount;
    }
    [Button]
    public void UpdateInventoryDisplay()
    {
        if (existingInventorySlotListObjects.Count > 0)
        {
            foreach (var listObject in existingInventorySlotListObjects)
            {
                Destroy(listObject.gameObject);
            }
            existingInventorySlotListObjects.Clear();
        }
        foreach (Item item in targetInventory.ItemList.Keys)
        {
            var slot = Instantiate(inventorySlotObject, this.transform);
            var listObject = Instantiate(inventoryDisplayListObject, slot.transform);
            listObject.GetComponent<InventorySlot>().Setup(item,this);
            existingInventorySlotListObjects.Add(slot);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if ((mouseOverInventoryDisplay is null || mouseOverInventoryDisplay != this && !isLocked) && !removeOnly)
            mouseOverInventoryDisplay = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOverInventoryDisplay = null;  
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        // throw new NotImplementedException();
    }
}
