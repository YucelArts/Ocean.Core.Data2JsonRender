using Ocean.Core.Data2JsonRender.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocean.Core.Data2JsonRender.Interfaces
{
    public interface IData2JsonRenderService
    {
        Task<string> ConvertToDynamicJSON(DataSet dataSet, List<ModelStructure> model);
    }
}
