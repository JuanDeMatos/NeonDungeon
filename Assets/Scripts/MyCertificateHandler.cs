using UnityEngine.Networking;

public class MyCertificateHandler : CertificateHandler
{
    // Override de ValidateCertificate para validar el certificado
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // Aqu� puedes implementar la l�gica para validar el certificado
        // Retorna true si el certificado es v�lido, false si no lo es
        return true;
    }
}
