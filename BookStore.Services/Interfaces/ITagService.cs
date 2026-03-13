using PurrfectPics.Data.Models;

namespace PurrfectPics.Services.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<Genre>> GetAllTagsAsync();
        Task<IEnumerable<Genre>> GetPopularTagsAsync(int count);
        Task<Genre?> GetTagByNameAsync(string name);
        Task<Genre> EnsureTagExistsAsync(string name);
        Task<IEnumerable<Genre>> GetTagsForImageAsync(int imageId);
        Task<int> GetTagUsageCountAsync(int tagId);
    }
}