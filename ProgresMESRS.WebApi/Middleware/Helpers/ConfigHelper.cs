using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgresMESRS.Middleware.API.Helpers
{
    public class Config
    {
        public List<string> StudentDetailsLabels { get; set; } = new List<string>();
        public static Config GetConfig()
        {
            try
            {
                var configFilePath = AppContext.BaseDirectory + @"\Config.json";
                var json = File.ReadAllText(configFilePath);
                return JsonConvert.DeserializeObject<Config>(json);
            }
            catch
            {
                return new Config();
            }
        }
    }
}
