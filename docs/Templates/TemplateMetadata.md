# KMD Logic Document Generation Template Metadata

## Introduction

Additional information about a [template](./Templates.md) can be provided in template metadata files.
For example, the schema definition of the merge data for a template might be provided in that template's metadata file so that an application can better determine what merge data to provide to the document generation process for a particular template.

## Location

Template metadata files should be copied into the template storage hierarchy along side their corresponding template files.
Template metadata files are discovered in the same way as templates themselves are discovered, by configuration, hierarchy path, template id, and language.

Template metadata file dicovery follows the same multilanguage search used for template discovery.  See [here](./Templates.md#multi-language-support "Multi Language Support").

## Extension

The filename extension of template metadata files is peculiar to a given configuration. The default extension is ```json```, but this can be overridden by setting a configuration's ```MetadataFilenameExtension``` property.

## Downloading Template Metadata

You can download the metadata for a template using the [document generation client](../Generation/GenerationAndConversionApi.md#getmetadata "GetMetadata()").
