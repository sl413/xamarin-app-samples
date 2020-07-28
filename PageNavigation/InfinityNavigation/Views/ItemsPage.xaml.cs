using System;
using System.ComponentModel;
using InfinityNavigation.Services;
using InfinityNavigation.ViewModels;
using Xamarin.Forms;

namespace InfinityNavigation.Views
{
    [DesignTimeVisible(false)]
    public partial class ItemsPage : ContentPage
    {
        private readonly ItemsViewModel viewModel;

        public ItemsPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new ItemsViewModel();
            ItemsViewModelDataSource.PushPageContent(viewModel);
        }

        public ItemsPage(ItemsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = this.viewModel = viewModel;
            Title = viewModel.Title;
        }

        private async void OnItemSelected(object sender, EventArgs args)
        {
            var itemsPage = new ItemsPage();
            itemsPage.viewModel.Title = ((Label) ((StackLayout) sender).Children[0]).Text;
            await Navigation.PushAsync(itemsPage);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.Items.Count == 0)
                viewModel.IsBusy = true;
        }

        protected override bool OnBackButtonPressed()
        {
            ItemsViewModelDataSource.PopPageContent();
            return base.OnBackButtonPressed();
        }
    }
}