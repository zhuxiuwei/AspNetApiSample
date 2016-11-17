using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using BookService.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class TestWebAPI
    {
        internal  string Serialize<T>(MediaTypeFormatter formatter, T value)
        {
            // Create a dummy HTTP Content.
            Stream stream = new MemoryStream();
            var content = new StreamContent(stream);
            /// Serialize the object.
            formatter.WriteToStreamAsync(typeof(T), value, stream, content, null).Wait();
            // Read the serialized string.
            stream.Position = 0;
            return content.ReadAsStringAsync().Result;
        }
        internal  T Deserialize<T>(MediaTypeFormatter formatter, string str) where T : class
        {
            // Write the serialized string to a memory stream.
            Stream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            // Deserialize to an object of type T
            return formatter.ReadFromStreamAsync(typeof(T), stream, null, null).Result as T;
        }
        [TestMethod]
        public  void TestSerialization()
        {
            var value = new Author() { Name = "Alice", Id = 23 };

            var xml = new XmlMediaTypeFormatter();
            string str = Serialize(xml, value);
            Console.WriteLine(str);

            var json = new JsonMediaTypeFormatter();
            str = Serialize(json, value);
            Console.WriteLine(str);

            // Round trip
            var value2 = Deserialize<Author>(json, str);
            Console.WriteLine(value2);
            Assert.AreEqual(value2.Name, "Alice");
            Assert.AreEqual(value2.Id, 23);
        }

    }
}
