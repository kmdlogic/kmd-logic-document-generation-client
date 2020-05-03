# KMD Logic DocumentGenerationConfiguration Api

Logic Document Generation is done relative to a hierarchy of template storage areas structured into a Configuration.
The only way to create a new Configuration is via Console (see [here](./Configuration.md)).
That document also provides an overview of configuration hierarchy semantics and should be read first.

Away from Logic Console, the Document Generation Client provides an API for editing an existing Configuration, and for running the document generation methods relative to that configuration and its descendant template storages.

## Motivating example

```c#
DocumentGenerationClient client;
Guid subscriptionId;
string aTemplateId;
JObject mergeData;

// ...

// Get all configurations for this subscription Id and select the Id of the first one with the name MyConfigurationName
var configurationId = 
    (await client.GetConfigurationsForSubscription(subscriptionId)
        .ConfigureAwait(false))
    .FirstOrDefault(c => c.Name == "MyConfigurationName")
    .ConfigurationId;

// Get the configuration with that Id
var documentGenerationConfiguration =
    documentGenerationClient.GetDocumentGenerationConfiguration(subscriptionId, configurationId);

// update its root template storage area to a SharePoint Online group folder
var rootTemplateStorageDirectory =
    documentGenerationConfiguration.SetRootTemplateStorageDirectory(
        "Master directory",
        new SharePointOnlineTemplateStorage(
            "123456789-abcd-ef01-2345-6789abcdef12",
            "012234567-89ab-cdef-0123-456789abcdef",
            "cLiEntSeCrEt",
            "Group Name"));

// Add a child Azure Blob storage area under the root
var customerDirectory =
    rootTemplateStorageDirectory.AddChild(
        "MyCustomerKey",
        "My Customer Name",
        new AzureBlobTemplateStorage(
            "DefaultEndpointsProtocol=https;AccountName=myaccountsto;AccountKey=myaccountkey-b64==;EndpointSuffix=core.windows.net",
            "MyContainerName",
            "MyBlobPrefix"));

// Save the configuration back to Logic
documentGenerationConfiguration.Save();

// Request a document generation relative to templates held in the customer directory
var documentGenerationRequestId =
    customerDirectory.RequestDocumentGeneration(
        aTemplateId,
        "en",
        DocumentFormat.Docx,
        mergeData)?.Id;
```

## From the DocumentGenerationClient object

The following DocumentGenerationClient methods are used to discover a DocumentGenerationConfiguration object that can be used to edit a Logic Document Generation Configuration.

### DocumentGenerationClient Instance Methods

#### GetConfigurationsForSubscription

##### Lists all Document Generation Configurations for a nominated Kmd Logic Subscription Id.

To list all configurations for a nominated subscription id:

```c#
var configurations =
    await documentGenerationClient.GetConfigurationsForSubscription(subscriptionId)
        .ConfigureAwait(false);
```

where:

* `subscriptionId` identifies a KMD Logic subscription.

The response is a list of `DocumentGenerationConfigurationListItem` objects [source](../../src/Kmd.Logic.DocumentGeneration.Client/ServiceMessages/DocumentGenerationConfigurationListItem.cs).  Each configuration item includes a ConfigurationId Guid that can be used to identify the configuration in other methods of document generation and configuration.


#### GetDocumentGenerationConfiguration 

##### Retrieves a DocumentGenerationConfiguration from the Logic server.

To retrieve a DocumentGenerationConfiguration:

```c#
var documentGenerationConfiguration =
    await documentGenerationClient.GetDocumentGenerationConfiguration(subscriptionId, configurationId)
        .ConfigureAwait(false);
```

where 

* `subscriptionId` identifies a KMD Logic subscription;
* `configurationId` identifies a KMD Logic Document Generation configuration belonging to that subscription.

The response is a `DocumentGenerationConfiguration` object ([source](../../src/Kmd.Logic.DocumentGeneration.Client/Configuration/DocumentGenerationConfiguration.cs)), detailed below.

There is no public constructor available.  This is the only way to create a DocumentGenerationConfiguration object.

## The DocumentGenerationConfiguration object

### Properties

#### `Id`

The `Guid` identifier of the document generation configuration.  This is the same configurationId argument expected by most of the DocumentGenerationClient methods.

