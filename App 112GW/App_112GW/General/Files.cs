using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;


namespace rMultiplatform
{
    class Files
    {
        static public async void SaveFile(string content)
        {
#if __ANDROID__
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#elif __IOS__
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#else
            var documentpicker = new Windows.Storage.Pickers.FileSavePicker();
            documentpicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            //documentpicker.FileTypeChoices.Add("Plain text.", new List<string>() { ".txt" });
            documentpicker.FileTypeChoices.Add("Comma seperated files.", new List<string>() { ".csv" });
            documentpicker.SuggestedFileName = "Logfile";

            var file = await documentpicker.PickSaveFileAsync();
            if (file != null)
            {
                Windows.Storage.FileIO.WriteTextAsync(file, content);
            }
#endif
        }
        static public async Task<string> LoadFile(string filename)
        {
#if __ANDROID__
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#elif __IOS__
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#else
            var documentpicker = new Windows.Storage.Pickers.FileOpenPicker();
            documentpicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            documentpicker.FileTypeFilter.Add(".txt"); 
            documentpicker.FileTypeFilter.Add(".csv");

            var file = await documentpicker.PickSingleFileAsync();
            if (file != null)
            {
                var content = await Windows.Storage.FileIO.ReadTextAsync(file);
                return content;
            }
#endif
            return "";
        }
    }
}
