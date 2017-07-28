using System;

namespace FacebookApi.Core.Tools
{
    public static class DotnetExtensions
    {
        public static DateTime FromEpochTime(int seconds)
        {
            return new DateTime(1970, 1, 1).AddSeconds(seconds);
        }
    }
}