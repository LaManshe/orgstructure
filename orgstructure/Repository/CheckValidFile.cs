using orgstructure.Repository.Interfaces;

namespace orgstructure.Repository
{
    public class CheckValidFile : ICheckValidFile
    {
        public bool ExcelValid(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            string fileExtension = Path.GetExtension(file.FileName);
            if (fileExtension != ".xls" && fileExtension != ".xlsx")
                return false;

            return true;
        }
    }
}
