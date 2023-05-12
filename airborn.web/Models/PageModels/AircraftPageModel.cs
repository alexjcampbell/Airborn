using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Airborn.web.Models.PerformanceData;

namespace Airborn.web.Models
{


    public class AircraftPageModel
    {
        public BookPerformanceDataList PerformanceDataList { get; set; }

        public Aircraft Aircraft { get; set; }

    }
}
