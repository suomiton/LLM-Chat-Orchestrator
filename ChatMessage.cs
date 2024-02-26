namespace Orchestrator {

    internal class ChatInput {
        public string Role { get; set; }
        public string Content { get; set; }
    }

    internal class ChatResponse {
        public string Id { get; set; }
        public string Object { get; set; }
        public int Created { get; set; }
        public string Model { get; set; }
        public ISet<Choice> Choices { get; set; }
        public UsageModel Usage { get; set; }

        internal class Choice {
            public int Index { get; set; }
            public Message Message { get; set; }
            public string Finish_Reason { get; set; }
        }

        internal class Message {
            public string Role { get; set; }
            public string Content { get; set; }
        }

        internal class UsageModel {
            public int Prompt_Tokens { get; set; }
            public int Completion_Tokens { get; set; }
            public int Total_Tokens { get; set; }
        }
    }
}
