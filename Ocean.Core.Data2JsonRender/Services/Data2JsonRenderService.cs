using Newtonsoft.Json;
using Ocean.Core.Data2JsonRender.Interfaces;
using Ocean.Core.Data2JsonRender.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocean.Core.Data2JsonRender.Services
{
    public class Data2JsonRenderService : IData2JsonRenderService
    {
        public async Task<string> ConvertToDynamicJSON(DataSet dataSet, List<ModelStructure> model)
        {
            var rootObjects = model.Where(m => m.FiledType == "Model" && string.IsNullOrEmpty(m.ParentName)).ToList();
            var result = new Dictionary<string, object>();

            foreach (var root in rootObjects)
            {
                var obj = new Dictionary<string, object>();
                BuildJsonObject(obj, model, root.Name, dataSet);
                result[root.Name] = obj;
            }

            return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
        }
        private void BuildJsonObject(Dictionary<string, object> obj, List<ModelStructure> model, string parentName, DataSet dataSet)
        {
            var properties = model.Where(m => m.ParentName == parentName).ToList();

            foreach (var property in properties)
            {
                string columnName = property.TableField ?? property.Name;
                int tableIndex = property.Index ?? 0;
                DataTable table = dataSet.Tables[tableIndex];

                if (property.FiledType == "Model")
                {
                    var childObj = new Dictionary<string, object>();
                    obj[property.Name] = childObj;
                    BuildJsonObject(childObj, model, property.Name, dataSet);
                }
                else if (property.FiledType == "Array")
                {
                    var arrayItems = table.AsEnumerable().Select(row =>
                    {
                        var itemObj = new Dictionary<string, object>();
                        BuildJsonObject(itemObj, model, property.Name, row);
                        return itemObj;
                    }).ToList();

                    obj[property.Name] = arrayItems;
                }
                else
                {
                    // TableField null ise, JSON anahtar değeri de null olur
                    var value = property.TableField != null && table.Columns.Contains(columnName) ? table.Rows[0][columnName] : null;
                    obj[property.Name] = property.FiledType == "int" ? Convert.ToInt32(value ?? 0) : value;
                }
            }
        }
        private void BuildJsonObject(Dictionary<string, object> obj, List<ModelStructure> model, string parentName, DataRow row)
        {
            var properties = model.Where(m => m.ParentName == parentName).ToList();

            foreach (var property in properties)
            {
                string columnName = property.TableField ?? property.Name;
                var value = property.TableField != null && row.Table.Columns.Contains(columnName) ? row[columnName] : null;
                obj[property.Name] = property.FiledType == "int" ? Convert.ToInt32(value ?? 0) : value;
            }
        }
    }
}
