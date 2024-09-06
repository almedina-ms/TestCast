using static Windows.Win32.PInvoke;
using WinRT;
using System.Reflection.Metadata;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;

namespace TestClassLibrary
{
    public class MyClass
    {
        public static void IterateThroughApps()
        {
            var appsFolderClsid = FOLDERID_AppsFolder;
            var iShellItemIID = typeof(IShellItem).GUID;
            using ComPtr<IShellItem> ppv = default;
            HRESULT hresult;

            unsafe
            {
                hresult = SHGetKnownFolderItem(&appsFolderClsid, KNOWN_FOLDER_FLAG.KF_FLAG_DONT_VERIFY, HANDLE.Null, &iShellItemIID, (void**)ppv.GetAddressOf());
            }

            if (hresult.Failed)
            {
                return;
            }

            var iEnumShellItemsIID = typeof(IEnumShellItems).GUID;
            using ComPtr<IEnumShellItems> enumItems = default;
            var enumItemsHandler = BHID_EnumItems;

            unsafe
            {
                var appsFolder = ppv.Get();
                appsFolder->BindToHandler(null, &enumItemsHandler, &iEnumShellItemsIID, (void**)enumItems.GetAddressOf());

                var app = new IShellItem[1];
                fixed (IShellItem* appPtr = app)
                {
                    while (true)
                    {
                        if (!(enumItems.Get()->Next(1, &appPtr, null).Succeeded))
                        {
                            break;
                        }

                        var shellItem2 = app[0].As<IShellItem2>();

                        PWSTR pszName = new();
                        shellItem2.GetDisplayName(SIGDN.SIGDN_NORMALDISPLAY, &pszName);

                        Console.WriteLine(pszName);
                    }
                }
            }
        }
    }
}
