using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;

namespace Perfect_Dark_Automation {
    class Web {

        private HttpListener httpListener;
        private Thread listenThread;
        delegate void SetTextCallback(string text);

        public Web() {
            this.httpListener = new HttpListener();
            this.httpListener.Prefixes.Add(string.Format("http://*:{0}/", 1337));
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
        }


        private void ListenForClients() {
            this.httpListener.Start();
            while (true) {
                HttpListenerContext context = this.httpListener.GetContext();
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(context);
            }
        }

        private void HandleClientComm(object client) {
            HttpListenerContext httpContext = (HttpListenerContext)client;
            if (httpContext.Request.HttpMethod == "GET") {
                String tmp = "<HTML><BODY>";
                tmp += Log.GetLog();
                tmp = tmp.Replace("\n", "<br>");
                tmp += "</BODY></HTML>";
                httpContext.Response.Close(System.Text.Encoding.UTF8.GetBytes(tmp), true);
            }
            else if (httpContext.Request.HttpMethod == "POST") {
                Console.Beep(300, 100);

            }


        }
    }
}
