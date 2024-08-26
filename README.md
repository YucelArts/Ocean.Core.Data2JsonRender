Data2JsonRender:How to Dynamically Transform SQL Data into Rich JSON Formats


## JSON Result

### Single Table Query 

#### A DataSet with Single Tables 

```c#
        private DataSet SingleDataSet()
        {
            DataSet dataSet = new DataSet();
            DataTable table = new DataTable();
            table.Columns.Add("OrderRefNr", typeof(string));
            table.Columns.Add("VendorCode", typeof(string));
            table.Columns.Add("odItemTreeID", typeof(string));
            table.Columns.Add("odPurchaseGroupCode", typeof(string));
            table.Columns.Add("odItemCategory", typeof(int));

            // Sample Data Add
            table.Rows.Add("PO0000504", "BB201707154", "01.004.100003", "01", 100003);
            table.Rows.Add("PO0000504", "BB201707154", "01.004.100011", "01", 100011);
            table.Rows.Add("PO0000504", "BB201707154", "08.010.100008", "08", 100008);

            dataSet.Tables.Add(table);
            return dataSet;
        }
```

Model Definition Example

| Name                | FieldType | ParentName     | TableField          | Index | DefaultValue |
|---------------------|-----------|----------------|---------------------|-------|--------------|
| H_ProcOrder         | Model     |                |                     |       |              |
| OrderRefNr          | string    | H_ProcOrder    | OrderRefNr          |       |              |
| VendorCode          | string    | H_ProcOrder    | VendorCode          |       |              |
| H_ProcOrderDet      | Array     | H_ProcOrder    |                     |       |              |
| odItemTreeID        | string    | H_ProcOrderDet | odItemTreeID        |       |              |
| odPurchaseGroupCode | string    | H_ProcOrderDet | odPurchaseGroupCode |       |              |
| odItemCategory      | int       | H_ProcOrderDet | odItemCategory      |       |              |

###

Below you can see how to convert data in a Single DataTable to Rich Json type.

```json
{
  "H_ProcOrder": { // MODEL
    "OrderRefNr": "PO0000504", // String
    "VendorCode": "BB201707154", // String
    "H_ProcOrderDet": [  //ARRAY
      {
        "odItemTreeID": "01.004.100003", // String
        "odPurchaseGroupCode": "01",  // String
        "odItemCategory": 100003  // Int
      },
      {
        "odItemTreeID": "01.004.100011", // String
        "odPurchaseGroupCode": "01", // String
        "odItemCategory": 100011 -- Int
      },
      {
        "odItemTreeID": "08.010.100008",  // String
        "odPurchaseGroupCode": "08",  // String
        "odItemCategory": 100008  // Int
      }
    ]
  }
}

```

### Multiple Table Query 


![image](https://github.com/user-attachments/assets/929a1545-799a-4f0e-a808-24c049dc7721)

#### A DataSet with Multiple Tables 

Let's complicate the example a bit.
First, let's have a Model and define propertyKeys in it.
Second, let's have a Model again and have an Array in it.
Inside the second model, let's have an Array that can pull data from a third DataTable.



```c#

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

```

```json

{
  "requestHeader": { // DataTable 0
    "applicationCode": "TPORT",
    "username": null,
    "password": null,
    "languageCode": null,
    "correlationId": null,
    "timeOut": null
  },
  "H_ProcOrder": {  // DataTable 1
    "OrderRefNr": "PO0000504",
    "VendorCode": "BB201707154",
    "H_ProcOrderDet": [  // DataTable 2
      {
        "odItemTreeID": "01.004.100003",
        "odPurchaseGroupCode": "01",
        "odItemCategory": 100003
      },
      {
        "odItemTreeID": "01.004.100011",
        "odPurchaseGroupCode": "01",
        "odItemCategory": 100011
      },
      {
        "odItemTreeID": "08.010.100008",
        "odPurchaseGroupCode": "08",
        "odItemCategory": 100008
      }
    ]
  }
}
```
https://yucelarts.medium.com/how-to-dynamically-transform-sql-data-into-rich-json-formats-cb435ae0b8cd
