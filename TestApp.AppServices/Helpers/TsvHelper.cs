using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace TestApp.AppServices.Helpers
{
    public static class TsvHelper
    {
        public static async Task<List<T>> ParseData<T>(IFormFile data, Func<List<string>, T> mapFunc)
        where T: class, new()
        {
            var list = new List<T>();
            using (var reader = new StreamReader(data.OpenReadStream()))
            {
                var skip = true;
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (skip)
                    {
                        skip = false;
                        continue;
                    }

                    var rowFields = line.Split("\t");
                    if(rowFields.All(x => x.Length == 0))
                        continue;
                    var fields = new List<string>();
                    foreach (var rowField in rowFields)
                    {
                        var field = rowField.Trim();
                        field = Regex.Replace(field, @"\s+", " ");
                        fields.Add(field);
                    }
                    var record = mapFunc(fields);
                    list.Add(record);
                }
            }
            return list;
        }
    }
}
