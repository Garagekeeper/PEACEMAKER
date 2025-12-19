using System;
using System.Collections.Generic;
using Resources.Script.Controller;
using Resources.Script.Managers;
using UnityEngine;

namespace Resources.Script.Inventory
{
    // 빈칸을 허용하지 않는 형태로 만들자.
    // 나중에 바꾸자
    public class InventoryCore : MonoBehaviour
    {
        private int _capacity;
        public int SelectedIndex { get; private set; }

        List<FirearmController> items = new List<FirearmController>();

        private void Awake()
        {
            _capacity = 4;
            SelectedIndex = -1;
        }

        public void AddItem2Inventory(FirearmController firearmController)
        {
            items.Add(firearmController);
        }

        public void SwapItemInInventory(int index1, int index2)
        {
            (items[index2], items[index1]) = (items[index1], items[index2]);
        }

        public bool RemoveItemFromInventory(int index)
        {
            items.RemoveAt(index);
            return true;
        }


        public void SelectItem(int index)
        {
            OffFirearm(SelectedIndex);
            SelectedIndex = index;
            OnFirearm(SelectedIndex);
        }

        public bool CheckValidIndex(int index)
        {
            if (index >= _capacity) return false;
            if (index >= items.Count) return false;
            if (index < 0) return false;

            return true;
        }

        public void OffFirearm(int index)
        {
            if (!CheckValidIndex(index)) return;
            items[SelectedIndex].gameObject.SetActive(false);
            if (!items[SelectedIndex].IsInitialized) return;
            SystemManager.UI.FirearmHUDs[items[index]].TurnOnOffFirearms(false);
        }

        public void OnFirearm(int index)
        {
            if (!CheckValidIndex(index)) return;
            items[index].gameObject.SetActive(true);
            if (!items[SelectedIndex].IsInitialized) return;
            SystemManager.UI.FirearmHUDs[items[index]].TurnOnOffFirearms(true);
            SystemManager.UI.Crosshair.Firearm = items[index];
        }

        public void OffAll()
        {
            foreach (var firearmController in items)
            {
                firearmController.gameObject.SetActive(false);
                if (!firearmController.IsInitialized) continue;
                SystemManager.UI.FirearmHUDs[firearmController].TurnOnOffFirearms(false);
            }
        }

        public FirearmController GetCurrentItem()
        {
            return items[SelectedIndex];
        }
    }
}