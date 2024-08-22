using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocean.Core.Data2JsonRender.Options
{
    public class ModelStructure
    {
        public string Name { get; set; }
        public string FiledType { get; set; }
        public string ParentName { get; set; }
        public int? Index { get; set; } // Tablo indexi (nullable)
        public string DefaultValue { get; set; }
        public string TableField { get; set; } = null;
    }
}
