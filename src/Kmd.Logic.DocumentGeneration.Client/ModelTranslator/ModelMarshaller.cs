﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Kmd.Logic.DocumentGeneration.Client.Configuration;
using Kmd.Logic.DocumentGeneration.Client.Configuration.TemplateStorageConfigurations;
using Kmd.Logic.DocumentGeneration.Client.Models;
using Kmd.Logic.DocumentGeneration.Client.ServiceMessages;
using Kmd.Logic.DocumentGeneration.Client.Types;
using Microsoft.Rest;

namespace Kmd.Logic.DocumentGeneration.Client.ModelTranslator
{
    internal static class ModelMarshaller
    {
        internal static async Task<DocumentGenerationTemplateStorageDirectoryDetails> ToEntryDetails(
            this Task<AzureBlobStorageConfigurationDocumentGenerationConfigurationEntryDetails> responseTask)
        {
            var response =
                await responseTask.ConfigureAwait(false);
            return response.ToEntryDetails();
        }

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

        internal static async Task<DocumentGenerationProgress> ToDocumentGenerationProgress(
            this Task<DocumentGenerationRequest> documentGenerationRequestTask)
        {
            var response =
                await documentGenerationRequestTask.ConfigureAwait(false);
            return response?.ToDocumentGenerationProgress();
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

        internal static async Task<DocumentGenerationUri> ToDocumentGenerationUri(
            this Task<DocumentUri> documentGenerationUri)
        {
            var response =
                await documentGenerationUri.ConfigureAwait(false);
            return response?.ToDocumentGenerationUri();
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

        internal static async Task<DocumentGenerationTemplateStorageDirectoryDetails> ToEntryDetails(
            this Task<SharePointOnlineTemplateStorageConfigurationDocumentGenerationConfigurationEntryDetails> responseTask)
        {
            var response =
                await responseTask.ConfigureAwait(false);
            return response.ToEntryDetails();
        }

        internal static DocumentGenerationTemplateStorageDirectoryDetails ToEntryDetails(
            this SharePointOnlineTemplateStorageConfigurationDocumentGenerationConfigurationEntryDetails response)
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
                documentGenerationRequestDetails.Debug,
                documentGenerationRequestDetails.SignConfigurationId);
        }

        internal static CreateConfigurationRequest ToCreateConfigurationRequest(
            this DocumentGenerationConfiguration documentGenerationConfiguration)
        {
            return new CreateConfigurationRequest(documentGenerationConfiguration.Name, documentGenerationConfiguration.HasLicense, documentGenerationConfiguration.LevelNames?.ToList(), documentGenerationConfiguration.MetadataFilenameExtension);
        }

        internal static UpdateConfigurationRequest ToUpdateConfigurationRequest(
            this DocumentGenerationConfiguration documentGenerationConfiguration)
        {
            return new UpdateConfigurationRequest(documentGenerationConfiguration.Name, documentGenerationConfiguration.HasLicense, documentGenerationConfiguration.LevelNames?.ToList(), documentGenerationConfiguration.MetadataFilenameExtension);
        }

        internal static Task<DocumentGenerationTemplateStorageDirectoryDetails> GetEntryDetailsFromServer(
            this DocumentGenerationTemplateStorageDirectorySkeleton entrySkeleton)
        {
            var templateStorageType = entrySkeleton.TemplateStorageType;
            var documentGenerationEntryDetails = templateStorageType switch
            {
                TemplateStorageType.AzureBlobStorage =>
                entrySkeleton
                    .InternalClient
                    .GetAzureBlobEntryAtIdWithHttpMessagesAsync(
                        entrySkeleton.SubscriptionId,
                        entrySkeleton.ConfigurationId,
                        entrySkeleton.Id)
                    .ValidateBody()
                    .ToEntryDetails(),
                TemplateStorageType.SharePointOnline =>
                entrySkeleton
                    .InternalClient
                    .GetSharePointOnlineEntryAtIdWithHttpMessagesAsync(
                        entrySkeleton.SubscriptionId,
                        entrySkeleton.ConfigurationId,
                        entrySkeleton.Id)
                    .ValidateBody()
                    .ToEntryDetails(),
                _ => throw new DocumentGenerationConfigurationException(
                    $"Unsupported Template Storage Type: {templateStorageType}")
            };
            return documentGenerationEntryDetails;
        }

