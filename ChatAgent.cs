using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Json;
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

        public IList<ChatMessage> Messages { get; init; } = new List<ChatMessage>();

        public static ChatAgent Create(string name, string serviceUrl, ChatAgentOptions options) {
            return new ChatAgent {
                Name = name,
                ServiceUrl = serviceUrl,
                Options = options
            };
        }

        public async Task<string> SendMessageAndProcessAnswer(string message, HttpClient client, CancellationToken token) {
            var payload = CreatePayload(message);

            using var response = await client.PostAsync(ServiceUrl, payload, token);
            
            return await HandleResponse(response, token);
        }

        private async Task<string> HandleResponse(HttpResponseMessage response, CancellationToken token) {
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ChatResponse>(token);
            var message = result?.Choices.FirstOrDefault()?.Message.Content ?? string.Empty;

            Messages.Add(new ChatMessage {
                Message = message
            });

            return message;
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
