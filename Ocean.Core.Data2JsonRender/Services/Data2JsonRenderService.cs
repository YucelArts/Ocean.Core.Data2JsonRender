using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                else if (property.FiledType == "Tree")
                {
                    obj[property.Name] = BuildTree(dataSet.Tables[tableIndex]);
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
        public List<JObject> BuildTree(DataTable table)
        {
            var lookup = table.AsEnumerable().ToDictionary(
                row => row["CompName"].ToString(),
                row => row
            );

            var result = new List<JObject>();

            foreach (var row in table.AsEnumerable())
            {
                var parentName = row["ParentName"].ToString();

                // ParentName herhangi bir CompName ile eşleşmiyorsa, bu en üst düğümdür
                if (!lookup.ContainsKey(parentName))
                {
                    var rootNode = CreateNode(row, lookup);
                    result.Add(rootNode);
                }
            }

            return result;
        }

        private JObject CreateNode(DataRow row, Dictionary<string, DataRow> lookup)
        {
            var node = new JObject();

            foreach (DataColumn col in row.Table.Columns)
            {
                node[col.ColumnName] = JToken.FromObject(row[col]);
            }

            var children = new JArray();
            var compName = row["CompName"].ToString();

            foreach (var childRow in lookup.Values.Where(r => r["ParentName"].ToString() == compName))
            {
                var childNode = CreateNode(childRow, lookup);
                children.Add(childNode);
            }

            node["Nodes"] = children;
            return node;
        }
    }
}
