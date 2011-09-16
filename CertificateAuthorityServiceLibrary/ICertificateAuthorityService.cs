using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace CertificateAuthorityServiceLibrary
{
    [ServiceContract]
    public interface ICertificateAuthorityService
    {
        [OperationContract]
        CertificateAuthorityHeader GetCertificateAuthority();
    }
}
