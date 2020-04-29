using System;
using System.Linq;
using Kmd.Logic.DocumentGeneration.Client.Configuration;
using Kmd.Logic.DocumentGeneration.Client.Configuration.TemplateStorageConfigurations;
using Kmd.Logic.DocumentGeneration.Client.Models;
using Kmd.Logic.DocumentGeneration.Client.ServiceMessages;
using Kmd.Logic.DocumentGeneration.Client.Types;

namespace Kmd.Logic.DocumentGeneration.Client.ModelTranslator
{
    internal static class ModelMarshaller
    {
        internal static DocumentGenerationTemplateStorageDirectoryDetails ToEntryDetails(
            this AzureBlobStorageConfigurationDocumentGenerationConfigurationEntryDetails response)
        {
            return new DocumentGenerationTemplateStorageDirectoryDetails
            {
                Id = response.Id ??
                     throw new DocumentGenerationConfigurationException("Found null entry Id in response"),
                Key = response.Key,
                Name = response.Name,
                TemplateStorageConfiguration = new AzureBlobTemplateStorage(response.TemplateStorageConfiguration),
            };
        }

        internal static DocumentGenerationProgress ToDocumentGenerationProgress(
            this DocumentGenerationRequest documentGenerationRequest)
        {
            return new DocumentGenerationProgress
            {
                Id = documentGenerationRequest.Id ?? throw new DocumentGenerationException("ConfigurationId cannot be null"),
                SubscriptionId = documentGenerationRequest.SubscriptionId ?? throw new DocumentGenerationException("SubscriptionId cannot be null"),
                TemplateId = documentGenerationRequest.TemplateId,
                Language = documentGenerationRequest.Language,
                DocumentFormat = documentGenerationRequest.DocumentFormat.ToDocumentFormat(),
                HierarchyPath = new HierarchyPath(documentGenerationRequest.HierarchyPath),
                State = documentGenerationRequest.State.ToDocumentGenerationState(),
                CallbackUrl = documentGenerationRequest.CallbackUrl,
                Debug = documentGenerationRequest.Debug,
                FailReason = documentGenerationRequest.FailReason,
            };
        }

        internal static DocumentGenerationUri ToDocumentGenerationUri(this DocumentUri documentGenerationUri)
        {
            return new DocumentGenerationUri
            {
                Uri = new Uri(documentGenerationUri.Uri),
                UriExpiryTime = documentGenerationUri.UriExpiryTime ?? DateTime.Now,
            };
        }

        internal static DocumentGenerationTemplate ToDocumentGenerationTemplate(this Template template)
        {
            return new DocumentGenerationTemplate
            {
                TemplateId = template.TemplateId,
                Languages = template.Languages?.ToArray(),
            };
        }

        internal static DocumentGenerationConfigurationListItem ToDocumentGenerationConfigurationListItem(this ConfigurationListResponse configurationListResponse)
        {
            return new DocumentGenerationConfigurationListItem
            {
                ConfigurationId = configurationListResponse.Id ?? throw new DocumentGenerationException("ConfigurationId cannot be null"),
                Name = configurationListResponse.Name,
            };
        }

        internal static AzureBlobTemplateModel ToModel(this AzureBlobTemplateStorage templateStorageConfiguration)
        {
            return new AzureBlobTemplateModel(
                templateStorageConfiguration.SecretKeyOrStorageConnectionString,
                templateStorageConfiguration.ContainerName,
                templateStorageConfiguration.BlobPrefix);
        }

        internal static DocumentGenerationTemplateStorageDirectoryDetails ToEntryDetails(this SharePointOnlineTemplateStorageConfigurationDocumentGenerationConfigurationEntryDetails response)
        {
            return new DocumentGenerationTemplateStorageDirectoryDetails
            {
                Id = response.Id ?? throw new DocumentGenerationConfigurationException("Found null entry Id in response"),
                Key = response.Key,
                Name = response.Name,
                TemplateStorageConfiguration = new SharePointOnlineTemplateStorage(response.TemplateStorageConfiguration),
            };
        }

        internal static SharePointOnlineTemplateModel ToModel(this SharePointOnlineTemplateStorage templateStorageConfiguration)
        {
            return new SharePointOnlineTemplateModel(
                templateStorageConfiguration.ClientId,
                templateStorageConfiguration.TenantId,
                templateStorageConfiguration.SecretKeyOrClientSecret,
                templateStorageConfiguration.GroupName);
        }

