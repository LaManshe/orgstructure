using orgstructure.Models.Entities;

namespace orgstructure.Models.ViewModels
{
    public class IndexModel
    {
        public AddUser addUserModel { get; set; }
        public DeleteUser deleteUserModel { get; set; }
        public ChangeUser changeUserModel { get; set; }
        public string departmentToFilter { get; set; }
    }
}
