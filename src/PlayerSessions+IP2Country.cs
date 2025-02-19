using CounterStrikeSharp.API.Core;
using IP2Country;
using IP2Country.MaxMind;


namespace PlayerSessions
{
    public partial class PlayerSessions : BasePlugin
    {
        private IP2CountryResolver? IpResolver = null;

        private void InitializeIP2Country()
        {
            string path = $"{ModuleDirectory}/{Config.Geolite2}";
            if (File.Exists(path))
            {
                IpResolver = new IP2CountryResolver(
                    new MaxMindGeoLiteFileSource(path)
                );
                Console.WriteLine(Localizer["ip2country.found"]);
            }
            else
            {
                Console.WriteLine(Localizer["ip2country.notfound"]);
            }
        }
    }
}