using System.ServiceModel;
using DAL.DTOs;

namespace Backend.Interfaces
{
    [ServiceContract]
    public interface IProfileSearchService
    {
        [OperationContract]
        List<SimilarProfileDto> SearchByKeyword(string keyword);
    }
}