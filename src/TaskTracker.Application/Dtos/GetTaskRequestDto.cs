using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Application.Dtos
{
    public sealed class GetTaskRequestDto
    {
        public const int DefaultPage = 1;
        public const int DefaultPageSize = 20;
        public const int MaxPageSize = 100;

        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
        public int Page { get; set; } = DefaultPage;

        [Range(1, MaxPageSize, ErrorMessage = "PageSize must be between 1 and 100.")]
        public int PageSize { get; set; } = DefaultPageSize;
    }
}
