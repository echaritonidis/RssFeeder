namespace RssFeeder.Server.Infrastructure.Logging
{
    public static partial class FeedLoggerMessage
    {
        [LoggerMessage(EventId = 1000, Level = LogLevel.Debug, Message = "Returned feeds: {count}")]
        public static partial void LogGetAll(this ILogger logger, int count);

        [LoggerMessage(EventId = 1001, Level = LogLevel.Debug, Message = "Returned for href: {href} content: {contentItemsCount}")]
        public static partial void LogContent(this ILogger logger, string href, int contentItemsCount);

        [LoggerMessage(EventId = 1002, Level = LogLevel.Debug, Message = "Content doesn't exist for href: {href}")]
        public static partial void LogContentDoesntExistError(this ILogger logger, string href);

        [LoggerMessage(EventId = 1003, Level = LogLevel.Debug, Message = "Error occurred during content fetch: {message}")]
        public static partial void LogContentOtherError(this ILogger logger, string message);

        [LoggerMessage(EventId = 1004, Level = LogLevel.Debug, Message = "FeedNavigation created with id: {id}")]
        public static partial void LogCreated(this ILogger logger, Guid id);

        [LoggerMessage(EventId = 1005, Level = LogLevel.Debug, Message = "Error occurred during creation: {message}")]
        public static partial void LogCreatedError(this ILogger logger, string message);

        [LoggerMessage(EventId = 1006, Level = LogLevel.Debug, Message = "FeedNavigation id: {id} updated")]
        public static partial void LogUpdated(this ILogger logger, Guid id);

        [LoggerMessage(EventId = 1007, Level = LogLevel.Debug, Message = "Error occurred during update: {message}")]
        public static partial void LogUpdatedError(this ILogger logger, string message);

        [LoggerMessage(EventId = 1008, Level = LogLevel.Debug, Message = "FeedNavigation id: {id} deleted")]
        public static partial void LogDeleted(this ILogger logger, Guid id);

        [LoggerMessage(EventId = 1009, Level = LogLevel.Debug, Message = "Error occurred during deletion: {message}")]
        public static partial void LogDeletedError(this ILogger logger, string message);
    }
}
