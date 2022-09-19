namespace orgstructure.Models.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ParentDepartmentId { get; set; }
    }
}
