using Microsoft.AspNetCore.Mvc;
using UserService.Models;
using UserService.Services.Interfaces;
using UserService.Helpers;
using Common.GenericRepository;
using Common.Services;

namespace UserService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly IConfiguration _cfg;

		public UsersController(IUserService userService, IConfiguration cfg)
		{
			_userService = userService;
			_cfg = cfg;
		}
		[HttpPut("update")]
		public async Task<IActionResult> UpdateUser(UpdateUserModel user)
		{
			//Todo replace with extract from token from auth service
			int id = int.Parse(HttpContext.User.Claims.First(c => c.Type == "userId").Value.ToString()!);

			var result = await _userService.UpdateUser(new UserModel
			{ Id = id, FirstName = user.FirstName, LastName = user.LastName });
			return result.MapToResponse();
		}

		[HttpGet]
		public async Task<string> Test()
		{
			var e = new UserModel() { Id = 13, FirstName = "Ostap", LastName = "Klymochko" };
			var repo = new GenericRepository<UserModel>(new DataAccessService(new DbContext(_cfg)));
			//var k = repo.GetKeyColumnName();
			//var t = repo.GetTableName();
			//var t1 = repo.GetColumns(e);
			//string query = $"update {t} set ";
			//var asd = t1.Count;
			//string[] statements = new string[t1.Count];
			//int i = 0;
			//foreach (var c in t1)
			//{
			//	var formatValue = repo.ParseValue(c.Value);
			//	var s = $"{c.Key} = {formatValue} ";
			//	statements[i++] = s;
			//}

			//query += string.Join(',', statements);
			//var key = repo.GetKeyColumnName();
			//var value = repo.GetKeyColumnValue(e);
			//query += $" where {key} = {value}";
			//var res = repo.GetAllAsync().Result;
			var a = repo.GetKeyColumnName();
			var b = repo.GetColumns(e);
			var res = repo.GetByIdAsync(e.Id).Result;
			await repo.DeleteAsync(e);
			return "okkk";
		}
	}
}
