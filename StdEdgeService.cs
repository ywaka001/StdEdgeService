using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text.Json;




namespace WindowsService1
{
    public partial class StdEdgeService : ServiceBase
    {
        private HttpListener _listener;

        public StdEdgeService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:8080/");
            _listener.Start();
            _listener.BeginGetContext(OnRequest, null);


            // EventLogの初期化
            this.eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("StdEdgeService"))
            {
                System.Diagnostics.EventLog.CreateEventSource("StdEdgeService", "Application");
            }
            this.eventLog1.Source = "StdEdgeService";
            this.eventLog1.Log = "Application";


        }

        protected override void OnStop()
        {
            _listener.Stop();
        }

        private void OnRequest(IAsyncResult ar)
        {
            var context = _listener.EndGetContext(ar);
            _listener.BeginGetContext(OnRequest, null);

            if (context.Request.HttpMethod == "POST")
            {
                this.eventLog1.WriteEntry("OnRequest was called.", EventLogEntryType.Information);
                using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    var json = reader.ReadToEnd();
                    var data = JsonSerializer.Deserialize<MyData>(json);

                    // ここでdata.Param1とdata.Param2を使用します。

                    var responseString = "Success";
                    var buffer = Encoding.UTF8.GetBytes(responseString);
                    context.Response.ContentLength64 = buffer.Length;
                    context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                    context.Response.OutputStream.Close();
                }
            }
        }
    }

    public class MyData
    {
        public string Param1 { get; set; }
        public string Param2 { get; set; }
    }


}

