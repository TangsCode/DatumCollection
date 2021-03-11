using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace DatumCollection.Utility.Extensions
{
    public static class DynamicObjectExtension
    {
        public static object Clone(this object source)
        {
            if (!source.GetType().IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", source.GetType().Name);
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return null;
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(stream);
            }
        }

        public static bool ContainsProperty(this object dynamicObject,string key)
        {
            if (!dynamicObject.GetType().IsAssignableFrom(typeof(ExpandoObject)))
            {
                return false;
            }
            IDictionary<string, object> properties = (IDictionary<string, object>)dynamicObject;
            return properties.ContainsKey(key) || properties.ContainsKey(key.Substring(0, 1).ToLower() + key.Substring(1));
        }

        public static dynamic AddProperty(this object dynamicObject, string key, object value)
        {
            if (dynamicObject.GetType().IsAssignableFrom(typeof(ExpandoObject)))
            {
                IDictionary<string, object> properties = (IDictionary<string, object>)dynamicObject;
                if (properties.ContainsKey(key))
                {
                    properties.Add(key, value);
                }
                else
                {
                    properties[key] = value;
                }
                return properties;
            }

            return dynamicObject;
        }

        public static dynamic RemoveProperty(this object dynamicObject, string key)
        {
            if (dynamicObject.GetType().IsAssignableFrom(typeof(ExpandoObject)))
            {
                IDictionary<string, object> properties = (IDictionary<string, object>)dynamicObject;
                if (properties.ContainsKey(key))
                {
                    properties.Remove(key);
                    return properties;
                }
            }

            return dynamicObject;
        }

        public static dynamic EmptyDynamic()
        {
            return new ExpandoObject();
        }
    }
}
