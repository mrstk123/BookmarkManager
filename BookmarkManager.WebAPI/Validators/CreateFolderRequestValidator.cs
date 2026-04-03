using BookmarkManager.Application.DTOs;
using FluentValidation;

namespace BookmarkManager.WebAPI.Validators;

public class CreateFolderRequestValidator : AbstractValidator<CreateFolderRequest>
{
    public CreateFolderRequestValidator()
    {
        // RuleFor(x => x.UserId)
        //     .GreaterThan(0).WithMessage("UserId must be a positive number.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Folder name is required.")
            .MaximumLength(100).WithMessage("Folder name must not exceed 100 characters.");
    }
}