#### `SubscriptionId`

The `Guid` identifier of the KMD Logic subscription that owns the configuration.

#### `Name`

The `string` name of the configuration.

#### `TemplateStorageDirectory`

The `DocumentGenerationTemplateStorageDirectory` ([source](../../src/Kmd.Logic.DocumentGeneration.Client/Configuration/DocumentGenerationTemplateStorageDirectory.cs)) root template storage object.

#### `LevelNames`

The `IList<string>` names of the hierarchy levels.

#### `HasLicense`

The `bool` declaration that the customer has a valid Aspose license.

### Instance Methods

The following instance methods are available on a DocumentGenerationConfiguration object.

#### `Save`

##### Saves the object back to the Logic server.

To sync the DocumentGenerationConfiguration object back to the Logic server:

```c#
    documentGenerationConfiguration.Save();
```

#### `Load`

##### Reloads the object from the Logic server.

To reload the DocumentGenerationConfiguration object from the Logic server:

```c#
    documentGenerationConfiguration.Load();
```

#### `Delete`

##### Deletes the configuration from the Logic server.

To delete the document generation configuration from the Logic server:

```c#
    documentGenerationConfiguration.Delete();
```

#### `SetRootTemplateStorageDirectory`

##### Locally updates root template storage area.

To set the root of a configuration to a new TemplateStorageDirectory object:

```c#
var rootTemplateStorageDirectory =
    documentGenerationConfiguration.SetRootTemplateStorageDirectory(
        rootTemplateStorageAreaName,
        templateStorageConfiguration);
```

where:

* `rootTemplateStorageAreaName` specifies the Name of the template storage (the key of the root configuration will always be the empty string);
* `templateStorageConfiguration` is an `ITemplateStorageConfiguration`.

There are two classes that implement `ITemplateStorageConfiguration`:

* `AzureBlobTemplateStorage` ([source](../../src/Kmd.Logic.DocumentGeneration.Client/Configuration/TemplateStorageConfigurations/AzureBlobTemplateStorage.cs))

* `SharePointOnlineTemplateStorage` ([source](../../src/Kmd.Logic.DocumentGeneration.Client/Configuration/TemplateStorageConfigurations/SharePointOnlineTemplateStorage.cs))

The response is the new `DocumentGenerationTemplateStorageDirectory` object belonging locally to the configuration.  This object can be updated independently of its DocumentGenerationConfiguration.  Its details will be only synced back to the Logic server when the `Save` method is called on its `DocumentGenerationConfiguration`.  You should call `Save` on its parent `DocumentGenerationConfiguration` before using the object's `RequestDocumentGeneration` or `GetTemplates` methods.


#### `FindDirectoryByPath`

##### Finds the descendant DocumentGenerationTemplateStorageDirectory object at the nominated hierarchy path

To translate a hierarchy path to a TemplateStorageDirectory object:

```c#
var templateStorageDirectory =
    documentGenerationConfiguration.FindDirectoryByPath(
        new HierarchyPath(new[] { string.Empty, "MySchool", "MyDepartment" } ));

var sameTemplateStorageDirectory =
    documentGenerationConfiguration.FindDirectoryByPath(
        new HierarchyPath(@"\MySchool\MyDepartment"));
```

where the `HierarchyPath` parameter is a [HierarchyPath](../../src/Kmd.Logic.DocumentGeneration.Client/Configuration/HierarchyPath.cs) (see [Hierarchy Path](../HierarchyPath.md))

The response is the documentGenerationConfiguration's descendant `DocumentGenerationTemplateStorageDirectory` object at the nomiated hierarchy path.


### DocumentGenerationConfiguration Instance Methods

The same document generation methods available via a DocumentGenerationClient object are also available on one of its DocumentGenerationConfiguration objects.

#### `RequestDocumentConversionToPdfA`

##### Requests document rendering to Pdf/A.

To submit a document rendering request:

```c#
var documentGenerationProgress =
    await documentGenerationConfiguration.RequestDocumentConversionToPdfA(
        new DocumentConversionToPdfARequestDetails(
            sourceDocumentUrl,
            sourceDocumentFormat,
            callbackUrl
        )
        .ConfigureAwait(false);
```

where:

