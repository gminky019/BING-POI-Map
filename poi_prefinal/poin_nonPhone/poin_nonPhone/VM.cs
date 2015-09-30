using Bing.Maps;
using NavteqPoiSchema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace poin_nonPhone
{
    /// <summary>
    /// Please note most functions are async as to not hold up main UI thread.
    /// 
    /// This is the view model class, used ot seperate Code behind from business logic
    /// </summary>
    public class VM
    {
        //class members
        public MainPage view;
        public Map maper;
        public MapLayer mLay;
        public string key;
        public double lat;
        public double lon;
        public string searchI;
        public double _rad;
        public bool byPoint;
        public Search _searchObj;
        public List<string> fileText;

        /// <summary>
        /// base constructor
        /// </summary>
        /// <param name="v"></param>
        public VM(MainPage v)
        {
            view = v;
            byPoint = true;
            _searchObj = new Search();
            _rad = view._changedRadius;
        }

        /// <summary>
        /// This method is the intermediate layer for a poi search
        /// It determines which search to use an instantiates that object
        /// </summary>
        /// <param name="place">bool describing whether its a place search or not</param>
        /// <param name="location">the location given in the APP</param>
        public async void search(bool place, MyLocation location)
        {
            try
            {
                if (place)
                {
                    //this is the place search
                    IREST placeSearch = new PlaceSearch(location, _searchObj);
                    await placeSearch.performSearch();
                    fileText = _searchObj.searchOutput(placeSearch._poiResponse);
                }
                else
                {
                    //this is for all other searches
                    IREST locSearch = new LocationSearch(location, _searchObj);
                    await locSearch.performSearch();
                    fileText = _searchObj.searchOutput(locSearch._poiResponse);
                }
            }
            catch (Exception e)
            {
                var messageDialog = new MessageDialog(e.Message);

                messageDialog.ShowAsync();

            }
        }


        /// <summary>
        /// Microsoft FILE picker, NOTE from MSFT code snippet to choose file
        /// Added in own file writer, to write custom info to the file chosen
        /// </summary>
        public async void SaveFile()
        {

            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Document";
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file
                try
                {
                    await FileIO.WriteLinesAsync(file, fileText);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    // File was saved
                }
                else
                {
                    // File was not saved
                }
            }
            else
            {
                // File Operation cancelled
            }
        }
    }
}
