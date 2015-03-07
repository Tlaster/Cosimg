using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosImg.CosImg.Model
{
    public class SelectionModel
    {
        public string Name { get; set; }

        public string Link { get; set; }

        public Func<Windows.Data.Json.IJsonValue, ListModel> Generator { get; set; }
    }
}
