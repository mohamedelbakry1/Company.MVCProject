namespace Company.MVCProject.PL.Helpers
{
    public static class DocumentSettings
    {
        // 1. Upload
        public static string UploadFile(IFormFile file, string folderName)
        {
            // 1. Get Folder Location
            //string folderPath = "C:\\.NET Route\\MVC\\MVC_Project\\Company.MVCProject\\Company.MVCProject.PL\\wwwroot\\files\\images\\";

            //var folderPath = Directory.GetCurrentDirectory() + "\\wwwroot\\files\\" + folderName;

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\files", folderName);

            // 2. FileName and Make It Unique

            var fileName = $"{Guid.NewGuid()}{file.FileName}";

            // FilePath

            var filePath = Path.Combine(folderPath, fileName);

            using var fileStream = new FileStream(filePath,FileMode.Create);

            file.CopyTo(fileStream);

            return fileName;
        }
        // 2. Delete

        public static void DeleteFile(string fileName,string folderName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\files", folderName, fileName);
            if(File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
