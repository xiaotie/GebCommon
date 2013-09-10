using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace Geb.Utils
{
	public sealed class TypeTemplate
	{
		public class ObjectContainer
		{
			private List<Object> Container = new List<object>();
			private List<Type> TypeContainer = new List<Type>();

			public ObjectContainer(Object[] objs, Type[] tlist)
			{
				Container.AddRange(objs);
				TypeContainer.AddRange(tlist);
			}

			public Object Get(Int32 index)
			{
				if (index >= 0 && index < Container.Count)
					return Container[index];
				else
					return null;
			}

			public Type GetType(Int32 index)
			{
				if (index >= 0 && index < TypeContainer.Count)
					return TypeContainer[index];
				else
					return null;
			}

			public static MethodInfo GetGetMethodInfo()
			{
				MethodInfo[] mis = typeof(ObjectContainer).GetMethods(BindingFlags.Public | BindingFlags.Instance);
				foreach (var mi in mis)
				{
					if (mi.Name == "Get") return mi;
				}
				return null;
			}
		}

		private class MethodInfoHolder
		{
			public Int32 ObjectIndex { get; private set; }
			public MethodInfo MethodInfo { get; private set; }
			public Type ObjectType { get; private set; }
			public MethodInfoHolder(MethodInfo pi, Int32 index, Type type)
			{
				MethodInfo = pi;
				ObjectIndex = index;
				ObjectType = type;
			}
		}

		public delegate void Handler<TImple>(TImple imple) where TImple: class;

		public static TInterface Create<TInterface>(Type type)
			where TInterface : class
		{
			return Activator.CreateInstance(type) as TInterface;
		}

		public static TInterface Create<TInterface>(String typeName)
			where TInterface : class
		{
			return Activator.CreateInstance(null, typeName) as TInterface;
		}

		public static TInterface Create<TInterface, TImple>(TImple instance) 
			where TInterface : class
			where TImple : class
		{
			Type type = DynamicTypeGen<TInterface>( new Object[] { instance }, new Type[] { instance.GetType()});
			return Activator.CreateInstance(type, new ObjectContainer(new Object[] { instance }, new Type[] { instance.GetType() })) as TInterface;
		}

		public static TInterface CreateObjectMixin<TInterface>(params Object[] impleInstances)
			where TInterface : class
		{
			List<Type> tlist = new List<Type>();
			foreach (var item in impleInstances)
			{
				tlist.Add(item.GetType());
			}
			Type type = DynamicTypeGen<TInterface>(impleInstances, tlist.ToArray());
			return Activator.CreateInstance(type, new ObjectContainer( impleInstances, tlist.ToArray())) as TInterface;
		}

		public static TInterface CreateIntercepted<TInterface, TImple>(TImple instance, Handler<TImple> before, Handler<TImple> after)
			where TInterface : class
			where TImple : class
		{
			throw new NotImplementedException();
		}

		public static Type DynamicTypeGen<TInterface>(Object[] instances, Type[] typeList)
			where TInterface: class
		{
			Type tInterface = typeof(TInterface);

			PropertyInfo[] pisInterface = tInterface.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			MethodInfo[] misInterface = tInterface.GetMethods(BindingFlags.Public | BindingFlags.Instance);
			List<MethodInfo> misInterfaceList = new List<MethodInfo>();
			
			List<Type> tList = new List<Type>();
			foreach (var obj in instances)
			{
				tList.Add(obj.GetType());
			}

			Type[] tArray = tList.ToArray();

			foreach (var item in misInterface)
			{
				if (item.IsSpecialName == false) misInterfaceList.Add(item);
			}

			List<MethodInfoHolder> miHolderList = new List<MethodInfoHolder>();
			for (int i = 0; i < tArray.Length; i++)
			{
				MethodInfo[] misImple = tArray[i].GetMethods(BindingFlags.Public | BindingFlags.Instance);
				foreach (var item in misImple)
				{
					MethodInfoHolder h = new MethodInfoHolder(item, i, typeList[i]);
					miHolderList.Add(h);
				}
			}

			AssemblyName aName = new AssemblyName("Orc.Generics.DynamicTypes");
			AssemblyBuilder ab =
				AppDomain.CurrentDomain.DefineDynamicAssembly(
					aName,
					AssemblyBuilderAccess.RunAndSave);
			
			ModuleBuilder mb =
				ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");

			TypeBuilder tb = mb.DefineType(GetDynamicTypeName<TInterface>(instances),
				TypeAttributes.Public, null, new Type[] { tInterface });
			FieldBuilder fbInstances = tb.DefineField(
				"_container",
				typeof(TypeTemplate.ObjectContainer),
				FieldAttributes.Private);

			ConstructorBuilder ctor1 = tb.DefineConstructor(
				MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName,
				CallingConventions.Standard,
				new Type[1] { typeof(TypeTemplate.ObjectContainer) });

			ILGenerator ctor1IL = ctor1.GetILGenerator();
			ctor1IL.Emit(OpCodes.Ldarg_0);
			ctor1IL.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
			ctor1IL.Emit(OpCodes.Ldarg_0);
			ctor1IL.Emit(OpCodes.Ldarg_1);
			ctor1IL.Emit(OpCodes.Stfld, fbInstances);
			ctor1IL.Emit(OpCodes.Ret);

			foreach (var item in pisInterface)
			{
				MethodInfoHolder getMi = FindGetMethodInfo(miHolderList, item);
				MethodInfoHolder setMi = FindSetMethodInfo(miHolderList, item);
				CreateProperty(tb, fbInstances, item, getMi, setMi);
			}

			foreach (var item in misInterfaceList)
			{
				MethodInfoHolder instanceMi = FindMethodInfo(miHolderList, item);
				CreateMethod(tb, fbInstances, item, instanceMi);
			}
			Type type = tb.CreateType();
			ab.Save(aName.Name + ".dll");
			return type;
		}

		private static MethodInfoHolder FindGetMethodInfo(IEnumerable<MethodInfoHolder> miList, PropertyInfo pi)
		{
			foreach (var item in miList)
			{
				if (item.MethodInfo.Name.Equals("get_" + pi.Name) && item.MethodInfo.IsSpecialName) return item;
			}

			return null;
		}

		private static MethodInfoHolder FindSetMethodInfo(IEnumerable<MethodInfoHolder> miList, PropertyInfo pi)
		{
			foreach (var item in miList)
			{
				if (item.MethodInfo.Name.Equals("set_" + pi.Name) && item.MethodInfo.IsSpecialName) return item;
			}
			
			return null;
		}

		private static MethodInfoHolder FindMethodInfo(IEnumerable<MethodInfoHolder> miList, MethodInfo mi)
		{
			foreach (var item in miList)
			{
				if (MethodInfoEqual(item.MethodInfo,mi)) return item;
			}

			return null;
		}

		private static Boolean MethodInfoEqual(MethodInfo mi1, MethodInfo mi2)
		{
			if (mi1.IsSpecialName == true || mi2.IsSpecialName == true) return false;
			if (mi1.Name != mi2.Name) return false;
			if (mi1.ReturnType != mi2.ReturnType) return false;
			ParameterInfo[] pis1 = mi1.GetParameters();
			ParameterInfo[] pis2 = mi2.GetParameters();
			if (pis1.Length != pis2.Length) return false;
			for (int i = 0; i < pis1.Length; i++)
			{
				ParameterInfo pi1 = pis1[i];
				ParameterInfo pi2 = pis2[i];
				if (pi1.ParameterType != pi2.ParameterType) return false;
			}
			return true;
		}

		private static Boolean IsParentType(Type type0, Type type1)
		{
			if(type0 == null || type1 == null) return false;
			if(type0 == type1) return true;

			Type[] interfaces = type1.GetInterfaces();
			if (interfaces != null)
			{
				foreach (var type in interfaces)
					if (type == type0) return true;
			}

			Type parent = type1.BaseType;
			while (parent != null)
			{
				if (parent == type0) return true;
				else parent = parent.BaseType;
			}

			return false;
		}

		private static void CreateProperty(TypeBuilder tb, FieldBuilder fbInstance, PropertyInfo pi, MethodInfoHolder getMi, MethodInfoHolder setMi)
		{
			String name = pi.Name;
			Type type = pi.PropertyType;

			PropertyBuilder pb = tb.DefineProperty(
				name,
				PropertyAttributes.HasDefault,
				type,
				null);

			MethodAttributes getSetAttr = MethodAttributes.Public |
				MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final ;
			MethodBuilder mbGetAccessor = tb.DefineMethod(
				"get_" + name,
				getSetAttr,
				type,
				Type.EmptyTypes);

			ILGenerator getIL = mbGetAccessor.GetILGenerator();
			
			if (getMi == null)
			{
				getIL.Emit(OpCodes.Newobj, typeof(NotImplementedException).GetConstructor(new Type[]{}));
				getIL.Emit(OpCodes.Throw);
			}
			else
			{
				getIL.Emit(OpCodes.Ldarg_0);
				getIL.Emit(OpCodes.Ldfld, fbInstance);
				getIL.Emit(OpCodes.Ldc_I4, getMi.ObjectIndex);
				getIL.Emit(OpCodes.Callvirt, ObjectContainer.GetGetMethodInfo());
				getIL.Emit(OpCodes.Isinst, getMi.ObjectType);
				getIL.Emit(OpCodes.Callvirt, getMi.MethodInfo);
				getIL.Emit(OpCodes.Ret);
			}

			MethodBuilder mbSetAccessor = tb.DefineMethod(
				"set_"+ name,
				getSetAttr,
				null,
				new Type[] { type });

			ILGenerator setIL = mbSetAccessor.GetILGenerator();
			if (setMi == null)
			{
				setIL.Emit(OpCodes.Newobj, typeof(NotImplementedException).GetConstructor(new Type[] { }));
				setIL.Emit(OpCodes.Throw);
			}
			else
			{
				setIL.Emit(OpCodes.Ldarg_0);
				setIL.Emit(OpCodes.Ldfld, fbInstance);
				setIL.Emit(OpCodes.Ldc_I4, setMi.ObjectIndex);
				setIL.Emit(OpCodes.Callvirt, ObjectContainer.GetGetMethodInfo());
				setIL.Emit(OpCodes.Isinst, setMi.ObjectType);
				setIL.Emit(OpCodes.Ldarg_1);
				setIL.Emit(OpCodes.Callvirt, setMi.MethodInfo);
				setIL.Emit(OpCodes.Ret);
			}

			pb.SetGetMethod(mbGetAccessor);
			pb.SetSetMethod(mbSetAccessor);
		}

		private static void CreateMethod(TypeBuilder tb, FieldBuilder fbInstance, MethodInfo mi, MethodInfoHolder instanceMi)
		{
			List<Type> paramTyleList = new List<Type>();
			foreach(var item in mi.GetParameters())
				paramTyleList.Add(item.ParameterType);

			MethodBuilder mb = tb.DefineMethod(
			  mi.Name,
			  MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final,
			  mi.ReturnType,
			  paramTyleList.ToArray());

			ILGenerator il = mb.GetILGenerator();
			if (instanceMi == null)
			{
				il.Emit(OpCodes.Newobj, typeof(NotImplementedException).GetConstructor(new Type[] { }));
				il.Emit(OpCodes.Throw);
			}
			else
			{
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldfld, fbInstance);
				il.Emit(OpCodes.Ldc_I4, instanceMi.ObjectIndex);
				il.Emit(OpCodes.Callvirt, ObjectContainer.GetGetMethodInfo());
				il.Emit(OpCodes.Isinst, instanceMi.ObjectType);
				switch (paramTyleList.Count)
				{
					case 0:
						break;
					case 1:
						il.Emit(OpCodes.Ldarg_1);
						break;
					case 2:
						il.Emit(OpCodes.Ldarg_1);
						il.Emit(OpCodes.Ldarg_2);
						break;
					case 3:
						il.Emit(OpCodes.Ldarg_1);
						il.Emit(OpCodes.Ldarg_2);
						il.Emit(OpCodes.Ldarg_3);
						break;
					default:
						il.Emit(OpCodes.Ldarg_1);
						il.Emit(OpCodes.Ldarg_2);
						il.Emit(OpCodes.Ldarg_3);

						Int32 sCount = Math.Min(paramTyleList.Count, 127);
						for (int i = 4; i <= sCount; i++)
						{
							il.Emit(OpCodes.Ldarg_S, i);
						}

						for (int i = 128; i <= paramTyleList.Count; i++)
						{
							il.Emit(OpCodes.Ldarg, i);
						}

						break;
				}

				il.Emit(OpCodes.Callvirt, instanceMi.MethodInfo);
				il.Emit(OpCodes.Ret);
			}
		}

		private static String GetDynamicTypeName<TInterface>(params Object[] instances)
	where TInterface : class
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("_DynamicTypes");
			sb.Append(typeof(TInterface).ToString());
			foreach (var obj in instances)
			{
				sb.Append("_");
				sb.Append(obj.GetType().ToString());
			}
			return sb.ToString();
		}
	}
}
