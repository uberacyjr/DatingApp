using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Acess-Control-Expose-Headers", message);
            response.Headers.Add("Acess-Control-Allow-Origin","*");
        }
    }
}