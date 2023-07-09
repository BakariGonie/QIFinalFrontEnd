using InterviewFrontEnd.Endpoints;
using InterviewFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;
using static InterviewFrontEnd.Models.Requests.Requests;
using static InterviewFrontEnd.Models.Responses.Responses;

namespace InterviewFrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult LandingPage()
        {
            return View();
        }

        public async Task<IActionResult> RegisterPatient()
        {
            //action to get all patients in the database
            //GetAllPatients_response allPatients = await GetAllPatients();

            //list to map results data
            //List<GetAllPatientsDatum> patientsDatum = new();

            //if (allPatients != null)
            //{
            //    if (allPatients.code == 200)
            //    {
            //        patientsDatum = allPatients.data;
            //    }
            //}

            //ViewBag.Patients = patientsDatum;

            return View();
        }

        public IActionResult AddVitals()
        {
            return View();
        }

        public IActionResult Reports()
        {
            return View();
        }

        [HttpPost]
        public async Task<string> AddPatient(PatientRegRequest param)
        {
            RestClient _client = new(Connections.BaseURL);
            int code = 0;
            string message = String.Empty;

            try
            {
                PatientRegRequest requestParameters = new()
                {
                    DOB = param.DOB,
                    FirstName = param.FirstName,
                    LastName = param.LastName,
                    Gender = param.Gender,
                };

                string serializedReqParameters = JsonConvert.SerializeObject(requestParameters);

                var request = new RestRequest(Connections.AddNewPatient).AddJsonBody(serializedReqParameters);

                var response = await _client.ExecutePostAsync<PatientRegResponse>(request);

                if (!response.IsSuccessful)
                {
                    code = (int)response.StatusCode;
                    message = "Failed to Send Request";
                }

                try
                {
                    var json = JsonConvert.DeserializeObject<PatientRegResponse>(response.Content);

                    //code = (int)json.code;
                    //message = json.message.ToString();

                    string stringjson1 = JsonConvert.SerializeObject(json);

                    return stringjson1;
                }
                catch (JsonSerializationException exx)
                {
                    code = (int)response.StatusCode;
                    message = exx.Message;

                    return message;
                }
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }

        [HttpGet]
        public async Task<GetAllPatients_response> GetAllPatients()
        {
            RestClient client = new(Connections.BaseURL);

            try
            {
                var request = new RestRequest(Connections.AllPatients);

                var response = await client.ExecuteGetAsync<GetAllPatients_response>(request);

                if (!response.IsSuccessful)
                {
                    return new GetAllPatients_response()
                    {
                        code = (int)response.StatusCode,
                        message = response.ErrorMessage
                    };

                }

                try
                {
                    var json = JsonConvert.DeserializeObject<GetAllPatients_response>(response.Content);

                    return new GetAllPatients_response()
                    {
                        code = json.code,
                        message = json.message,
                        data = json.data
                    };
                }
                catch (JsonSerializationException exx)
                {
                    return new GetAllPatients_response()
                    {
                        code = (int)response.StatusCode,
                        message = exx.Message
                    };
                }
            }
            catch (Exception ex)
            {
                return new GetAllPatients_response()
                {
                    code = 500,
                    message = ex.Message
                };
            }
        }
    }
}