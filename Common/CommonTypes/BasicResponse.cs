using Common.CommonTypes.Interfaces;

namespace Common.CommonTypes
{
	public class BasicResponse: IBasicResponse
	{
		public BasicResponse()
		{
		}

		public BasicResponse(string message)
		{
			this.Message = message;
		}
		public string Message { get; set; } = null!;
	}
}
