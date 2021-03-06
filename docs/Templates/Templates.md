# KMD Logic Document Generation Templates

## Introduction

KMD Logic Document Generation uses MS Word files (Microsoft Open XML `.docx` files) as templates from which to generate documents.  


## Template Syntax

KMD Logic Document Generation utilizes [Aspose](https://www.aspose.com) as the underlying library to generate documents.  
Because of this, it supports out of the box all the syntax constructs available in the Aspose Reporting Engine.  
A comprehensive guide about this can be found at:  [Template Syntax - Aspose.Words for .NET - Documentation](https://docs.aspose.com/display/wordsnet/Template+Syntax).  


## KMD Logic Document Generation Template Helpers

In addition to the built-in functionality provided by Aspose, KMD Logic Document Generation provides functionality to help template authors to create documents by retrieving resources that are located in storage areas from a KMD Logic Document Generation Configuration hierarchy.  

To learn more about KMD Logic Document Generation Configuration, please refer to [this guide](../Configuration/Configuration.md).

KMD Logic Document Generation Template supports the following list of helpers:

### Template helper

Use the `template` helper to load external `.docx` files (e.g. signatures, disclaimers, legal notes, etc.) stored in your storage provider or higher up the hierarchy.  

#### Examples

```
<<doc [template.Load(“word-partial-footer.docx”)]>>
```

#### Note

For the Aspose documentation about inserting documents dynamically please follow [this guide](https://docs.aspose.com/display/wordsnet/Template+Syntax#TemplateSyntax-InsertingDocumentsDynamically).


### Image helper

Use the `image` helper to load external graphic files or images stored in your storage provider or higher up the hierarchy.  


#### Example - Loading image from external url

No need to use the `image` helper, e.g.:

```
<<image ["http://www.example.com/image.jpg"] >>
```

#### Example - Loading images from your storage provider or higher up the hierarchy

Use `image.Load`, e.g.:

```
<<image [image.Load("image.jpg")] >>
```

#### Example - Loading image from image data encoded as base64 in the mergeData property  

Use `image.Decode`, e.g.:

```
<<image [image.Decode(SignatureImage)] >>
```

#### Note  

For the Aspose documentation about inserting images dynamically please follow [this guide](https://docs.aspose.com/display/wordsnet/Template+Syntax#TemplateSyntax-InsertingImagesDynamically).


### Number and Date/Time helper

Use the `number.format` helper to format `numeric`, `currency` or `percentage` values in your templates.   

Use the `date.format` to format date/time values in your templates.  

`number.format` and `date.format` provide additional functionality from the regular formatting helpers in Aspose, mostly focused on customizing the localization culture used to format its values.    


#### Examples

- Numeric (The following examples use a merge data field named Amount)  

    - Amount (no format):  
    `<<[Amount]>>`  

    - Amount (as Percent (`P`) Format Specifier):  
    `<<[Amount]:”P”>>`  

    - Amount (as Numeric (`N`) Format Specifier): 
    `<<[Amount]:”N”>>`  

    - Amount (as Numeric (`N`) Format Specifier `FR` culture):  
    `<<[number.Format(Amount, “N”, “fr”)]>>`  

    - Amount (as Numeric (`N`) Format Specifier `EN-GB` culture):  
    `<<[number.Format(Amount, “N”, “en-GB”)]>>`  

    - Amount (as Fixed-Point (`F`) Format Specifier):  
    `<<[Amount]:”F”>>`  

    - Amount (as Fixed-Point (`F`) `FR` culture):  
    `<<[number.Format(Amount, “F”, “fr”)]>>`  

    - Amount (as Fixed-Point (`F`) `EN-GB` culture):  
    `<<[number.Format(Amount, “F”, “en-GB”)]>>`  

- Currency (The following examples use a merge data field named Price)  

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

- Percentage (The following examples use a merge data field named Percentage)  

    - Percent (as Percent (`P`) Format Specifier):  
    `<<[Percent]:”P”>>`  

    - Percent (as Fixed-Point (`F`) Format Specifier):  
    `<<[Percent]:”F”>>`  

    - Percent (as Digit placeholder):  
    `<<[Percent]:”#.##”>>`  

    - Percent (as Percentage placeholder):  
    `<<[Percent]:”%#0.00”>>` 


- Date/time (The following examples use a merge data fields named DateOfBirth and Appointment)  

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

For the Aspose documentation about outputting expression results and related formatting please follow [this guide](https://docs.aspose.com/display/wordsnet/Template+Syntax#TemplateSyntax-OutputtingExpressionResults).

### String case helper

Use the `stringCase` helper to format case of the passed string value.

As for now there is only one method implemented: `stringCase.SentenceCase`. _Sentence case_ is the conventional way of using capital letters in a sentence or capitalizing only the first word.

#### Examples - basic scenario

Use `stringCase.SentenceCase`

```
<<[stringCase.SentenceCase("someStringValue")]>>

<<[stringCase.SentenceCase(SomeField)]>>
```

#### Examples - nested helpers

Use `stringCase.SentenceCase` to format case of the date helper result

```
<<[stringCase.SentenceCase(date.Format(Forløb_sluttermin, ”MMMM yyyy”, “da-DK”))]>>
```


## Templates and document generation

Logic Document Generation uses `.docx` templates and creates documents out of them.  

There are a few conventions that are followed when resolving a template from a configuration entry point of view (to know more about the configuration model visit the [Configuration](../Configuration/Configuration.md) documentation).  

Logic Document Generation uses a hierarchical configuration model where each node is able to override a template from its parent node.  

This design allows a master storage to define a set of generic templates for their customers to use, while allowing their customers to customize/extend all or some of them.   

### How to override a Master template  

#### Using SharePoint

- Create a `.docx` file in your SharePoint Group.
- Name it using the same name as the document you want to override and update its content as needed.  


#### Using Azure Blob Storage

- Upload a `.docx` file to your Azure Blob Storage.
- Name it using the same name as the document you want to override and update its content as needed.  
    > Make sure you upload the file to the declared container and directory (`Blob prefix`) in Console for your configuration entry.  


### How to hide a template from the list of available templates for a given entry

#### Using SharePoint

- Create an empty `.docx` file in your SharePoint Group.
- Name it using the same name as the document you want to hide but adding a `.hidden` suffix to it.  
    > For instance, if you want to hide `Template.docx`, the name of the document in your local SharePoint Group should be `Template.docx.hidden`


#### Using Azure Blob Storage

- Upload a `.docx` file to your Azure Blob Storage account.
- Name it using the same name as the document you want to hide.  
    > Make sure you upload the file to the declared container and directory (`Blob prefix`) in Console for your configuration entry.  
- Attach a new metadata record to the document with key `Hidden`.  
    > The actual value of the `Hidden` metadata entry doesn't matter as long as the key is equal to `Hidden`.  

You can set metadata for blobs using one of the following:

- The Storage Explorer in the Azure Portal ([link](https://azure.microsoft.com/en-au/updates/storage-explorer-preview-now-available-in-azure-portal/)).

- The Storage Explorer client ([link](https://azure.microsoft.com/en-us/features/storage-explorer/)).

- The Azure Storage client library for .NET ([link](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-properties-metadata)).  

- The Azure Blob REST API ([link](https://docs.microsoft.com/en-us/rest/api/storageservices/set-blob-metadata)).  

### How to tag document templates for searching using Logic Document Generation API

The Logic Document Generation API allows you to list all templates available for a given configuration entry.  

```csharp
var templates = await documentGenerationClient.GetTemplates(configurationId, hierarchyPath, null).ConfigureAwait(false);
```


It also allows you to pass in a `subject` parameter to retrieve only those documents that match that filter.  

Below we retrieve all the templates that have a subject equal to `invoice`:  

```csharp
var templates = await documentGenerationClient.GetTemplates(configurationId, hierarchyPath, "invoice").ConfigureAwait(false);
```

#### Configuring template subjects in SharePoint

The SharePoint storage provider for Logic Document Generation relies on the directory name to filter out templates by `subject`.   

Locate the `.docx` template file in your SharePoint instance and put them in a directory matching your `subject` filter (e.g. add the template to an `invoice` directory as per the example above).  


#### Configuration template subjects in Azure Blob Storage  

The Azure Blob Storage provider from Logic Document Generation relies on the document metadata to filter out templates by `subject`.  

When filtering by `subject`, only the metadata key will be used as matching criteria, the actual value is not considered.  

You can set metadata for blobs using one of the following:

- The Storage Explorer in the Azure Portal ([link](https://azure.microsoft.com/en-au/updates/storage-explorer-preview-now-available-in-azure-portal/)).

- The Storage Explorer client ([link](https://azure.microsoft.com/en-us/features/storage-explorer/)).

- The Azure Storage client library for .NET ([link](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-properties-metadata)).  

- The Azure Blob REST API ([link](https://docs.microsoft.com/en-us/rest/api/storageservices/set-blob-metadata)).  


### Multi-language support

Logic Document Generation supports multi-language support for templates.  

This allows Logic Document Generation API consumers, to generate documents targeting a specific language.  

Logic Document Generation infers the language of a template if its file name adheres to the following pattern: 

`<file_name>.<lang>.docx`

where `<lang>` is one of the [ISO 639-1 two letter language codes](https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes).

In the following snippet, we request a document be generated using the `template.docx` template, but we also specify `en` as the target language.  
Because of that setting, Logic Document Generation will ascend the storage configuration hierarchy choosing the first file named `template.en.docx`, and use that template to generate the requested PDF document.  

```csharp
await documentGenerationClient.RequestDocumentGeneration(configurationId, new DocumentGenerationRequestDetails(hierarchyPath,
                "template.docx",
                // use the English version of the template: template.en.docx
                "en",
                DocumentFormat.Pdf,
                mergeData));
```


#### Overriding rules for multi-language templates

You must override every version of the template for every language you wish to make available for your entry.   

### Template overriding and resolution examples

Given the following configuration entry hierarchy and template files:

    - [\] Master entry 
        - Template1.docx
        - Template2.docx
        - Template3.en.docx
        - Template3.es.docx
        - Template3.da.docx            
    - [\CustomerA\] Customer A entry
        - Template1.docx.hidden
        - Template2.docx
        - Template3.en.docx
        - Template3.es.docx
        - Template3.da.docx
        - Template4.docx
    - [\CustomerA\DepartmentA] Department A entry
        - Template3.en.docx
    - [\CustomerB\] Customer B entry
        - Template3.da.docx 
        - Template5.docx


----

- List all templates for Master entry:  
    - Template1.docx
    - Template2.docx
    - Template3.docx [en, es, da]

- List all templates for Customer A entry
    - Template2.docx
    - Template3.docx [en, es, da]
    - Template4.docx

- List all templates for Department A entry
    - Template2.docx
    - Template3.docx [en]  
    - Template4.docx

- List all templates for Customer B entry
    - Template1.docx
    - Template2.docx
    - Template3.docx [da] 
    - Template5.docx

## Template Metadata

Additional information about a template can be provided in template metadata files.  
For example, the schema definition of the merge data for a template might be provided in that template's metadata file so that an application can better determine what merge data to provide to the document generation process for a particular template.

For more information about template metadata files see [Template Metadata Files](./TemplateMetadata.md).

