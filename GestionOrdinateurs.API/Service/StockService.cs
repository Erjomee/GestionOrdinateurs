namespace GestionOrdinateurs.API;

public class StockService
{
    public int StockSSD = 10;
    private readonly object _lock = new object();

    public async Task ConsommerSSD()
    {
        Task.Delay(10).Wait();

        lock (_lock)
        {
            StockSSD = StockSSD - 1 ;
        }
        // StockSSD = StockSSD - 1 ;
        // await Task.Delay(1);
    }
}