﻿using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    public partial class Notification
    {
        public long IdNotification { get; set; }
        public long IdAccount { get; set; }
        public string Description { get; set; }
        public bool ReadNotification { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public DateTime NotificationDate { get; set; }

        public Account IdAccountNavigation { get; set; }
    }
}
