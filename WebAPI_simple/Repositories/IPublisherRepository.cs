using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;
using System.Threading.Tasks;

namespace WebAPI_simple.Repositories
{
    public interface IPublisherRepository
    {
        List<PublisherDTO> GetAllPublishers();
        PublisherNoIdDTO GetPublisherById(int id);
        AddPublisherRequestDTO AddPublisher(AddPublisherRequestDTO addPublisherRequestDTO);
        PublisherNoIdDTO UpdatePublisherById(int id, PublisherNoIdDTO publisherNoIdDTO);
        Publisher? DeletePublisherById(int id);
        Task<bool> IsNameUniqueAsync(string name);
        Task<bool> ExistsAsync(int id);

        Task<bool> HasBooksAsync(int publisherId);

        Task<int> GetBooksCountForYearAsync(int publisherId, int year);
    }
}