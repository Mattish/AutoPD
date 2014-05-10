using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;
using System.Threading;
using System.Windows.Forms;

namespace Perfect_Dark_Automation {
    class PerfectDark {
        public PerfectDark() {
            //Memory.Update(false);
            Filters.filters = new List<Filter>(5);
        }

        public void AddDownload(string hash) {
            UI.AddDownload(hash);
        }
    }
}
