using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairPlatform.Services
{
    public static class ImageHelper
    {
        public static string ToBase64String(byte[] imageBytes)
        {
            return Convert.ToBase64String(imageBytes);
        }
    }
}
