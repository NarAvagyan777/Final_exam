using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class UpdateRatingDto
    {
        public int Score { get; set; }
        public string? Comment { get; set; }
    }
}
