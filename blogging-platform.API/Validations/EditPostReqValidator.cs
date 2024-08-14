using blogging_platform.API.Models.DTO;
using FluentValidation;

namespace blogging_platform.API.Validations
{
    public class EditPostReqValidator : AbstractValidator<EditPostReqDto>
    {
        public EditPostReqValidator()
        {
            RuleFor(x => x.PostId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Content).NotEmpty();
            RuleFor(x => x.CategoryId).NotEmpty();
        }
    }
}
