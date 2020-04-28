# KMD Logic Document Generation Api Client

A dotnet client library for submitting document generation requests to the KMD Logic platform and retrieving the corresponding generated documents.

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
    await documentGenerationClient.RequestDocumentGeneration(
        subscriptionId,
        configurationId,
        new DocumentGenerationRequestDetails(
            hierarchyPath,
            templateId,
            twoLetterIsoLanguageName,
            documentFormat,
            mergeData,
            callbackUrl
        )
        .ConfigureAwait(false);
```

where:

* `subscriptionId` identifies a KMD Logic subscription;
* `configurationId` identifies a KMD Logic Document Generation configuration belonging to that subscription;
* `hierarchyPath` encodes the hierarchy of possible template sources not including the master location ([Hierarchy Path](./docs/HierarchyPath.md));
* `templateId` identifies the name of the document generation template;
* `twoLetterIsoLanguageName` specifies a language code in ISO 639-1 format (eg. en, da);
* `documentFormat` declares the format of the generated document (see [DocumentFormat](./src/Kmd.Logic.DocumentGeneration.Client/Types/DocumentFormat.cs) );
* `mergeData` provides merge data compatible with [Aspose Words Reporting](https://apireference.aspose.com/net/words/aspose.words.reporting/) data sources;
* `callbackUrl` declares a URL that is to be called when document generation completes.

The returned DocumentGenerationProgress response object [DocumentGenerationProgress.cs](./src/Kmd.Logic.DocumentGeneration.Client/ServiceMessages/DocumentGenerationProgress.cs) includes an Id property that can be passed to later client methods.

### GetDocumentGenerationProgress

#### Gets document generation progress.

To retrieve an already submitted document generation request in order to read its status:

```c#
var documentGenerationProgress =
    await documentGenerationClient.GetDocumentGenerationProgress(subscriptionId, id)
        .ConfigureAwait(false);
```

where `subscriptionId` identifies a KMD Logic subscription,
and where `id` is the Id property of an earlier response to `RequestDocumentGeneration`. 

The `DocumentGenerationProgress` response object [DocumentGenerationProgress.cs](./src/Kmd.Logic.DocumentGeneration.Client/ServiceMessages/DocumentGenerationProgress.cs) includes a State property ( see [DocumentGenerationState.cs](./src/Kmd.Logic.DocumentGeneration.Client/Types/DocumentGenerationState.cs) ) which can take one of the following values:

* `Requested`

* `Completed`

* `Failed`


### GetDocument

#### Gets the document generated for the nominated request

To retrieve a generated document (once the document `State` is `Completed`):

```c#
var documentUri =
    await documentGenerationClient.GetDocumentGenerationUri(subscriptionId, id)
        .ConfigureAwait(false);
```

where `subscriptionId` identifies a KMD Logic subscription,
and where `id` is the Id property of an earlier response to `RequestDocumentGeneration`. 

The response is a `DocumentGenerationUri` object that includes a `Uri` property using which the document can be downloaded, and a `UriExpiryTime` DateTime property up until which time the Uri link should continue to function.

### GetTemplates

#### List all templates

To list all templates for a nominated configuration:

```c#
var templates =
    await documentGenerationClient.GetTemplates(subscriptionId, configurationId, hierarchyPath, subject)
        .ConfigureAwait(false);
```

where:

* `subscriptionId` identifies a KMD Logic subscription;

* `configurationId` identifies a KMD Logic Document Generation configuration;

* `hierarchyPath` encodes the hierarchy of possible template sources not including the master location ([Hierarchy Path](./docs/HierarchyPath.md));

* `subject` is the subject of the created document.

The response is a list of `DocumentGenerationTemplate` objects.  Each template includes a TemplateId string property and a Languages property which lists the relevent document languages as ISO 2 Letter Language code values.  E.g. en, da.


## How to configure the Document Generation client

Perhaps the easiest way to configure the Document Generation client is though Application Settings.

```json
{
  "TokenProvider": {
    "ClientId": "",
    "ClientSecret": "",
    "AuthorizationScope": ""
  },
  "DocumentGeneration": {
    "SubscriptionId": ""
  }
}
```

To get started:

1. Create a subscription in [Logic Console](https://console.kmdlogic.io). This will provide you the `SubscriptionId`.
2. Request a client credential. Once issued you can view the `ClientId`, `ClientSecret` and `AuthorizationScope` in [Logic Console](https://console.kmdlogic.io).

## Sample application

A simple console application is included to demonstrate how to call the Logic Document Generation API. You will need to provide the settings described above in `appsettings.json`.

Before running the sample application, upload the template documents provided in the folder:

[sample\Kmd.Logic.DocumentGeneration.Client.Sample\templates](sample\Kmd.Logic.DocumentGeneration.Client.Sample\templates\ "Sample Templates")

to your template storage area.
