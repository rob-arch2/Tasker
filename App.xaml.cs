using Tasker.MVVM.Views;

namespace Tasker
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainView());
        }

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    return new Window(new NavigationPage(new MainPage()));
        //}
    }
}