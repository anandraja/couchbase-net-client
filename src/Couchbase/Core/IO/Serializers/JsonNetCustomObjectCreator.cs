﻿using System;
using Newtonsoft.Json;

namespace Couchbase.Core.IO.Serializers
{
    /// <summary>
    /// <see cref="JsonConverter"/> that wraps an <see cref="ICustomObjectCreator"/> to support Json.Net deserialization.
    /// </summary>
    /// <remarks>
    /// Used by <see cref="DefaultSerializer"/> if an <see cref="ICustomObjectCreator"/> is supplied.
    /// </remarks>
    internal class JsonNetCustomObjectCreatorWrapper : JsonConverter
    {
        private readonly ICustomObjectCreator _creator;

        public JsonNetCustomObjectCreatorWrapper(ICustomObjectCreator creator)
        {
            _creator = creator ?? throw new ArgumentNullException(nameof(creator));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            object obj = _creator.CreateObject(objectType);
            if (obj == null)
            {
                throw new NullReferenceException("ICustomObjectCreator returned a null reference.");
            }

            serializer.Populate(reader, obj);

            return obj;
        }

        public override bool CanConvert(Type objectType)
        {
            return _creator.CanCreateObject(objectType);
        }
    }
}
