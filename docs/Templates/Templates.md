# KMD Logic Document Generation Templates

## Introduction

KMD Logic Document Generation uses MS Word files (`.docx` files) as templates to generate documents from.  


## Template Syntax

KMD Logic Document Generation utilizes ASPOSE as the underlying library to generate documents.  
Because of this, it supports out of the box all the syntax constructs available in ASPOSE Reporting Engine.  
A comprehensive guide about this can be found in the following link:  [Template Syntax - Aspose.Words for .NET - Documentation](https://docs.aspose.com/display/wordsnet/Template+Syntax).  


## KMD Logic Document Generation Template Helpers

Additionally to the built-in functionality provided by ASPOSE, KMD Logic Document Generation provides functionality to help template authors to create documents with ease, specially focussing on retrieving resources that are located in storage providers from a KMD Logic Document Generation Configuration entries hierarchy.  

To learn more about KMD Logic Document Generation Configuration, please refer to the following [guide](../Configuration/Configuration.md).

Find below a list of helpers that KMD Logic Document Generation Template supports:

### Template helper

Use the `template` helper to load external `.docx` files (e.g. signatures, disclaimers, legal notes, etc.) stored in your storage provider or higher up the hierarchy.  

#### Examples

```
<<doc [template.Load(“word-partial-footer.docx”)]>>
```

#### Note

For the ASPOSE documentation about inserting documents dynamically please follow [this](https://docs.aspose.com/display/wordsnet/Template+Syntax#TemplateSyntax-InsertingDocumentsDynamically) guide.


### Image helper

Use the `image` helper to load external graphic files or images stored in your storage provider or higher up the hierarchy. 


#### Example - Loading image from external url

```
<<image ["http://www.example.com/image.jpg"] >>
```


#### Example - Loading images from your storage provider or higher up the hierarchy

Use `image.Load`, e.g.:

```
<<image [image.Load("image.jpg")] >>
```

#### Example - Loading image from bas64 image data encoded in model property  

Use `image.Decode`, e.g.:

```
<<image [image.Decode(SignatureImage)] >>
```

#### Note  

For the ASPOSE documentation about inserting images dynamically please follow [this](https://docs.aspose.com/display/wordsnet/Template+Syntax#TemplateSyntax-InsertingImagesDynamically) guide.


### Number and Date/Time helper

Use the `number.format` helper to format `numeric`, `currency` or `percentage` values in your templates.   

Use the `date.format` to format date/time values in your templates.  

`number.format` and `date.format` provide additional functionality from the regular formatting helpers in ASPOSE, mostly focused on customizing the localization culture used to format its values.    


#### Examples

- Numeric

    - No (no format):  
    `<<[No]>>`  

    - No (as Percent (`P`) Format Specifier):  
    `<<[No]:”P”>>`  

    - No (as Numeric (`N`) Format Specifier): 
    `<<[No]:”N”>>`  

    - No (as Numeric (`N`) Format Specifier `FR` culture):  
    `<<[number.Format(No, “N”, “fr”)]>>`  

    - No (as Numeric (`N`) Format Specifier `EN-GB` culture):  
    `<<[number.Format(No, “N”, “en-GB”)]>>`  

    - No (as Fixed-Point (`F`) Format Specifier):  
    `<<[No]:”F”>>`  

    - No (as Fixed-Point (`F`) `FR` culture):  
    `<<[number.Format(No, “F”, “fr”)]>>`  

    - No (as Fixed-Point (`F`) `EN-GB` culture):  
    `<<[number.Format(No, “F”, “en-GB”)]>>`  

- Currency  

    - Price (no format):  
    `<<[Price]>>`  

    - Price (as Numeric (`N`) Format Specifier):  
    `<<[Price]:”N”>>`  

    - Price (as Currency (`C`) Format Specifier):  
    `<<[Price]:”C”>>`  

    - Price (as Currency (`C3`) Format Specifier):  
    `<<[Price]:”C3”>>`  

    - Price (as Currency (`C`) Format Specifier `FR` culture):  
    `<<[number.Format(Price, “C”, “fr”)]>>`  

    - Price (as Currency (`C`) Format Specifier `EN-GB` culture):  
    `<<[number.Format(Price, “C”, “en-GB”)]>>`  

    - Price (as Fixed-Point (`F`) Format Specifier):  
    `<<[Price]:”F”>>`  

    - Price (as Fixed-Point (`F1`) Format Specifier):  
    `<<[Price]:”F1”>>`  

    - Price (as Digit placeholder):  
    `<<[Price]:”#.##”>>`  

- Percentage

    - Percent (as Percent (`P`) Format Specifier):  
    `<<[Percent]:”P”>>`  

    - Percent (as Fixed-Point (`F`) Format Specifier):  
    `<<[Percent]:”F”>>`  

    - Percent (as Digit placeholder):  
    `<<[Percent]:”#.##”>>`  

    - Percent (as Percentage placeholder):  
    `<<[Percent]:”%#0.00”>>` 


- Date/time

    - DateOfBirth (format `”dd.MM.yyyy”`):  
    `<<[DateOfBirth]:”dd.MM.yyyy”>>`  

    - DateOfBirth (format `”MMMM dd”`):  
    `<<[DateOfBirth]:”MMMM dd”>>`  

    - DateOfBirth (format `”MMMM dd” fr`):   
    `<<[date.Format(DateOfBirth, ”MMMM dd”, “fr”)]>>`  

    - DateOfBirth (format `”MMMM dd” en-GB`):  
    `<<[date.Format(DateOfBirth, ”MMMM dd”, “en-GB”)]>>`  
    
    - Appointment (format `“dd.MMMM.yyyy HH:mm”`): 
    `<<[Appointment]:”dd.MMMM.yyyy HH:mm“>>`  

    - Appointment (format `“dddd, dd MMMM yyyy HH:mm:ss”`): 
    `<<[Appointment]:”dddd, dd MMMM yyyy HH:mm:ss“>>`  

    - Appointment (format `“dddd, dd MMMM yyyy HH:mm:ss” fr`): 
    `<<[date.Format(Appointment, ”dddd, dd MMMM yyyy HH:mm:ss“, “fr”)]>>`  

    - Appointment (format `“dddd, dd MMMM yyyy HH:mm:ss” en-GB`): 
    `<<[date.Format(Appointment, ”dddd, dd MMMM yyyy HH:mm:ss“, “en-GB”)]>>`  

#### Notes

For the ASPOSE documentation about outputting expression results and their formatting please follow [this](https://docs.aspose.com/display/wordsnet/Template+Syntax#TemplateSyntax-OutputtingExpressionResults) guide.


## Templates and document generation

Logic Document Generation uses `.docx` templates and create documents out of them.  

There are a few conventions that are followed when resolving a template from a configuration entry point of view (to know more about the configuration model visit the [Configuration](../Configuration/Configuration.md) documentation).  

Logic Document Generation uses a hierarchical configuration model where each node is able to override a template from its parent node.  

This design allows a master storage to define a set of generic templates for their customers to use, while allowing their customers to customize/extend all or some of them.   


### How to override a Master template  

#### Using SharePoint

- Create a `.docx` file in your SharePoint Group.
- Name it as the document you want to override and update its content as needed.  


#### Using Azure Blob Storage

- Upload a `.docx` file to your Azure Blob Storage.
- Name it as the document you want to override and update its content as needed.  
- Make sure you upload the file to the declared container and directory (`Blob prefix`) in Console for your configuration entry.  


### How to hide a template from the list of available templates for a given entry

#### Using SharePoint

- Create an empty `.docx` file in your SharePoint Group.
- Name it as the document you want to hide. 
- For instance, if you want to hide `Template.docx`, the name of the document in your local SharePoint Group should be `Template.hidden.docx`


#### Using Azure Blob Storage

- Upload a `.docx` file to your Azure Blob Storage account.
- Name it as the document you want to hide.  
- Make sure you upload the file to the declared container and directory (`Blob prefix`) in Console for your configuration entry.  
- Attach a new metadata record to the document with key `Hidden`.  
- The actual value of the `Hidden` metadata entry doesn't matter as long as the key is equal to `Hidden`.  

You can set metadata for blobs using one of the following methods:

- The Storage Explorer in the Azure Portal ([link](https://azure.microsoft.com/en-au/updates/storage-explorer-preview-now-available-in-azure-portal/)).

- The Storage Explorer client ([link](https://azure.microsoft.com/en-us/features/storage-explorer/)).

- Programmatically using the Azure Storage client library for .NET ([link](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-properties-metadata)).  

- Using the Azure Blob REST API ([link](https://docs.microsoft.com/en-us/rest/api/storageservices/set-blob-metadata)).  

### How to tag document templates for searching using Logic Document Generation API

The Logic Document Generation API allows you to list all templates available for a given configuration entry.  

```csharp
var templates = await documentGenerationClient.GetTemplates(subscriptionId, configurationId, hierarchyPath, null).ConfigureAwait(false);
```


It also allows you to pass in a `subject` parameter to retrieve only those documents that match that filter.  

Below we retrieve all the templates that have a subject equal to `invoice`:  

```csharp
var templates = await documentGenerationClient.GetTemplates(subscriptionId, configurationId, hierarchyPath, "invoice").ConfigureAwait(false);
```

#### Configuring template subjects in SharePoint

The SharePoint storage provider for Logic Document Generation relies on the document tags to filter out templates by `subject`.   

Locate the `.docx` template file in your SharePoint instance and tag them accordingly (e.g. add the tag `invoice` as per our example above).  


#### Configuration template subjects in Azure Blob Storage  

The Azure Blob Storage provider from Logic Document Generation relies on the document metadata to filter out templates by `subject`.  

When filtering by `subject`, only the metadata key will be used as matching criteria, the actual value is not considered.  

You can set metadata for blobs using one of the following methods:

- The Storage Explorer in the Azure Portal ([link](https://azure.microsoft.com/en-au/updates/storage-explorer-preview-now-available-in-azure-portal/)).

- The Storage Explorer client ([link](https://azure.microsoft.com/en-us/features/storage-explorer/)).

- Programmatically using the Azure Storage client library for .NET ([link](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-properties-metadata)).  

- Using the Azure Blob REST API ([link](https://docs.microsoft.com/en-us/rest/api/storageservices/set-blob-metadata)).  