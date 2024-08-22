using Ocean.Core.Data2JsonRender.Interfaces;
using System.Data;
using NSubstitute;
using Ocean.Core.Data2JsonRender.Services;
using Ocean.Core.Data2JsonRender.Options;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Ocean.Core.xUnitTest
{
    public class UnitTest1
    {
        private readonly IData2JsonRenderService _service;

        public UnitTest1()
        {
            var services = new ServiceCollection();
            services.AddTransient<IData2JsonRenderService, Data2JsonRenderService>();
            // Diğer bağımlılıklarınızı burada ekleyin.

            var serviceProvider = services.BuildServiceProvider();

            _service = serviceProvider.GetService<IData2JsonRenderService>();
        }
        private DataSet MultipleDataSet()
        {
            DataSet dataSet = new DataSet();

            DataTable table0 = new DataTable();
            table0.Columns.Add("applicationCode", typeof(string));
            table0.Columns.Add("username", typeof(string));
            table0.Columns.Add("password", typeof(string));
            table0.Columns.Add("languageCode", typeof(string));
            table0.Columns.Add("correlationId", typeof(string));
            table0.Columns.Add("timeOut", typeof(string));
            table0.Rows.Add("TPORT", "TUSER", "123456", "2", new Guid().ToString(), "120");


            DataTable table1 = new DataTable();
            table1.Columns.Add("OrderRefNr", typeof(string));
            table1.Columns.Add("VendorCode", typeof(string));
            table1.Rows.Add("PO0000504", "BB201707154");

            DataTable table2 = new DataTable();
            table2.Columns.Add("odItemTreeID", typeof(string));
            table2.Columns.Add("odPurchaseGroupCode", typeof(string));
            table2.Columns.Add("odItemCategory", typeof(int));
            table2.Rows.Add("01.004.100003", "01", 100003);
            table2.Rows.Add("01.004.100011", "01", 100011);
            table2.Rows.Add("08.010.100008", "08", 100008);

            dataSet.Tables.Add(table0);
            dataSet.Tables.Add(table1);
            dataSet.Tables.Add(table2);
            return dataSet;
        }

        private DataSet SingleDataSet()
        {
            DataSet dataSet = new DataSet();
            DataTable table = new DataTable();
            table.Columns.Add("OrderRefNr", typeof(string));
            table.Columns.Add("VendorCode", typeof(string));
            table.Columns.Add("odItemTreeID", typeof(string));
            table.Columns.Add("odPurchaseGroupCode", typeof(string));
            table.Columns.Add("odItemCategory", typeof(int));

            // Örnek verileri ekliyoruz.
            table.Rows.Add("PO0000504", "BB201707154", "01.004.100003", "01", 100003);
            table.Rows.Add("PO0000504", "BB201707154", "01.004.100011", "01", 100011);
            table.Rows.Add("PO0000504", "BB201707154", "08.010.100008", "08", 100008);

            dataSet.Tables.Add(table);
            return dataSet;
        }


        [Fact]
        public async void OceanCoreData2JsonRender_Multiple_Data_Test()
        {

            // Arrange

            var dataSet = MultipleDataSet();

            // Sample ModelStructure
            var model = new List<ModelStructure>
        {
                   new ModelStructure { Name = "requestHeader", FiledType = "Model", ParentName = "",Index = 0 },
                   new ModelStructure { Name = "applicationCode", FiledType  = "string", ParentName = "requestHeader", Index = 0, TableField="applicationCode" },
                   new ModelStructure { Name = "username", FiledType = "string", ParentName = "requestHeader", Index = 0 },
                   new ModelStructure { Name = "password", FiledType  = "string", ParentName = "requestHeader", Index = 0 },
                   new ModelStructure { Name = "languageCode", FiledType  = "string", ParentName = "requestHeader", Index = 0 },
                   new ModelStructure { Name = "correlationId", FiledType  = "string", ParentName = "requestHeader", Index = 0 },
                   new ModelStructure { Name = "timeOut", FiledType  = "string", ParentName = "requestHeader", Index = 0 },


                  new ModelStructure { Name = "H_ProcOrder", FiledType  = "Model", ParentName = "",Index = 1 },
                  new ModelStructure { Name = "OrderRefNr", FiledType  = "string", ParentName = "H_ProcOrder", Index = 1, TableField="OrderRefNr" },
                  new ModelStructure { Name = "VendorCode", FiledType  = "string", ParentName = "H_ProcOrder", Index = 1, TableField="VendorCode" },

                  new ModelStructure { Name = "H_ProcOrderDet", FiledType  = "Array", ParentName = "H_ProcOrder", Index = 2 },
                  new ModelStructure { Name = "odItemTreeID", FiledType  = "string", ParentName = "H_ProcOrderDet", Index = 1, TableField="odItemTreeID" },
                  new ModelStructure { Name = "odPurchaseGroupCode", FiledType  = "string", ParentName = "H_ProcOrderDet", Index = 2,TableField="odPurchaseGroupCode" },
                  new ModelStructure { Name = "odItemCategory", FiledType  = "int", ParentName = "H_ProcOrderDet", Index = 2, TableField="odItemCategory"},
        };



            var jsonResult = await _service.ConvertToDynamicJSON(dataSet, model);


        }
        [Fact]
        public async void OceanCoreData2JsonRender_Single_Data_Test()
        {
            var dataSet = SingleDataSet();

            var model = new List<ModelStructure>
              {

                  new ModelStructure { Name = "H_ProcOrder", FiledType  = "Model", ParentName = "" },
                  new ModelStructure { Name = "OrderRefNr", FiledType = "string", ParentName = "H_ProcOrder", TableField="OrderRefNr"  },
                  new ModelStructure { Name = "VendorCode", FiledType  = "string", ParentName = "H_ProcOrder",TableField="VendorCode" },
                  new ModelStructure { Name = "H_ProcOrderDet", FiledType = "Array", ParentName = "H_ProcOrder" },
                  new ModelStructure { Name = "odItemTreeID", FiledType  = "string", ParentName = "H_ProcOrderDet", TableField="odItemTreeID" },
                  new ModelStructure { Name = "odPurchaseGroupCode", FiledType  = "string", ParentName = "H_ProcOrderDet",TableField="odPurchaseGroupCode" },
                  new ModelStructure { Name = "odItemCategory", FiledType  = "int", ParentName = "H_ProcOrderDet",TableField="odItemCategory" }
              };




            var jsonResult = await _service.ConvertToDynamicJSON(dataSet, model);
        }
    }
}