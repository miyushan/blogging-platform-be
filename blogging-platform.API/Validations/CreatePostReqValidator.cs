using blogging_platform.API.Models.Domain;
using blogging_platform.API.Models.DTO;
using FluentValidation;

namespace blogging_platform.API.Validations
{
    public class CreatePostReqValidator : AbstractValidator<CreatePostReqDto>
    {
        public CreatePostReqValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Content).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.CategoryId).NotEmpty();
        }
    }
}
