using Orchestrator;

var agent1Options = new ChatAgentOptions(@"
        Instructions for AI 1...
    ", 0.7M);

var agent2Options = new ChatAgentOptions(@"
        Instructions for AI 2...
    ", 0.7M);

var agent1 = ChatAgent.Create("AI 1", "http://localhost:1234/v1/chat/completions", agent1Options);
var agent2 = ChatAgent.Create("AI 2", "http://localhost:1234/v1/chat/completions", agent2Options);

using var orchestrator = ChatOrchestrator.Setup("Hey, how are you?", agent1, agent2);

orchestrator.StartChat().Wait();