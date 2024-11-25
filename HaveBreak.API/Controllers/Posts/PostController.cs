using AutoMapper;
using HaveBreak.API.Controllers.Posts.Dtos;
using HaveBreak.Common;
using HaveBreak.Domain.Posts;
using HaveBreak.Manager.Posts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;

namespace HaveBreak.API.Controllers.Posts
{
    [Authorize]
    public class PostsController : BaseApiController
    {
        private readonly IPostManager _postManager;
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;

        public PostsController(IPostManager postManager,
            IMapper mapper, IMemoryCache cache,
            IStringLocalizerFactory factory) : base(factory)
        {
            _postManager = postManager;
            _mapper = mapper;
            _cache = cache;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePostAsync([FromBody] CreatePostDto input)
        {
            var toBeCreated = await _postManager.CreatePostAsync(input.Content);

            return Ok(_mapper.Map<PostDto>(toBeCreated));
        }

        [HttpGet("Feed")]
        public async Task<IActionResult> GetFeedAsync([FromQuery] GetPostsFeedRequestDto input)
        {
            var posts = await _postManager.GetFeedAsync(_mapper.Map<GetPostsFeedRequestQuery>(input));

            return Ok(_mapper.Map<List<PostDto>>(posts));
        }

        [HttpPost("{postId}/like")]
        public async Task<IActionResult> LikePostAsync(int postId)
        {
            await _postManager.ToggleLikeAsync(postId);

            return Ok();
        }

        [HttpPost("{postId}/comment")]
        public async Task<IActionResult> AddCommentAsync(int postId, [FromBody] AddCommentRequestDto request)
        {
            await _postManager.AddCommentAsync(postId, request.Comment);

            return Ok();
        }

        [HttpGet("{postId}/Details")]
        public async Task<IActionResult> GetPostDetailsAsync(int postId)
        {
            #region In-Memory Cache
            string cacheKey = $"Post-{postId}";

            // Try to get the cached value
            if (!_cache.TryGetValue(cacheKey, out object data))
            {
                // If not found in cache, I'll fetch it from the data source
                data = await _postManager.GetPostById(postId);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5))  // Reset the timer if accessed
                     .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)); // Cache entry will expire in 10 minutes regardless of access.

                _cache.Set(cacheKey, data, cacheEntryOptions);
            }

            // If the data was found in the cache, it is already in 'data'
            #endregion

            return Ok(data);
        }

    }

}
