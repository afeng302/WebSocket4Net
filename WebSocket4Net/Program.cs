using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using SuperSocket.ClientEngine.Proxy;

namespace WebSocket4Net
{
    class Program
    {
        static WebSocket wsClient = null;
        static AutoResetEvent OpenedEvent = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            // remote sevice information
            string remoteHost = "112.74.207.57";
            //string remoteHost = "10.193.228.69"; //"a23126-04"; // 10.193.228.69
            string remotePort = "2012";

            // get default web proxy
            //Uri serviceUrl = new Uri(string.Format("http://{0}", remoteHost));
            Uri serviceUrl = new Uri(string.Format("http://www.baidu.com", remoteHost));
            Uri proxyUri = WebRequest.DefaultWebProxy.GetProxy(serviceUrl);
            if (serviceUrl == proxyUri)
            {
                proxyUri = null;
            }

            //wsClient = new WebSocket("ws://112.74.207.57:2012/");
            //wsClient = new WebSocket("ws://112.74.207.5:2012/", version: WebSocketVersion.None,
                //httpConnectProxy: new DnsEndPoint("wwwgate0.mot.com", 1080));
            //wsClient = new WebSocket("ws://a23126-04:2012/", "", null, null, "", "", WebSocketVersion.None, 
            //    new DnsEndPoint("wwwgate0.mot.com", 1080));


            //List<KeyValuePair<String, String>> list_customHeaderItems = new List<KeyValuePair<string, string>>();
            //String userName = "testuser1";
            //String userPassword = "12345678";
            //string authInfo = userName + ":" + userPassword;
            //authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            //list_customHeaderItems.Add(new KeyValuePair<string, string>("Authorization", "Basic " + authInfo));

            wsClient = new WebSocket(string.Format("ws://{0}:{1}/", remoteHost, remotePort));
                //customHeaderItems: list_customHeaderItems);

            if (proxyUri != null)
            {
                Console.WriteLine("use proxy: [" + proxyUri.ToString() + "]");
                HttpConnectProxy proxy = new HttpConnectProxy(new DnsEndPoint(proxyUri.Host, proxyUri.Port));
                wsClient.Proxy = proxy;
            }

            wsClient.Opened += wsClient_Opened;
            wsClient.DataReceived += wsClient_DataReceived;
            wsClient.MessageReceived += wsClient_MessageReceived;
            wsClient.Closed += wsClient_Closed;
            wsClient.Error += wsClient_Error;

            wsClient.Open();
            if (!OpenedEvent.WaitOne(5000))
            {
                Console.WriteLine("Failed to open!");
                Console.ReadKey();
                return;
            }

            Random rnd = new Random();
            string msg = "echo " + rnd.Next(0, 100).ToString();
            Console.WriteLine("sending msg ... [" + msg + "]");
            wsClient.Send(msg);

            Console.ReadKey();
        }

        static void wsClient_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Console.WriteLine("wsClient_Error: " + e.Exception.Message);
        }

        static void wsClient_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("wsClient_Closed");
        }

        static void wsClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine("wsClient_MessageReceived: [" + e.Message + "]");

            byte[] data = new byte[10];

            Console.WriteLine("sending data ... length=[" + data.Length + "]");
            ((WebSocket)sender).Send(data, 0, data.Length);
        }

        static void wsClient_DataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("wsClient_DataReceived. Length= [" + e.Data.Length + "]");

            Console.WriteLine("will close connect ...");
            Thread.Sleep(1000);

            ((WebSocket)sender).Close();
        }

        static void wsClient_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("wsClient_Opened");

            OpenedEvent.Set();
        }
    }
}
