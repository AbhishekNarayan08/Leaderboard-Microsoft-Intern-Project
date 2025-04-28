// <copyright file="EncryptionUtil.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Configs
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    public class EncryptionUtil
    {
        private static readonly IDictionary<string, X509Certificate2> Certificates = new Dictionary<string, X509Certificate2>();

        public static X509Certificate2 GetCertificate(
            string thumbprint,
            StoreName storeName = StoreName.My,
            StoreLocation storeLocation = StoreLocation.CurrentUser,
            StoreLocation fallBackStoreLocation = StoreLocation.LocalMachine)
        {
            Certificates.TryGetValue(thumbprint, out var cert);
            if (cert == null)
            {
                cert = RetrieveCertificate(thumbprint, storeName, storeLocation) ??
                        RetrieveCertificate(thumbprint, storeName, fallBackStoreLocation);
                if (cert == null)
                {
                    throw new Exception($"Could not retrieve the certificate with thumbprint : {thumbprint}");
                }

                Certificates.Add(thumbprint, cert);
            }

            return cert;
        }

        public static string DecryptUsingCertificate(string data, string thumbprint)
        {
            try
            {
                byte[] byteData = Convert.FromBase64String(data);
                X509Certificate2 certificate = GetCertificate(thumbprint);

                if (certificate.HasPrivateKey)
                {
                    var csp = certificate.GetRSAPrivateKey();
                    var keys = Encoding.UTF8.GetString(csp.Decrypt(byteData, RSAEncryptionPadding.OaepSHA1));
                    return keys;
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to decrypt the data with thumbprint : {thumbprint}");
            }

            return null;
        }

        public static string EncryptUsingCertificate(string data, string thumbprint)
        {
            try
            {
                var byteData = Encoding.UTF8.GetBytes(data);
                var certificate = GetCertificate(thumbprint);
                var output = string.Empty;
                using (var csp = certificate.GetRSAPrivateKey())
                {
                    var bytesEncrypted = csp.Encrypt(byteData, RSAEncryptionPadding.OaepSHA1);
                    output = Convert.ToBase64String(bytesEncrypted);
                }

                return output;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static X509Certificate2 RetrieveCertificate(string thumb, StoreName storeName = StoreName.My, StoreLocation storeLocation = StoreLocation.CurrentUser)
        {
            X509Certificate2 cert = null;
            var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);
            try
            {
                var certCollection = store.Certificates.Find(X509FindType.FindByThumbprint, thumb, false);
                if (certCollection.Count > 0)
                {
                    return certCollection[0];
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                store.Close();
            }

            return cert;
        }
    }
}
