
namespace PredictFXCharts.Market.Internals
{
    // ************************************************************************

    sealed class NullReceiver : IDataReceiver
    {
        void IDataReceiver.PutMessage(Message msg) { }
        void IDataReceiver.PutStock(Quote[] quotes, Spread spread) { }
        void IDataReceiver.PutTick(Tick tick) { }
        void IDataReceiver.PutSetting(Setting setting) { } 

        void IDataReceiver.PutTrade(Trade trade) { }
        void IDataReceiver.PutOwnOrder(OwnOrder order) { }
        void IDataReceiver.PutPosition(int quantity, double price) { }
    }

    // ************************************************************************

}
