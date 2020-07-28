using InfinityNavigation.Services;
using InfinityNavigation.Views;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace InfinityNavigation
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            DependencyService.Register<MockDataStore>();
            RestoreAppState();
        }

        private void RestoreAppState()
        {
            ItemsViewModelDataSource.readPagesContentInMemory();
            if (ItemsViewModelDataSource.PagesContent.Count == 0)
            {
                MainPage = new NavigationPage(new ItemsPage());
            }
            else
            {
                Log.Warning("Loaded pages count ", ItemsViewModelDataSource.PagesContent.Count.ToString());
                var firstPage = true;
                ItemsViewModelDataSource.PagesContent
                    .ForEach(model =>
                    {
                        if (firstPage)
                        {
                            MainPage = new NavigationPage(new ItemsPage(model));
                            firstPage = false;
                            return;
                        }

                        Current.MainPage.Navigation.PushAsync(new ItemsPage(model));
                    });
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}