* `sourceDocumentUrl` URL that identifies the document to be converted;
* `sourceDocumentFormat` declares the format of the original document (see [DocumentFormat](../../src/Kmd.Logic.DocumentGeneration.Client/Types/DocumentFormat.cs) );
* `callbackUrl` declares a URL that is to be called when document rendering completes.

The returned DocumentGenerationProgress object ([source](../../src/Kmd.Logic.DocumentGeneration.Client/ServiceMessages/DocumentGenerationProgress.cs)) includes an Id property that can be passed to later client or configuration methods.

#### `RequestDocumentConversion`

##### Requests document format conversion to some supported format.

To submit a document conversion request:

```c#
var documentGenerationProgress =
    await documentGenerationConfiguration.RequestDocumentConversion(
        new DocumentConversionRequestDetails(
            sourceDocumentUrl,
            sourceDocumentFormat,
            convertedDocumentFormat,
            convertedDocumentPdfFormat,
            callbackUrl
        )
        .ConfigureAwait(false);
```

where:

* `sourceDocumentUrl` URL that identifies the document to be converted;
* `sourceDocumentFormat` declares the format of the original document (see [DocumentFormat](../../src/Kmd.Logic.DocumentGeneration.Client/Types/DocumentFormat.cs) );
* `convertedDocumentFormat` declares the format of the target document (see [DocumentFormat](../../src/Kmd.Logic.DocumentGeneration.Client/Types/DocumentFormat.cs) );
* `convertedDocumentPdfFormat` optionally declares the pdf version of the target document (see [DocumentFormat](../../src/Kmd.Logic.DocumentGeneration.Client/Types/PdfFormat.cs) );
* `callbackUrl` declares a URL that is to be called when document rendering completes.

The returned DocumentGenerationProgress object ([source](../../src/Kmd.Logic.DocumentGeneration.Client/ServiceMessages/DocumentGenerationProgress.cs)) includes an Id property that can be passed to later client or configuration methods.

#### `GetDocumentGenerationProgress`

##### Gets document generation progress.

To retrieve an already submitted document generation request in order to read its status:

```c#
var documentGenerationProgress =
    await documentGenerationConfiguration.GetDocumentGenerationProgress(requestId)
        .ConfigureAwait(false);
```

where `requestId` is the Id property of an earlier response to one of the `RequestDocumentGeneration` or `RequestDocumentConversionToPdfA` or `RequestDocumentConversion` methods.

The `DocumentGenerationProgress` response object [source](../../src/Kmd.Logic.DocumentGeneration.Client/ServiceMessages/DocumentGenerationProgress.cs) includes a State property ( see [DocumentGenerationState.cs](../../src/Kmd.Logic.DocumentGeneration.Client/Types/DocumentGenerationState.cs) ) which can take one of the following values:

* `Requested`
* `Completed`
* `Failed`


#### `GetDocumentGenerationUri`

##### Gets the document generated for the nominated request.

To retrieve a generated document (once the document `State` is `Completed`):

```c#
var documentUri =
    await documentGenerationConfiguration.GetDocumentGenerationUri(requestId)
        .ConfigureAwait(false);
```

where `requestId` is the Id property of an earlier response to one of the `RequestDocumentGeneration` or `RequestDocumentConversionToPdfA` or `RequestDocumentConversion` methods. 

The response is a `DocumentGenerationUri` object that includes a `Uri` property using which the document can be downloaded, and a `UriExpiryTime` DateTime property up until which time the Uri link should continue to function.



## The DocumentGenerationTemplateStorageDirectory object

### Properties

#### `Id`

The `Guid` identifier of the object.  This is generated by the Logic server on Save and cannot be locally assigned.

#### `Key`

The `string` key that will form part of the hierarchy path ([see](../HierarchyPath.md)).

#### `Name`

The `string` name describing the template storage.

#### `TemplateStorageConfiguration`

The `ITemplateStorageConfiguration` object name describing the template storage.

There are two classes that implement `ITemplateStorageConfiguration`:

* `AzureBlobTemplateStorage` ([source](../../src/Kmd.Logic.DocumentGeneration.Client/Configuration/TemplateStorageConfigurations/AzureBlobTemplateStorage.cs))

