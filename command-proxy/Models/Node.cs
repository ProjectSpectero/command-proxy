namespace Spectero.Cproxy.Models
{
    public class Node : BaseModel
    {
        public long id { get; set; }
        public Protocol protocol { get; set; }
        public string ip { get; set; }
        public int port { get; set; }
        public Credential credentials { get; set; }
    }

    public enum Protocol
    {
        http,
        https
    }

    public class Token : BaseModel
    {
        public string token { get; set; }

        public long expires { get; set; }
    }

    public class Credential : BaseModel
    {
        public Token access { get; set; }
        public Token refresh { get; set; }
    }

}