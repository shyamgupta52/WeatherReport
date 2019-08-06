using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WeatherReport.Class;
using WeatherReport.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WeatherReport.Controllers
{
    public class WeatherMapController : Controller
    {
        // GET: WeatherMap
        public ActionResult Index()
        {
            return View();

        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            if (postedFile != null && postedFile.ContentLength > 0)
            {
                try
                {
                    string filePath = string.Empty;

                    string path = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    filePath = path + Path.GetFileName(postedFile.FileName);
                    string extension = Path.GetExtension(postedFile.FileName);
                    postedFile.SaveAs(filePath);

                    //Read the contents of txt file.
                    string txtData = System.IO.File.ReadAllText(filePath);

                    Dictionary<string, string> cities = new Dictionary<string, string>();


                    //Execute a loop over the lines.
                    foreach (string row in txtData.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(row))
                        {
                            cities.Add((row.Split('=')[1]), (row.Split('=')[0]));
                        }
                    }

                    if (cities != null)
                    {
                        foreach (var city in cities)
                        {
                            string apiKey = "aa69195559bd4f88d79f9aadeb77a8f6";

                            HttpWebRequest apiRequest = WebRequest.Create("http://api.openweathermap.org/data/2.5/weather?id=" + Convert.ToInt32(city.Value) + "&appid=" + apiKey + "&units=metric") as HttpWebRequest;

                            //Getting API's Response

                            string apiResponse = "";
                            using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
                            {
                                StreamReader reader = new StreamReader(response.GetResponseStream());
                                apiResponse = reader.ReadToEnd();
                            }
                            //Converting the response
                            ResponseWeather rootObject = JsonConvert.DeserializeObject<ResponseWeather>(apiResponse);

                            //Dumping the response into String Builder
                            StringBuilder sb = new StringBuilder();
                            sb.Append("Weather Description" + "\n");
                            sb.Append("City:" + rootObject.name + "\n");
                            sb.Append("Country:" + rootObject.sys.country + "\n");
                            sb.Append("Sun Rise:" + rootObject.sys.sunrise + "\n");
                            sb.Append("Country Sun Set:" + rootObject.sys.sunset + "\n");
                            sb.Append("Wind:" + rootObject.wind.speed + " Km/h" + "\n");
                            sb.Append("Current Temperature:" + rootObject.main.temp + " °C" + "\n");
                            sb.Append("Max. Temperature:" + rootObject.main.temp_max + " °C" + "\n");
                            sb.Append("Min. Temperature:" + rootObject.main.temp_min + " °C" + "\n");
                            sb.Append("Pressure:" + rootObject.main.pressure + "\n");
                            sb.Append("Humidity:" + rootObject.main.humidity + "\n");
                            sb.Append("Weather:" + rootObject.weather[0].description + "\n");

                            //Creating the required files with output
                            string directory = Server.MapPath("~");
                            string filename = String.Format("{0:yyyy-MM-dd}__{1}.txt", DateTime.Now, city.Key.ToString().Replace("\r", ""));
                            string filepath = Path.Combine(directory, filename);
                            if (System.IO.File.Exists(filepath))
                            {
                                System.IO.File.Delete(filepath);
                            }
                            using (StreamWriter swriter = System.IO.File.CreateText(filepath))
                            {
                                swriter.Write(sb.ToString());
                                ViewBag.Message = "SUCCESS. Your generated weather reports are at:" + directory.ToString();
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Exception Occured:" + ex.Message;
                }

            }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }

            return View();
        }
    }
}

