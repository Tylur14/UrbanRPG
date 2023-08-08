using System.Collections;
using System.Collections.Generic;
using I302.Manu;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Shop", menuName = "Items and Inventory/Shop")]
public class Shop : SerializedScriptableObject
{
    [SerializeField, FoldoutGroup("Details")] private string storeName;
    public Inventory targetInventory; // Where the purchases go -- most go to pockets but some might go to mail
    [FoldoutGroup("Data")] public List<Item> storeItems;

    public void OpenShop()
    {
        GameEvents.OnOpenShop.Raise(this);
    }
}
