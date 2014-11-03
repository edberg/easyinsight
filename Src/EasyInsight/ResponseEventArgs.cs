using System;
using System.Net;

namespace EasyInsight
{
    public class ResponseEventArgs : EventArgs
    {
        public HttpStatusCode Code { get; set; }
        public string Xml { get; set; }
    }
}