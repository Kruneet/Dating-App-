using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Helpers
{

// no new instance of static class allowed when we want to use any of its methods.
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Exposed-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
}