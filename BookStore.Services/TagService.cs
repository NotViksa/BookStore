using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using BookStore.Data.Repositories;
using BookStore.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.Services
{
    public class TagService : ITagService
    {
        private readonly IGenreRepository _tagRepository;
        public TagService(IGenreRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<IEnumerable<Genre>> GetAllTagsAsync()
        {
            return await _tagRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Genre>> GetPopularTagsAsync(int count)
        {
            return await _tagRepository.GetPopularTagsAsync(count);
        }

        public async Task<Genre?> GetTagByNameAsync(string name)
        {
            return await _tagRepository.GetByNameAsync(name);
        }

        public async Task<Genre> EnsureTagExistsAsync(string name)
        {
            var existingTag = await _tagRepository.GetByNameAsync(name);
            if (existingTag != null)
            {
                return existingTag;
            }

            var newTag = new Genre { Name = name };
            return await _tagRepository.AddAsync(newTag);
        }

        public async Task<IEnumerable<Genre>> GetTagsForImageAsync(int imageId)
        {
            return await _tagRepository.GetTagsForImageAsync(imageId);
        }

        public async Task<int> GetTagUsageCountAsync(int tagId)
        {
            return await _tagRepository.GetUsageCountAsync(tagId);
        }
    }
}