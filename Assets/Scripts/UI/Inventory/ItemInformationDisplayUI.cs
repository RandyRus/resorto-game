﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInformationDisplayUI : UIObject
{
    public ItemInformationDisplayUI(): 
        base(ResourceManager.Instance.ItemInformationDisplay, ResourceManager.Instance.IndicatorsCanvas.transform)
    {

    }

    public void SetItem(InventoryItemInstance itemInstance)
    {
        Transform t = ObjectInScene.transform;

        foreach (Transform tr in t)
        {
            if (tr.tag == "Name Field")
            {
                OutlinedText itemNameText = new OutlinedText(tr.gameObject);
                itemNameText.SetText(itemInstance.ItemInformation.ItemName);
            }
            else if (tr.tag == "Item Icon Field")
            {
                Image itemIcon = tr.GetComponent<Image>();
                itemIcon.sprite = itemInstance.ItemInformation.ItemIcon;

            }
            else if (tr.tag == "Tag Field")
            {
                OutlinedText itemTagText = new OutlinedText(tr.gameObject);
                itemTagText.SetText(itemInstance.ItemInformation.Tag.ToString());
                itemTagText.SetColor(ResourceManager.Instance.ItemTagColors[(int)itemInstance.ItemInformation.Tag]);
            }
        }
    }
}
