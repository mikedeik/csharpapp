using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CSharpApp.Core.Dtos.Converters {

    /// <summary>
    /// Custom Converter for the images array since the response from the external api is returning a malformed json
    /// </summary>
    public class ImagesArrayJsonConverter : JsonConverter<List<string>> {

        public override List<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            var images = new List<string>();

            // Read the JSON array
            while (reader.Read()) {
                if (reader.TokenType == JsonTokenType.EndArray) {
                    break;
                }

                if (reader.TokenType == JsonTokenType.String) {
                    var imageUrl = reader.GetString();
                    if (imageUrl != null) {
                        // Step 1: Remove all occurrences of \"
                        imageUrl = imageUrl.Replace("\\\"", "");

                        // Step 2: Trim any leading or trailing [, ], or "
                        imageUrl = imageUrl.TrimStart('[').TrimEnd(']').Trim('"');
                    }
                    images.Add(imageUrl);
                }
            }

            return images;
        }


        public override void Write(Utf8JsonWriter writer, List<string> value, JsonSerializerOptions options) {
            writer.WriteStartArray();
            foreach (var image in value) {
                writer.WriteStringValue(image);
            }
            writer.WriteEndArray();
        }

    }
}
    
