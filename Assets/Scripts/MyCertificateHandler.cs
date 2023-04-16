using UnityEngine.Networking;

public class MyCertificateHandler : CertificateHandler
{
    // Override de ValidateCertificate para validar el certificado
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // Aquí puedes implementar la lógica para validar el certificado
        // Retorna true si el certificado es válido, false si no lo es
        return true;
    }
}
