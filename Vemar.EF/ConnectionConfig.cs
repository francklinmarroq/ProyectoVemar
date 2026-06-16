namespace Vemar.EF
{
    public static class ConnectionConfig
    {
        private const string DefaultConnection =
            "Server=.\\SQLEXPRESS;Database=vemar;Trusted_Connection=True;TrustServerCertificate=True;";

        public static string ConnectionString { get; set; } = DefaultConnection;
    }
}
