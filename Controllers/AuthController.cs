using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Http.Cors;

namespace WebApiSch.Controllers
{
    [EnableCors(origins: "http://localhost:3003/", headers: "*", methods: "*")]
    public class AuthController : ApiController
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["connStringEnsafeSql"].ConnectionString;

        [System.Web.Http.HttpPost]
        public HttpResponseMessage Login(LoginModel login)
        {
            if (login == null || string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Password))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid login details");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", login.Username);
                        command.Parameters.AddWithValue("@Password", login.Password);
                        int count = (int)command.ExecuteScalar();

                        if (count > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, "Login successful");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid username or password");
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Log the detailed SqlException (if Somee allows logging)
                    // For now, let's return a more informative, but still secure, error message
                    Console.WriteLine(ex.Message);
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Database error occurred. Please contact administrator.");
                }
                catch (Exception ex)
                {
                    // Log the general exception (if Somee allows logging)
                    Console.WriteLine(ex.Message);
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred. Please contact administrator.");
                }
            }
        }

        [System.Web.Http.HttpOptions]
        public HttpResponseMessage Options()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            return response;
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
