# KMD Logic Document Generation Api Client

A dotnet client library for submitting document generation requests the KMD Logic platform and retrieving the corresponding generated documents.

## The purpose of the Document Generation API

To allow products using the Logic platform to generate documents for well-defined business scenarios using templates that their customers can easily manage.

## Configuring a Document Generation environment in Console

For a guide on how to configure a Document Generation environment in Console please go [here](./docs/Configuration/Configuration.md).


## Getting started in ASP.NET Core

To use this library in an ASP.NET Core application, 
add a reference to the [Kmd.Logic.DocumentGeneration.Client](https://www.nuget.org/packages/Kmd.Logic.DocumentGeneration.Client) nuget package, 
and add a reference to the [Kmd.Logic.Identity.Authorization](https://www.nuget.org/packages/Kmd.Logic.Identity.Authorization) nuget package.

## Interacting With The Document Generation API

The DocumentGenerationClient methods each accept a subscriptionId, which identifies a KMD Logic subscription.  If this is null, the SubscriptionId property of the DocumentGeneration configuration options is used.

### RequestDocumentGeneration

#### Requests document generation.

To submit a document generation request:

```c#
var documentGenerationRequest =
    await documentGenerationClient.RequestDocumentGenerationAsync(subscriptionId, new GenerateDocumentRequest
        {
            ConfigurationId = configurationId,
            HierarchyPath = hierarchyPath,
            TemplateId = templateId,
            Language = language,
            DocumentFormat = documentFormat,
            MergeData = mergeData,
        })
        .ConfigureAwait(false);
```

where subscriptionId identifies a KMD Logic subscription,
and where configurationId, hierarchyPath, templateId, language identify the target template, 
and where documentFormat declares the format of the generated document,
and where mergeData provides merge data compatible with [Aspose Words Reporting](https://apireference.aspose.com/net/words/aspose.words.reporting/) data sources.

The returned DocumentGenerationRequest includes an Id property that can be passed to later client methods.

### GetDocumentGeneration

#### Gets document generation request.

To retrieve an already submitted document generation request in order to read its status:

```c#
var documentGenerationRequest =
    await documentGenerationClient.GetDocumentGenerationAsync(subscriptionId, id)
        .ConfigureAwait(false);
```

where subscriptionId identifies a KMD Logic subscription,
and where id is the Id property of an earlier response to RequestDocumentGeneration. 

The documentGenerationRequest object includes a State property which can take one of the following string values:

* Requested

* Completed

* Failed


### GetDocument

#### Gets document generated for the nominated request

To retrieve a generated document (once the document State is Completed):

```c#
var documentUri =
    await documentGenerationClient.GetDocumentAsync(subscriptionId, id)
        .ConfigureAwait(false);
```

where subscriptionId identifies a KMD Logic subscription,
and where id is the Id property of an earlier response to RequestDocumentGeneration. 

The response is a DocumentUri object which includes a Uri string property from which the document can be downloaded, and a UriExpiryTime DateTime property up until which time the Uri link should continue to function.

### WriteDocumentToStream

#### Writes the document generated for provided request to the output stream provided

To write the contents of a generated document to a nominated stream (once the document State is Completed):

```c#
    await documentGenerationClient.WriteDocumentToStreamAsync(subscriptionId, id, outputStream)
        .ConfigureAwait(false);
```

where subscriptionId identifies a KMD Logic subscription,
and where id is the Id property of an earlier response to RequestDocumentGeneration,
and where outputStream provides an open Stream to which the contents of the document can be copied.

The output stream remains open.

### GetTemplates

#### List all templates

To list all templates for a nominated configuration:

```c#
var templates =
    await documentGenerationClient.GetTemplatesAsync(subscriptionId, configurationId, hierarchyPath, subject)
        .ConfigureAwait(false);
```

where subscriptionId identifies a KMD Logic subscription,
and where configurationId nominates the configuration to use,
and where hierarchyPath specifies the hierarchy of possible template sources,
and where subject is the subject of the created document. 

The response is a list of Template objects.  Each template includes a TemplateId string property and a Languages property which lists the relevent document languages as ISO 2 Letter Language code values.  E.g. EN,DA.


## How to configure the Document Generation client

Perhaps the easiest way to configure the Document Generation client is from Application Settings.

```json
{
  "TokenProvider": {
    "ClientId": "",
    "ClientSecret": "",
    "AuthorizationScope": ""
  },
  "DocumentGeneration": {
    "DocumentGenerationServiceUri": "",
    "SubscriptionId": ""
  }
}
```

To get started:

1. Create a subscription in [Logic Console](https://console.kmdlogic.io). This will provide you the `SubscriptionId`.
2. Request a client credential. Once issued you can view the `ClientId`, `ClientSecret` and `AuthorizationScope` in [Logic Console](https://console.kmdlogic.io).

## Sample application

A simple console application is included to demonstrate how to call Logic Document Generation API. You will need to provide the settings described above in `appsettings.json`.

Before running the sample application, upload the template documents provided in the folder:

[sample\Kmd.Logic.DocumentGeneration.Client.Sample\templates](sample\Kmd.Logic.DocumentGeneration.Client.Sample\templates\ "Sample Templates")

to your template storage area.
