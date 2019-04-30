using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Zoo.Systems;
using UnityEngine;
using System.Runtime.Remoting;
using Zoo.Utilities;

namespace Zoo.Attributes
{
    public static class AttributeHelper
    {

        public static Dictionary<string, Type> AttributeDictionary = new Dictionary<string, Type>();
       
        public static void InitializeAttributeDictionary()
        {
            var types = FindDerivedTypes(Assembly.GetAssembly(typeof(IAttribute)), typeof(IAttribute));
            foreach (var t in types)
            {
                AttributeDictionary.Add(t.Name, t);
            }
        }

        /// <summary>
        /// Adds given types to attribute dictionary, if they do not already exist.
        /// </summary>
        /// <param name="typesToAdd"></param>
        public static void AddToAttributeDictionary(IEnumerable<Type> typesToAdd)
        {
            foreach (var t in typesToAdd)
            {
                if (!AttributeDictionary.ContainsKey(t.Name))
                    AttributeDictionary.Add(t.Name, t);

            }
        }

        private static IEnumerable<Type> FindDerivedTypes(Assembly assembly, Type baseType)
        {
            return assembly.GetTypes().Where(t => t != baseType &&
                                                  baseType.IsAssignableFrom(t));
        }

        public static object GetNewAttributeInstance(string attributeName, int parentAssetID)
        {
            var newInstance =  Activator.CreateInstance(AttributeDictionary[attributeName], parentAssetID);
            return newInstance;
        }

    }

}