        internal static TemplateStorageType ToTemplateStorageType(this string templateStorageTypeString)
        {
            try
            {
                return (TemplateStorageType)Enum.Parse(typeof(TemplateStorageType), templateStorageTypeString);
            }
            catch (ArgumentException)
            {
                return TemplateStorageType.Deprecated;
            }
        }

        internal static bool IsDeprecated(this DocumentGenerationConfigurationEntrySummary documentGenerationConfigurationEntrySummary)
        {
            return
                documentGenerationConfigurationEntrySummary.TemplateStorageType.ToTemplateStorageType() == TemplateStorageType.Deprecated;
        }

        internal static bool IsDeprecated(this DocumentGenerationTemplateStorageDirectorySkeleton documentGenerationTemplateStorageDirectorySkeleton)
        {
            return
                documentGenerationTemplateStorageDirectorySkeleton.TemplateStorageType == TemplateStorageType.Deprecated;
        }

        internal static DocumentFormat ToDocumentFormat(this string documentFormatString)
        {
            return (DocumentFormat)Enum.Parse(typeof(DocumentFormat), documentFormatString);
        }

        internal static DocumentGenerationState ToDocumentGenerationState(this string documentGenerationState)
        {
            return (DocumentGenerationState)Enum.Parse(typeof(DocumentGenerationState), documentGenerationState);
        }

        internal static PdfFormat ToPdfFormat(this string pdfFormatString)
        {
            return (PdfFormat)Enum.Parse(typeof(PdfFormat), pdfFormatString);
        }

        internal static GenerateDocumentConversionRequest ToWebRequest(
            this DocumentConversionRequestDetails documentConversionRequestDetails, Guid configurationId)
        {
            return new GenerateDocumentConversionRequest(
                configurationId,
                documentConversionRequestDetails.SourceDocumentUrl.ToString(),
                documentConversionRequestDetails.SourceDocumentFormat.ToString(),
                documentConversionRequestDetails.ConvertedDocumentFormat.ToString(),
                documentConversionRequestDetails.ConvertedDocumentPdfFormat.ToString(),
                documentConversionRequestDetails.CallbackUrl?.ToString(),
                documentConversionRequestDetails.Debug);
        }

        internal static GenerateDocumentRequest ToWebRequest(
            this DocumentGenerationRequestDetails documentGenerationRequestDetails, Guid configurationId)
        {
            return new GenerateDocumentRequest(
                configurationId,
                documentGenerationRequestDetails.HierarchyPath,
                documentGenerationRequestDetails.TemplateId,
                documentGenerationRequestDetails.TwoLetterIsoLanguageName,
                documentGenerationRequestDetails.DocumentFormat.ToString(),
                documentGenerationRequestDetails.MergeData,
                documentGenerationRequestDetails.CallbackUrl?.ToString(),
                documentGenerationRequestDetails.Debug);
        }

        internal static CreateConfigurationRequest ToCreateConfigurationRequest(
            this DocumentGenerationConfiguration documentGenerationConfiguration)
        {
            return new CreateConfigurationRequest(documentGenerationConfiguration.Name, documentGenerationConfiguration.HasLicense, documentGenerationConfiguration.LevelNames?.ToList());
        }

        internal static UpdateConfigurationRequest ToUpdateConfigurationRequest(
            this DocumentGenerationConfiguration documentGenerationConfiguration)
        {
            return new UpdateConfigurationRequest(documentGenerationConfiguration.Name, documentGenerationConfiguration.HasLicense, documentGenerationConfiguration.LevelNames?.ToList());
        }

        internal static DocumentGenerationTemplateStorageDirectoryDetails GetEntryDetailsFromServer(
            this DocumentGenerationTemplateStorageDirectorySkeleton entrySkeleton)
        {
            var templateStorageType = entrySkeleton.TemplateStorageType;
            var documentGenerationEntryDetails = templateStorageType switch
            {
                TemplateStorageType.AzureBlobStorage =>
                entrySkeleton.InternalClient.GetAzureBlobEntryAtId(
                    entrySkeleton.SubscriptionId,
                    entrySkeleton.ConfigurationId,
                    entrySkeleton.Id).ToEntryDetails(),
                TemplateStorageType.SharePointOnline =>
                entrySkeleton.InternalClient.GetSharePointOnlineEntryAtId(
                    entrySkeleton.SubscriptionId,
                    entrySkeleton.ConfigurationId,
                    entrySkeleton.Id).ToEntryDetails(),
                _ => throw new DocumentGenerationConfigurationException(
                    $"Unsupported Template Storage Type: {templateStorageType}")
            };
            return documentGenerationEntryDetails;
        }

