using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaveBreak.Domain.Posts
{
    public interface IPostProvider
    {
        /// <summary>
        /// Create new Post.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        Task<PostDomain> CreatePostAsync(string content);

        /// <summary>
        /// Get list of posts, paged, sorted and filtered.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IEnumerable<PostDomain>> GetFeedAsync(GetPostsFeedRequestQuery query);

        /// <summary>
        /// Like/Dislike post.
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        Task ToggleLikeAsync(int postId, long userId);

        /// <summary>
        /// Add new comment on post.
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Task AddCommentAsync(int postId, string content);

        /// <summary>
        /// Get post details by id.
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        Task<PostDomain> GetPostById(int postId);
    }
}
