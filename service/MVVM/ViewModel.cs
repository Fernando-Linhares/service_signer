using Service.MVVM.Models;

namespace Service.MVVM;

public class ViewModel
{
    private readonly Application _app = new Application();

    public async Task<string> RunApplication(string request)
    {
        return await _app.Flush(request);
    }
}