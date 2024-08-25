Data2JsonRender:How to Dynamically Transform SQL Data into Rich JSON Formats


## JSON Result

### Simple Query 

Model Definition Example
![image](https://github.com/user-attachments/assets/3c63a4a3-2e4e-450f-bcc5-4d6211937f37)

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

![image](https://github.com/user-attachments/assets/929a1545-799a-4f0e-a808-24c049dc7721)


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
