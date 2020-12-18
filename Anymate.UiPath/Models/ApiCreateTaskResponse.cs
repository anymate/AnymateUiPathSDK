using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anymate.UiPath
{
    public class ApiCreateTaskResponse
    {
        public bool Succeeded { get; set; } = false;
        public string Message { get; set; }
        public long TaskId { get; set; }
    }
}
