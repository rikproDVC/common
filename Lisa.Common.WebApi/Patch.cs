using Newtonsoft.Json.Linq;

namespace Lisa.Common.WebApi
{
    public class Patch
    {
        public Patch()
        {
        }

        public Patch(string action, string field, string value)
        {
            Action = action;
            Field = field;
            Value = JProperty.Parse(value ?? "null");
        }

        public string Action { get; set; }
        public string Field { get; set; }
        public JToken Value { get; set; }
    }
}