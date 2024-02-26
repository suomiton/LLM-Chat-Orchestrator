using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace Orchestrator {

    internal class ChatAgentOptions {
        public decimal Temperature { get; init; }
        public int Max_Tokens { get; init; }
        public bool Stream { get; init; }
        public ChatInput SystemMessage { get; init; }

        public ChatAgentOptions(string systemGuidance, decimal temp = 0.7m, int tokens = -1, bool stream = false) {
            Temperature = temp;
            Max_Tokens = tokens;
            Stream = stream;
            SystemMessage = new ChatInput {
                Role = "system",
                Content = systemGuidance
            };
        }
    }

    internal class ChatAgent {
        public required string Name { get; init; }
        public required string ServiceUrl { get; init; }

        public required ChatAgentOptions Options { get; init; }

        public static ChatAgent Create(string name, string serviceUrl, ChatAgentOptions options) {
            return new ChatAgent {
                Name = name,
                ServiceUrl = serviceUrl,
                Options = options
            };
        }

        public async Task<string> Chat(string message, HttpClient client, CancellationToken token) {
            var payload = CreatePayload(message);

            using var response = await client.PostAsync(ServiceUrl, payload, token);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync(token);
            var model = JsonConvert.DeserializeObject<ChatResponse>(result);

            return model?.Choices.FirstOrDefault()?.Message.Content ?? string.Empty;
        }

        private StringContent CreatePayload(string message) {
            var options = new {
                Options.Temperature,
                Options.Max_Tokens,
                Options.Stream,
                Messages = new[] {
                    Options.SystemMessage,
                    new ChatInput {
                        Role = "user",
                        Content = message
                    }
                }
            };
            
            var json = JsonConvert.SerializeObject(options, new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