* `SharePointOnlineTemplateStorage` ([source](../../src/Kmd.Logic.DocumentGeneration.Client/Configuration/TemplateStorageConfigurations/SharePointOnlineTemplateStorage.cs))

#### `Children`

The `IReadOnlyList<DocumentGenerationTemplateStorageDirectory>` collection of template storage areas that sit directly under this DocumentGenerationTemplateStorageDirectory.


#### `HierarchyPath`

The `HierarchyPath` to this DocumentGenerationTemplateStorageDirectory.  This property cannot be assigned.

#### `ParentHierarchyPath`

The `HierarchyPath` to this object's parent DocumentGenerationTemplateStorageDirectory.  This property cannot be assigned.


### DocumentGenerationTemplateStorageDirectory Instance Methods

The following instance methods are available on a DocumentGenerationTemplateStorageDirectory object.

#### `AddChild`

##### Adds a new DocumentGenerationTemplateStorageDirectory as a child.

To add a new template storage directory to the hierarchy:

```c#
var childTemplateStorageDirectory =
    parentTemplateStorageDirectory.AddChild(
        childKey,
        childName,
        childTemplateStorageConfiguration);
```

where:

The `string` key that will form part of the hierarchy path ([see](../HierarchyPath.md)).

* `childKey` specifies a `string` key that will be the leaf part of the hierarchy path ([see](../HierarchyPath.md)) to the child template storage directory;
* `childName` specifies a `string` Name of the new child template storage;
* `childTemplateStorageConfiguration` is an `ITemplateStorageConfiguration`.

There are two classes that implement `ITemplateStorageConfiguration`:

* `AzureBlobTemplateStorage` ([source](../../src/Kmd.Logic.DocumentGeneration.Client/Configuration/TemplateStorageConfigurations/AzureBlobTemplateStorage.cs))

* `SharePointOnlineTemplateStorage` ([source](../../src/Kmd.Logic.DocumentGeneration.Client/Configuration/TemplateStorageConfigurations/SharePointOnlineTemplateStorage.cs))

The response is the new `DocumentGenerationTemplateStorageDirectory` object belonging locally to the configuration.  This object can be updated independently of its DocumentGenerationConfiguration.  Its details will be only synced back to the Logic server when the `Save` method is called on its `DocumentGenerationConfiguration`.  You should call `Save` on its parent `DocumentGenerationConfiguration` before using the object's `RequestDocumentGeneration` or `GetTemplates` methods.

#### `GetTemplates`

##### Lists all templates of this directory and ancestor directories.

To list all templates:

```c#
var templates =
    await templateStorageDirectory.GetTemplates(subject)
        .ConfigureAwait(false);
```

where `subject` is the subject of the created document.

The response is a list of `DocumentGenerationTemplate` objects.  Each template includes a TemplateId string property and a Languages property which lists the relevent document languages as ISO 2 Letter Language code values.  E.g. en, da.

This method is equivalent to the method on DocumentGenerationClient, but since this object is already aware of its configurationId, subscriptionId and hierarchyPath, there is no need to pass these.

#### `RequestDocumentGeneration`

##### Requests a document generation.

To submit a document generation request:

```c#
var documentGenerationProgress =
    await templateStorageDirectory.RequestDocumentGeneration(
        templateId,
        twoLetterIsoLanguageName,
        documentFormat,
        mergeData,
        callbackUrl
    )
    .ConfigureAwait(false);
```

where:

* `templateId` identifies the name of the document generation template;
* `twoLetterIsoLanguageName` specifies a language code in ISO 639-1 format (eg. en, da);
* `documentFormat` declares the format of the generated document (see [DocumentFormat](../../src/Kmd.Logic.DocumentGeneration.Client/Types/DocumentFormat.cs) );
* `mergeData` provides merge data compatible with [Aspose Words Reporting](https://apireference.aspose.com/net/words/aspose.words.reporting/) data sources;
* `callbackUrl` declares a URL that is to be called when document generation completes.

The returned DocumentGenerationProgress response object [source](../../src/Kmd.Logic.DocumentGeneration.Client/ServiceMessages/DocumentGenerationProgress.cs) includes an Id property that can be passed to later client methods.

This method is equivalent to the method on DocumentGenerationClient, but since this object is already aware of its configurationId, subscriptionId and hierarchyPath, there is no need to pass these.
