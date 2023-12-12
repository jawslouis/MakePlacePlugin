// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.ObjectToInferredTypesConverter
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MakePlacePlugin; 

public class ObjectToInferredTypesConverter : JsonConverter<object> {
    public override object Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) {
        object obj;
        switch (reader.TokenType) {
            case JsonTokenType.String:
                DateTime dateTime;
                obj = !reader.TryGetDateTime(out dateTime) ? reader.GetString() : dateTime;
                break;
            case JsonTokenType.Number:
                long num;
                obj = !reader.TryGetInt64(out num) ? reader.GetDouble() : (object)num;
                break;
            case JsonTokenType.True:
                obj = true;
                break;
            case JsonTokenType.False:
                obj = false;
                break;
            default:
                obj = JsonDocument.ParseValue(ref reader).RootElement.Clone();
                break;
        }

        return obj;
    }

    public override void Write(
        Utf8JsonWriter writer,
        object objectToWrite,
        JsonSerializerOptions options) {
        JsonSerializer.Serialize(writer, objectToWrite, objectToWrite.GetType(), options);
    }
}
