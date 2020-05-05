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


## DocumentGenerationClient document generation and conversion

The Logic DocumentGenerationClient provides APIs for:

* listing document generation Templates;
* requesting the generation of documents based on those templates;
* requesting conversion of documents to other formats, in particular to Pdf/A;
* checking on the progress of document generation and format conversion;
* requesting a signed download url from which to stream the generated or converted document.

More details about the DocumentGenerationClient API can be found [here](./docs/Generation/GenerationAndConversionApi.md)


## DocumentGenerationClient Configuration editing

Logic Document Generation is done relative to a hierarchy of template storage areas structured into a Configuration.
The only way to create a new Configuration is via Console (see [here](./docs/Configuration/Configuration.md)).

But the Document Generation Client does provide an API for editing an existing Configuration, and for running the document generation methods relative to that configuration and its descendent template storages.

The response is a list of `DocumentGenerationTemplate` objects.  Each template includes a TemplateId string property and a Languages property which lists the relevant document languages as ISO 2 Letter Language code values.  E.g. en, da.

More details about the DocumentGenerationConfiguration API can be found [here](./docs/Configuration/DocumentGenerationConfigurationApi.md)


## Template authoring

Follow our [Templates](./docs/Templates/Templates.md) guide.

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

## Sample applications

Two sample console applications are included to demonstrate how to call the Logic Document Generation API. You will need to provide the settings described above in their `appsettings.json`.

Before running the sample applications, upload the template documents provided in the folder:

[sample\Kmd.Logic.DocumentGeneration.Client.Sample\templates](sample\Kmd.Logic.DocumentGeneration.Client.Sample\templates\ "Sample Templates")

to your template storage area.
