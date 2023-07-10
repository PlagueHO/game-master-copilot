using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMCopilot.Shared.Models
{
    public enum GeneratedContentMethod
    {
        None,
        Manual,
        AI,
        Random
    }

    /// <summary>
    /// Class to represent how the source content was generated.
    /// </summary>
    public class GeneratedContent
    {
        /// <summary>
        /// The method that was used to generate this content.
        /// </summary>
        [JsonProperty(PropertyName = "method")]
        public GeneratedContentMethod Method { get; set; } = GeneratedContentMethod.Manual;

        /// <summary>
        /// The Semantic Kernel plugin that was used to generate this content if the method was FoundationalModel.
        /// </summary>
        [JsonProperty(PropertyName = "plugin")]
        public String? Plugin { get; set; }

        /// <summary>
        /// The Semantic Kernel function in the plugin that was used to generate this content if the method was FoundationalModel.
        /// </summary>
        [JsonProperty(PropertyName = "function")]
        public String? Function { get; set; }

        /// <summary>
        /// The input parameters that was passed to the Semantic Kernel function that was used to generate this content if the method was FoundationalModel.
        /// </summary>
        [JsonProperty(PropertyName = "parameters")]
        public Dictionary<String, String>? Parameters { get; set; }
    }
}
