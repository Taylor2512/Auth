using AutoMapper;

namespace Auth.Shared.Extensions.Mapping
{
    /// <summary>
    /// Contains extension methods for mapping objects using AutoMapper library.
    /// </summary>
    public static class MapperExtensions
    {
        /// <summary>
        /// Represents the destination type for mapping.
        /// </summary>
        /// <typeparam name="TDestination">The type to which the source objects will be mapped.</typeparam>
        /// <param name="mapper">The IMapper instance used for mapping.</param>
        /// <param name="sources">The source objects to be mapped.</param>
        /// <returns>The mapped destination object.</returns>
        public static TDestination Map<TDestination>(this IMapper mapper, params object[] sources) where TDestination : new()
        {
            return Map(mapper, new TDestination(), sources);
        }

        /// <summary>
        /// Maps the provided sources to the destination object using the AutoMapper library.
        /// </summary>
        /// <typeparam name="TDestination">The type of the destination object.</typeparam>
        /// <param name="mapper">The IMapper instance used for mapping.</param>
        /// <param name="destination">The destination object to be mapped.</param>
        /// <param name="sources">The source objects to be mapped from.</param>
        /// <returns>The mapped destination object.</returns>
        public static TDestination Map<TDestination>(this IMapper mapper, TDestination destination, params object[] sources) where TDestination : new()
        {
            if (!sources.Any())
            {
                return destination;
            }

            foreach (object? src in sources.Where(e => e != null).ToList())
            {
                destination = mapper.Map(src, destination);
            }

            return destination;
        }

        /// <summary>
        /// Ignores members during mapping if the source member is empty.
        /// </summary>
        /// <typeparam name="TSource">The type of the source object.</typeparam>
        /// <typeparam name="TDestination">The type of the destination object.</typeparam>
        /// <param name="expression">The mapping expression.</param>
        /// <returns>The updated mapping expression.</returns>
        public static IMappingExpression<TSource, TDestination> IgnoreIfEmpty<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            IgnoreMembers(expression);
            IMappingExpression<TDestination, TSource> newExpresion = expression.ReverseMap();
            IgnoreMembers(newExpresion);
            return expression;
        }

        /// <summary>
        /// Represents a mapping expression between source and destination types.
        /// </summary>
        /// <param name="expression">The mapping expression.</param>
        /// <returns>The updated mapping expression.</returns>
        public static IMappingExpression IgnoreIfEmpty(this IMappingExpression expression)
        {
            IgnoreMembers(expression);
            IMappingExpression newExpression = expression.ReverseMap();
            IgnoreMembers(newExpression);
            return expression;
        }

        /// <summary>
        /// Includes the mappings defined in the specified TypeMap configuration into the current IMappingExpression.
        /// </summary>
        /// <param name="typeMapExpression">The current IMappingExpression to include the mappings into.</param>
        /// <param name="typeMapConfiguration">The TypeMap configuration containing the mappings to include.</param>
        /// <returns>The updated IMappingExpression with the included mappings.</returns>
        public static IMappingExpression Includes(this IMappingExpression typeMapExpression, TypeMap typeMapConfiguration)
        {
            foreach (PropertyMap? propertyMap in typeMapConfiguration.PropertyMaps)
            {
                if (propertyMap.SourceMember != null)
                {
                    _ = typeMapExpression.ForMember(propertyMap.DestinationMember.Name, opt =>
                    {
                        opt.MapFrom(propertyMap.SourceMember.Name);
                    });
                }
            }

            return typeMapExpression;
        }

        /// <summary>
        /// Ignores members during mapping based on a specified condition.
        /// </summary>
        /// <param name="expression">The mapping expression.</param>
        private static void IgnoreMembers(IMappingExpression expression)
        {
            expression.ForAllMembers(opts => opts.Condition((src, dest, srcMember, destMember, context) =>
            {
                return srcMember != null && ((srcMember is not int && srcMember is not decimal) || Convert.ToDecimal(srcMember) != 0);
            }));
        }

        /// <summary>
        /// Ignores members during mapping based on a specified condition.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TDestination">The destination type.</typeparam>
        /// <param name="expression">The mapping expression.</param>
        private static void IgnoreMembers<TSource, TDestination>(IMappingExpression<TSource, TDestination> expression)
        {
            expression.ForAllMembers(opts => opts.Condition((src, dest, srcMember, destMember, context) =>
            {
                return srcMember != null && ((srcMember is not int && srcMember is not decimal) || Convert.ToDecimal(srcMember) != 0);
            }));
        }
    }
}
