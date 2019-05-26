using Api.Controllers;
using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Test
{
    public class Controllers
    {

        public Controllers()
        {
            Mapper.Reset();
            //static api
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile(new AutomapperProfile());
                cfg.AllowNullCollections = true;
            });

            Mapper.AssertConfigurationIsValid();
        }

        public static UserManager<TUser> TestUserManager<TUser>(IUserStore<TUser> store = null) where TUser : class
        {
            store = store ?? new Mock<IUserStore<TUser>>().Object;
            var options = new Mock<IOptions<IdentityOptions>>();
            var idOptions = new IdentityOptions();
            idOptions.Lockout.AllowedForNewUsers = false;
            options.Setup(o => o.Value).Returns(idOptions);
            var userValidators = new List<IUserValidator<TUser>>();
            var validator = new Mock<IUserValidator<TUser>>();
            userValidators.Add(validator.Object);
            var pwdValidators = new List<PasswordValidator<TUser>>();
            pwdValidators.Add(new PasswordValidator<TUser>());
            var userManager = new UserManager<TUser>(store, options.Object, new PasswordHasher<TUser>(),
                userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(), null,
                new Mock<ILogger<UserManager<TUser>>>().Object);
            validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<TUser>()))
                .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();
            return userManager;
        }


        [Fact]
        public async void PostController()
        {
            //arrange
            var genericService = new Mock<IGenericService>();
            var dateTimeService = new Mock<IDateTimeService>();
            var userManager = TestUserManager<User>();

            dateTimeService.Setup(x => x.UtcNow()).Returns(DateTime.UtcNow);

            var author = new User() { FirstName = "Javier", LastName = "Acrich" };

            var blog = new Blog()
            {
                Id = 1,
                Author = author,
                Title = "test blog"
            };

            var posts = new List<BlogPost>
            {
                new BlogPost()
                {
                    Id = 1,
                    Blog = blog,
                    Status = BlogPostStatus.@public,
                    Text = "post text"
                }
            };

            genericService.Setup(x => x.GetAsync<Blog>(1, "Author")).Returns(Task.FromResult(blog));
            genericService.Setup(x => x.FindAsync(
                It.IsAny<Expression<Func<BlogPost, bool>>>(),
                It.IsAny<Func<IQueryable<BlogPost>, IOrderedQueryable<BlogPost>>>(),
                true,
                It.IsAny<Expression<Func<BlogPost, object>>>(),
                It.IsAny<Expression<Func<BlogPost, object>>>()
                )).Returns(Task.FromResult(posts));

            var currentUserProvider = new Mock<ICurrentUserProvider>();
            currentUserProvider.Setup(x => x.GetCurrentAuthor()).Returns(Task.FromResult(author));
            var controller = new PostController(genericService.Object, dateTimeService.Object, currentUserProvider.Object, userManager);
            const int blogId = 1;

            //act
            var result = await controller.GetAll(blogId);

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var resultPosts = Assert.IsType<List<BlogPostModel>>(okResult.Value);
            Assert.Equal(posts.Count, resultPosts.Count);
        }
    }
}