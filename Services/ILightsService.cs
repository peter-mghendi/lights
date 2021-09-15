using System.Collections.Generic;

namespace Lights.Services
{
    public interface ILightsService
    {
        public KeyValuePair<string, bool> Get(string key);

        public Dictionary<string, bool> GetAll();

        public void Set(string key, bool value);
    }
}