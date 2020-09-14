﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    [SerializeField] private GameObject itemGainMessageBox = null;
    public GameObject ItemGainMessageBox => itemGainMessageBox;

    [SerializeField] private GameObject warningMessageBox = null;
    public GameObject WarningMessageBox => warningMessageBox;

    [SerializeField] private GameObject fishGainMessageBox = null;
    public GameObject FishGainMessageBox => fishGainMessageBox;

    [SerializeField] private GameObject cosmeticGainMessageBox = null;
    public GameObject CosmeticGainMessageBox => cosmeticGainMessageBox;

    [SerializeField] private Canvas canvas = null;
    public Canvas Canvas => canvas;

    private float boxFadeSpeed = 1f;
    private int paddingBetween = 50;
    private int maxMessages = 6;

    private Queue<MessageBox> messageBoxes;


    private static MessageManager _instance;
    public static MessageManager Instance { get { return _instance; } }
    private void Awake()
    {
        //Singleton
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        messageBoxes = new Queue<MessageBox>();

        InventoryManager.OnItemGained += (InventoryItemInstance item, int count) => { item.ShowMessage(count); };
    }

    public void ShowMessage(MessageBox newMessageBox)
    {
        newMessageBox.RectTransform.anchoredPosition = new Vector2(-80, 40);
        foreach (MessageBox messageBox in messageBoxes)
        {
            Vector2 pos = messageBox.RectTransform.anchoredPosition;
            messageBox.RectTransform.anchoredPosition = new Vector2(pos.x, pos.y + paddingBetween);
        }

        messageBoxes.Enqueue(newMessageBox);

        if (messageBoxes.Count > maxMessages)
        {
            messageBoxes.Peek().Destroy();
            messageBoxes.Dequeue();
        }
    }

    private void Update()
    {
        foreach (MessageBox messageBox in messageBoxes)
        {
            if (messageBox.timeRemaining <= 0 && messageBox.alpha > 0)
                messageBox.alpha -= boxFadeSpeed * Time.deltaTime;
            else
                messageBox.timeRemaining -= Time.deltaTime;
        }

        while (messageBoxes.Count != 0 && messageBoxes.Peek().alpha <= 0)
        {
            messageBoxes.Peek().Destroy();
            messageBoxes.Dequeue();
        }
    } 

}