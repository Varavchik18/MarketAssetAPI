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
            var realTimeData = _fintaChartsClientService_WS.GetRealTimeData(instrumentId);
            await Clients.Caller.SendAsync("ReceivePriceUpdate", realTimeData);
        }

        public async Task UnsubscribeFromPriceUpdates()
        {
            base.Dispose();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
