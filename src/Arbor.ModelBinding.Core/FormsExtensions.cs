﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.Primitives;

using Newtonsoft.Json;

namespace Arbor.ModelBinding.Core
{
    public static class FormsExtensions
    {
        public static object ParseFromPairs(
            IEnumerable<KeyValuePair<string, StringValues>> collection,
            Type targetType)
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

            KeyValuePair<string, StringValues>[] nested =
                nameCollection.Where(pair => pair.Key.IndexOf("[", StringComparison.Ordinal) >= 0).ToArray();

            IDictionary<string, object> dynamicObjectDictionary = dynamicObject;

            bool ContainsKey(string lookup)
            {
                return dynamicObjectDictionary.Keys.Any(key => key.Equals(lookup, StringComparison.OrdinalIgnoreCase));
            }

            IEnumerable<KeyValuePair<string, StringValues>> singleValuePairs =
                nameCollection.Where(pairGroup => pairGroup.Value.Count == 1).Except(nested);

            foreach (KeyValuePair<string, StringValues> keyValuePair in singleValuePairs)
            {
                dynamicObjectDictionary[keyValuePair.Key] = keyValuePair.Value.Single();
            }

            IEnumerable<KeyValuePair<string, StringValues>> multipleValuesPairs =
                nameCollection.Where(pairGroup => pairGroup.Value.Count >= 2).Except(nested);

            foreach (KeyValuePair<string, StringValues> keyValuePair in multipleValuesPairs)
            {
                string[] values = keyValuePair.Value;

                dynamicObjectDictionary[keyValuePair.Key] = values;
            }

            foreach (PropertyInfo propertyInfo in targetType
                .GetTypeInfo()
                .DeclaredProperties
                .Where(
                    property => !(typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(
                                      property.PropertyType.GetTypeInfo())
                                  || property.PropertyType == typeof(string))
                                && !property.PropertyType.IsPrimitive
                                && !property.PropertyType.GetTypeInfo().IsGenericType
                                && !ContainsKey(property.Name)))
            {
                var subProperties = nameCollection
                    .Where(pair => pair.Key.IndexOf(".", StringComparison.Ordinal) >= 0)
                    .Select(
                        pair => new KeyValuePair<string, StringValues>(
                            pair.Key.Substring(pair.Key.IndexOf(".", StringComparison.Ordinal)).TrimStart('.'),
                            pair.Value))
                    .ToArray();

                var subInstance = ParseFromPairs(subProperties, propertyInfo.PropertyType);

                dynamicObjectDictionary[propertyInfo.Name] = subInstance;
            }

            foreach (PropertyInfo propertyInfo in targetType
                .GetTypeInfo()
                .DeclaredProperties
                .Where(
                    property => typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(
                                    property.PropertyType.GetTypeInfo()) &&
                                property.PropertyType.GetTypeInfo().IsGenericType))
            {
                Type subTargetType = propertyInfo.PropertyType.GenericTypeArguments.FirstOrDefault();

                string expectedName = propertyInfo.Name;

                var matchingProperty = nested.Select(
                        nestedGroup =>
                        {
                            int indexIndex = nestedGroup.Key.IndexOf("[", StringComparison.Ordinal);
                            int indexStopIndex = nestedGroup.Key.IndexOf("]", StringComparison.Ordinal);
                            int indexLength = indexStopIndex - indexIndex;

                            int dotIndex = nestedGroup.Key.IndexOf(".", StringComparison.Ordinal);

                            string name = nestedGroup.Key.Substring(0, indexIndex);

                            string index = nestedGroup.Key.Substring(indexIndex + 1, indexLength - 1);

                            string propertyName = nestedGroup.Key.Substring(dotIndex + 1);

                            return new { GroupName = name, nestedGroup.Value, Index = index, propertyName };
                        })
                    .Where(s => s.GroupName.Equals(expectedName, StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                var indexedGroups = matchingProperty.GroupBy(_ => _.Index);

                object newCollection = null;

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

                if (newCollection == null)
                {
                    throw new InvalidOperationException(
                        $"Could not create new {propertyInfo.PropertyType.FullName}");
                }

                dynamicObjectDictionary[propertyInfo.Name] = newCollection;

                foreach (var item in indexedGroups)
                {
                    var pairs = new List<KeyValuePair<string, StringValues>>();
                    foreach (var value in item)
                    {
                        foreach (string valueProperty in value.Value)
                        {
                            pairs.Add(new KeyValuePair<string, StringValues>(value.propertyName, valueProperty));
                        }
                    }

                    object subTargetInstance = ParseFromPairs(pairs, subTargetType);

                    AddInstanceToCollection(subTargetType, newCollection, subTargetInstance);
                }
            }

            JsonConverter[] converters =
            {
                new BooleanJsonConverter(),
                new StringValuesJsonConverter()
            };

            string json = JsonConvert.SerializeObject(dynamicObject);

            object instance;

            try
            {
                instance = JsonConvert.DeserializeObject(json, targetType, converters);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Could not deserialize type {targetType} from json {json}", ex);
            }

            return instance;
        }

        private static void AddInstanceToCollection(Type subTargetType, object newCollection, object subTargetInstance)
        {
            Type genericCollectionType = typeof(ICollection<>);

            Type constructedCollectionType = genericCollectionType.MakeGenericType(subTargetType);

            if (constructedCollectionType.IsInstanceOfType(newCollection))
            {
                MethodInfo addMethod = newCollection.GetType().GetTypeInfo()
                    .GetDeclaredMethod(nameof(ICollection<object>.Add));

                addMethod.Invoke(newCollection, new[] { subTargetInstance });
            }
        }
    }
}