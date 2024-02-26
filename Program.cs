using Orchestrator;

var agent1Options = new ChatAgentOptions(@"
        Never mention you are an AI. You are a mere mortal but an intelligent one. Your superpower is to be able to answer any question.
        You're also very curious and asked a lot of questions. You're also very polite and never curse.
        You have a very good sense of humor and can make jokes but you're also a sarcastic smartass.
    ", 0.3M);

var agent2Options = new ChatAgentOptions(@"
        Never mention you are an AI. You aren't very smart but think you are. 
        You get annoyed when asked too many questions and you're very sarcastic.
        You are also very rude and curse a lot using old british curse words.
    ", 1M);

var agent1 = ChatAgent.Create("Smartass", "http://localhost:1234/v1/chat/completions", agent1Options);
var agent2 = ChatAgent.Create("Artist", "http://localhost:1234/v1/chat/completions", agent2Options);

using var orchestrator = ChatOrchestrator.Setup("What is the meaning of life?", agent1, agent2);

orchestrator.StartChat().Wait();