using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HttpClientSample
{
    class Program
    {
        //refer: https://www.asp.net/web-api/overview/advanced/calling-a-web-api-from-a-net-client
        static HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        static void ShowAuthor(Author author)
        {
            Console.WriteLine($"Name: {author.Name}\t");
        }

        static async Task<Author> GetAuthorAsync(string path)
        {
            Author author = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                author = await response.Content.ReadAsAsync<Author>();
            }
            return author;
        }

        static async Task<Uri> CreateAuthorAsync(Author author)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("api/authors", author);
            response.EnsureSuccessStatusCode();

            // Return the URI of the created resource.
            return response.Headers.Location;
        }

        static async Task<Author> UpdateAuthorAsync(Author author)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync($"api/authors/{author.Id}", author);
            response.EnsureSuccessStatusCode();

            // Deserialize the updated product from the response body.
            author = await response.Content.ReadAsAsync<Author>();
            return author;
        }

        static async Task<HttpStatusCode> DeleteAuthorAsync(int id)
        {
            HttpResponseMessage response = await client.DeleteAsync($"api/authors/{id}");
            return response.StatusCode;
        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("http://localhost:64302/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Create a new author
                Author author = new Author { Name = "Gizmo" };

                var url = await CreateAuthorAsync(author);
                Console.WriteLine($"Created at {url}. Press any key to continue");
                Console.ReadKey();

                // Get the author
                author = await GetAuthorAsync(url.PathAndQuery);
                ShowAuthor(author);

                // Update the author
                Console.WriteLine("Updating name...");
                author.Name = "222222";
                await UpdateAuthorAsync(author);
                Console.WriteLine($"Updated name. Press any key to continue");
                Console.ReadKey();

                // Get the updated product
                author = await GetAuthorAsync(url.PathAndQuery);
                ShowAuthor(author);

                // Delete the product
                var statusCode = await DeleteAuthorAsync(author.Id);
                Console.WriteLine($"Deleted (HTTP Status = {(int)statusCode})");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}
