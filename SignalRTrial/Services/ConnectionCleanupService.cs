
using Microsoft.Extensions.Options;
using SignalRTry.Configurations;

namespace SignalRTry.Services
{
    public class ConnectionCleanupService : BackgroundService
    {
        private readonly UserConnectionService _userConnectionService;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _cleanupInterval;

        public ConnectionCleanupService(UserConnectionService userConnectionService, IServiceProvider serviceProvider, IOptions<ConnectionCleanupSettings> options)
        {
            _userConnectionService = userConnectionService;
            _serviceProvider = serviceProvider;
            _cleanupInterval = TimeSpan.FromSeconds(options.Value.IntervalSeconds);

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using(var scope = _serviceProvider.CreateScope())
                {
                    _userConnectionService.RemoveInactiveConnections(_cleanupInterval);
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }
        }
    }
}
