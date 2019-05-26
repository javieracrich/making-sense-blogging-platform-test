using AutoMapper;
using Domain;

namespace Services
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<UserRegistrationModel, User>()
                .ForMember(user => user.UserName, map => map.MapFrom(vm => vm.Email))
                .ForMember(user => user.FirstName, map => map.MapFrom(model => model.FirstName))
                .ForMember(user => user.LastName, map => map.MapFrom(model => model.LastName))
                .ForMember(user => user.PhoneNumber, map => map.MapFrom(model => model.Phone))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<BlogPostModel, BlogPost>()
                .ForMember(model => model.Text, map => map.MapFrom(post => post.Text))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<BlogPost, BlogPostModel>()
                .ForMember(model => model.Author, map => map.MapFrom(post => post.Author.FirstName + " " + post.Author.LastName));

            CreateMap<Blog, BlogModel>()
               .ForMember(model => model.AuthorId, map => map.MapFrom(post => post.Author.Id));
        }
    }
}
