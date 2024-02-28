// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;

namespace Microsoft.SemanticKernel.Connectors.Mistral;

/// <summary>
/// Extensions for <see cref="KernelFunctionMetadata"/> specific to the Mistral connector.
/// </summary>
public static class MistralKernelFunctionMetadataExtensions
{
    /// <summary>
    /// Convert a <see cref="KernelFunctionMetadata"/> to an <see cref="MistralFunction"/>.
    /// </summary>
    /// <param name="metadata">The <see cref="KernelFunctionMetadata"/> object to convert.</param>
    /// <returns>An <see cref="MistralFunction"/> object.</returns>
    public static MistralFunction ToMistralFunction(this KernelFunctionMetadata metadata)
    {
        IReadOnlyList<KernelParameterMetadata> metadataParams = metadata.Parameters;

        var MistralParams = new MistralFunctionParameter[metadataParams.Count];
        for (int i = 0; i < MistralParams.Length; i++)
        {
            var param = metadataParams[i];

            MistralParams[i] = new MistralFunctionParameter(
                param.Name,
                GetDescription(param),
                param.IsRequired,
                param.ParameterType,
                param.Schema);
        }

        return new MistralFunction(
            metadata.PluginName,
            metadata.Name,
            metadata.Description,
            MistralParams,
            new MistralFunctionReturnParameter(
                metadata.ReturnParameter.Description,
                metadata.ReturnParameter.ParameterType,
                metadata.ReturnParameter.Schema));

        static string GetDescription(KernelParameterMetadata param)
        {
            if (InternalTypeConverter.ConvertToString(param.DefaultValue) is string stringValue && !string.IsNullOrEmpty(stringValue))
            {
                return $"{param.Description} (default value: {stringValue})";
            }

            return param.Description;
        }
    }
}
