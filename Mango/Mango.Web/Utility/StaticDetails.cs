namespace Mango.Web.Utility
{
    public class StaticDetails
    {
        //this will have base url for couponAPI and we will store this url in appsettings
        //then populate this in program cs
        public static string CouponAPIBase { get; set; }
        public enum ApiType
        {
            GET,
            POST, 
            PUT, 
            DELETE
        }
    }
}
