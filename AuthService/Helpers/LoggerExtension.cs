using System.Runtime.CompilerServices;

namespace AuthService.Helpers
{
	public static class LoggerExtension
	{
		public static Serilog.ILogger Enrich(this Serilog.ILogger logger,
			[CallerMemberName] string memberName = "",
			[CallerLineNumber] int sourceLineNumber = 0)
		{
			return logger
				.ForContext("MemberName", memberName)
				.ForContext("LineNumber", sourceLineNumber);
		}
	}
}
