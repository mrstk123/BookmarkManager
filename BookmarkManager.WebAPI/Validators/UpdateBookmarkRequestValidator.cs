using BookmarkManager.Application.DTOs;
using FluentValidation;

namespace BookmarkManager.WebAPI.Validators;

public class UpdateBookmarkRequestValidator : AbstractValidator<UpdateBookmarkRequest>
{
    public UpdateBookmarkRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("URL is required.")
            .MaximumLength(500).WithMessage("URL must not exceed 500 characters.")
            .Must(BeValidUrl).WithMessage("Invalid URL format.");

        RuleFor(x => x.FolderName)
            .MaximumLength(100).WithMessage("Folder name must not exceed 100 characters.");

        RuleForEach(x => x.Tags)
            .MaximumLength(50).WithMessage("Tag name must not exceed 50 characters.");
    }

    private bool BeValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}