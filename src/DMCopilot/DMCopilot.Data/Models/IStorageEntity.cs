using Newtonsoft.Json;

namespace DMCopilot.Data.Models
{
    public interface IStorageEntity<T>
    {
        /// <summary>
        /// Gets or sets the unique identifier for the storage entity.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public T Id { get; set; }
    }
}
