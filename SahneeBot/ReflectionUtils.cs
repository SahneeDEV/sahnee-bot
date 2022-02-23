using System.Reflection;

namespace SahneeBot;

public static class ReflectionUtils<T>
{
    private static readonly TypeInfo ObjectTypeInfo = typeof (object).GetTypeInfo();
    
    internal static Func<IServiceProvider, T> CreateBuilder<THandler>(
        TypeInfo typeInfo,
        THandler handler)
    {
        var constructor = GetConstructor(typeInfo);
        var parameters = constructor.GetParameters();
        var properties = GetProperties(typeInfo);
        return services =>
        {
            var args = new object[parameters.Length];
            for (var index = 0; index < parameters.Length; ++index)
                args[index] = GetMember<THandler>(handler, services, parameters[index].ParameterType, typeInfo);
            var builder = InvokeConstructor(constructor, args, typeInfo);
            foreach (var propertyInfo in properties)
                propertyInfo.SetValue(builder, GetMember<THandler>(handler, services, propertyInfo.PropertyType, typeInfo));
            return builder;
        };
    }

    private static T InvokeConstructor(
        ConstructorInfo constructor,
        object[] args,
        TypeInfo ownerType)
    {
        try
        {
            return (T) constructor.Invoke(args);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to create \"" + ownerType.FullName + "\".", ex);
        }
    }

    private static ConstructorInfo GetConstructor(TypeInfo ownerType)
    {
        var array = ownerType.DeclaredConstructors.Where<ConstructorInfo>(x => !x.IsStatic).ToArray<ConstructorInfo>();
        if (array.Length == 0)
            throw new InvalidOperationException("No constructor found for \"" + ownerType.FullName + "\".");
        return array.Length <= 1 ? array[0] : throw new InvalidOperationException("Multiple constructors found for \"" + ownerType.FullName + "\".");
    }

    private static PropertyInfo[] GetProperties(TypeInfo ownerType)
    {
        var propertyInfoList = new List<PropertyInfo>();
        for (; ownerType != ObjectTypeInfo; ownerType = ownerType.BaseType!.GetTypeInfo())
        {
            foreach (var declaredProperty in ownerType.DeclaredProperties)
            {
                var setMethod1 = declaredProperty.SetMethod;
                if ((setMethod1 != null ? !setMethod1.IsStatic ? 1 : 0 : 0) != 0)
                {
                    var setMethod2 = declaredProperty.SetMethod;
                    if ((setMethod2 != null ? setMethod2.IsPublic ? 1 : 0 : 0) != 0)
                        propertyInfoList.Add(declaredProperty);
                }
            }
        }
        return propertyInfoList.ToArray();
    }

    private static object GetMember<THandler>(
        THandler handler,
        IServiceProvider services,
        Type memberType,
        TypeInfo ownerType)
    {
        if (memberType == typeof (THandler) || memberType == handler?.GetType())
            return handler!;
        if (memberType == typeof (IServiceProvider) || memberType == services.GetType())
            return services;
        var service = services.GetService(memberType);
        if (service != null)
            return service;
        throw new InvalidOperationException($"Failed to create \"{ownerType.FullName}\", dependency \"{memberType.Name}\" was not found.");
    }
}