using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AsyncVoidCommandTesting;

public class MainWindowViewModel : ObservableObject
{
    private ICommand? _click;
    private IAsyncRelayCommand? _otherClick;
    private string _upper = "before";

    public ICommand Click => _click ??= new AsyncRelayCommand(Execute);

    public IAsyncRelayCommand OtherClick => _otherClick ??= new AsyncRelayCommand(Execute);

    public string Upper
    {
        get => _upper;
        set => SetProperty(ref _upper, value);
    }

    private async Task Execute()
    {
        await Task.Delay(2000);

        Upper = Upper.ToUpper();
    }
}