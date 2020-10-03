﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private class SavedUI
    {
        public SavedUI(SidebarPanel panel, bool inventoryOpen)
        {
            SavedPanel = panel;
            BSavedInventoryIsOpen = inventoryOpen;
        }

        public SidebarPanel SavedPanel { get; }
        public bool BSavedInventoryIsOpen { get; }
    }
    private SavedUI savedUI;

    private HashSet<UIObject> otherExternalOpenUI = new HashSet<UIObject>();

    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }
    private void Awake()
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

    public void ShowExternalUI(UIObject ui)
    {
        ui.Show(true);
        otherExternalOpenUI.Add(ui);
    }

    //Returns true if something was closed
    public bool CloseAllUI(bool save)
    {
        SavedUI proposedSavedUI = save ? new SavedUI(Sidebar.Instance.CurrentPanel, InventoryManager.Instance.IsInventoryOpen) : null;
        bool somethingClosed = false;

        if (Sidebar.Instance.CurrentPanel != null)
        {
            Sidebar.Instance.CloseSidebar();
            somethingClosed = true;
        }

        if (InventoryManager.Instance.IsInventoryOpen)
        {
            InventoryManager.Instance.TryHideInventory();
            somethingClosed = true;
        }

        //External ui will not be saved when reloaded
        foreach (UIObject ui in otherExternalOpenUI)
        {
            ui.Show(false);
            somethingClosed = true;
        }
        otherExternalOpenUI.Clear();

        if (somethingClosed)
            savedUI = proposedSavedUI;

        return somethingClosed;
    }

    public void ReloadSavedUI()
    {
        if (savedUI == null)
            return;

        if (savedUI.SavedPanel != null)
            Sidebar.Instance.OpenSidebar(savedUI.SavedPanel);

        if (savedUI.BSavedInventoryIsOpen)
            InventoryManager.Instance.ShowInventory();
    }
}
