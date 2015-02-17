﻿using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Linq;
using ExHentaiLib.Prop;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using TBase;

namespace ExHentaiLib.Common
{
    public static class ParseHelper
    {
        public static int GetMaxImageCount(string Str)
        {
            var mates = Regex.Matches(Str, "[0-9][0-9]{0,}");
            int returnint;
            if (int.TryParse(mates[2].Value, out returnint))
            {
                return returnint;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// To get all image information ,image page,image name,image preview source
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public async static Task<List<ImageListInfo>> GetImagePageListAsync(string uri, string cookie)
        {
            var ItemInfo = await ParseHelper.GetDetailAsync(uri, cookie);
            List<ImageListInfo> imagePageUri = ItemInfo.ImageList;
            if (ItemInfo.DetailPageCount != 1)
            {
                for (int i = 1; i < ItemInfo.DetailPageCount; i++)
                {
                    var temp = await ParseHelper.GetDetailAsync(uri + "?p=" + i, cookie);
                    imagePageUri = imagePageUri.Concat<ImageListInfo>(temp.ImageList).ToList<ImageListInfo>();
                }
            }
            return imagePageUri;
        }

        /// <summary>
        /// To get image source from a image page
        /// </summary>
        /// <param name="uri">image page</param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public async static Task<string> GetImageAync(string uri, string cookie)
        {
            string htmlstring = await HttpHelper.HttpReadStringWithCookie(uri, cookie);
            return ImagePageString2ImageString(htmlstring);
        }

        private static string ImagePageString2ImageString(string htmlstring)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlstring);
            return doc.GetElementbyId("img").Attributes["src"].Value;
        }


        public async static Task<DetailProp> GetDetailAsync(string uri, string cookie)
        {
            string htmlstring = await HttpHelper.HttpReadStringWithCookie(uri, cookie + ";" + ExHentaiLib.Properties.Resources.uconfig);
            return String2Detail(htmlstring);
        }

        private static DetailProp String2Detail(string htmlstring)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlstring);
            return HtmlNode2Detail(doc);
        }

        private static DetailProp HtmlNode2Detail(HtmlDocument htmldoc)
        {
            return new DetailProp
                         {
                             HeaderInfo = new HeaderInfo
                                          {
                                              HeaderImage = (htmldoc.GetElementbyId("gd1").Element("img").Attributes["src"].Value),
                                              TitleEn = HtmlEntity.DeEntitize(htmldoc.GetElementbyId("gn").InnerText),
                                              TitleJp = HtmlEntity.DeEntitize(htmldoc.GetElementbyId("gj").InnerText),
                                              TagInfo = (from a in htmldoc.GetElementbyId("taglist").FirstChild.ChildNodes
                                                         select new TagInfo
                                                         {
                                                             Name = (a.FirstChild.InnerText),
                                                             Value = (from b in a.LastChild.ChildNodes
                                                                      select new StringBuilder((b.InnerText)).ToString()).ToArray<string>(),
                                                         }).ToList<TagInfo>(),
                                          },
                             ImageCountString = htmldoc.DocumentNode.GetNodebyClassName("ip").InnerText,
                             DetailPageCount = htmldoc.DocumentNode.GetNodebyClassName("ptt").FirstChild.ChildNodes.Count - 2,
                             ImageList = (from a in htmldoc.GetElementbyId("gdt").ChildNodes
                                          where a.HasChildNodes
                                          select new ImageListInfo
                                          {
                                              ImageName = (a.InnerText),
                                              ImgeSrc = a.FirstChild.FirstChild.Attributes["src"].Value,
                                              ImagePage = (a.FirstChild.Attributes["href"].Value),
                                          }).ToList<ImageListInfo>(),
                         };
        }

        public async static Task<ObservableCollection<MainListProp>> GetMainListAsync(string uri, string cookie)
        {
            string htmlstring = await HttpHelper.HttpReadStringWithCookie(uri, cookie + ";" + ExHentaiLib.Properties.Resources.uconfig);
            return MainString2List(htmlstring);
        }
        private static ObservableCollection<MainListProp> MainString2List(string htmlstring)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlstring);
            var htmlnode = doc.DocumentNode.GetNodebyClassName("itg");
            return MainHtmlNode2List(htmlnode);
        }

        private static ObservableCollection<MainListProp> MainHtmlNode2List(HtmlNode htmlnode)
        {
            var result = from a in htmlnode.ChildNodes
                         where a.HasChildNodes
                         select new MainListProp
                         {
                             Title = HtmlEntity.DeEntitize(a.GetNodebyClassName("id2").InnerText),
                             ImageView = (a.GetNodebyClassName("id3").Element("a").Element("img").Attributes["src"].Value),
                             Link = (a.GetNodebyClassName("id2").Element("a").Attributes["href"].Value),
                             FliesCount = (a.GetNodebyClassName("id42").InnerText),
                         };
            return new ObservableCollection<MainListProp>(result);
        }

    }
}
