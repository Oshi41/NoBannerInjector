using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace example
{
    public class WishModel
    {
    }

    public class WishManager
    {
        public Dictionary<string, List<WishModel>> Wishes = new Dictionary<string, List<WishModel>>();
        public string FileName => "wishes_history.json";

        public void Load()
        {
            var models = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(FileName));
            
        }
    }
}