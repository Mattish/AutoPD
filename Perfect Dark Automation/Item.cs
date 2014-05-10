using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perfect_Dark_Automation {
    public class Item {
        public string fileName, uploader, hash;
        public string[] tags;
        public int face;
        public long fileSize, count;
        public bool isDownload, isComplete, isSearch, selected, doDispose, hasBeenDownloaded;
        public float progress;
        public IntPtr location;

        public Item() {
            fileName = "";
            uploader = "";
            hash = "";
        }

        public string GetHash() {
            return hash;
        }

    }
}
