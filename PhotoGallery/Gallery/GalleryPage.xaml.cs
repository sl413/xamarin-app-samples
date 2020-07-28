using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace Gallery
{
    [DesignTimeVisible(false)]
    public partial class GalleryPage : ContentPage
    {
        public GalleryPage()
        {
            Title = "Gallery";
            ImageModels = new List<Image>();
            for (var i = 0; i < 7337; i++)
            {
                var a = new Image
                {
                    Title = Guid.NewGuid().ToString(), ImagePath = "cat.png"
                };
                ImageModels.Add(a);
            }

            var listView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                HasUnevenRows = true,
                ItemsSource = ImageModels,
                ItemTemplate = new DataTemplate(() =>
                {
                    var imageCell = new ImageCell {TextColor = Color.Black, DetailColor = Color.Green};
                    imageCell.SetBinding(TextCell.TextProperty, "Title");
                    imageCell.SetBinding(ImageCell.ImageSourceProperty, "ImagePath");
                    return imageCell;
                })
            };
            listView.ItemTapped += OnItemTapped;
            Content = new StackLayout {Children = {listView}};
        }

        private List<Image> ImageModels { get; }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Image selectedImage) await Navigation.PushAsync(new ImageViewer(selectedImage.ImagePath));
        }
    }
}