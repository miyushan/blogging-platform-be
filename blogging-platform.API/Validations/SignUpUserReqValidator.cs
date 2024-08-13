using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blogging_platform.API.Models.Domain;
using blogging_platform.API.Models.DTO;
using FluentValidation;

namespace blogging_platform.API.Validations
{
    public class SignUpUserReqValidator : AbstractValidator<CreateUserReqDto>
    {
        public SignUpUserReqValidator(){
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            RuleFor(x => x.UserType).Must(x => Enum.IsDefined(typeof(UserType), x));
        }
    }
}