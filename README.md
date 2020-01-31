# KMD Logic Document Generation Api Client

A dotnet client library for submitting document generation requests the KMD Logic platform and retrieving the corresponding generated documents.

## The purpose of the Document Generation API

To allow products using the Logic platform to generate documents for well-defined business scenarios using templates that their customers can easily manage.

## Getting started in ASP.NET Core

To use this library in an ASP.NET Core application, 
add a reference to the [Kmd.Logic.DocumentGeneration.Client](https://www.nuget.org/packages/Kmd.Logic.DocumentGeneration.Client) nuget package, 
and add a reference to the [Kmd.Logic.Identity.Authorization](https://www.nuget.org/packages/Kmd.Logic.Identity.Authorization) nuget package.

## Interacting With The Document Generation API

The DocumentGenerationClient methods each accept a subscriptionId, which identifies a KMD Logic subscription.  If this is null, the SubscriptionId property of the DocumentGeneration configuration options is used.

### RequestDocumentGeneration

To submit a document generation request:

```c#
var documentGenerationRequest =
    await documentGenerationClient.RequestDocumentGenerationAsync(subscriptionId, new GenerateDocumentRequest
        {
            ConfigurationId = configurationId,
            Key = configurationKey,
            TemplateId = templateId,
            Language = language,
            MergeData = mergeData,
        })
        .ConfigureAwait(false);
```

where subscriptionId identifies a KMD Logic subscription,
and where configurationId, configurationKey, templateId, language identify the target template, 
and where mergeData provides merge data compatible with [Aspose Words Reporting](https://apireference.aspose.com/net/words/aspose.words.reporting/) data sources.

The returned DocumentGenerationRequest contains an Id property that can be passed to later client methods.

TODO update as api becomes more concrete

### GetDocumentGeneration

To retrieve a document generation request:

```c#
var documentGenerationRequest =
    await documentGenerationClient.GetDocumentGenerationAsync(subscriptionId, id)
        .ConfigureAwait(false);
```

where subscriptionId identifies a KMD Logic subscription,
and where id is the Id property of an earlier response to RequestDocumentGeneration. 

### GetDocument

To retrieve a generated document:

```c#
var documentStream =
    await documentGenerationClient.GetDocumentAsync(subscriptionId, documentGenerationRequest.Id.Value)
        .ConfigureAwait(false);
```

where subscriptionId identifies a KMD Logic subscription,
and where id is the Id property of an earlier response to RequestDocumentGeneration. 

The response is an output Stream containing the document data.

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