        internal static Task DeleteEntryOnServer(
            this DocumentGenerationTemplateStorageDirectorySkeleton entrySkeleton)
        {
            switch (entrySkeleton.TemplateStorageType)
            {
                case TemplateStorageType.AzureBlobStorage:
                    return entrySkeleton.InternalClient.DeleteAzureBlobEntryAtIdWithHttpMessagesAsync(
                        entrySkeleton.SubscriptionId,
                        entrySkeleton.ConfigurationId,
                        entrySkeleton.Id);
                case TemplateStorageType.SharePointOnline:
                    return entrySkeleton.InternalClient.DeleteSharePointOnlineEntryAtIdWithHttpMessagesAsync(
                        entrySkeleton.SubscriptionId,
                        entrySkeleton.ConfigurationId,
                        entrySkeleton.Id);
                default:
                    return entrySkeleton.InternalClient.DeleteAzureBlobEntryAtIdWithHttpMessagesAsync(
                        entrySkeleton.SubscriptionId,
                        entrySkeleton.ConfigurationId,
                        entrySkeleton.Id);
            }
        }

        internal static Task<DocumentGenerationTemplateStorageDirectoryDetails> UpdateEntryOnServer(
            this DocumentGenerationTemplateStorageDirectory entry,
            DocumentGenerationTemplateStorageDirectorySkeleton serverSkeleton)
        {
            var hierarchyPathString = entry.HierarchyPath.ReplaceLeaf(serverSkeleton.Key).ToString();
            switch (entry.TemplateStorageConfiguration.TemplateStorageType)
            {
                case TemplateStorageType.AzureBlobStorage:
                    return
                        entry.InternalClient
                            .UpdateAzureBlobEntryAtPathWithHttpMessagesAsync(
                                entry.SubscriptionId,
                                entry.ConfigurationId,
                                hierarchyPathString,
                                new AzureBlobTemplateModelUpdateEntryAtPathRequest(
                                    entry.Key,
                                    entry.Name,
                                    (entry.TemplateStorageConfiguration as AzureBlobTemplateStorage).ToModel()))
                            .ValidateBody()
                            .ToEntryDetails();
                case TemplateStorageType.SharePointOnline:
                    return
                        entry.InternalClient
                            .UpdateSharePointOnlineEntryAtPathWithHttpMessagesAsync(
                                entry.SubscriptionId,
                                entry.ConfigurationId,
                                hierarchyPathString,
                                new SharePointOnlineTemplateModelUpdateEntryAtPathRequest(
                                    entry.Key,
                                    entry.Name,
                                    (entry.TemplateStorageConfiguration as SharePointOnlineTemplateStorage).ToModel()))
                            .ValidateBody()
                            .ToEntryDetails();
                default:
                    throw new DocumentGenerationConfigurationException($"Unknown template storage type {entry.TemplateStorageConfiguration.TemplateStorageType}");
            }
        }

        internal static Task<DocumentGenerationTemplateStorageDirectoryDetails> CreateEntryOnServer(this DocumentGenerationTemplateStorageDirectory entry)
        {
            var parentHierarchyPathString = entry.ParentHierarchyPath.ToString();
            switch (entry.TemplateStorageConfiguration.TemplateStorageType)
            {
                case TemplateStorageType.AzureBlobStorage:
                    return
                        entry.InternalClient
                            .CreateAzureBlobEntryUnderPathWithHttpMessagesAsync(
                                entry.SubscriptionId,
                                entry.ConfigurationId,
                                parentHierarchyPathString,
                                new AzureBlobTemplateModelCreateEntryUnderPathRequest(
                                    entry.Key,
                                    entry.Name,
                                    (entry.TemplateStorageConfiguration as AzureBlobTemplateStorage).ToModel()))
                            .ValidateBody()
                            .ToEntryDetails();
                case TemplateStorageType.SharePointOnline:
                    return
                        entry.InternalClient
                            .CreateSharePointOnlineEntryUnderPathWithHttpMessagesAsync(
                                entry.SubscriptionId,
                                entry.ConfigurationId,
                                parentHierarchyPathString,
                                new SharePointOnlineTemplateModelCreateEntryUnderPathRequest(
                                    entry.Key,
                                    entry.Name,
                                    (entry.TemplateStorageConfiguration as SharePointOnlineTemplateStorage).ToModel()))
                            .ValidateBody()
                            .ToEntryDetails();
                default:
                    throw new DocumentGenerationConfigurationException($"Unknown template storage type {entry.TemplateStorageConfiguration.TemplateStorageType}");
            }
        }

