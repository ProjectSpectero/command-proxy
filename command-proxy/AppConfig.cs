namespace Spectero.Cproxy
{
    public class AppConfig
    {
        public string PoolSigningKey { get; set; }
        public string JWTSigningKey { get; set; }
        public string LoggingConfig { get; set; }
        public bool RedirectHttpToHttps { get; set; }
    }
}