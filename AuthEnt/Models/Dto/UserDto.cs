namespace AuthEnt.Models.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
