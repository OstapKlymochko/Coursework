namespace AuthService.Models
{
	public class RoleModel
	{
        public RoleModel()
        {
        }

        public RoleModel(string name)
        {
            Name = name;    
        }

        public int Id { get; set; }
		public string Name { get; set; } = null!;
	}
}
