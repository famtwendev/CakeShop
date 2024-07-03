namespace CakeShop.ModelsView.ForgotPassword
{
    public class AuthOTP
    {
        public string? email { get; set; }
        public string? first { get; set; } 
        public string? second { get; set; } 
        public string? third { get; set; } 
        public string? fourth { get; set; } 
        public string? fifth { get; set; } 
        public string? sixth { get; set; }
        public string? Total => first + second + third + fourth + fifth + sixth;
    }
}
