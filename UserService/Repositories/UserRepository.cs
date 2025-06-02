using Common.Services.Interfaces;
using UserService.Models;
using UserService.Repositories.Interfaces;

namespace UserService.Repositories
{
    public class UserRepository : IUserRepository<UserModel>
    {
        private readonly IDataAccessService _accessService;

        public UserRepository(IDataAccessService accessService)
        {
            _accessService = accessService;
        }

        public Task<IEnumerable<UserModel>> GetAllAsync() =>
            _accessService.QueryDataAsync<UserModel, dynamic>("select * from users;", null);


        public Task<UserModel?> GetByIdAsync(int id) =>
            _accessService.QuerySingleRecordAsync<UserModel, dynamic>("select * from users where id=@Id", new { Id = id });


        public Task AddAsync(int id) =>
            _accessService.ExecuteStatementAsync("insert into users (Id) values(@Id)", new { Id = id });


        public Task UpdateAsync(UserModel entity) =>
            _accessService.ExecuteStatementAsync("update users set firstname=@FirstName, lastname=@LastName where id=@Id", entity);


        public Task DeleteAsync(int id) =>
            _accessService.ExecuteStatementAsync("delete from users where id=@Id", new { Id = id });

    }
}
