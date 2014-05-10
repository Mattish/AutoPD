using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Perfect_Dark_Automation {
    public static class UI {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "PostMessageA", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_CHAR = 0x0102;

        public static AutomationElement pdWindow, pdDownload, pdSearch, addDownload, searchTree, pdComplete;
        public static bool getNew = true;

        public static bool GetUIElements() {
            IntPtr workingWindow = GetForegroundWindow();
            AutomationElement ae = AutomationElement.RootElement;
            Log.WriteLine("UI - Looking for Perfect Dark...");
            pdWindow = FindChildElementByName(ref ae, TreeScope.Children,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window),
                "perfect dark ");
            if (pdWindow != null) {
                Log.WriteLine("UI - Got Perfect Dark window, getting main pane...");
                AutomationElement mainPane = FindChildElementByName(ref pdWindow, TreeScope.Children,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Pane),
                    "mane pain0");
                if (mainPane != null) {
                    //MAIN PANE
                    pdDownload = FindChildElementByName(ref mainPane, TreeScope.Children,
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button),
                        "download"); // GET DOWNLOAD BUTTON
                    pdSearch = FindChildElementByName(ref mainPane, TreeScope.Children,
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button),
                        "search"); // GET SEARCH BUTTON
                    pdComplete = FindChildElementByName(ref mainPane, TreeScope.Children,
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button),
                        "complete"); // GET COMPLETE BUTTON
                    SwitchToDownload(); //DOWNLOAD WINDOW
                    AutomationElement pdList = FindChildElementByName(ref mainPane, TreeScope.Children,
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Pane),
                        "pd list");
                    if (pdList != null) {
                        addDownload = FindChildElementByName(ref pdList, TreeScope.Children,
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button),
                        "add download");
                    }
                    SwitchToSearch(); //SEARCH WINDOW
                    AutomationElement pdSearchpane = FindChildElementByName(ref mainPane, TreeScope.Children,
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Pane),
                        "pd search");
                    if (pdSearchpane != null) {
                        searchTree = FindChildElementByName(ref pdSearchpane, TreeScope.Children,
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Pane),
                        "pd search tree");
                        getNew = false;
                        Log.WriteLine("UI - Ready");
                        SetForegroundWindow(workingWindow);
                        return true;
                    }
                }
            }

            if (getNew)
                Log.WriteLine("UI - Error getting all UI elements, is PD open?");
            SetForegroundWindow(workingWindow);
            return false;
        }

        public static AutomationElement FindChildElementByName(ref AutomationElement parent, TreeScope treescope, PropertyCondition condition, string name) {
            if (parent != null) {
                AutomationElementCollection aeCol = parent.FindAll(treescope, condition);
                foreach (AutomationElement ae in aeCol) {
                    if (ae.Current.Name.Contains(name)) {
                        return ae;
                    }
                }
            }
            return null;
        }

        public static void SwitchToDownload() {
            IntPtr workingWindow = GetForegroundWindow();
            InvokePattern invokePattern;
            invokePattern = pdDownload.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            invokePattern.Invoke();
            SetForegroundWindow(workingWindow);
        }

        public static void SwitchToComplete() {
            IntPtr workingWindow = GetForegroundWindow();
            InvokePattern invokePattern;
            invokePattern = pdComplete.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            invokePattern.Invoke();
            SetForegroundWindow(workingWindow);
        }

        public static void SwitchToSearch() {
            IntPtr workingWindow = GetForegroundWindow();
            InvokePattern invokePattern;
            invokePattern = pdSearch.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            invokePattern.Invoke();
            SetForegroundWindow(workingWindow);
        }

        public static bool AddDownload(string hash) {
            IntPtr handle = (IntPtr)FindWindowByIndex(Memory.process.MainWindowHandle, 4);
            //if (handle == IntPtr.Zero) return false;
            //handle = (IntPtr)FindWindowByIndex(handle, 7);
            if (handle == IntPtr.Zero) return false;
            handle = (IntPtr)FindWindowEx(handle, IntPtr.Zero, null, "pd list");
            if (handle == IntPtr.Zero) return false;
            handle = (IntPtr)FindWindowEx(handle, IntPtr.Zero, null, "add download");
            if (handle == IntPtr.Zero) return false;
            PostMessage(handle, WM_KEYDOWN, (IntPtr)Keys.Space, IntPtr.Zero);
            PostMessage(handle, WM_KEYUP, (IntPtr)Keys.Space, IntPtr.Zero);
            Thread.Sleep(500);
            handle = (IntPtr)FindWindow(null, "add download");
            if (handle == IntPtr.Zero) return false;
            handle = (IntPtr)FindWindowByIndex(handle, 4);
            if (handle == IntPtr.Zero) return false;
            Thread.Sleep(250);
            foreach (char c in hash) {
                PostMessage(handle, WM_CHAR, (IntPtr)(int)c, IntPtr.Zero);

            }
            Thread.Sleep(250);
            handle = (IntPtr)FindWindow(null, "add download");
            handle = (IntPtr)FindWindowEx(handle, IntPtr.Zero, null, "ok");
            PostMessage(handle, WM_KEYDOWN, (IntPtr)Keys.Space, IntPtr.Zero);
            PostMessage(handle, WM_KEYUP, (IntPtr)Keys.Space, IntPtr.Zero);
            return true;
        }

        public static bool Update() {
            if (!getNew)
                return true;
            else {
                if (GetUIElements()) {
                    getNew = false;
                    return true;
                }
            }
            return false;
        }

        public static void StartInvokeWorkerThread(object invPattern) {
            (invPattern as InvokePattern).Invoke();
        }

        public static IntPtr FindWindowByIndex(IntPtr hwndParent, int index) {
            if (index == 0)
                return hwndParent;
            else {
                int ct = 0;
                IntPtr result = IntPtr.Zero;
                do {
                    result = FindWindowEx(hwndParent, result, null, null);
                    if (result != IntPtr.Zero)
                        ++ct;
                } while (ct < index && result != IntPtr.Zero);
                return result;
            }
        }
    }
}
