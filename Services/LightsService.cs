using System.Collections.Generic;
using System.Linq;

namespace Lights.Services
{
    public class LightsService : ILightsService
    {
        private readonly Dictionary<string, bool> _lights = new();

        public KeyValuePair<string, bool> Get(string key) => _lights.SingleOrDefault(p => p.Key == key);

        public Dictionary<string, bool> GetAll() => _lights;

        public void Set(string key, bool value) => _lights[key] = value;
    }
}