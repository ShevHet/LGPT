namespace TaskTracker.Application.Exceptions
{
    public sealed class ConflictException : DomainException
    {
        public ConflictException(string message) : base(message) { }
    }
}
