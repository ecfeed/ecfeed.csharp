using System.Net;

namespace EcFeed
{
    public sealed class StatusEventArgs
    {
        public StatusMessage Schema { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string DataRaw { get; set; }
        public bool IsCompleted { get; set; }


        public override string ToString()
        {
            return 
                $"StatusEventArgs:\n" + 
                $"\t[string: { !string.IsNullOrEmpty(DataRaw) }]\n" +
                $"\t[struct: { Schema.Status != null }]\n" +
                $"\t[completed: { IsCompleted }]\n" +
                $"\t[code: { StatusCode }]\n";
        }
    } 
}