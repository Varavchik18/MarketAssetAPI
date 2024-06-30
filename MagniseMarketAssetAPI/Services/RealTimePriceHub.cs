using Microsoft.AspNetCore.SignalR;

namespace MagniseMarketAssetAPI.Services
{
    public class RealTimePriceHub : Hub
    {
        private readonly FintaChartsClientService_WS _fintaChartsClientService_WS;

        public RealTimePriceHub(FintaChartsClientService_WS fintaChartsClientService)
        {
            _fintaChartsClientService_WS = fintaChartsClientService;
        }

        public async Task SubscribeToPriceUpdates(string instrumentId, string provider)
        {
            await _fintaChartsClientService_WS.SubscribeAsync(instrumentId, provider);
            // Тут ми можемо надсилати початкові дані клієнту
            var realTimeData = _fintaChartsClientService_WS.GetRealTimeData(instrumentId);
            await Clients.Caller.SendAsync("ReceivePriceUpdate", realTimeData);
        }

        public async Task UnsubscribeFromPriceUpdates(string instrumentId, string provider)
        {
            await _fintaChartsClientService_WS.UnsubscribeAsync(instrumentId, provider);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Відписуємося від оновлень ціни, коли клієнт відключається
            // Потрібно додати логіку для відписки, якщо вона необхідна
            await base.OnDisconnectedAsync(exception);
        }
    }
}
