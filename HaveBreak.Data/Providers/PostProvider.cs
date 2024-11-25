using AutoMapper;
using System.Linq.Dynamic.Core;
using HaveBreak.Data.Models;
using HaveBreak.Domain.Posts;
using HaveBreak.Shared;
using HaveBreak.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HaveBreak.Data.Providers
{
    internal class PostProvider : GenericProvider<Post>, IPostProvider
    {

        private readonly RoleManager<UserRole> _roleManager;
        private readonly UserManager<UserAccount> _userManager;
        private readonly IMapper _mapper;

        public PostProvider(HaveBreakDbContext dbContext,
             RoleManager<UserRole> roleManager,
             UserManager<UserAccount> userManager,
             IMapper mapper)
        {
            DbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task AddCommentAsync(int postId, string content)
        {
            var comment = new Comment
            {
                PostId = postId,
                Content = content
            };

            DbContext.Comments.Add(comment);
            await DbContext.SaveChangesAsync();
        }

        public async Task<PostDomain> CreatePostAsync(string content)
        {
            var postToBeAdded = new Post { Content = content };

            DbContext.Posts.Add(postToBeAdded);

            await DbContext.SaveChangesAsync();

            return _mapper.Map<PostDomain>(postToBeAdded);
        }

        public async Task<IEnumerable<PostDomain>> GetFeedAsync(GetPostsFeedRequestQuery query)
        {
            var data = ActiveDbSet.Include(p => p.Comments.Where(x => !x.IsDeleted.Value))
                                         .Include(p => p.Likes.Where(x => !x.IsDeleted.Value))
                                         .OrderByDescending(p => p.CreatedAt)
                                         .AsNoTracking();

            var totalCount = await data.CountAsync();

            data = ApplySorting(data, query);

            if (query.IsPaginated())
                data = ApplyPaging(data, query);

            return _mapper.Map<IEnumerable<PostDomain>>(await data.ToListAsync());
            
        }

        public async Task<PostDomain> GetPostById(int postId)
        {
            var data = await ActiveDbSet.Include(p => p.Comments.Where(x => !x.IsDeleted.Value))
                             .Include(p => p.Likes.Where(x=> !x.IsDeleted.Value))
                             .OrderByDescending(p => p.CreatedAt)
                             .AsNoTracking().FirstOrDefaultAsync(x=>x.Id == postId);

            return _mapper.Map<PostDomain>(data);
        }

        public async Task ToggleLikeAsync(int postId, long userId)
        {
            var like = await DbContext.Likes.FirstOrDefaultAsync(l => l.PostId == postId && l.CreatedBy == userId);

            if (like == null)            
                DbContext.Likes.Add(new Like { PostId = postId });               
            else
                //SoftDelete
                like.IsDeleted = true;

            await DbContext.SaveChangesAsync();
        }

        private static IQueryable<Post> ApplySorting(IQueryable<Post> query, GetPostsFeedRequestQuery request)
        {
            if (string.IsNullOrEmpty(request.SortingField))
                return request.SortingDir == SortingDirection.Desc ?
                    query.OrderByDescending(x => x.ModifiedAt ?? x.CreatedAt) :
                    query.OrderBy(x => x.ModifiedAt ?? x.CreatedAt);

            var sortDir = request.SortingDir == SortingDirection.Desc ? " DESC" : " ASC";
            var sortField = nameof(Post.Content) + sortDir;

            return query.OrderBy(sortField);
        }

        private static IQueryable<Post> ApplyPaging(IQueryable<Post> query, GetPostsFeedRequestQuery request)
        {
            var take = request.PageSize ?? Constants.DefaultPageSize;
            var skip = ((request.PageIndex ?? Constants.DefaultPageIndex) - 1) * take;

            return query.Skip(skip).Take(take);
        }
    }
}
