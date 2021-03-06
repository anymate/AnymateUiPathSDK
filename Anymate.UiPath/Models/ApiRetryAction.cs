﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anymate.UiPath
{
    public class ApiRetryAction
    {
        public ApiRetryAction() { }
        public ApiRetryAction(long taskId, string reason)
        {
            TaskId = taskId;
            Reason = reason;
        }
        public ApiRetryAction(long taskId, string reason, string newNote)
        {
            TaskId = taskId;
            Reason = reason;
            Comment = newNote;
        }
        public long TaskId { get; set; }
        public string Reason { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int? OverwriteSecondsSaved { get; set; } = null;
        public int? OverwriteEntries { get; set; } = null;
        public DateTimeOffset? ActivationDate { get; set; } = null;
    }
}
