namespace UserService.Repositories.Interfaces
{
	public interface IUserRepository<T>
	{
		public Task<IEnumerable<T>> GetAllAsync();
		public Task<T?> GetByIdAsync(int id);
		public Task AddAsync(int id);
		public Task UpdateAsync(T entity);
		public Task DeleteAsync(int id);
	}
}
