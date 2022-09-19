using orgstructure.Repository.Interfaces;

namespace orgstructure.Models.Entities
{
    public class vUser
    {
        public int Id { get; set; }
        public string PostTitle { get; set; }
        public string DepartmentTitle { get; set; }
        public string? FullName { get; set; }
    }
}
