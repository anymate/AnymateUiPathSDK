using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anymate.UiPath.Models
{
    public class ApiResponse
    {
        public bool Succeeded { get; set; } = false;
        public string Message { get; set; }
    }
}
