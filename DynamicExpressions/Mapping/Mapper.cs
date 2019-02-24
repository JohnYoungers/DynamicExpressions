using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace DynamicExpressions.Mapping
{
    public static class Mapper
    {
        private static ModuleBuilder ModuleBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicMaps"), AssemblyBuilderAccess.Run).DefineDynamicModule("Module");
        private static int TypeCounter;

        public static MappedType<TSource, TEntity> GenerateMappedType<TSource, TEntity>(DynamicTypeDefinition definition)
        {
            var sourceParam = Expression.Parameter(typeof(TSource), "");

            (Type type, Expression map) Generate(Type entityType, DynamicTypeDefinition def)
            {
                TypeBuilder typeBuilder = ModuleBuilder.DefineType($"MappedType_{++TypeCounter}", TypeAttributes.Public);
                var entityParam = Expression.Parameter(entityType, "");

                var bindings = def.Fields.Select(fieldDef =>
                {
                    var exp = fieldDef.ExpressionGenerator != null
                                ? fieldDef.ExpressionGenerator(sourceParam, entityParam)
                                : Linq.DynamicExpression.Parse(new[] { entityParam }, null, fieldDef.Expression, null);

                    typeBuilder.DefineField(fieldDef.Name, exp.Type, FieldAttributes.Public);

                    return new { Name = fieldDef.Name, Expression = exp };
                }).Union(def.Lists.Select(listDef =>
                {
                    var exp = listDef.ExpressionGenerator != null
                                ? listDef.ExpressionGenerator(sourceParam, entityParam)
                                : Linq.DynamicExpression.Parse(new[] { entityParam }, null, listDef.Expression, null);

                    var enumerableInterface = (new[] { exp.Type }).Union(exp.Type.GetInterfaces()).FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                    if (enumerableInterface == null)
                    {
                        throw new ArgumentException($"List {listDef.Name} must result in a type that implements IEnumerable<>");
                    }

                    var itemType = enumerableInterface.GetGenericArguments()[0];

                    var childMappedType = Generate(itemType, listDef);
                    typeBuilder.DefineField(listDef.Name, typeof(List<object>), FieldAttributes.Public);

                    var asQueryable = Expression.Call(typeof(Queryable), "AsQueryable", new[] { itemType }, exp);
                    var asQueryableSelect = Expression.Call(typeof(Queryable), "Select", new[] { itemType, typeof(object) }, asQueryable, childMappedType.map);
                    var asQueryableSelectToList = Expression.Call(typeof(Enumerable), "ToList", new[] { typeof(object) }, asQueryableSelect);

                    return new { Name = listDef.Name, Expression = asQueryableSelectToList as Expression };
                })).ToList();

                var generatedType = typeBuilder.CreateTypeInfo().AsType();
                var newItemExpression = Expression.MemberInit(Expression.New(generatedType),
                                                              bindings.Select(f => Expression.Bind(generatedType.GetField(f.Name), f.Expression)));

                var lamdaFuncType = (typeof(Func<,>)).MakeGenericType(entityType, typeof(object));
                return (generatedType, Expression.Lambda(lamdaFuncType, newItemExpression, entityParam));
            }

            var (type, map) = Generate(typeof(TEntity), definition);

            return new MappedType<TSource, TEntity>
            {
                GeneratedType = type,
                Map = Expression.Lambda<Func<TSource, Expression<Func<TEntity, object>>>>(map, sourceParam).Compile()
            };
        }
    }
}
