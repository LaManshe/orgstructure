using orgstructure.Repository.Interfaces;
using ClosedXML.Excel;
using orgstructure.Data;
using orgstructure.Entities;
using orgstructure.Models.Entities;
using orgstructure.Models.ViewModels;

namespace orgstructure.Repository
{
    public class EFUsersRepository : IEFUsersRepository
    {
        private static CheckValidFile validFile;
        private readonly Context _context;

        public EFUsersRepository(Context context)
        {
            validFile = new CheckValidFile();   
            _context = context;
        }
        public bool Import(IFormFile file)
        {
            if (!validFile.ExcelValid(file))
            {
                return false;
            }

            using (XLWorkbook workBook = new(file.OpenReadStream()))
            {
                foreach(IXLWorksheet worksheet in workBook.Worksheets)
                {
                    foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                    {
                        if(
                            row.Cell(1).Value.ToString() == String.Empty ||
                            row.Cell(3).Value.ToString() == String.Empty ||
                            row.Cell(4).Value.ToString() == String.Empty)
                        {
                            continue;
                        }

                        User newUser = new User() { Id = GetNextUserId()};

                        string departmentTitle = row.Cell(1).Value.ToString();
                        string? parentDepartmentTitle = row.Cell(2).Value.ToString();  
                        newUser.DepartmentId = GetDepartmentIdByTitle(departmentTitle) == -1 ?
                            CreateDepartment(departmentTitle, parentDepartmentTitle) :
                            GetDepartmentIdByTitle(departmentTitle);

                        string postTitle = row.Cell(3).Value.ToString();
                        newUser.PostId = GetPostIdByTitle(postTitle) == -1 ?
                            CreatePost(postTitle, newUser.DepartmentId) :
                            GetPostIdByTitle(postTitle);

                        string fullName = row.Cell(4).Value.ToString();
                        string userSurname = fullName.Split(' ')[0];
                        string userName = fullName.Split(' ')[1];
                        string userPatronymic = fullName.Split(' ')[2];
                        newUser.Name = userName;
                        newUser.Surname = userSurname;
                        newUser.Patronymic = userPatronymic;

                        _context.Add(newUser);
                        _context.SaveChanges();
                    }
                }
            }

            return true;
        }
        public IEnumerable<vUser> Show()
        {
            List<vUser> users = new List<vUser>();
            List<User> usersTable = _context.Users.ToList();

            foreach(var userTable in usersTable)
            {
                users.Add(new vUser()
                {
                    Id = userTable.Id,
                    PostTitle = GetPostById(userTable.PostId),
                    DepartmentTitle = GetDepartmentById(userTable.DepartmentId),
                    FullName = String.Format($"{userTable.Surname} {userTable.Name} {userTable.Patronymic}")
                });
            }

            return users;
        }
        public void CreateUser(AddUser user)
        {
            User newUser = new User() { 
                Id = GetNextUserId(), 
                DepartmentId = GetDepartmentIdByTitle(user.DepartmentTitle) == -1 ? 
                    CreateDepartment(user.DepartmentTitle, String.Empty) : 
                    GetDepartmentIdByTitle(user.DepartmentTitle),
                PostId = GetPostIdByTitle(user.PostTitle) == -1 ?
                    CreatePost(user.PostTitle, GetDepartmentIdByTitle(user.DepartmentTitle)) :
                    GetPostIdByTitle(user.PostTitle),
                Name = user.Name ?? "Unknown",
                Surname = user.Surname ?? "Unknown",
                Patronymic = user.Patronymic ?? "Unknown"
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();
        }
        public bool DeleteUser(DeleteUser user)
        {
            User userToDelete = _context.Users.FirstOrDefault(x => x.Id == user.Id);

            if (userToDelete != null)
            {
                _context.Remove(userToDelete);
                _context.SaveChanges();

                return true;
            }
            else
            {
                return false;
            }
        }
        public bool ChangeUser(ChangeUser user)
        {
            User? userToChange = _context.Users.FirstOrDefault(x => x.Id == user.Id);

            if (userToChange != null)
            {
                userToChange.Name = user.Name;
                userToChange.Surname = user.Surname;
                userToChange.Patronymic = user.Patronymic;
                userToChange.DepartmentId = GetDepartmentIdByTitle(user.DepartmentTitle) == -1 ?
                    CreateDepartment(user.DepartmentTitle, String.Empty) :
                    GetDepartmentIdByTitle(user.DepartmentTitle);
                userToChange.PostId = GetPostIdByTitle(user.PostTitle) == -1 ?
                    CreatePost(user.PostTitle, userToChange.DepartmentId) :
                    GetPostIdByTitle(user.PostTitle);

                _context.SaveChanges();

                return true;
            }
            else
            {
                return false;
            }
        }
        public int GetCountUserByDepartmentName(string departmentName)
        {
            int depId = GetDepartmentIdByTitle(departmentName);

            if(depId == -1)
            {
                return 0;
            }

            return _context.Users.Count(x => x.DepartmentId == depId);
        }
        public int GetCountPositionsByDepartmentName(string departmentName)
        {
            int depId = GetDepartmentIdByTitle(departmentName);

            if (depId == -1)
            {
                return 0;
            }

            return _context.Posts.Count(x => x.DepartmentId == depId);
        }
        private int CreatePost(string? postTitle, int departmentId)
        {
            if (postTitle == String.Empty || postTitle == null) { postTitle = "Unknown"; }

            int createdId = GetNextPostId();
            _context.Posts.Add(new Post() { Id = createdId, Title = postTitle, DepartmentId = departmentId });
            _context.SaveChanges();

            return createdId;
        }
        private int CreateDepartment(string? departmentTitle, string? parentDepartmentTitle)
        {
            int createdId = GetNextDepartmentId();
            int parentDepartmentId = -1;
            if (parentDepartmentTitle != String.Empty)
            {
                parentDepartmentId = GetDepartmentIdByTitle(parentDepartmentTitle) == -1 ?
                    CreateDepartment(parentDepartmentTitle, String.Empty) :
                    GetDepartmentIdByTitle(parentDepartmentTitle);
            }
            
            _context.Departments.Add(new Department() { Id = createdId, Title = departmentTitle, ParentDepartmentId = parentDepartmentId });
            _context.SaveChanges();

            return createdId;
        }

        private int GetDepartmentIdByTitle(string? title)
        {
            if(title == String.Empty || title == null) { return -1; }
            return _context.Departments.FirstOrDefault(x => x.Title.ToLower() == title.ToLower())?.Id ?? -1;
        }
        private int GetPostIdByTitle(string? postTitle)
        {
            if (postTitle == String.Empty || postTitle == null) { return -1; }
            return _context.Posts.FirstOrDefault(x => x.Title.ToLower() == postTitle.ToLower())?.Id ?? -1;
        }

        private int GetNextUserId()
        {
            return _context.Users.Count() == 0 ? 1 : _context.Users.Max(x => x.Id) + 1;
        }
        private int GetNextDepartmentId()
        {
            return _context.Departments.Count() == 0 ? 1 : _context.Departments.Max(x => x.Id) + 1;
        }
        private int GetNextPostId()
        {
            return _context.Posts.Count() == 0 ? 1 : _context.Posts.Max(x => x.Id) + 1;
        }

        private string GetDepartmentById(int departmentId)
        {
            return _context.Departments.FirstOrDefault(x => x.Id == departmentId).Title;
        }
        private string GetPostById(int postId)
        {
            return _context.Posts.FirstOrDefault(x => x.Id == postId).Title;
        }

        
    }
}
