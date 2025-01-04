
using Ollama.JsonConverters;
using Ollama;
using System.Net;
using System.Reflection.Metadata;
using System.Reflection;
using System.Text.Json;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using Ollama.IntegrationTests;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace WinFormsApp2
{

    public partial class Form1 : Form
    {
        [StructLayout(LayoutKind.Sequential)]
        class Masterstruct
        {
            public string child;
        }
        Chat chat;
        public Form1()
        {
            InitializeComponent();
            IntPtr data;
            Creo.FetchChildParts(out data);

            int n_Comps = 3; // Replace with the actual count returned by the native function
            Masterstruct[] mans = new Masterstruct[n_Comps];
            IntPtr curr = data;
/*
            for (int i = 0; i < n_Comps; i++)
            {
                // Marshal the struct
                mans[i] = Marshal.PtrToStructure<Masterstruct>(curr);

                // Convert IntPtr to string
                string childName = Marshal.PtrToStringAnsi(mans[i].child);
                Console.WriteLine($"Child {i}: {childName}");

                // Move the pointer to the next struct
                curr += Marshal.SizeOf(typeof(Masterstruct));
            }

            // Free allocated memory
            for (int i = 0; i < n_Comps; i++)
            {
                Marshal.FreeCoTaskMem(mans[i].child);
            }
            Marshal.FreeCoTaskMem(data);*/

            for(int i =0;i<n_Comps;i++)
            {
                mans[i] = new Masterstruct();
                mans[i] = (Masterstruct)Marshal.PtrToStructure(curr, typeof(Masterstruct));
                Marshal.DestroyStructure(curr, typeof(Masterstruct));
                curr= (IntPtr)((long)curr+Marshal.SizeOf(mans[i]));
            }
       Marshal.FreeCoTaskMem(data);     
    }
        async void Initial(string promt)
        {
            using var ollama = new OllamaApiClient();
            var chat = ollama.Chat(
                model: "llama3.2:3b",
                systemMessage: "You chatbot",
                autoCallTools: true);

            var service = new WeatherService();
            chat.AddToolService(service.AsTools().AsOllamaTools(), service.AsCalls());

            try
            {
                _ = await chat.SendAsync(promt);
            }
            finally
            {
                Console.WriteLine(chat.PrintMessages());
                richTextBox1.Text = chat.PrintMessages();
            } 
        }
        static void WriteMail()
        {
            MessageBox.Show("email called");
        }

        static void CreateAppointment(string appointment, DateTime date)
        {
            Console.WriteLine($"Appointment \"{appointment}\" scheduled for {date}.");
        }
        async void LLM()
        {
            var ollamaEndpoint = "http://127.0.0.1:11434";
            var ollamaClient = new HttpClient
            {
                BaseAddress = new Uri(ollamaEndpoint)
            };
            var modeName = "llama3.2-vision:latest";
            if (modeName != string.Empty)
            {
                // chat with the model
                Console.WriteLine("Chatting with the model...");
                Console.WriteLine();
            }


        }
        static async Task<string> SelectOllamaModel(HttpClient ollamaClient)
        {
            var responseMessage = await ollamaClient.GetAsync("/api/tags");
            var content = await responseMessage.Content.ReadAsStringAsync();

            if (responseMessage.StatusCode == HttpStatusCode.OK && content != null)
            {
                // Deserialize the JSON string to the ModelsResponse object
                ModelsResponse modelsResponse = JsonSerializer.Deserialize<ModelsResponse>(content);

                if (modelsResponse != null)
                {
                    // Output the deserialized object
                    for (int i = 0; i < modelsResponse.Models.Count; i++)
                    {
                        // Model? model = modelsResponse.Models[i];
                        //  Console.WriteLine($"({i}) {model.Name}");
                    }

                    Console.WriteLine();
                    Console.WriteLine("Please use the numeric value for the model to interact with.");

                    var userInput = Console.ReadLine();

                    if (!int.TryParse(userInput, out int modelIndex) || modelIndex < 0 || modelIndex >= modelsResponse.Models.Count)
                    {
                        Console.WriteLine("Invalid model index.");

                        return string.Empty;
                    }

                    return "modelsResponse.Models[modelIndex]";
                }
            }

            return string.Empty;
        }

        async void InitalLLM()
        {
            using var ollama = new OllamaApiClient();

            var models = await ollama.Models.ListModelsAsync();

            // Pulling a model and reporting progress
            await foreach (var response in ollama.Models.PullModelAsync("all-minilm", stream: true))
            {
                Console.WriteLine($"{response.Status}. Progress: {response.Completed}/{response.Total}");
            }
            // or just pull the model and wait for it to finish
            await ollama.Models.PullModelAsync("all-minilm").EnsureSuccessAsync();

            // Generating an embedding
            var embedding = await ollama.Embeddings.GenerateEmbeddingAsync(
                model: "all-minilm",
                prompt: "hello");

            // Streaming a completion directly into the console
            // keep reusing the context to keep the chat topic going
            IList<long>? context = null;
            var enumerable = ollama.Completions.GenerateCompletionAsync("llama3.2", "answer 5 random words");
            await foreach (var response in enumerable)
            {
                Console.WriteLine($"> {response.Response}");

                context = response.Context;
            }

            var lastResponse = await ollama.Completions.GenerateCompletionAsync("llama3.2", "answer 123", stream: false, context: context).WaitAsync();
            Console.WriteLine(lastResponse.Response);

            var chat = ollama.Chat("mistral");
            while (true)
            {
                var message = await chat.SendAsync("answer 123");

                Console.WriteLine(message.Content);

                var newMessage = Console.ReadLine();
                await chat.SendAsync(newMessage);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
             Initial(textBox1.Text);            
            
        }
        private async void SendButton_Click(object sender, EventArgs e)
        {
            // Get user input and display it in the chat
            var userInput = textBox1.Text;
            if (string.IsNullOrWhiteSpace(userInput))
                return;

            richTextBox1.Text += $"User: {userInput}\n";

            // Send the input to the chat and await the response
            await chat.SendAsync(userInput);

            // Display the bot's response
            var botResponse = chat.History.Last().Content.ToString();
            richTextBox1.Text += $"Bot: {botResponse}\n\n";

            // Clear the user input box
            textBox1.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
