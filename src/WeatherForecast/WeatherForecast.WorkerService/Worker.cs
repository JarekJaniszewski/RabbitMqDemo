using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeatherForecast.CommonData.Models;
using WeatherForecast.CommonData.RabbitQueue;

namespace WeatherForecast.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IBus _busControl;
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _busControl = RabbitHutch.CreateBus("localhost");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _busControl.ReceiveAsync<IEnumerable<WeatherForecastRequest>>(Queue.Processing, x =>
            {
                Task.Run(() => { DidJob(x); }, stoppingToken);
            });
        }

        private void DidJob(IEnumerable<WeatherForecastRequest> weatherForecasts)
        {
            foreach (var weatherForecast in weatherForecasts)
            {
                _logger.LogInformation($"Location: {weatherForecast.Location}, date: {weatherForecast.Date}, {weatherForecast.TemperatureC}°C, {weatherForecast.TemperatureF}°F, Summary: {weatherForecast.Summary}");
            }
        }
    }
}
