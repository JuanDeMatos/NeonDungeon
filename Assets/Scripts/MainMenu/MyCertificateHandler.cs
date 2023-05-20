using UnityEngine.Networking;

public class MyCertificateHandler : CertificateHandler
{
    // Override de ValidateCertificate para validar el certificado
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // Aqui puedes implementar la logica para validar el certificado
        // Retorna true si el certificado es vlido, false si no lo es
        return true;
    }
}
