using FluentValidation;

namespace Guardian.Domain.Target
{
    public class TargetDtoValidator : AbstractValidator<TargetDto>
    {
        public TargetDtoValidator()
        {
            DefaultValidatorExtensions.NotNull(RuleFor(x => x.Domain)).NotEmpty();
            RuleFor(s => s.Port).GreaterThan(0).LessThanOrEqualTo(65536);
        }
    }
}
