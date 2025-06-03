# Dynamic Forms
The Dynamic Forms project was developed as an experimental framework for building forms based on JSON configuration, and manipulating data without a fixed object structure.

Configuration includes the type and placement of input controls, as well as various properties controlling the specific attributes of a given input control type.  

For example, a numeric textbox control has configuration properties controlling the maximum number of integer and decimal digits, and whether it accepts negative values, while a regular textbox control has properties controlling the maximum text length, and whether it is a multi-line textbox.

Data is serialized to JSON, and deserialized to an ExpandoObject, which allows for dynamic structures.

Next Steps:
1) Implement the Formula Engine project.  This will involve assigning a mathematical formula to a set of controls, capturing data changes within these controls, and processing the data through the Formula Engine's calculations.  It will also involve converting to a consistent unit set before serializing to JSON, and converting to user-selected units when deserializing.
2) Implement this functionality in other technologies, including web and mobile.
3) Implement a UI for generating the configuration JSON.


A sample configuration is below:
{
  "ConfigId": 1,
  "CustId": 17,
  "FormType": 1,
  "Title": "Transportation Quote",
  "PublishedDate": "2023-12-11T17:13:05.1757926-07:00",
  "PublishedByUserId": 23,
  "Sections": [
    {
      "SectionName": "Header",
      "SectionHeader": "",
      "IsItemsSection": false,
      "DisplaySectionHeader": false,
      "DisplayOrder": 0,
      "ColumnCount": 3,
      "RowCount": 3,
      "Controls": [
        {
          "Name": "Customer",
          "Label": "Customer",
          "ControlType": {
            "DataType": "Text",
            "TextControlConfig": {
              "MaxLength": 100,
              "IsMultiLine": false
            }
          },
          "IsReadOnly": false,
          "IsRequired": true,
          "Position": {
            "Row": 0,
            "Column": 0,
            "RowSpan": 0,
            "ColumnSpan": 2,
            "HorizontalAlignment": "Stretch",
            "VerticalAlignment": "Center"
          }
        },
        {
          "Name": "PriorityShipping",
          "Label": "Priority Shipping",
          "ControlType": {
            "DataType": "CheckBox"
          },
          "IsReadOnly": false,
          "IsRequired": false,
          "Position": {
            "Row": 1,
            "Column": 0,
            "RowSpan": 0,
            "ColumnSpan": 1,
            "HorizontalAlignment": "Left",
            "VerticalAlignment": "Center"
          }
        },
        {
          "Name": "ScheduledDate",
          "Label": "Scheduled",
          "ControlType": {
            "DataType": "Date",
            "DateControlConfig": {
              "DateOnly": false,
              "HasButton": true,
              "HasDropDown": true
            }
          },
          "IsReadOnly": false,
          "IsRequired": true,
          "Position": {
            "Row": 0,
            "Column": 2,
            "RowSpan": 0,
            "ColumnSpan": 1,
            "HorizontalAlignment": "Left",
            "VerticalAlignment": "Center"
          }
        },
        ...
        }
      ]
    },
    ...
    {
      "SectionName": "ShippingItemDetails",
      "SectionHeader": "Shipping Item Details",
      "IsItemsSection": true,
      "DisplaySectionHeader": true,
      "DisplayOrder": 20,
      "ColumnCount": 1,
      "RowCount": 1,
      "Controls": [
        {
          "Name": "grdItems",
          "ControlType": {
            "DataType": "DetailsGrid",
            "DetailGridConfig": {
              "Columns": [
                {
                  "Name": "ItemId",
                  "Label": "Item ID",
                  "ControlType": {
                    "DataType": "Text",
                    "TextControlConfig": {
                      "MaxLength": 10,
                      "IsMultiLine": false
                    }
                  },
                  "IsReadOnly": false,
                  "IsRequired": true,
                  "TabIndex": 0
                },
                {
                  "Name": "Description",
                  "Label": "Description",
                  "ControlType": {
                    "DataType": "Text",
                    "TextControlConfig": {
                      "MaxLength": 100,
                      "IsMultiLine": false
                    }
                  },
                  "IsReadOnly": false,
                  "IsRequired": true,
                  "TabIndex": 1
                },
                {
                  "Name": "Weight",
                  "Label": "Weight (kg)",
                  "ControlType": {
                    "DataType": "Numeric",
                    "NumericControlConfig": {
                      "MaxDecimalDigits": 3,
                      "AllowNegative": false
                    }
                  },
                  "IsReadOnly": false,
                  "IsRequired": true,
                  "TabIndex": 2
                },
                {
                  "Name": "UnitPrice",
                  "Label": "Unit Price ($/kg)",
                  "ControlType": {
                    "DataType": "Numeric",
                    "NumericControlConfig": {
                      "MaxDecimalDigits": 3,
                      "AllowNegative": false
                    }
                  },
                  "IsReadOnly": false,
                  "IsRequired": true,
                  "TabIndex": 3
                },
                {
                  "Name": "Price",
                  "Label": "Price ($)",
                  "ControlType": {
                    "DataType": "Numeric",
                    "NumericControlConfig": {
                      "MaxDecimalDigits": 2,
                      "AllowNegative": false
                    }
                  },
                  "IsReadOnly": true,
                  "IsRequired": true,
                  "TabIndex": 4
                }
              ]
            }
          },
          "IsReadOnly": false,
          "IsRequired": true,
          "Position": {
            "Row": 0,
            "Column": 0,
            "RowSpan": 0,
            "ColumnSpan": 1,
            "HorizontalAlignment": "Stretch",
            "VerticalAlignment": "Stretch"
          }
        }
      ],
      "MathematicalFormula": "VAR:Weight * VAR:UnitPrice"
    }
  ]
}
