using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ZEDRatApp.ZEDRAT.NetSerializer
{
	internal static class Helpers
	{
		public static readonly ConstructorInfo ExceptionCtorInfo = typeof(Exception).GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null);

		public static IEnumerable<FieldInfo> GetFieldInfos(Type type)
		{
			IOrderedEnumerable<FieldInfo> orderedEnumerable = (from fi in type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				where (fi.Attributes & FieldAttributes.NotSerialized) == 0
				select fi).OrderBy((FieldInfo f) => f.Name, StringComparer.Ordinal);
			if (type.BaseType == null)
			{
				return orderedEnumerable;
			}
			return Helpers.GetFieldInfos(type.BaseType).Concat(orderedEnumerable);
		}

		public static DynamicMethod GenerateDynamicSerializerStub(Type type)
		{
			DynamicMethod dynamicMethod = new DynamicMethod("Serialize", null, new Type[3]
			{
				typeof(Serializer),
				typeof(Stream),
				type
			}, typeof(Serializer), skipVisibility: true);
			dynamicMethod.DefineParameter(1, ParameterAttributes.None, "serializer");
			dynamicMethod.DefineParameter(2, ParameterAttributes.None, "stream");
			dynamicMethod.DefineParameter(3, ParameterAttributes.None, "value");
			return dynamicMethod;
		}

		public static DynamicMethod GenerateDynamicDeserializerStub(Type type)
		{
			DynamicMethod dynamicMethod = new DynamicMethod("Deserialize", null, new Type[3]
			{
				typeof(Serializer),
				typeof(Stream),
				type.MakeByRefType()
			}, typeof(Serializer), skipVisibility: true);
			dynamicMethod.DefineParameter(1, ParameterAttributes.None, "serializer");
			dynamicMethod.DefineParameter(2, ParameterAttributes.None, "stream");
			dynamicMethod.DefineParameter(3, ParameterAttributes.Out, "value");
			return dynamicMethod;
		}
	}
}
