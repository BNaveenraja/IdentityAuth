using System.Collections.Generic;

namespace AuthEnt.Models.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public IList<string> Roles { get; set; }

        public UserDto(int id, string email, string fullName, IList<string> roles)
        {
            Id = id;
            Email = email;
            FullName = fullName;
            Roles = roles;
        }
    }
}
