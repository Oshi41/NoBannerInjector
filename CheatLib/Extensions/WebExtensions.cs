using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CheatLib.Extensions;

public class WebExtensions
{
    public static async Task<(bool, JObject)> GetJson(string url)
    {
        try
        {
            var req = WebRequest.CreateHttp(url);
            req.Accept = "application/json";
            var resp = await req.GetResponseAsync();
            var raw = await new StreamReader(resp.GetResponseStream()).ReadToEndAsync();
            var result = JsonConvert.DeserializeObject<JObject>(raw);
            return (true, result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return (false, null);
        }
    }
}