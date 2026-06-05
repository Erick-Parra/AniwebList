using Microsoft.JSInterop;

namespace AnimeList.Web.Services;

public class ThemeService(IJSRuntime js)
{
    public bool IsDark { get; private set; }
    public event Action? OnChange;

    public async Task InitAsync()
    {
        try { IsDark = await js.InvokeAsync<bool>("getTheme"); }
        catch { IsDark = false; }
    }

    public async Task ToggleAsync()
    {
        IsDark = !IsDark;
        try { await js.InvokeVoidAsync("setTheme", IsDark); }
        catch { }
        OnChange?.Invoke();
    }
}
