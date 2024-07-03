using System.Text;

namespace CakeShop.Helpers
{
	public class MyUtil
	{
		/* public static string UploadHinh(IFormFile Hinh, string folder)
         {
             try
             {
                 var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folder, Hinh.FileName);
                 using (var myfile = new FileStream(fullPath, FileMode.CreateNew))
                 {
                     Hinh.CopyTo(myfile);
                 }
                 return Hinh.FileName;
             }
             catch (Exception ex)
             {
                 return string.Empty;
             }
         }*/
		public static string UploadHinh(IFormFile Hinh, string folder)
		{
			try
			{
				var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Hinh.FileName); // Tạo tên file ngẫu nhiên
				var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folder, fileName);

				using (var myfile = new FileStream(fullPath, FileMode.Create))
				{
					Hinh.CopyTo(myfile);
				}
				return fileName; // Trả về tên file đã lưu để lưu vào cơ sở dữ liệu
			}
			catch (Exception ex)
			{
				// Xử lý lỗi và trả về tên rỗng nếu có lỗi
				Console.WriteLine(ex.Message);
				return string.Empty;
			}
		}

		public static bool DeleteHinh(string fileName, string folder)
		{
			try
			{
				var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folder, fileName);

				if (File.Exists(fullPath))
				{
					File.Delete(fullPath);
					return true; // Trả về true nếu xóa thành công
				}
				else
				{
					return false; // Trả về false nếu file không tồn tại
				}
			}
			catch (Exception ex)
			{
				// Xử lý lỗi và trả về false nếu có lỗi
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public static string GenerateRamdomKey(int length = 6)
		{
			var pattern = @"qazwsxedcrfvtgbyhnujmiklopQAZWSXEDCRFVTGBYHNUJMIKLOP!";
			var sb = new StringBuilder();
			var rd = new Random();
			for (int i = 0; i < length; i++)
			{
				sb.Append(pattern[rd.Next(0, pattern.Length)]);
			}

			return sb.ToString();
		}
		public string MaskEmail(string email)
		{
			if (string.IsNullOrEmpty(email))
				return email;

			var atIndex = email.IndexOf('@');
			if (atIndex <= 6)
			{
				return email;
			}

			var userName = email.Substring(0, atIndex);
			var domain = email.Substring(atIndex);

			var maskedUserName = userName.Substring(0, 6) + new string('*', userName.Length - 6);

			return maskedUserName + domain;
		}
		public static string getSoNgauNhien(int length = 6)
		{
			var pattern = @"1234567890"; // Removed @ since it's not needed here
			var sb = new StringBuilder();
			var rd = new Random();
			for (int i = 0; i < length; i++)
			{
				sb.Append(pattern[rd.Next(0, pattern.Length)]);
			}

			return sb.ToString();
		}

		public static string GeneratePassword(int length = 10)
		{
			var pattern = @"01289abcdefghijklmnopqrstuvwx456yzABCDEFGHIJKL37MNOPQRSTUVWXYZ!@#&/";
			var sb = new StringBuilder();
			var rd = new Random();
			for (int i = 0; i < length; i++)
			{
				sb.Append(pattern[rd.Next(0, pattern.Length)]);
			}
			return sb.ToString();
		}

	}
}
