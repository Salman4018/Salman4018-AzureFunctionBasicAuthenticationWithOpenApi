// See https://aka.ms/new-console-template for more information
using ApiClient.Client;
using System.Net.Http.Headers;

Console.WriteLine("Starting ApiClient Test");


const string clientId = "Salman";
const string clientSecret = @"12345";

var authenticationString = $"{clientId}:{clientSecret}";
var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authenticationString));

var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);

var testClient = new Client(client);

while (true)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Please enter your name.");
    var name = Console.ReadLine();

    var userGreetings = await testClient.RunAsync($"{name}", CancellationToken.None);
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{userGreetings}\r\n\r\n");
}