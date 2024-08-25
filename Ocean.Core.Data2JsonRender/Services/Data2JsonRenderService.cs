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
                    var prop = model.Where(m => m.ParentName == property.Name).ToList();
                    obj[property.Name] = BuildTree(dataSet.Tables[tableIndex], prop);
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
        public List<JObject> BuildTree(DataTable table, List<ModelStructure> model)
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
                    var rootNode = CreateNode(row, lookup, model);
                    result.Add(rootNode);
                }
            }

            return result;
        }
        public JObject BuildTreeFromDataTable(DataTable table, List<ModelStructure> model, string parentName)
        {
            var treeNodes = new JArray();
            var treeFields = model.Where(m => m.ParentName == parentName).ToList();
            foreach (var row in table.AsEnumerable())
            {
                var node = new JObject();

                foreach (var field in treeFields)
                {
                    string columnName = field.TableField ?? field.Name;
                    var value = row.Table.Columns.Contains(columnName) ? row[columnName] : null;
                    node[field.Name] = JToken.FromObject(value);
                }

                treeNodes.Add(node);
            }
            return new JObject
            {
                ["Nodes"] = treeNodes
            };
        }


        private JObject CreateNode(DataRow row, Dictionary<string, DataRow> lookup, List<ModelStructure> model)
        {
            var node = new JObject();

            foreach (DataColumn col in GetColumnsFromDataRow(row, model))
            {
                var _model = model.Where(x => x.TableField == col.ColumnName).FirstOrDefault();
                var _columnName = (_model != null ? _model.Name : col.ColumnName);
                node[_columnName] = JToken.FromObject(row[col]);
            }

            var children = new JArray();
            var compName = row["CompName"].ToString();

            foreach (var childRow in lookup.Values.Where(r => r["ParentName"].ToString() == compName))
            {
                var childNode = CreateNode(childRow, lookup, model);
                children.Add(childNode);
            }

            node["Nodes"] = children;
            return node;
        }
        private IEnumerable<DataColumn> GetColumnsFromDataRow(DataRow row, List<ModelStructure> model)
        {
            // LINQ sorgusu: ModelStructure'daki alanlara göre DataRow'daki DataColumn nesnelerini alın
            var columns = row.Table.Columns.Cast<DataColumn>()
                .Where(col => model.Any(m => (m.TableField ?? m.Name) == col.ColumnName));

            return columns;
        }

    }
}
