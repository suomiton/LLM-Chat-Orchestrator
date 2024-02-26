using Orchestrator;

try {
    var agent1 = ChatAgent.Create("Törppö", "http://localhost:1234/v1/chat/completions", new ChatAgentOptions("Give a brief answer and ask more quetions about the matter"));
    var agent2 = ChatAgent.Create("Tenho", "http://localhost:1234/v1/chat/completions", new ChatAgentOptions("Curse a lot"));

    using var orchestrator = Orchestrator.Orchestrator.Setup(agent1, agent2);

    var cancellationToken = new CancellationTokenSource();
    var chatTask = orchestrator.Chat("What is the meaning of life?", cancellationToken.Token);


    while (!chatTask.IsCompleted) {
        Console.WriteLine("Enter command ('c: change subject', 'q: stop conversation'): ");
        var command = Console.ReadLine()?.ToLower();

        switch (command) {
        case "c":
            Console.WriteLine("Enter new subject: ");
            var newSubject = Console.ReadLine();

            cancellationToken.Cancel();
            await chatTask; // Ensure previous task is properly finished

            cancellationToken = new CancellationTokenSource(); // Create a new token source

            if(string.IsNullOrWhiteSpace(newSubject)) {
                Console.WriteLine("Subject cannot be empty, you dolt!");
                break;
            }

            chatTask = orchestrator.Chat(newSubject, cancellationToken.Token); // Start new chat
            break;
        case "q":
            cancellationToken.Cancel();
            return;
        default:
            Console.WriteLine("Invalid command, you dunderhead!");
            break;
        }
    }
    await chatTask; // Ensure the chat task is completed before exiting
}
catch (OperationCanceledException) {
    Console.WriteLine("\nChat was canceled.");
}
catch (Exception e) {
    Console.WriteLine("\nException Caught!");
    Console.WriteLine("Message :{0} ", e.Message);
}