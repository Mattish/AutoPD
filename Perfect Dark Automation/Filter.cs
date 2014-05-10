using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perfect_Dark_Automation {
    public class Filter {
        public string fileName, uploader, hash;
        public bool persistant;

        public Filter(string fileName, string uploader, string hash, bool persistant) {
            this.fileName = fileName;
            this.uploader = uploader;
            this.hash = hash;
            this.persistant = persistant;
        }
    }
}
