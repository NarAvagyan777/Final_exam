using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace Domain.DTOs
{
    public class CreateUploadImage
    {
        public Guid RecipeId { get; set; }
        public IFormFile File { get; set; } = default!;
    }
}
