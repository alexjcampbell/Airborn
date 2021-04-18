using System;

namespace Airborn.web.Models
{
    public class ErrorPageModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
