﻿namespace Mango.Web.Utility
{
    public class StaticDetails
    {
        //this will have base url for couponAPI and we will store this url in appsettings
        //then populate this in program cs
        public static string CouponAPIBase { get; set; }
        public static string ProductAPIBase { get; set; }
        public static string AuthAPIBase { get; set; }
        public static string ShoppingCartAPIBase { get; set; }
        public static string OrderAPIBase { get; set; }
        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";
        public const string TokenCookie = "JWTToken";

        public enum ApiType
        {
            GET,
            POST, 
            PUT, 
            DELETE
        }
    }
}
