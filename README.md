# Dynamic Forms
The Dynamic Forms project was developed as an experimental framework for building forms based on JSON configuration, and manipulating data without a fixed object structure.

Configuration includes the type and placement of input controls, as well as various properties controlling the specific attributes of a given input control type.  

For example, a numeric textbox control has configuration properties controlling the maximum number of integer and decimal digits, and whether it accepts negative values, while a regular textbox control has properties controlling the maximum text length, and whether it is a multi-line textbox.

Data is serialized to JSON, and deserialized to an ExpandoObject, which allows for dynamic structures.

Next Steps:
1) Implement the Formula Engine project.  This will involve assigning a mathematical formula to a set of controls, capturing data changes within these controls, and processing the data through the Formula Engine's calculations.  It will also involve converting to a consistent unit set before serializing to JSON, and converting to user-selected units when deserializing.
2) Implement this functionality in other technologies, including web and mobile.
3) Implement a UI for generating the configuration JSON.
