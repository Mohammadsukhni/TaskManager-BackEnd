using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Core.Dto
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }
}
