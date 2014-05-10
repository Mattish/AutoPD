using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perfect_Dark_Automation {
    public class Table {
        public List<Item> items;

        public Table() {
            Reset();
        }

        public Item Item
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public int ContainsItemByHash(string hash) {
            for (int t = 0; t < items.Count; t++) {
                if (items[t].GetHash() == hash)
                    return t;
            }
            return -1;
        }
        public void Update() {

        }
        public void Add(Item item) {
            items.Add(item);
        }
        public void Reset() {
            items = new List<Item>(10);
        }

        public Item GetItemByIndex(int i) {
            return items[i];
        }
        public void SetAllDisposable() {
            for (int t = 0; t < items.Count; t++) {
                items[t].doDispose = true;
            }
        }
        public void CheckForDisposable() {
            for (int t = 0; t < items.Count; t++) {
                if (items[t].doDispose) {
                    items.RemoveAt(t);
                    t--;
                }
            }
        }
    }
}
