using CounterStrikeSharp.API.Core;
using MaxMind.Db;
using MaxMind.GeoIP2;

namespace PlayerSessions
{
    public partial class PlayerSessions : BasePlugin
    {
        private const string Separator = ":";
        private DatabaseReader? IpResolver = null;

        private void InitializeIP2Country()
        {
            string path = $"{ModuleDirectory}/{Config.Geolite2}";
            DebugPrint($"Geolite2 path: {path}");
            if (File.Exists(path))
            {
                IpResolver = new DatabaseReader(path, FileAccessMode.Memory);
                Console.WriteLine(Localizer["ip2country.found"]);
            }
            else
            {
                Console.WriteLine(Localizer["ip2country.notfound"]);
            }
        }

        private Dictionary<string, string> IP2Country(string IP)
        {
            Dictionary<string, string> returnDict = [];
            DebugPrint($"Geolite2 IP to check: {IP}");
            if (IpResolver == null) return returnDict;
            try
            {
                var response = IpResolver.City(IP);
                if (response == null) return returnDict;
                if (response.City != null && response.City.Name != null) returnDict.Add("city", response.City.Name.ToString());
                if (response.Country != null && response.Country.Name != null) returnDict.Add("country", response.Country.Name.ToString());
            }
            catch (Exception error)
            {
                DebugPrint($"Geolite2 error checking IP: ${error}");
            }
            return returnDict;
        }
    }
}
