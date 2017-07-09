using System;

namespace Microsoft.Extensions.Logging
{
    public static class DotnetExtensions
    {
        public static void LogError(this ILogger logger, Exception ex)
        {
            logger.LogError(default(EventId), ex, null);
        }
    }
}