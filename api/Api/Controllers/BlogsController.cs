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
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BlogController : BloggingPlatformControllerBase
    {

        private readonly IGenericService _service;

        public BlogController(IGenericService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves all blogs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<BlogModel>>> GetAll()
        {
            try
            {
                var blogs = await _service.GetAllAsync<Blog>();

                return Ok(blogs.MapTo<List<BlogModel>>());

            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnermostMsg());
            }
        }
    }
}
