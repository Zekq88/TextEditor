using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using System.Reflection.Metadata.Ecma335;
using Windows.ApplicationModel.Store;
using Windows.Storage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Lab_3___Texteditor
{
    /// <summary>
    /// Denna klass innehåller funktionalliteten i själva work editor applicationen.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ApplicationView appView = ApplicationView.GetForCurrentView(); // Inspiration till kod https://stackoverflow.com/questions/43414593/how-to-change-the-app-title-in-a-uwp-app
        private bool oneTimeChange = true;
      
        private StorageFile quickSaveFile = null;

        //Main funktionen i applicationen som innehåller ett if-vilkor som avgör om app titeln ska heta "namnlös.txt" eller inte.
        public MainPage()
        {
            this.InitializeComponent();

            if (appView.Title != null)
            {
                appView.Title = "namnlös.txt";
            }
        }

        /// <summary>
        /// När knappen "New File" trycks på, startas denna metod som avgör om användaren kan skapa ett nytt dokument direkt eller behöver frågas om hen 
        /// behöver spara sitt arbete innan. 
        /// Allt detta avgörs utav bool <c>oneTimeChange</c> tillstånd, som sätts till <c>true</c> efter att funktionen har användts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void New_btn_Click(object sender, RoutedEventArgs e)
        {

            if (oneTimeChange == false)
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    Title = "Confirm",
                    Content = "You're about to create a new file without saving first.\nDo you want to save?\nAre you sure to create a new file?",
                    CloseButtonText = "Cancel",
                    SecondaryButtonText = "No, save first",
                    PrimaryButtonText = "Yes, create new file"

                };
                var result1 = await contentDialog.ShowAsync();

                if (result1 == ContentDialogResult.Secondary)
                {
                    FileSavePicker sp = new FileSavePicker();
                    sp.FileTypeChoices.Add("Text Dokument", new List<string>() { ".txt" });
                    sp.SuggestedStartLocation = PickerLocationId.Desktop;
                    sp.SuggestedFileName = appView.Title.Replace(".txt*", "");
                    var result = await sp.PickSaveFileAsync();
                

                    if (result != null)
                    {
                        quickSaveFile = result;
                        appView.Title = result.DisplayName + result.FileType;
                        ResultatText.Text = "";
                        appView.Title = "namnlös.txt";
                    }
                }
                if (result1 == ContentDialogResult.Primary)
                {
                    ResultatText.Text = "";
                    appView.Title = "namnlös.txt";
                }
                if (result1 == ContentDialogResult.None)
                {
                    return;
                }
            }
            else
            {
                ResultatText.Text = "";
                appView.Title = "namnlös.txt";
            }

            oneTimeChange = true;
        }

        /// <summary>
        /// När knappen "Open File" trycks på, startas denna metod som avgör om användaren kan öppna ett nytt dokument direkt eller behöver frågas om hen 
        /// behöver spara sitt arbete innan. 
        /// Allt detta avgörs utav bool <c>oneTimeChange</c> tillstånd, som sätts till <c>true</c> efter att funktionen har användts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Open_btn_Click(object sender, RoutedEventArgs e)
        {
            if (oneTimeChange)
            {
                FileOpenPicker op = new FileOpenPicker();
                op.SuggestedStartLocation = PickerLocationId.Desktop;
                op.FileTypeFilter.Add(".txt");
                var result = await op.PickSingleFileAsync();

                if (result != null)
                {
                    quickSaveFile = result;
                    var text = await Windows.Storage.FileIO.ReadTextAsync(result);
                    ResultatText.Text = text;
                    appView.Title = result.DisplayName + result.FileType;
                }
            }
            else
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    Title = "Confirm",
                    Content = "You're about to open a file without saving first.\nDo you want to save?\nAre you sure to create a new file?",
                    CloseButtonText = "Cancel",
                    SecondaryButtonText = "No, save first",
                    PrimaryButtonText = "Yes, open a file"

                };
                var result1 = await contentDialog.ShowAsync();

                if (result1 == ContentDialogResult.Secondary)
                {
                    FileSavePicker sp = new FileSavePicker();
                    sp.FileTypeChoices.Add("Text Dokument", new List<string>() { ".txt" });
                    sp.SuggestedStartLocation = PickerLocationId.Desktop;
                    sp.SuggestedFileName = appView.Title.Replace(".txt*", "");
                    var result = await sp.PickSaveFileAsync();

                    if (result != null)
                    {
                        await Windows.Storage.FileIO.WriteTextAsync(result, ResultatText.Text);
                        appView.Title = result.DisplayName + result.FileType;

                        FileOpenPicker op = new FileOpenPicker();
                        op.SuggestedStartLocation = PickerLocationId.Desktop;
                        op.FileTypeFilter.Add(".txt");
                        var result2 = await op.PickSingleFileAsync();

                        if (result2 != null)
                        {

                            var text = await Windows.Storage.FileIO.ReadTextAsync(result2);
                            ResultatText.Text = text;
                            appView.Title = result2.DisplayName + result2.FileType;
                        }
                    }
                }
                if (result1 == ContentDialogResult.Primary)
                {
                    FileOpenPicker op = new FileOpenPicker();
                    op.SuggestedStartLocation = PickerLocationId.Desktop;
                    op.FileTypeFilter.Add(".txt");
                    var result = await op.PickSingleFileAsync();
                    if (result != null)
                    {
                        var text = await Windows.Storage.FileIO.ReadTextAsync(result);
                        ResultatText.Text = text;
                        appView.Title = result.DisplayName + result.FileType;
                    }
                }
                if (result1 == ContentDialogResult.None)
                {
                    return;
                }
            }

            oneTimeChange = true;
        }

        /// <summary>
        /// När knappen "Save as" trycks på, startas denna metod som sparar användarens arbete till vald plats och fil namn.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SaveAs_btn_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker sp = new FileSavePicker();
            sp.FileTypeChoices.Add("Text Dokument", new List<string>() { ".txt" });
            sp.SuggestedStartLocation = PickerLocationId.Desktop;
            sp.SuggestedFileName = appView.Title.Replace(".txt*", "");
            var result = await sp.PickSaveFileAsync();

            if (result != null)
            {
                quickSaveFile = result;
               
                await Windows.Storage.FileIO.WriteTextAsync(result, ResultatText.Text);
                appView.Title = result.DisplayName + result.FileType;
            }

            oneTimeChange = true;
        }


        /// <summary>
        /// Denna funktion aktiveras så fort användaren börjar skriva i textruntan.
        /// Funktionen lägger även till en asterisk i titeln som singalerar till användaren att texten har ändrats och  bool <c>oneTimeChange</c> sätts till <c>false</c>.
        /// För att förindra dubbla asterisks så har en <code>Replace("**","*");</code> implementerats.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ResultatText_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            if (oneTimeChange)
            {
                appView.Title = appView.Title + "*";
                appView.Title = appView.Title.Replace("**", "*");

                oneTimeChange = false;
            }
        }

        /// <summary>
        /// Denna funktion aktiveras så fort användaren trycker på Quick Save och sparar arbetet till den filen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void QuickSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (quickSaveFile == null)
            {
                SaveAs_btn_Click(sender, e);
            }
            else
            {
                try
                {
           
                    if (quickSaveFile != null)
                    {

                         await Windows.Storage.FileIO.WriteTextAsync(quickSaveFile, ResultatText.Text);
                        appView.Title = appView.Title.Replace("*", "");
                        oneTimeChange= true;
                    }

                }
                catch (Exception ex)
                {
                    ResultatText.Text = ("jävla fanskap: " + quickSaveFile.Path + "  " + ex.Message);
                }
                
            }

        }
    }
}
    

