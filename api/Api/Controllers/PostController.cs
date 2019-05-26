using Api.Auth;
using Common;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PostController : BloggingPlatformControllerBase
    {

        private readonly IGenericService _service;
        private readonly UserManager<User> _userManager;
        private readonly IDateTimeService _dateTimeService;
        private readonly ICurrentUserProvider _currentUserProvider;

        public PostController(IGenericService service,
            IDateTimeService dateTimeService,
            ICurrentUserProvider currentUserProvider,
            UserManager<User> userManager)
        {
            _service = service;
            _userManager = userManager;
            _dateTimeService = dateTimeService;
            _currentUserProvider = currentUserProvider;
        }

        /// <summary>
        /// Retrieves all blog posts from a blog
        /// </summary>
        /// <returns></returns>
        [HttpGet("blog/{blogId}")]
        [Authorize(Policy = AuthPolicy.Reader)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<BlogPostModel>>> GetAll(int blogId)
        {
            try
            {
                var user = await _currentUserProvider.GetCurrentAuthor();

                var blog = await _service.GetAsync<Blog>(blogId, "Author");

                var list = await _service.FindAsync<BlogPost>(x => x.Blog.Id == blogId, x => x.OrderBy(y => y.Date), true, x => x.Blog, x => x.Author);

                if (user.Id != blog.Author.Id)
                {
                    //show only public posts if logged in user is not blog author
                    list = list.Where(x => x.Status == BlogPostStatus.@public).ToList();
                }
                // draft posts do not show up in results.
                return Ok(list.MapTo<List<BlogPostModel>>());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnermostMsg());
            }
        }


        [HttpGet("search/{textToSearch}")]
        [Authorize(Policy = AuthPolicy.Reader)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<BlogPostModel>>> GetSearch(string textToSearch)
        {
            try
            {
                var user = await _currentUserProvider.GetCurrentAuthor();
                //searches only public posts
                var list = await _service.FindAsync<BlogPost>(x => x.Text.Contains(textToSearch) && x.Status == BlogPostStatus.@public,
                    x => x.OrderBy(y => y.Date), true, x => x.Blog);

                return Ok(list.MapTo<List<BlogPostModel>>());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnermostMsg());
            }
        }

        /// <summary>
        /// Retrieves a post
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [HttpGet("{postId}")]
        [Authorize(Policy = AuthPolicy.Reader)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BlogPostModel>> Get(int postId)
        {
            try
            {
                var post = await _service.GetAsync<BlogPost>(postId);

                if (post == null)
                {
                    return NotFound(ErrorConstants.PostNotFound);
                }
                return Ok(post.MapTo<BlogPostModel>());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnermostMsg());
            }
        }


        /// <summary>
        /// Creates a new post
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = AuthPolicy.Contributor)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BlogPostModel>> Post(BlogPostUpsertModel model)
        {
            try
            {
                var author = await _currentUserProvider.GetCurrentAuthor();

                var post = new BlogPost
                {
                    Text = model.Text,
                    Author = author,
                    Date = _dateTimeService.UtcNow()
                };

                var entityId = await _service.CreateAsync(post);

                var url = $"/post/{entityId}";
                return Created(url, null);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnermostMsg());
            }
        }

        /// <summary>
        /// Updates a post
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{postId}")]
        [Authorize(Policy = AuthPolicy.Contributor)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> Put(int postId, BlogPostUpsertModel model)
        {
            try
            {
                var post = await _service.GetAsync<BlogPost>(postId);

                if (post == null)
                {
                    return NotFound(ErrorConstants.PostNotFound);
                }

                var author = await _currentUserProvider.GetCurrentAuthor();

                if (post.Author.Id != author.Id)
                {
                    return BadRequest("Only the post author can update it.");
                }

                post.Text = model.Text;

                var rowsAffected = await _service.UpdateAsync(post);

                return Ok(rowsAffected);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnermostMsg());
            }
        }

        /// <summary>
        /// Deletes a post
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [HttpDelete("{postId}")]
        [Authorize(Policy = AuthPolicy.Contributor)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> Delete(int postId)
        {
            try
            {

                var post = await _service.GetAsync<BlogPost>(postId);

                if (post == null)
                {
                    return NotFound(ErrorConstants.PostNotFound);
                }

                var author = await _currentUserProvider.GetCurrentAuthor();


                if (post.Author.Id != author.Id)
                {
                    return BadRequest("Only the post author can delete it.");
                }

                var rowsAffected = await _service.DeleteAsync(post);

                if (rowsAffected == 0)
                {
                    return NotFound(ErrorConstants.PostNotFound);
                }
                return Ok(rowsAffected);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnermostMsg());
            }


        }



    }
}
