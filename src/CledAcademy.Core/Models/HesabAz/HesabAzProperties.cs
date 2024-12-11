using System;
using System.Security.Cryptography;

namespace CledAcademy.Core.Models.HesabAz
{
    public static class HesabAzProperties
    {
        public const string RequestToServerUrl = "https://rest.goldenpay.az/";
        public const string RequestToServerUrlGetPaymentKey = RequestToServerUrl + "web/service/merchant/getPaymentKey";
        public const string RequestToServerUrlGetPaymentResult = RequestToServerUrl + "web/service/merchant/getPaymentResult";
        public const string RequestToServerUrlPayPage = RequestToServerUrl + "web/paypage?payment_key=";

        public const string MerchantName = "your_merchant_name";
        public const string AuthKey = "your_auth_key";

        public static string GetMd5HashCode(string input)
        {
            try
            {
                var md5 = MD5.Create();
                var data = md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(input));
                return BitConverter.ToString(data).Replace("-", "").ToLower();
            }
            catch (Exception exc)
            {
                return exc.Message;
            }
        }
    }
}
