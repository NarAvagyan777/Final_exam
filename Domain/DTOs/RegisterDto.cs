using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class RegisterDto
    {
        public CreateUserDto User { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
