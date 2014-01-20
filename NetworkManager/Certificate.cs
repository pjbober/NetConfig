using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace NetworkManager
{

    public class Certificate
    {
        public string Name { get { return name; } }
        public string TrustedRootCA { get { return cert.Thumbprint; } }
        public X509Certificate2 Cert { get { return cert; } }


        public string name;
        public X509Certificate2 cert;


        public Certificate(X509Certificate2 cert)
        {
            this.cert = cert;
            this.name = GetName(cert);
        }


        private string GetName(X509Certificate2 cert)
        {
            string name = cert.IssuerName.Name;

            int start = name.IndexOf("CN=");
            if (start >= -1)
            {
                name = name.Substring(start + 3);
                int end = name.IndexOf(',');
                if (end != -1)
                    name = name.Substring(0, end);
            }

            return name;
        }

        public override string ToString()
        {
            return Name;
        }
    }


    public static class CertificateCollection
    {

        public static IList<Certificate> Certificates { get { return GetAllNetworkCertificates(); } }

        private static IList<Certificate> GetAllNetworkCertificates()
        {
            IList<Certificate> certs = new List<Certificate>();

            X509Store store = new X509Store(StoreName.Root);
            store.Open(OpenFlags.OpenExistingOnly);
            

            foreach (var cert in store.Certificates)
                certs.Add(new Certificate(cert));

            return certs;
        }
    }
}
