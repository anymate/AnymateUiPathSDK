using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anymate.UiPath.Models
{
    public class ApiOkToRun
    {
        public bool GateOpen { get; set; }
        public bool TasksAvailable { get; set; }
        public bool NotBlockedDate { get; set; }
        public bool OkToRun { get; set; }
    }
}
