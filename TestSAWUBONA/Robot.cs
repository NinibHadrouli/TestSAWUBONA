using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSAWUBONA
{
    public class Robot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Job { get; set; }
        public string StartPonit { get; set; }
        public string CurrentPonit { get; set; }


        public List<string> RaportAllPosition = new List<string>();
        public List<string> RaportUniquePosition = new List<string>();

    }
}
