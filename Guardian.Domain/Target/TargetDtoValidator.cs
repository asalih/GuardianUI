using FluentValidation;
using System;

namespace Guardian.Domain.Target
{
    public class TargetDtoValidator : AbstractValidator<TargetDto>
    {
        public TargetDtoValidator()
        {
            DefaultValidatorExtensions.NotNull(RuleFor(x => x.Domain)).NotEmpty();

            RuleFor(x => x.Domain)
            .Must(LinkMustBeAUri)
            .WithMessage("Domain '{PropertyValue}' must be a valid URI atleast. eg: www.example.com");
        }

        private static bool LinkMustBeAUri(string link)
        {
            if (string.IsNullOrWhiteSpace(link))
            {
                return false;
            }

            if (!link.StartsWith("http"))
            {
                link = "http://" + link;
            }

            //Courtesy of @Pure.Krome's comment and https://stackoverflow.com/a/25654227/563532
            Uri outUri;
            return Uri.TryCreate(link, UriKind.Absolute, out outUri)
                   && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
