using orgstructure.Repository.Interfaces;

namespace orgstructure.Entities
{
    public class User : IUser
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int DepartmentId { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Patronymic { get; set; }
    }
}
