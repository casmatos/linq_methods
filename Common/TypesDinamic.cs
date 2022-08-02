using System.Reflection;
using System.Reflection.Emit;

namespace Linq_Methods.Common
{
    public static class TypesDinamic
    {
        private static TypeBuilder BaseBuildAssembly(string assemblyName, string typeName)
        {
            // Let's start by creating a new assembly
            AssemblyName dynamicAssemblyName = new AssemblyName(assemblyName);
            AssemblyBuilder dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(dynamicAssemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder dynamicModule = dynamicAssembly.DefineDynamicModule(assemblyName);

            // Now let's build a new type
            TypeBuilder dynamicAnonymousType = dynamicModule.DefineType(typeName, TypeAttributes.Public | TypeAttributes.BeforeFieldInit);

            return dynamicAnonymousType;
        }

        public static Type? CreateNewType(string assemblyName, string typeName, params Type[] types)
        {
            TypeBuilder dynamicAnonymousType = BaseBuildAssembly(assemblyName, typeName);

            // Let's add some fields to the type.
            int itemNo = 1;
            foreach (Type type in types)
            {
                dynamicAnonymousType.DefineField("Item" + itemNo++, type, FieldAttributes.Public);
            }

            // Return the type to the caller
            return dynamicAnonymousType.CreateType();
        }

        public static Type? CreateNewType(string assemblyName, string typeName, Type objectBaseType, params string[] fields)
        {
            TypeBuilder dynamicAnonymousType = BaseBuildAssembly(assemblyName, typeName);

            foreach (var prop in objectBaseType.GetProperties())
            {
                var fieldName = fields.FirstOrDefault(s => string.Equals(s, prop.Name, StringComparison.CurrentCultureIgnoreCase));

                if (fields.Contains(prop.Name, StringComparer.CurrentCultureIgnoreCase) || fields.Count() == 0)
                {
                    var fieldNew = (fieldName != null ? fieldName : prop.Name);

                    FieldBuilder newField = dynamicAnonymousType.DefineField($"<{fieldNew}>k_BackingField", prop.PropertyType, FieldAttributes.Private);
                    
                    PropertyBuilder newProp = dynamicAnonymousType.DefineProperty(fieldNew, PropertyAttributes.HasDefault, prop.PropertyType, null);

                    MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

                    // get
                    MethodBuilder customGetPropMtdBldr = dynamicAnonymousType.DefineMethod("get_" + fieldNew, getSetAttr, prop.PropertyType, Type.EmptyTypes);

                    ILGenerator customGetIL = customGetPropMtdBldr.GetILGenerator();
                    customGetIL.Emit(OpCodes.Ldarg_0);
                    customGetIL.Emit(OpCodes.Ldfld, newField);
                    customGetIL.Emit(OpCodes.Ret);

                    // set
                    MethodBuilder customSetPropMtdBldr = dynamicAnonymousType.DefineMethod("set_" + fieldNew, getSetAttr, null, new Type[] { prop.PropertyType });

                    ILGenerator customSetIL = customSetPropMtdBldr.GetILGenerator();
                    customSetIL.Emit(OpCodes.Ldarg_0);
                    customSetIL.Emit(OpCodes.Ldarg_1);
                    customSetIL.Emit(OpCodes.Stfld, newField);
                    customSetIL.Emit(OpCodes.Ret);

                    newProp.SetGetMethod(customGetPropMtdBldr);
                    newProp.SetSetMethod(customSetPropMtdBldr);
                }
            }

            // Let's add some fields to the type.

            // Return the type to the caller
            return dynamicAnonymousType.CreateType();
        }

    }
}
