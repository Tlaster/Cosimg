﻿using CosImg.CosImg.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBase;
using TBase.RT;
using Windows.Data.Json;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace CosImg.CosImg.Model
{
    public class SearchModel<T> : BaseViewModel<T>
    {
        private Func<IJsonValue, T> _generator;

        public SearchModel(Func<IJsonValue, T> generator)
        {
            //Link = link;
            _generator = generator;
        }

        public System.Windows.Input.ICommand SearchCommand
        {
            get
            {
                return new DelegateCommand<string>(async (str) =>
                {
#if DEBUG
                    if (str == "go")
#else
                    if (str == "I AM HENTAI")
#endif
                    {
#if DEBUG
                        try
                        {
                            SettingHelpers.GetSetting<string>("cookie", true);
                            App.rootFrame.Navigate(typeof(ExHentai.View.ExMainPage));
                        }
                        catch (SettingException)
                        {
                            App.rootFrame.Navigate(typeof(ExHentai.View.LoginPage));
                        }
#else
                        MessageDialog dialog = new MessageDialog("Sure?");
                        dialog.Commands.Add(new UICommand("Yes", (action) =>
                        {
                            try
                            {
                                SettingHelpers.GetSetting<string>("cookie", true);
                                App.rootFrame.Navigate(typeof(ExHentai.View.ExMainPage));
                            }
                            catch (SettingException)
                            {
                                App.rootFrame.Navigate(typeof(ExHentai.View.LoginPage));
                            }
                        }));
                        dialog.Commands.Add(new UICommand("No"));
                        await dialog.ShowAsync();
#endif
                    }

                    Link = "http://worldcosplay.net/api/photo/search?q=" + str;
                    OnLoaded();
                });
            }
        }

        public void OnLoaded()
        {
            List = new GeneratorIncrementalLoadingClass<T>(Link, _generator);
            List.LoadFailed += (s, e) => { isLoadFail = true; isOnLoading = false; };
            List.OnLoading += (s, e) => { isOnLoading = true; };
            List.LoadSucceed += (s, e) => { isOnLoading = false; };
        }

        public override System.Windows.Input.ICommand ReTryCommand
        {
            get { throw new NotImplementedException(); }
        }

        public override System.Windows.Input.ICommand ItemTapped
        {
            get
            {
                return new DelegateCommand<ItemClickEventArgs>((item) =>
                {
                    App.rootFrame.Navigate(typeof(CosImg.View.ImagePage), item.ClickedItem as ListModel);
                });
            }
        }
    }
}
