using System;

namespace EasyInsight
{
    public class RequestEventArgs : EventArgs
    {
        public string Url { get; set; }
        public string Xml { get; set; }
    }
}