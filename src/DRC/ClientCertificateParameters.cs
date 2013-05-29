namespace DRC
{
    using System.Security.Cryptography.X509Certificates;

    public class ClientCertificateParameters
    {
        public string FindString { get; set; }
        public X509FindType FindBy { get; set; }
        public StoreLocation StoreLocation { get; set; }
        public StoreName StoreName { get; set; }
    }
}