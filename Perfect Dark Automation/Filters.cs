using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Perfect_Dark_Automation {
    public static class Filters {
        public static List<Filter> filters;
        public static bool checking;
        public static System.Windows.Forms.Label active;

        public static Filter Filter
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public static void UpdateLabel() {
            active.Text = "Active filters: " + Filters.filters.Count;
        }
        public static void Check() {
            UpdateLabel();
            checking = true;
            if (filters == null)
                filters = new List<Filter>(5);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int filterCount = filters.Count;
            for (int t = 0; t < filters.Count; t++) {
                bool matched = false;
                Item matchedItem = new Item();
                foreach (Item item in Memory.SearchTable.items) {
                    int counter = 0;
                    if (filters[t].hash != "") {
                        if (filters[t].hash == item.hash) {
                            counter++;
                            if (!filters[t].persistant)
                                matched = true;
                            break;
                        }
                    }
                    else
                        counter++;

                    if (filters[t].fileName != "") {
                        if (item.fileName.Contains(filters[t].fileName))
                            counter++;
                    }
                    else
                        counter++;

                    if (filters[t].uploader == "" || filters[t].uploader == null) {
                        counter++;
                    }
                    else {
                        if (item.uploader.Contains(filters[t].uploader))
                            counter++;
                    }

                    if (counter == 3) {
                        if (!filters[t].persistant)
                            matched = true;
                        if (!item.hasBeenDownloaded) {
                            if (UI.AddDownload(item.hash))
                            {
                                item.hasBeenDownloaded = true;
                                matchedItem = item;
                                if (matchedItem.fileName != "" && matchedItem.hash != "" && matchedItem.uploader != "")
                                {
                                    Log.WriteLine("Filter matched - fileName='" + matchedItem.fileName + 
                                        "' hash='" + matchedItem.hash + "' uploader='" + matchedItem.uploader + "'");
                                }
                            }
                        }
                    }


                }
                if (matched) {
                    filters.RemoveAt(t);
                    t--;
                }
            }
            sw.Stop();
            long past = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L));
            checking = false;
            UpdateLabel();
        }

        public static void Add(Filter filter) {
            foreach (Filter f in filters) {
                if (f.fileName == filter.fileName &&
                    f.hash == filter.hash &&
                    f.uploader == filter.uploader) {
                    Log.WriteLine("Attempt of same filter adding!");
                    return;
                }
            }

            filters.Add(filter);
            UpdateLabel();
        }
    }
}
