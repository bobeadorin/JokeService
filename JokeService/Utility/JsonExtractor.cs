using System.Text.Json;


namespace JokeService.Utility
{
    public  class JsonExtractor
    {
        public static T GetValue<T>(string FilePath, string FieldName)
        {
            string jsonData = File.ReadAllText(FilePath);
            
            T value = JsonSerializer.Deserialize<T>(jsonData);
            
            if (value == null)
            {
                throw new Exception($"Field {FieldName} not found");
            }
            return value;
        }
    }
}
