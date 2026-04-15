using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.Primitives;

namespace Arbor.ModelBinding.Core
{
    internal static class FormsParser
    {
        public static object? ParseFromPairs(
            IEnumerable<KeyValuePair<string, StringValues>> collection,
            Type targetType,
            Func<object, string> serializer,
            Func<string, Type, object?> deserializer)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            if (targetType.IsAbstract)
            {
                return default;
            }

            if (targetType.IsGenericType && targetType.GenericTypeArguments.Any(argType => argType.IsAbstract))
            {
                return default;
            }

            KeyValuePair<string, StringValues>[] nameCollection = collection.ToArray();

            if (nameCollection.Length == 0)
            {
                return default;
            }

            var dynamicObject = new ExpandoObject();
            IDictionary<string, object?> dynamicObjectDictionary = dynamicObject;
            var existingKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var nested = new List<KeyValuePair<string, StringValues>>();
            var dotted = new List<KeyValuePair<string, StringValues>>();

            void SetValue(string key, object? value)
            {
                dynamicObjectDictionary[key] = value;
                existingKeys.Add(key);
            }

            foreach (KeyValuePair<string, StringValues> keyValuePair in nameCollection)
            {
                string key = keyValuePair.Key;
                int dotIndex = key.IndexOf(".", StringComparison.Ordinal);
                int bracketIndex = key.IndexOf("[", StringComparison.Ordinal);

                if (dotIndex >= 0)
                {
                    dotted.Add(keyValuePair);
                }

                if (bracketIndex >= 0)
                {
                    nested.Add(keyValuePair);
                    continue;
                }

                StringValues values = keyValuePair.Value;

                if (values.Count == 1 && dotIndex < 0)
                {
                    SetValue(key, values[0]);
                }
                else if (values.Count >= 2)
                {
                    SetValue(key, values);
                }
            }

            PropertyInfo[] declaredProperties = targetType.GetTypeInfo().DeclaredProperties.ToArray();

            foreach (PropertyInfo propertyInfo in declaredProperties.Where(
                         property => !(typeof(IEnumerable).IsAssignableFrom(property.PropertyType) || property.PropertyType == typeof(string))
                                     && !property.PropertyType.IsPrimitive
                                     && !property.PropertyType.IsGenericType
                                     && !existingKeys.Contains(property.Name)))
            {
                string propertyNamePrefix = propertyInfo.Name + ".";
                var subProperties = new List<KeyValuePair<string, StringValues>>();

                foreach (KeyValuePair<string, StringValues> pair in dotted)
                {
                    string key = pair.Key;

                    if (!key.StartsWith(propertyNamePrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    int dotIndex = key.IndexOf(".", StringComparison.Ordinal);
                    subProperties.Add(new KeyValuePair<string, StringValues>(key.Substring(dotIndex + 1), pair.Value));
                }

                if (subProperties.Count > 0)
                {
                    SetValue(propertyInfo.Name, ParseFromPairs(subProperties, propertyInfo.PropertyType, serializer, deserializer));
                }
            }

            var nestedIndexedProperties = nested.Select(
                    nestedGroup =>
                    {
                        int indexIndex = nestedGroup.Key.IndexOf("[", StringComparison.Ordinal);
                        int indexStopIndex = nestedGroup.Key.IndexOf("]", StringComparison.Ordinal);
                        int indexLength = indexStopIndex - indexIndex;

                        int dotIndex = nestedGroup.Key.IndexOf(".", StringComparison.Ordinal);

                        string name = nestedGroup.Key.Substring(0, indexIndex);

                        string index = nestedGroup.Key.Substring(indexIndex + 1, indexLength - 1);

                        string propertyName = nestedGroup.Key.Substring(dotIndex + 1);

                        return new { GroupName = name, nestedGroup.Value, Index = index, PropertyName = propertyName };
                    })
                .ToArray();

            foreach (PropertyInfo propertyInfo in declaredProperties.Where(
                         property => typeof(IEnumerable).IsAssignableFrom(property.PropertyType) &&
                                     property.PropertyType.IsGenericType))
            {
                Type? subTargetType = propertyInfo.PropertyType.GenericTypeArguments.FirstOrDefault();

                if (subTargetType is null)
                {
                    continue;
                }

                string expectedName = propertyInfo.Name;

                var matchingProperty = nestedIndexedProperties
                    .Where(s => s.GroupName.Equals(expectedName, StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                var indexedGroups = matchingProperty.GroupBy(_ => _.Index);

                object? newCollection = null;

                Type listType = typeof(List<>);
                Type constructedListType = listType.MakeGenericType(subTargetType);

                try
                {
                    newCollection = Activator.CreateInstance(constructedListType);
                }
                catch (Exception)
                {
                    // Ignore exception
                }

                dynamicObjectDictionary[propertyInfo.Name] = newCollection ?? throw new InvalidOperationException(
                    $"Could not create new {propertyInfo.PropertyType.FullName}");

                foreach (var item in indexedGroups)
                {
                    var pairs = new List<KeyValuePair<string, StringValues>>();
                    foreach (var value in item)
                    {
                        foreach (string valueProperty in value.Value)
                        {
                            pairs.Add(new KeyValuePair<string, StringValues>(value.PropertyName, valueProperty));
                        }
                    }

                    object? subTargetInstance = ParseFromPairs(pairs, subTargetType, serializer, deserializer);

                    if (subTargetInstance is { })
                    {
                        AddInstanceToCollection(newCollection, subTargetInstance);
                    }
                }
            }

            string json = serializer.Invoke(dynamicObject);

            object? instance;

            try
            {
                instance = deserializer(json, targetType);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Could not deserialize type {targetType} from json {json}", ex);
            }

            return instance;
        }

        private static void AddInstanceToCollection(object newCollection, object subTargetInstance)
        {
            if (newCollection is IList list)
            {
                list.Add(subTargetInstance);
            }
        }
    }
}
