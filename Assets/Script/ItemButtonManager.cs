using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemButtonManager : MonoBehaviour
{
    private string itemName;
    private string itemDescription;
    private Sprite itemImage;
    private GameObject item3DModel;
    private ARInteractionManager aRInteractionManager;
    public string ItemName 
    {
        set
        {
            itemName = value;
        }
    }
    public string ItemDescription{ set => itemDescription = value;}
    public Sprite ItemImage{set => itemImage = value;}
    public GameObject Item3DModel{set => item3DModel = value;}

    private void Start()
    {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = itemName;
        transform.GetChild(1).GetComponent<RawImage>().texture = itemImage.texture;
        transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = itemDescription;
        
        var button = GetComponent<Button>();
        button.onClick.AddListener(GameManager.instance.ARPosition);
        button.onClick.AddListener(Create3DModel);

        aRInteractionManager = FindObjectOfType<ARInteractionManager>();
    }

    private void Create3DModel()
    {
        aRInteractionManager.Item3DModel = Instantiate(item3DModel);
    }
}
