using Ocean.Core.Data2JsonRender.Interfaces;
using System.Data;
using NSubstitute;
using Ocean.Core.Data2JsonRender.Services;
using Ocean.Core.Data2JsonRender.Options;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using NSubstitute.Core;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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



        private DataSet dbDataSet()
        {

            string query = "select * from  TmlEkranTANIM where BaseID='7'; select BaseID,MasterID,CompTip,CompName,CompText,ParentName from  TmlEkranTANIM_DET where MasterID='7'";
            DataSet dataSet = new DataSet();

            using (SqlConnection connection = new SqlConnection("Data Source=172.16.0.205\\dev12;Initial Catalog=dev_SetBaseVizyon;User ID=setuser;Password=atlas71;Trusted_Connection=False;TrustServerCertificate=True;"))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataSet);
                    }
                }
            }


            return dataSet;
        }
        public List<ModelStructure> ConvertDataTableToModel(DataTable dataTable)
        {
            var models = new List<ModelStructure>();

            foreach (DataRow row in dataTable.Rows)
            {
                models.Add(new ModelStructure
                {
                    Name = row["CompName"].ToString(),
                    ParentName = row["ParentName"].ToString(),
                    FiledType = "string",
                    TableField = row["CompName"].ToString(),
                    Index = 1,
                });
            }
            return models;
        }



        [Fact]
        public async void OceanCoreData2JsonRender_DB_Data_Test()
        {
            var dataSet = dbDataSet();


            var model2 = ConvertDataTableToModel(dataSet.Tables[1]);

            var model = new List<ModelStructure>
        {
                   new ModelStructure { Name = "TmlEkranTANIM", FiledType = "Model", ParentName = "",Index = 0 },
                   new ModelStructure { Name = "EkranADI", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0, TableField="applicationCode" },
                   new ModelStructure { Name = "TabloADI", FiledType = "string", ParentName = "TmlEkranTANIM", Index = 0,TableField="EkranADI" },
                   new ModelStructure { Name = "password", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0,TableField="TabloADI" },
                   new ModelStructure { Name = "SpADI", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "SpADI"},
                   //new ModelStructure { Name = "ListeEkran_FL", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "ListeEkran_FL"},
                   //new ModelStructure { Name = "ParentModul", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "ParentModul"},
                   //new ModelStructure { Name = "SET2007", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "SET2007"},
                   //new ModelStructure { Name = "DFI_MENU", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "DFI_MENU"},
                   //new ModelStructure { Name = "AFTERSAVE", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "AFTERSAVE"},
                   //new ModelStructure { Name = "BEFOREOPEN", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "BEFOREOPEN"},
                   //new ModelStructure { Name = "SpUpdate", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "SpUpdate"},
                   //new ModelStructure { Name = "MAS_CRDATE", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "MAS_CRDATE"},
                   //new ModelStructure { Name = "MAS_UPDDATE", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "MAS_UPDDATE"},
                   //new ModelStructure { Name = "MAS_CRUSER", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "MAS_CRUSER"},
                   //new ModelStructure { Name = "MAS_USERID", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "MAS_USERID"},
                   //new ModelStructure { Name = "Rapor_Fl", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "Rapor_Fl"},
                   //new ModelStructure { Name = "Toolbar_Fl", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "Toolbar_Fl"},
                   //new ModelStructure { Name = "AFTEROPEN", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "AFTEROPEN"},
                   //new ModelStructure { Name = "BEFORESAVE", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "BEFORESAVE"},
                   //new ModelStructure { Name = "APPPLATFORM", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "APPPLATFORM"},
                   //new ModelStructure { Name = "YUKSEKLIK", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "YUKSEKLIK"},
                   //new ModelStructure { Name = "GENISLIK", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "GENISLIK"},
                   //new ModelStructure { Name = "GENEL_FL", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "GENEL_FL"},
                   //new ModelStructure { Name = "EKRANID", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "EKRANID"},
                   //new ModelStructure { Name = "EslestirmeEkrani_Fl", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "EslestirmeEkrani_Fl"},
                   //new ModelStructure { Name = "ListeToolbar_FL", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "ListeToolbar_FL"},
                   //new ModelStructure { Name = "EditorToolbar_FL", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "EditorToolbar_FL"},
                   //new ModelStructure { Name = "GizliEditorToolbar", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "GizliEditorToolbar"},
                   //new ModelStructure { Name = "GizliListeToolbar", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "GizliListeToolbar"},
                   //new ModelStructure { Name = "ModulTanimlama_FL", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "ModulTanimlama_FL"},
                   //new ModelStructure { Name = "ENotInMenu_FL", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "ENotInMenu_FL"},
                   //new ModelStructure { Name = "STATICPAGE_FL", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "STATICPAGE_FL"},
                   //new ModelStructure { Name = "PAGEPATH", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "PAGEPATH"},
                   //new ModelStructure { Name = "PRINTBUTTONCLICK", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "PRINTBUTTONCLICK"},
                   //new ModelStructure { Name = "PRINTTEMPLATEID", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "PRINTTEMPLATEID"},
                   //new ModelStructure { Name = "PRINTPARAMS", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "PRINTPARAMS"},
                   //new ModelStructure { Name = "KAYITLOADPARAMS", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "KAYITLOADPARAMS"},
                   //new ModelStructure { Name = "AFTERSAVE_Encrypted", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "AFTERSAVE_Encrypted"},
                   //new ModelStructure { Name = "BEFOREOPEN_Encrypted", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "BEFOREOPEN_Encrypted"},
                   //new ModelStructure { Name = "AFTEROPEN_Encrypted", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "AFTEROPEN_Encrypted"},
                   //new ModelStructure { Name = "BEFORESAVE_Encrypted", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "BEFORESAVE_Encrypted"},
                   //new ModelStructure { Name = "PRINTBUTTONCLICK_Encrypted", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "PRINTBUTTONCLICK_Encrypted"},
                   //new ModelStructure { Name = "LOGOLUSTUR_FL", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "LOGOLUSTUR_FL"},
                   //new ModelStructure { Name = "FILEREFERENCES", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "FILEREFERENCES"},
                   //new ModelStructure { Name = "STATUCODE", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "STATUCODE"},
                   //new ModelStructure { Name = "OPENWITHURL_FL", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "OPENWITHURL_FL"},
                   //new ModelStructure { Name = "EkranBaslikGizli_FL", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "EkranBaslikGizli_FL"},
                   //new ModelStructure { Name = "FILEREFERENCES_CSS", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "FILEREFERENCES_CSS"},
                   //new ModelStructure { Name = "MobileParentModul", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "MobileParentModul"},
                   //new ModelStructure { Name = "DesktopParentModul", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "DesktopParentModul"},
                   //new ModelStructure { Name = "STATICMODULID", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "STATICMODULID"},
                   //new ModelStructure { Name = "SCREENTHEMECODE", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "SCREENTHEMECODE"},
                   //new ModelStructure { Name = "STATICMODULIDMOB", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "STATICMODULIDMOB"},
                   //new ModelStructure { Name = "DBCode", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "DBCode"},
                   //new ModelStructure { Name = "BaseGUIDID", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "BaseGUIDID"},
                   //new ModelStructure { Name = "ParentModulGUIDID", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "ParentModulGUIDID"},
                   //new ModelStructure { Name = "MobileParentModulGUIDID", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "MobileParentModulGUIDID"},
                   //new ModelStructure { Name = "DesktopParentModulGUIDID", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "DesktopParentModulGUIDID"},
                   //new ModelStructure { Name = "STATICMODULGUIDID", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "STATICMODULGUIDID"},
                   //new ModelStructure { Name = "STATICMODULMOBGUIDID", FiledType  = "string", ParentName = "TmlEkranTANIM", Index = 0 , TableField = "STATICMODULMOBGUIDID"},



                   new ModelStructure { Name = "TmlEkranTANIM_DET", FiledType  = "Tree", ParentName = "TmlEkranTANIM",Index = 1 },
                    new ModelStructure { Name = "BaseID", FiledType  = "string",ParentName = "TmlEkranTANIM_DET", TableField = "BaseID",Index = 1 },
                    new ModelStructure { Name = "MasterID", FiledType  = "string",ParentName = "TmlEkranTANIM_DET", TableField = "MasterID",Index = 1 },
                    new ModelStructure { Name = "CompTip", FiledType  = "string", ParentName = "TmlEkranTANIM_DET", TableField = "CompTip",Index = 1 },
                    new ModelStructure { Name = "CompName", FiledType  = "string",ParentName = "TmlEkranTANIM_DET", TableField = "CompName",Index = 1 },
                    new ModelStructure { Name = "CompText", FiledType  = "string", ParentName = "TmlEkranTANIM_DET",TableField = "CompText",Index = 1 },
                    new ModelStructure { Name = "CompName", FiledType  = "Tree", ParentName = "TmlEkranTANIM_DET",Index = 1,TableField="ParentName" },

                    //new ModelStructure { Name = "CompName", FiledType  = "Tree", ParentName = "TmlEkranTANIM.",Index = 1,TableField="ParentName" },





        };

            //var TreeDeneme = BuildTree(dataSet.Tables[1]);

            var jsonResult = await _service.ConvertToDynamicJSON(dataSet, model);



        }



    }
}