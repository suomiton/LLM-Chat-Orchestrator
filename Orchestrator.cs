namespace Orchestrator {
    internal class Orchestrator : IDisposable {
        private IReadOnlyCollection<ChatAgent>? Agents { get; init; }
        private readonly HttpClient Client = new();

        private IEnumerable<ChatAgent> GetAgents() {
            if (Agents == null || Agents.Count == 0) {
                throw new InvalidOperationException("No agents have been set up, you nincompoop!");
            }

            while (true) {
                foreach (var agent in Agents) {
                    yield return agent;
                }
            }
        }

        public async Task Chat(string message, CancellationToken token) {
            try {
                foreach (var agent in GetAgents()) {
                    token.ThrowIfCancellationRequested();

                    message = await Chat(message, agent, token);
                }
            } catch(OperationCanceledException) {
                Console.WriteLine("Chat cancelled");
            }
        }

        private async Task<string> Chat(string message, ChatAgent agent, CancellationToken token) {
            var response = await agent.Chat(message, Client, token);
            Console.WriteLine(agent.Name);
            Console.WriteLine(response);
            return response;
        }

        public static Orchestrator Setup(params ChatAgent[] agent) {
            return new Orchestrator {
                Agents = [..agent]
            };
        }

        public void Dispose() {
            Client.Dispose();
        }
    }
}