        internal static void DeleteEntryOnServer(
            this DocumentGenerationTemplateStorageDirectorySkeleton entrySkeleton)
        {
            switch (entrySkeleton.TemplateStorageType)
            {
                case TemplateStorageType.AzureBlobStorage:
                    entrySkeleton.InternalClient.DeleteAzureBlobEntryAtId(entrySkeleton.SubscriptionId, entrySkeleton.ConfigurationId, entrySkeleton.Id);
                    break;
                case TemplateStorageType.SharePointOnline:
                    entrySkeleton.InternalClient.DeleteSharePointOnlineEntryAtId(entrySkeleton.SubscriptionId, entrySkeleton.ConfigurationId, entrySkeleton.Id);
                    break;
                default:
                    entrySkeleton.InternalClient.DeleteAzureBlobEntryAtId(entrySkeleton.SubscriptionId, entrySkeleton.ConfigurationId, entrySkeleton.Id);
                    break;
            }
        }

        internal static DocumentGenerationTemplateStorageDirectoryDetails UpdateEntryOnServer(
            this DocumentGenerationTemplateStorageDirectory entry,
            DocumentGenerationTemplateStorageDirectorySkeleton serverSkeleton)
        {
            var hierarchyPathString = entry.HierarchyPath.ReplaceLeaf(serverSkeleton.Key).ToString();
            switch (entry.TemplateStorageConfiguration.TemplateStorageType)
            {
                case TemplateStorageType.AzureBlobStorage:
                    return entry.InternalClient.UpdateAzureBlobEntryAtPath(
                        entry.SubscriptionId,
                        entry.ConfigurationId,
                        hierarchyPathString,
                        new AzureBlobTemplateModelUpdateEntryAtPathRequest(
                            entry.Key,
                            entry.Name,
                            (entry.TemplateStorageConfiguration as AzureBlobTemplateStorage).ToModel()))
                        .ToEntryDetails();
                case TemplateStorageType.SharePointOnline:
                    return entry.InternalClient.UpdateSharePointOnlineEntryAtPath(
                        entry.SubscriptionId,
                        entry.ConfigurationId,
                        hierarchyPathString,
                        new SharePointOnlineTemplateModelUpdateEntryAtPathRequest(
                            entry.Key,
                            entry.Name,
                            (entry.TemplateStorageConfiguration as SharePointOnlineTemplateStorage).ToModel()))
                        .ToEntryDetails();
                default:
                    throw new DocumentGenerationConfigurationException($"Unknown template storage type {entry.TemplateStorageConfiguration.TemplateStorageType}");
            }
        }

        internal static DocumentGenerationTemplateStorageDirectoryDetails CreateEntryOnServer(this DocumentGenerationTemplateStorageDirectory entry)
        {
            var parentHierarchyPathString = entry.ParentHierarchyPath.ToString();
            switch (entry.TemplateStorageConfiguration.TemplateStorageType)
            {
                case TemplateStorageType.AzureBlobStorage:
                    return entry.InternalClient.CreateAzureBlobEntryUnderPath(
                            entry.SubscriptionId,
                            entry.ConfigurationId,
                            parentHierarchyPathString,
                            new AzureBlobTemplateModelCreateEntryUnderPathRequest(
                                entry.Key,
                                entry.Name,
                                (entry.TemplateStorageConfiguration as AzureBlobTemplateStorage).ToModel()))
                        .ToEntryDetails();
                case TemplateStorageType.SharePointOnline:
                    return entry.InternalClient.CreateSharePointOnlineEntryUnderPath(
                            entry.SubscriptionId,
                            entry.ConfigurationId,
                            parentHierarchyPathString,
                            new SharePointOnlineTemplateModelCreateEntryUnderPathRequest(
                                entry.Key,
                                entry.Name,
                                (entry.TemplateStorageConfiguration as SharePointOnlineTemplateStorage).ToModel()))
                        .ToEntryDetails();
                default:
                    throw new DocumentGenerationConfigurationException($"Unknown template storage type {entry.TemplateStorageConfiguration.TemplateStorageType}");
            }
        }
    }
}
