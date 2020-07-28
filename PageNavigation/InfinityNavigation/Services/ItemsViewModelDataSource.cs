using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfinityNavigation.ViewModels;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace InfinityNavigation.Services
{
    public static class ItemsViewModelDataSource
    {
        public static Stack<ItemsViewModel> PagesContent { get; private set; } = new Stack<ItemsViewModel>();
        private const string SharedPrefKey = "pagesState";

        public static void PushPageContent(ItemsViewModel viewModel)
        {
            PagesContent.Push(viewModel);
            WriteChangesToMemory();
        }

        public static void PopPageContent()
        {
            PagesContent.Pop();
            WriteChangesToMemory();
        }

        private static async Task WriteChangesToMemory()
        {
            if (PagesContent.Count < 2)
            {
                return;
            }

            var serializeObject = JsonConvert.SerializeObject(PagesContent);
            Application.Current.Properties[SharedPrefKey] = serializeObject;
            await Application.Current.SavePropertiesAsync();
            Log.Warning("Stack Items Count: ", PagesContent.Count.ToString());
            Log.Warning("Current Stack State", serializeObject);
        }

        public static void readPagesContentInMemory()
        {
            if (!Application.Current.Properties.ContainsKey(SharedPrefKey)) return;
            var property = Application.Current.Properties[SharedPrefKey] as string;
            var deserializeObject = JsonConvert.DeserializeObject<Stack<ItemsViewModel>>
                (property ?? throw new InvalidOperationException());
            PagesContent = deserializeObject;
            Log.Warning("Loaded Stack State", property);
        }
    }
}