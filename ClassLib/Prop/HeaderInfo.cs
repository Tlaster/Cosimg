using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExHentaiLib.Prop
{
    public class HeaderInfo
    {
        public string HeaderImage { get; set; }
        public string TitleEn { get; set; }
        public string TitleJp { get; set; }
        public List<TagInfo> TagInfo { get; set; }
    }
}
