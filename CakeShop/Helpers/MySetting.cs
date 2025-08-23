namespace CakeShop.Helpers
{
    public class MySetting
    {
        public static string CART_KEY = "MYCART";
        public static string CLAIM_CUSTOMERID = "CustomerID";
        public static string CLAIM_ADMINID = "AdminID";
    }
    public class PaymentType
    {
        public static string COD = "COD";
        public static string Paypal = "Paypal";
        public static string VNPAY = "VnPay";
        public static string MOMO = "MoMo";
        public static string STRIPE = "Stripe";
    }

    public static class SD
    {
        public const string Role_Admin = "Admin";


        public const string RolePB_BGD = "BGD";         // Ban giám đốc
        public const string RolePB_PKT = "PKT";         // Phòng kỹ thuật
        public const string RolePB_PNS = "PNS";         // Phòng nhân sự
        public const string RolePB_PKTo = "PKTo";       // Phòng Kế Toán

    }
}
