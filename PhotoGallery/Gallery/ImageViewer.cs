using Xamarin.Forms;

namespace Gallery
{
    public class ImageViewer : ContentPage
    {
        public ImageViewer(string selectedImageImagePath)
        {
            Content = new Xamarin.Forms.Image {Source = selectedImageImagePath};
        }
    }
}