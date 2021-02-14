using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Stardust.Flux.Crosscutting.Extensions
{
    public static class EnumExtensions
    {
        public static string GetEnumMemberValue<T>(this T value) where T : struct, IConvertible
        {
            var enumValue = typeof(T)
                .GetTypeInfo()
                .DeclaredMembers
                .SingleOrDefault(x => x.Name == value.ToString());

            return enumValue?.GetCustomAttribute<EnumMemberAttribute>(false)
                ?.Value ?? enumValue.ToString();
        }
    }
}