using System.Threading.Tasks;
using NUnit.Framework;

namespace AsyncVoidCommandTesting.Tests;

[TestFixture]
public class MainWindowViewModelTests
{
    [Test]
    public void Click_FailingToAwaitCommandExecution()
    {
        var mainWindowViewModel = new MainWindowViewModel();

        mainWindowViewModel.Click.Execute(null);
        
        Assert.That(mainWindowViewModel.Upper, Is.EqualTo("BEFORE"));
    }

    [Test]
    public async Task Click_ExpectUpperToBeUpperCase()
    {
        var mainWindowViewModel = new MainWindowViewModel();

        await AsyncVoidSynchronizationContext.Run(() => mainWindowViewModel.Click.Execute(null));
        
        Assert.That(mainWindowViewModel.Upper, Is.EqualTo("BEFORE"));
    }

    [Test]
    public async Task OtherClick_ExpectUpperToBeUpperCase()
    {
        var mainWindowViewModel = new MainWindowViewModel();

        await mainWindowViewModel.OtherClick.ExecuteAsync(null);

        Assert.That(mainWindowViewModel.Upper, Is.EqualTo("BEFORE"));
    }
}