using System;
using System.Reflection;
using System.Reflection.Emit;

namespace EFAuto
{
    public static class TypeBuilderExtension
    {
        public static void AddProperty(this TypeBuilder typeBuilder, string nombre, Type tipo)
        {
            FieldBuilder fieldBuilder;
            PropertyBuilder propertyBuilder;
            MethodBuilder getPropMthdBldr;
            ILGenerator getIl;
            MethodBuilder setPropMthdBldr;
            ILGenerator setIl;
            Label modifyProperty;
            Label exitSet;

            fieldBuilder = typeBuilder.DefineField(nombre.ToLower(), tipo, FieldAttributes.Private);

            propertyBuilder = typeBuilder.DefineProperty(nombre, PropertyAttributes.HasDefault, tipo, null);
            getPropMthdBldr = typeBuilder.DefineMethod("get_" + nombre, MethodAttributes.Public , tipo, Type.EmptyTypes);
            getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            setPropMthdBldr = typeBuilder.DefineMethod("set_" + nombre,
                 MethodAttributes.Public ,
                 null, new[] { tipo });

            setIl = setPropMthdBldr.GetILGenerator();
            modifyProperty = setIl.DefineLabel();
            exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }
    }
}
