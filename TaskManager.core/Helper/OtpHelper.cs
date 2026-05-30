using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Core.Helper
{
    public static class OtpHelper
    {
        public static string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
