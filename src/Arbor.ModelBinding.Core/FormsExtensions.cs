using System;
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

            KeyValuePair<string, StringValues>[] nameCollection = collection.ToArray();

            var dynamicObject = new ExpandoObject();

            KeyValuePair<string, StringValues>[] nested =
                nameCollection.Where(pair => pair.Key.Contains("[")).ToArray();

            IDictionary<string, object> dynamicObjectDictionary = dynamicObject;

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

            string json = JsonConvert.SerializeObject(dynamicObject);

            Console.WriteLine("parsing " + json + " into type " + targetType);
            object instance = JsonConvert.DeserializeObject(json, targetType);
            JsonConverter[] converts = { new BooleanJsonConverter() };
            object instance = JsonConvert.DeserializeObject(json, targetType, converts);

            if (instance != null)
            {
                foreach (PropertyInfo propertyInfo in targetType
                    .GetTypeInfo()
                    .DeclaredProperties
                    .Where(property => property.CanWrite &&
                                       typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(
                                           property.PropertyType.GetTypeInfo()) &&
                                       property.PropertyType.GetTypeInfo().IsGenericType))
                {
                    Type subTargetType = propertyInfo.PropertyType.GenericTypeArguments.FirstOrDefault();

                    string expectedName = propertyInfo.Name;

                    var matchingProperty = nested.Select(nestedGroup =>
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

                        object currentCollection = propertyInfo.GetValue(instance);

                        if (currentCollection == null)
                        {
                            object newCollection = null;

                            if (!propertyInfo.PropertyType.GetTypeInfo().IsAbstract)
                            {
                                try
                                {
                                    newCollection = Activator.CreateInstance(propertyInfo.PropertyType);
                                }
                                catch (Exception)
                                {
                                    // Ignore exception
                                }
                            }
                            else
                            {
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
                            }

                            if (newCollection == null)
                            {
                                throw new InvalidOperationException(
                                    $"Could not create new {propertyInfo.PropertyType.FullName}");
                            }

                            if (propertyInfo.PropertyType.GetTypeInfo()
                                .IsAssignableFrom(newCollection.GetType().GetTypeInfo()))
                            {
                                propertyInfo.SetValue(instance, newCollection);

                                currentCollection = newCollection;
                            }
                        }

                        object subTargetInstance = ParseFromPairs(pairs, subTargetType);

                        Type genericCollectionType = typeof(ICollection<>);

                        Type constructedCollectionType = genericCollectionType.MakeGenericType(subTargetType);

                        if (currentCollection != null)
                        {
                            if (constructedCollectionType.GetTypeInfo()
                                .IsAssignableFrom(currentCollection.GetType().GetTypeInfo()))
                            {
                                MethodInfo addMethod = currentCollection.GetType().GetTypeInfo()
                                    .GetDeclaredMethod("Add");

                                addMethod.Invoke(currentCollection, new[] { subTargetInstance });
                            }
                        }
                    }
                }
            }

            return instance;
        }
    }
}
