using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lights.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Lights.Controllers
{
    public record StatusRequest(bool Value);

    [ApiController]
    [Route("api/[controller]")]
    public class LightsController : ControllerBase, IDisposable
    {
        private const int Red = 18;
        private const int Yellow = 24;
        private const int Green = 25;

        private static readonly string _red = nameof(Red).ToLower();
        private static readonly string _yellow = nameof(Yellow).ToLower();
        private static readonly string _green = nameof(Green).ToLower();

        private readonly int[] _pins = new int[] { Red, Yellow, Green };
        private readonly Dictionary<string, int> _map = new ()
        { 
            [_red] = Red, 
            [_yellow] = Yellow, 
            [_green] = Green 
        };

        private readonly GpioController _controller;
        private readonly ILightsService _lightsService;

        public LightsController(ILightsService lightsService) 
        {
            _controller = new();
            _lightsService = lightsService;

            foreach (var pin in _pins)
            {
                _controller.OpenPin(pin, PinMode.Output);
                // _controller.Write(pin, 0);
            }
        }

        [HttpGet]
        public Dictionary<string, bool> GetAll() =>
            _lightsService.GetAll();

        [HttpGet("{key}")]
        public KeyValuePair<string, bool> Get(string key) => 
            _lightsService.Get(key);

        [HttpPost("{key}")]
        public KeyValuePair<string, bool> Set(string key, [FromBody]StatusRequest request) 
        {
            var pin = _map[key];
            _controller.Write(pin, request.Value ? 1 : 0);
            _lightsService.Set(key, request.Value);
            return _lightsService.Get(key);
        }

        public void Dispose() => _controller.Dispose();
    }
}
