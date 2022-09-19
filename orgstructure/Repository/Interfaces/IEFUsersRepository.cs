using orgstructure.Entities;
using orgstructure.Models.Entities;
using orgstructure.Models.ViewModels;

namespace orgstructure.Repository.Interfaces
{
    public interface IEFUsersRepository
    {
        public bool Import(IFormFile file);
        public IEnumerable<vUser> Show();
        public void CreateUser(AddUser user);
        public bool DeleteUser(DeleteUser user);
        public bool ChangeUser(ChangeUser user);
        public int GetCountUserByDepartmentName(string departmentName);
        public int GetCountPositionsByDepartmentName(string departmentName);
    }
}
