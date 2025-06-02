namespace UserService.Models
{
    public class UserDto
    {
        public UserDto()
        {
        }

        public UserDto(UserModel user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            UserName = user.UserName;
        }
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? AvatarUrl { get; set; }
        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}
