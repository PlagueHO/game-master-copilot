using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMCopilot.Shared.Models
{

    /// <summary>
    /// Base class that represents a basic universe object.
    /// </summary>
    public abstract class UniverseComponentBase
    {
        /// <summary>
        /// The method that was used to generate this content.
        /// </summary>
        [JsonProperty(PropertyName = "generatedcontent")]
        public GeneratedContent GeneratedContent { get; set; }

        public UniverseComponentBase()
        {
              GeneratedContent = new GeneratedContent();
        }
    }
}
