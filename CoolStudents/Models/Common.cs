using System.IO;
using Newtonsoft.Json;

namespace CoolStudents.Models
{
    public static class RequestConverter
    {
        public static T FromJsonRequest<T>(Stream stream)
        {
            var reader = new StreamReader(stream);
            string json = reader.ReadToEnd();

            T model = default(T);
            model = JsonConvert.DeserializeObject<T>(json);
            return model;
        }

        //public static T FromPostRequest<T>(Stream stream)
        //{
        //    stream.Seek(0, System.IO.SeekOrigin.Begin);
        //    string json = new StreamReader(stream).ReadToEnd();

        //    T model = default(T);
        //    model = JsonConvert.DeserializeObject<T>(json);
        //    return model;
        //}
    }

    public class IdText
    {
        public int id { get; set; }
        public string text { get; set; }
    }
}
