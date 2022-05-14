using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CloudCqs;

[Serializable]
public abstract class StatusCodeException : Exception
{
    public record Details(
        [property: JsonPropertyName("title")]
        string Title,
        [property: JsonPropertyName("status")]
        int? Status,
        [property: JsonPropertyName("errors")]
        IDictionary<string, string[]>? Errors);

    public HttpStatusCode HttpStatusCode { get; }
    public IDictionary<string, string[]>? Errors { get; }

    public Details Object => new(
        HttpStatusCode.ToString(),
        (int)HttpStatusCode,
        Errors);

    public override string Message => JsonSerializer.Serialize(
        Object,
        options: new(JsonSerializerDefaults.Web)
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

    protected StatusCodeException(
        HttpStatusCode httpStatusCode,
        Dictionary<string, string[]>? errors = null)
    {
        HttpStatusCode = httpStatusCode;
        Errors = errors;
    }

    protected StatusCodeException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        HttpStatusCode = default;
    }
}
