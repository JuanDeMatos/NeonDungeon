using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public static class SSLHelper
{
    public static void OverrideCertificateChainValidation()
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
    }

    private static bool MyRemoteCertificateValidationCallback(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
    {
        // Siempre devuelve true para aceptar el certificado sin importar si es vlido o no.
        return true;
    }
}

