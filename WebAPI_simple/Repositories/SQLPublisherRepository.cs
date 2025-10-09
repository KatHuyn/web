using WebAPI_simple.Data;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace WebAPI_simple.Repositories
{
    public class SQLPublisherRepository : IPublisherRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLPublisherRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Triển khai các phương thức: GetAll, GetById, Add, Update, Delete
        public List<PublisherDTO> GetAllPublishers()
        {
            var allPublishersDomain = _dbContext.Publishers.ToList();
            var allPublisherDTO = new List<PublisherDTO>();
            foreach (var publisherDomain in allPublishersDomain)
            {
                allPublisherDTO.Add(new PublisherDTO()
                {
                    Id = publisherDomain.Id,
                    Name = publisherDomain.Name
                });
            }
            return allPublisherDTO;
        }

        public PublisherNoIdDTO GetPublisherById(int id)
        {
            var publisherWithIdDomain = _dbContext.Publishers.FirstOrDefault(x => x.Id == id);
            if (publisherWithIdDomain == null) return null;

            var publisherNoIdDTO = new PublisherNoIdDTO { Name = publisherWithIdDomain.Name };
            return publisherNoIdDTO;
        }

        public AddPublisherRequestDTO AddPublisher(AddPublisherRequestDTO addPublisherRequestDTO)
        {
            var publisherDomainModel = new Publisher { Name = addPublisherRequestDTO.Name };
            _dbContext.Publishers.Add(publisherDomainModel);
            _dbContext.SaveChanges();
            return addPublisherRequestDTO;
        }

        public PublisherNoIdDTO UpdatePublisherById(int id, PublisherNoIdDTO publisherNoIdDTO)
        {
            var publisherDomain = _dbContext.Publishers.FirstOrDefault(n => n.Id == id);
            if (publisherDomain != null)
            {
                publisherDomain.Name = publisherNoIdDTO.Name;
                _dbContext.SaveChanges();
                return publisherNoIdDTO;
            }
            return null;
        }

        public Publisher? DeletePublisherById(int id)
        {
            var publisherDomain = _dbContext.Publishers.FirstOrDefault(n => n.Id == id);
            if (publisherDomain != null)
            {
                _dbContext.Publishers.Remove(publisherDomain);
                _dbContext.SaveChanges();
            }
            return publisherDomain;
        }

        public async Task<bool> IsNameUniqueAsync(string name) 
        {
            return await _dbContext.Publishers.AnyAsync(p => p.Name == name);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbContext.Publishers.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> HasBooksAsync(int publisherId)
        {
            return await _dbContext.Books.AnyAsync(b => b.PublisherID == publisherId);
        }

        public async Task<int> GetBooksCountForYearAsync(int publisherId, int year)
        {
            return await _dbContext.Books
                .CountAsync(b => b.PublisherID == publisherId && b.DateAdded.Year == year);
        }

       
    }
}