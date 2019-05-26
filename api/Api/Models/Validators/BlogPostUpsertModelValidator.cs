
using Domain;
using FluentValidation;

namespace Api.Models.Validators
{
    public class BlogPostUpsertModelValidator : AbstractValidator<BlogPostUpsertModel>
    {
        public BlogPostUpsertModelValidator()
        {
            RuleFor(vm => vm.Text).NotEmpty().WithMessage("Blog post cannot be empty");
            RuleFor(vm => vm.Text).MaximumLength(5000).WithMessage("Blog post cannot have more than 5000 chars");
        }
    }
}
