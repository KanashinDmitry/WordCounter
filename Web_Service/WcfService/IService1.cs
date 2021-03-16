using System.Collections.Generic;
using System.ServiceModel;

namespace WcfService
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        Dictionary<string, int> CountUniqueWords(IEnumerable<string> text);

    }
}
