using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blogging_platform.API.Models.Domain;
using blogging_platform.API.Models.DTO;
using FluentValidation;

namespace blogging_platform.API.Validations
{
    public class SignInUserReqValidator : AbstractValidator<SignInUserReqDto>
    {
        public SignInUserReqValidator(){
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        }
    }
}