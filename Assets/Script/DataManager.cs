using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [SerializeField] private List<Item> items = new List<Item>();
    [SerializeField] private GameObject buttonContainer;
    [SerializeField] private ItemButtonManager itemButtonManager;

    void Start()
    {
        GameManager.instance.OnItemsMenu += CreateButtons;
    }

    private void CreateButtons()
    {
        foreach(var item in items)
        {
            ItemButtonManager itemButton;
            itemButton = Instantiate(itemButtonManager, buttonContainer.transform);
            itemButton.ItemName = item.itemName;
            itemButton.ItemImage = item.itemImage;
            itemButton.ItemDescription = item.itemDescription;
            itemButton.Item3DModel = item.item3DModel;
            itemButton.name = item.itemName;
        }

        GameManager.instance.OnItemsMenu -= CreateButtons;
    }
}
