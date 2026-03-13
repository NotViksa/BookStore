using Microsoft.EntityFrameworkCore;
using PurrfectPics.Data.Interfaces;
using PurrfectPics.Data.Models;
using PurrfectPics.Services.Interfaces;
using System.Linq.Expressions;

namespace PurrfectPics.Services
{
    public class CatImageService : ICatImageService
    {
        private readonly IBookRepository _catImageRepository;
        private readonly ITagService _tagService;

        public CatImageService(
            IBookRepository catImageRepository,
            ITagService tagService)
        {
            _catImageRepository = catImageRepository;
            _tagService = tagService;
        }

        public async Task<IEnumerable<Book>> GetAllImagesAsync()
        {
            return await _catImageRepository.GetAllAsync();
        }

        public async Task<Book?> GetImageByIdAsync(int id)
        {
            return await _catImageRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Book>> GetImagesByTagAsync(string tagName)
        {
            return await _catImageRepository.GetByTagAsync(tagName);
        }

        public async Task<IEnumerable<Book>> GetImagesByUserAsync(string userId)
        {
            return await _catImageRepository.GetByUserAsync(userId);
        }

        public async Task<IEnumerable<Book>> GetMostPopularImagesAsync(int count)
        {
            return await _catImageRepository.GetMostPopularAsync(count);
        }

        public async Task<IEnumerable<Book>> GetRecentImagesAsync(int count)
        {
            return await GetImagesQueryable()
                .Take(count)
                .ToListAsync();
        }

        public async Task<Book> AddImageAsync(Book image, IEnumerable<string> tags)
        {
            var tagList = new List<Genre>();

            foreach (var tagName in tags)
            {
                var tag = await _tagService.EnsureTagExistsAsync(tagName);
                tagList.Add(tag);
            }

            image.Tags = tagList;
            await _catImageRepository.AddAsync(image);
            return image;
        }

        public async Task AddCommentAsync(Review comment)
        {
            await _catImageRepository.UpdateAsync(comment.CatImage);
        }

        public async Task AddFavoriteAsync(Wishlist favorite)
        {
            await _catImageRepository.UpdateAsync(favorite.CatImage);
        }

        public async Task AddVoteAsync(Rating vote)
        {
            await _catImageRepository.UpdateAsync(vote.CatImage);
        }

        public async Task<bool> DeleteImageAsync(int id)
        {
            try
            {
                var image = await _catImageRepository.GetByIdWithDetailsAsync(id);
                if (image == null) return false;

                image.Comments.Clear();
                image.Votes.Clear();
                image.Favorites.Clear();

                await _catImageRepository.DeleteAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting image: {ex.Message}");
                return false;
            }
        }

        public async Task<int> GetImageCountByUserAsync(string userId)
        {
            return await _catImageRepository.CountAsync(ci => ci.UploadedById == userId);
        }

        public async Task<IEnumerable<Book>> SearchImagesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetRecentImagesAsync(20);

            return await GetSearchQueryable(searchTerm)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetRecentUserImagesAsync(string userId, int count)
        {
            return await _catImageRepository.FindAsync(
                ci => ci.UploadedById == userId,
                orderBy: q => q.OrderByDescending(ci => ci.UploadDate),
                take: count
            );
        }
        public IQueryable<Book> GetImagesQueryable()
        {
            return _catImageRepository.GetQueryable()
                .Include(c => c.Tags)
                .OrderByDescending(c => c.UploadDate);
        }

        public IQueryable<Book> GetSearchQueryable(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return GetImagesQueryable();

            query = query.ToLower();
            return _catImageRepository.GetQueryable()
                .Include(c => c.Tags)
                .Where(c => c.Title.ToLower().Contains(query) ||
                           c.Description.ToLower().Contains(query) ||
                           c.Tags.Any(t => t.Name.ToLower().Contains(query)))
                .OrderByDescending(c => c.UploadDate);
        }

    }
}