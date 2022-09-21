using Microsoft.AspNetCore.Mvc;
using orgstructure.Models;
using orgstructure.Models.ViewModels;
using orgstructure.Repository.Interfaces;
using System.Diagnostics;

namespace orgstructure.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public static IEFUsersRepository _usersRepository;

        public HomeController(ILogger<HomeController> logger, IEFUsersRepository usersRepository)
        {
            _logger = logger;
            _usersRepository = usersRepository;
        }

        public IActionResult Index()
        {
            IndexModel indexModel = new IndexModel();
            indexModel.parentDepartments = _usersRepository.GetParentDepartments();
            indexModel.departments = _usersRepository.GetAllDepartments();

            return View(indexModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Import(IFormFile fileExcel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Ошибка загрузки файла";
                return View("Index");
            }

            if (!_usersRepository.Import(fileExcel))
            {
                ViewBag.Message = "Файла не существует или не верный формат";
                return View("Index");
            }

            return Redirect("/Home/Show");
        }
        [HttpGet]
        public IActionResult Show()
        {
            ShowModel showModel = new ShowModel();
            showModel.users = _usersRepository.Show();

            return View(showModel);
        }
        [HttpGet]
        public IActionResult ShowFiltered(string parent)
        {
            ShowModel showModel = new ShowModel();
            showModel.users = _usersRepository.Show(parent);
            return View("Show", showModel);
        }
        [HttpPost]
        public IActionResult Add(AddUser addUserModel)
        {
            _usersRepository.CreateUser(addUserModel);

            return RedirectToAction("Show");
        }
        [HttpPost]
        public IActionResult Delete(DeleteUser deleteUserModel)
        {
            if (_usersRepository.DeleteUser(deleteUserModel))
            {
                return RedirectToAction("Show");
            }
            else
            {
                ViewBag.Message = "Не могу удалить пользователя";
                return View("Index");
            }
        }
        [HttpPost]
        public IActionResult Change(ChangeUser changeUserModel)
        {
            if (_usersRepository.ChangeUser(changeUserModel))
            {
                return RedirectToAction("Show");
            }
            else
            {
                ViewBag.Message = "Не могу изменить пользователя";
                return View("Index");
            }
        }
        [HttpGet]
        public IActionResult FilterCount(string departmentToFilter)
        {
            string count = _usersRepository.GetCountUserByDepartmentName(departmentToFilter).ToString();
            return Content(count);
        }
        [HttpGet]
        public IActionResult FilterPositions(string departmentToFilter)
        {
            string count = _usersRepository.GetCountPositionsByDepartmentName(departmentToFilter).ToString();
            return Content(count);
        }
    }
}