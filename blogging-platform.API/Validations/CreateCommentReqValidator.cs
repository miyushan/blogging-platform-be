using blogging_platform.API.Models.DTO;
using FluentValidation;

namespace blogging_platform.API.Validations
{
    public class CreateCommentReqValidator : AbstractValidator<CreateCommentReqDto>
    {
        public CreateCommentReqValidator()
        {
            RuleFor(x => x.Content).NotEmpty();
            RuleFor(x => x.PostId).NotEmpty();
        }

    }
}
