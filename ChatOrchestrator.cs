namespace Orchestrator {
    internal class ChatOrchestrator : IDisposable {
        private IReadOnlyCollection<ChatAgent>? Agents { get; init; }
        private readonly HttpClient Client = new();
        private CancellationTokenSource CancellationTokenSource { get; set; } = new();
        internal required string CurrentMessage { get; set; }

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

        public async Task StartChat() {
            try {
                await StartChatTask();
            } catch(OperationCanceledException) {
                Console.WriteLine("\n\nChat cancelled");
            }
        }

        public async Task ChangeSubject(string newSubject, CancellationToken token) {
            CurrentMessage = newSubject;
            await StartChat();
        }

        private async Task StartChatTask() {
            foreach (var agent in GetAgents()) {
                CancellationTokenSource.Token.ThrowIfCancellationRequested();

                CurrentMessage = await Chat(CurrentMessage, agent, CancellationTokenSource.Token);
            }
        }

        private async Task<string> Chat(string message, ChatAgent agent, CancellationToken token) {
            var response = await agent.SendMessageAndProcessAnswer(message, Client, token);

            Console.WriteLine($"\n\n");
            Console.WriteLine($">> {agent.Name} <<");
            Console.WriteLine(response);
            
            return response;
        }

        public static ChatOrchestrator Setup(string initialSubject, params ChatAgent[] agent) {
            return new ChatOrchestrator {
                CurrentMessage = initialSubject,
                Agents = [..agent]
            };
        }

        public void Dispose() {
            Client.Dispose();
        }
    }
}