        internal static async Task<T> ValidateBody<T>(
            this IHttpOperationResponse<T> httpOperationResponse,
            [System.Runtime.CompilerServices.CallerMemberName] string operation = "Unknown method")
        {
            if (httpOperationResponse != null && httpOperationResponse.Body != null)
            {
                return httpOperationResponse.Body;
            }

            if (httpOperationResponse?.Response == null)
            {
                throw new DocumentGenerationException($"{operation}: Failed.");
            }

            if (httpOperationResponse.Response.Content == null)
            {
                throw new DocumentGenerationException(
                    $"{operation}: {httpOperationResponse.Response.ReasonPhrase}");
            }

            var content = await httpOperationResponse.Response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new DocumentGenerationException(
                $"{operation}: {httpOperationResponse.Response.ReasonPhrase}: {content}");
        }

        internal static async Task<T> ValidateBody<T>(
            this Task<HttpOperationResponse<T>> httpOperationResponseTask,
            [System.Runtime.CompilerServices.CallerMemberName] string operation = "Unknown method")
        {
            var httpOperationResponse = await httpOperationResponseTask.ConfigureAwait(false);
            return await httpOperationResponse.ValidateBody(operation).ConfigureAwait(false);
        }

        internal static async Task<Stream> ValidateContentStream(
            this HttpOperationResponse httpOperationResponse,
            [System.Runtime.CompilerServices.CallerMemberName] string operation = "Unknown method")
        {
            if (httpOperationResponse?.Response == null)
            {
                throw new DocumentGenerationException($"{operation}: Failed.");
            }

            if (httpOperationResponse.Response.StatusCode == HttpStatusCode.OK)
            {
                if (httpOperationResponse.Response.Content == null)
                {
                    return Stream.Null;
                }

                return await httpOperationResponse.Response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }

            if (httpOperationResponse.Response.Content == null)
            {
                throw new DocumentGenerationException(
                    $"{operation}: {httpOperationResponse.Response.ReasonPhrase}");
            }

            var content = await httpOperationResponse.Response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new DocumentGenerationException(
                $"{operation}: {httpOperationResponse.Response.ReasonPhrase}: {content}");
        }

        internal static async Task ValidateResponse(
            this Task<HttpOperationResponse> httpOperationResponseTask,
            [System.Runtime.CompilerServices.CallerMemberName] string operation = "Unknown method")
        {
            var response = await httpOperationResponseTask.ConfigureAwait(false);
            if (response?.Response == null)
            {
                throw new DocumentGenerationException($"{operation}: Failed.");
            }

            if (response.Response.StatusCode == HttpStatusCode.OK)
            {
                return;
            }

            if (response.Response.Content == null)
            {
                throw new DocumentGenerationException(
                    $"{operation}: {response.Response.ReasonPhrase}");
            }

            var content = await response.Response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new DocumentGenerationException(
                $"{operation}: {response.Response.ReasonPhrase}: {content}");
        }

        internal static DocumentGenerationException DocumentGenerationThrow(
            this HttpOperationException httpOperationException,
            [System.Runtime.CompilerServices.CallerMemberName] string operation = "Unknown method")
        {
            var reason = httpOperationException.Response?.ReasonPhrase;
            var content = httpOperationException.Response?.Content;
            var message = $"{operation} Failed";
            if (!string.IsNullOrWhiteSpace(reason))
            {
                message += $": {reason}";
            }

            if (!string.IsNullOrWhiteSpace(content))
            {
                message += $": {content}";
            }

            return new DocumentGenerationException(message, httpOperationException);
        }
    }
}
