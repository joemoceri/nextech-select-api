using System.Configuration;

namespace NextechSelectApi
{
    class Program
    {
        static void Main(string[] args)
        {
            var practiceId = ConfigurationManager.AppSettings["PracticeId"];
            var clientId = ConfigurationManager.AppSettings["ClientId"];
            var username = ConfigurationManager.AppSettings["Username"];
            var password = ConfigurationManager.AppSettings["Password"];

            var api = new Api(practiceId, clientId, username, password);
            var patients = api.GetPatients();
        }
    }
}
