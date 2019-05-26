using Api.Auth;
using Api.Helpers;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Threading.Tasks;
using static Services.Extensions;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : BloggingPlatformControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IGenericService _service;
        private readonly IDateTimeService _dateTimeService;


        public UserController(UserManager<User> userManager,
            IGenericService service,
            IDateTimeService dateTimeService)
        {
            _userManager = userManager;
            _service = service;
            _dateTimeService = dateTimeService;
        }


        /// <summary>
        /// Creates a new blog platform user
        /// </summary>
        [HttpPost()]
        [Authorize(Policy = AuthPolicy.Contributor)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post(UserRegistrationModel model)
        {
            try
            {

                var user = model.MapTo<User>();

                user.Created = _dateTimeService.UtcNow();

                user.CreatedBy = GetCurrentUserId();

                var identityResult = await _userManager.CreateAsync(user, model.Password);

                if (!identityResult.Succeeded)
                    return BadRequest(Errors.AddErrorsToModelState(identityResult, ModelState));

                var url = $"/user/{user.Id}";
                return Created(url, null);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnermostMsg());
            }

        }






        private Task<User> GetUser(Guid userId)
        {
            return _userManager.FindByIdAsync(userId.ToString());
        }

    }

}