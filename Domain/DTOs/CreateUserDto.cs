using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class CreateUserDto
    {
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}
