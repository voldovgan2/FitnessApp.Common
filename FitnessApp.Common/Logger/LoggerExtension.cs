using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;

namespace FitnessApp.Common.Logger
{
    public static class LoggerExtension
    {
        private static string _newLine = Environment.NewLine;
        public static void WriteInformation(this ILogger logger, string message, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            logger.LogInformation("{message}{_newLine}Method: {memberName} Line number: {sourceLineNumber}", new object[] { message, _newLine, memberName, sourceLineNumber });
        }

        public static void WriteWarning(this ILogger logger, string message, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            logger.LogWarning("{message}{_newLine}Method: {memberName} Line number: {sourceLineNumber}", new object[] { message, _newLine, memberName, sourceLineNumber });
        }

        public static void WriteException(this ILogger logger, Exception ex, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            logger.LogError(ex, "Excepetion in Method: {memberName} Line number: {sourceLineNumber}", new object[] { memberName, sourceLineNumber });
        }
    }
}