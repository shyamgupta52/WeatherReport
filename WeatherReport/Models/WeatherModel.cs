using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WeatherReport.Models
{
    public class WeatherModel
    {
        public string apiResponse { get; set; }

        public Dictionary<string, string> Cities
        {
            get;
            set;
        }
        [Required]
        public HttpPostedFileBase postedFile { get; set; }
    }
}