namespace GPConnect.Provider.AcceptanceTests.Extensions
{
    using Hl7.Fhir.Model;
    using Hl7.Fhir.Serialization;
    using Newtonsoft.Json;

    public static class BaseExtensions
    {
        public static string ToFhirJson(this Base resource)
        {
            return new FhirJsonSerializer().SerializeToString(resource);
        }

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}