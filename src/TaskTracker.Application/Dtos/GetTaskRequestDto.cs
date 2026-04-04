using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DomainTaskStatus = TaskTracker.Domain.Models.TaskStatus;

namespace TaskTracker.Application.Dtos
{
    /// <summary>Query values for the task list.</summary>
    public sealed class GetTaskRequestDto
    {
        public const int DefaultPage = 1;
        public const int DefaultPageSize = 20;
        public const int MaxPageSize = 100;

        ///<summary>Page number</summary>
        [DefaultValue(DefaultPage)]
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
        public int Page { get; set; } = DefaultPage;

        ///<summary>Items per page.</summary>
        [DefaultValue(DefaultPageSize)]
        [Range(1, MaxPageSize, ErrorMessage = "PageSize must be between 1 and 100.")]
        public int PageSize { get; set; } = DefaultPageSize;

        ///<summary>Filters  by status.</summary>
        [EnumDataType(typeof(DomainTaskStatus), ErrorMessage = "Status must be one of: New, InProgress")]
        public DomainTaskStatus? Status { get; set; }

        ///<summary>Filters by project id.</summary>
        [Range(1,int.MaxValue, ErrorMessage = "ProjectId must be greater than 0.")]
        public int? ProjectId { get; set; }
    }
}
