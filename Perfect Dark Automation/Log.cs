using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Perfect_Dark_Automation {
    static public class Log {

        static public RichTextBox log;
        static public int maxLength;
        static public bool writeToFile;
        public delegate string GetLogTextCallback();
        public delegate void WriteCallback(string str);
        public delegate void WriteLineCallback(string str);

        static public void SetLogWindow(RichTextBox rtb) {
            log = rtb;
            maxLength = 5000;
            writeToFile = false;
        }

        static public void WriteLine(string s) {
            if (log.InvokeRequired) {
                log.Invoke(new WriteLineCallback(WriteLineText), s);
            }
            else {
                WriteLineText(s);
            }
        }

        static private void WriteLineText(string s) {
            if (log.TextLength > Log.maxLength)
                log.Text = "";
            log.Text += DateTime.Now.ToLongTimeString() + ": " + s + System.Environment.NewLine;
            log.SelectionStart = log.Text.Length;
            log.ScrollToCaret();
            if (writeToFile) {
                try {
                    using (StreamWriter sw = File.AppendText(Directory.GetCurrentDirectory() + "//log.txt")) {

                        sw.WriteLine(DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToLongTimeString() + ": " + s);
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        static public void Write(string s) {
            if (log.InvokeRequired) {
                log.Invoke(new WriteCallback(WriteText), s);
            }
            else {
                WriteText(s);
            }
        }

        static private void WriteText(string s) {
            log.Text += s;
            log.SelectionStart = log.Text.Length;
            log.ScrollToCaret();
            if (writeToFile) {
                try {
                    using (StreamWriter sw = File.AppendText(Directory.GetCurrentDirectory() + "//log.txt")) {
                        sw.Write(s);
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
        }

        static public string GetLog() {
            if (log.InvokeRequired) {
                return (string)log.Invoke(new GetLogTextCallback(GetLogText));
            }
            else {
                return GetLogText();
            }
        }

        static private string GetLogText() {
            return log.Text;
        }
    }
}
