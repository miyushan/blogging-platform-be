using blogging_platform.API.Models.Domain;
using blogging_platform.API.Models.DTO;
using FluentValidation;

namespace blogging_platform.API.Validations
{
    public class DeletePostReqValidator : AbstractValidator<DeletePostReqDto>
    {
        public DeletePostReqValidator()
        {
            RuleFor(x => x.AuthorId).NotEmpty()
                .Must(id=>Guid.TryParse(id.ToString(), out _));
        }
    }
}
