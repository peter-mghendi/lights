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

        private readonly int[] _pins = new int[] { Red, Yellow, Green };
        private readonly Dictionary<string, int> _map = new ()
        { 
            [nameof(Red).ToLower()] = Red, 
            [nameof(Yellow).ToLower()] = Yellow, 
            [nameof(Green).ToLower()] = Green 
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
                _controller.Write(pin, 0);
            }

            _lightsService.Set(nameof(Red).ToLower(), false);
            _lightsService.Set(nameof(Yellow).ToLower(), false);
            _lightsService.Set(nameof(Green).ToLower(), false);
        }

        [HttpGet]
        public Dictionary<string, bool> GetAll()
        {
            var red = nameof(Red).ToLower();
            var yellow = nameof(Yellow).ToLower();
            var green = nameof(Green).ToLower();

            return new ()
            {
                [red] = _lightsService.Get(red).Value,
                [yellow] = _lightsService.Get(yellow).Value,
                [green] = _lightsService.Get(green).Value,
            };
        }

        [HttpGet("{key}")]
        public KeyValuePair<string, bool> Get(string key) => 
            _lightsService.Get(key);

        [HttpPost("{key}")]
        public KeyValuePair<string, bool> Set(string key, [FromBody]StatusRequest request) 
        {
            Console.WriteLine($"{key}: {request.Value}");
            var pin = _map[key];
            _controller.Write(pin, request.Value ? 1 : 0);
            _lightsService.Set(key, request.Value);
            return _lightsService.Get(key);
        }

        public void Dispose() => _controller.Dispose();
    }
}
