# Document Generation Workshop

Document generation service helps in generating documents for well-defined business scenarios using templates that their customers can easily manage. 

## Prerequisites

1. You are a KMD User
2. Verify `cURL` command line tool is available in your system by executing `curl --help` in command prompt.
3. If you don't have `cURL` then please download the package from [here](https://curl.se/dlwiz/?type=bin) for your respective OS ([Refer](https://help.ubidots.com/en/articles/2165289-learn-how-to-install-run-curl-on-windows-macosx-linux) for help in how to install)

Document generation service is hosted on Logic platform among with many other services. To use Document generation service on Logic platform you need to do the followings

1. Create a subscription in Logic platform by logging in to [Logic Console](https://console.kmdlogic.io/)
2. Create a Document generation configuration.
3. Create client credentials for an authorized access to Document generation service.
4. Use the above created `Subscription Id`, `Configuration Id` and `Client Credentials` while accessing the Document generation APIs to request for a document.
5. Document generation APIs can be accessed using `cURL` commands or through a [dotnet client](https://www.nuget.org/packages/Kmd.Logic.DocumentGeneration.Client). Also refer to our [Github](https://github.com/kmdlogic/kmd-logic-document-generation-client) for more information.
6. Once document is generated you will receive a notification from Logic about the status of your request.
7. There are APIs available in Document geneation to get the URL where the document is generated and uploaded to.

Below are the detailed steps which will help you in getting started with Logic Document Generation.

1. Login to [Logic Console](https://console.kmdlogic.io/) using your KMD credentials. Select `KMD ADFS`. Console is our front end to generate configurations for different services on Logic. These configurations will be used to consume the APIs.
2. To use services on Logic you need to create a subscription. First time users when logging into Console, will be asked to create the same. Please refer to the screenshot below which a first time logged in user to Console will see.
3. ## Image
4. You will landup in the Resource overview page where you can see all the configurations created under you subscription for different services. For a new user it will be empty. Click on the `Create new resource`.
5. ## Image 
6. You will reach the Logic Marketplace. Here you can find all services hosted on Logic. Search for `Logic Document Generation`. You will see the service. Select the service to know more about it.

    ![Image of Marketplace](./images/marketplace.jpg)

7. Once you select the service in previous step you can see various details like Overview, API Reference etc. To create a configuration click on `Create`.

    ![Image of Product](./images/product.jpg)

8. It will take you to the configuration creation page as shown below.

    ![Image of Product](./images/configuration-create.jpg)

9. Each configuration has 3 sections such as `Configuration settings`, `Template storage settings` and `Output storage settings`. Each of the section is explained below.

10. ### Configuration settings
    Configuration values like Name, template metadata file extension and levels are defined here. Template metadata files are used to provide additional information about a template. [Refer](../Templates/TemplateMetadata.md) for more information.

    Levels are used to define the hierarchy in a configuration. We can have template files at each level which will override the parent template files during document generation. Hierarchies are particularly useful in scenarios where same configuration is used by multiple departments to generate documents using templates at their respective level. For each level separate Template storage configurations are defined which is explained in the next section `Template storage settings`.

11. ### Template storage settings
    Document generation service uses templates to generate documents. As part of configuration creation, the storage where templates are available are configured. Currently Logic supports two types storage, `Share point` and `Azure blob`. Please refer to the screenshot below.

    ![Image of Template storage](./images/template-storage.jpg)

    The template storage that is configured during creation of document generation configuration becomes the root level template storage. Once the configuration is created, new entries can be added under this root level which can further signify the template storage for levels defined during configuration creation. These entries are known as [Configuration entries](#configuration-entries)

12. ### Output storage settings
    The generated documents are stored in this storage and there are APIs available to get the the url to the storage. This storage can be owned by Logic or the user. Default it is Logic storage and the document will be available for 24 hrs. You can provide your storage configuration where the document will be uploaded by the document generation service. Currently only Azure blob storage is supported as Output storage. Refer to the screenshot below.

    ![Image of Output storage](./images/output-storage.jpg)

13. After providing all the inputs, click on `Save` to create the configuration.

14. Next we will be creating client credentials in the Console using the same subscription. These will later be used while accessing the document generation APIs.

15. Click on the `Logic` heart icon on top left of the page. It will take you to the `Resource overview` page.

    ![Image of Redirect to Home](./images/redirect-home.jpg)

16. In the `Resource Overview` page, click on the ellipsis and select `Client credentials`

17. ## Image

18. You will be redicted to the Client credentials page where you can see the list of client credentials created. As of now you will not see any existing records. Click on `Add new`.

    ![Image of Add new Client credential](./images/add-client-credential.jpg)

19. Provide a `Display name` and click on `Create` in the pop-up window.

    ![Image of Add new Client credential pop-up](./images/add-client-credential-popup.jpg)

20. After the creation you can see the record in the same page. Expand the record to view more details. Some properties like `client_id`, `client_secret` etc will be used further to access the Document generation APIs.

    ![Image of Client credential](./images/client-credential.jpg)



## Configuration entries
Once a configuration is created you can view the template storage defined at `Root` level whose storage setting was provided during configuration creation. This becomes the `Master`. Screenshot below.

![Image of Configuration Entry at root](./images/configuration-entry-root.jpg)
    
You can add further child entries to the Root or Master entry. These child entries will correspond to your levels' template storage setting. E.g. If your configuration has two levels such as `city` and `school` where the hierarchy is root->city->school, you can add one or more child entries to Master which will correspond to the city level. Each of those entries will have their own Template storage setting (Shared point or Azure blob). You can further add one or more child entries to city which will correspond to school level template storage setting. Please refer to the screenshot below.

![Image of Output storage](./images/add-configuration-entry.jpg)

While adding the child entries there are two more fields `Name` and `Key` for which values have to be provided. Name specfies the name of the entry. For Root entry which is added default during configuration creation, the name is Master. Key signifies the logical path in the hierarchy. For Root it is `\`. This path value is **important** as it is used in the body of API request while requesting for document generation.

Consider the above example of levels city and school. Say we have one entry for city under Master with Name as `Copenhagen` and key as `\copenhagen` and one entry for school under city Copenhagen with Name as `Copenhagen International School` and key as `cis`. If you want to generate document for a template for Copenhagen entry, request should have path as `\copenhagen`. For school under Copenhagen request path should be `\copenhagen\cis` and if you want to generate document using template at root, path should be `\\`. Below is the screenshot showing the Configuration entries for the example.

![Image of Output storage](./images/configuration-entries.jpg)


