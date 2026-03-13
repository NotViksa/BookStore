using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using BookStore.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.Services
{
    public class CommentService : ICommentService
    {
        private readonly IReviewRepository _commentRepository;

        public CommentService(IReviewRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<Review> AddCommentAsync(string content, int catImageId, string userId)
        {
            var comment = new Review
            {
                Content = content,
                CatImageId = catImageId,
                PostedById = userId,
                PostedDate = System.DateTime.UtcNow
            };

            return await _commentRepository.AddAsync(comment);
        }

        public async Task<bool> DeleteCommentAsync(int commentId, string userId, bool isAdmin = false)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null) return false;

            if (comment.PostedById == userId || isAdmin)
            {
                return await _commentRepository.DeleteAsync(comment);
            }

            return false;
        }

        public async Task<bool> EditCommentAsync(int commentId, string userId, string newContent)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null || comment.PostedById != userId) return false;

            comment.Content = newContent;
            return await _commentRepository.UpdateAsync(comment);
        }

        public async Task<IEnumerable<Review>> GetCommentsForImageAsync(int catImageId)
        {
            return await _commentRepository.GetCommentsForImageAsync(catImageId);
        }

        public async Task<int> GetCommentCountByUserAsync(string userId)
        {
            return await _commentRepository.GetCountByUserAsync(userId);
        }
    }
}