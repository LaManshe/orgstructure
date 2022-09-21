using orgstructure.Models.Entities;

namespace orgstructure.Models.ViewModels
{
    public class IndexModel
    {
        public AddUser addUserModel { get; set; }
        public DeleteUser deleteUserModel { get; set; }
        public ChangeUser changeUserModel { get; set; }
        public IEnumerable<string> parentDepartments { get; set; }
        public IEnumerable<string> departments { get; set; }
        public string departmentToFilter { get; set; }
    }
}
