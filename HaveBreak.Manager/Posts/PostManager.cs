using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaveBreak.Data;
using HaveBreak.Domain.Posts;
using HaveBreak.Shared;
using HaveBreak.Shared.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace HaveBreak.Manager.Posts
{
    public class PostManager(IPostProvider postProvider, IConfiguration Configuration,
         ICurrentUserService currentUserService,
         IStringLocalizerFactory factory) : IPostManager
    {
        private readonly IPostProvider _postProvider = postProvider;
        private readonly ICurrentUserService _currentUserService = currentUserService;
        private readonly IStringLocalizer _localizer = factory.Create(typeof(CommonResource));
        private IConfiguration Configuration { get; set; } = Configuration;

        public async Task<PostDomain> CreatePostAsync(string content)
        {
            return await _postProvider.CreatePostAsync(content);
        }

        public async Task<IEnumerable<PostDomain>> GetFeedAsync(GetPostsFeedRequestQuery query)
        {
            //Initialize default values if not provided
            if (string.IsNullOrEmpty(query.SortingField))
            {
                query.SortingDir ??= SortingDirection.Desc;
            }

            query.SortingDir ??= SortingDirection.Asc;
            query.PageIndex ??= Constants.DefaultPageIndex;
            query.PageSize ??= Constants.DefaultPageSize;

            return await _postProvider.GetFeedAsync(query);
        }

        public async Task ToggleLikeAsync(int postId)
        {
            var userId = _currentUserService.GetUserId();

            await _postProvider.ToggleLikeAsync(postId, userId.Value);
        }

        public async Task AddCommentAsync(int postId, string content)
        {
            await _postProvider.AddCommentAsync(postId, content);
        }

        public async Task<PostDomain> GetPostById(int postId)
        {
            return await _postProvider.GetPostById(postId);
        }
    }
}
