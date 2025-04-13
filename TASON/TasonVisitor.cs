using System.Globalization;
using System.Runtime.CompilerServices;
using Antlr4.Runtime.Tree;
using TASON.Grammar;
using TASON.Types;
using TASON.Util;

namespace TASON;

public partial class TasonVisitor(TasonTypeRegistry registry, SerializerOptions options)
{
    
    /// <summary>
    /// Visit a parse tree produced by <see cref="TASONParser.start"/>.
    /// </summary>
    /// <param name="context">The parse tree.</param>
    /// <return>The visitor result.</return>
    public object? Start(TASONParser.StartContext context)
    {
        return ValueContext(context.value());
    }   
    
    internal object? ValueContext(TASONParser.ValueContext ctx)
    {
        return ctx switch
        {
            TASONParser.NullValueContext => null,
            TASONParser.BooleanValueContext booleanValue => BooleanValue(booleanValue),
            TASONParser.StringValueContext stringValue => StringValue(stringValue),
            TASONParser.NumberValueContext numberValue => NumberValue(numberValue),
            TASONParser.ArrayValueContext arrayValue => ArrayValue(arrayValue),
            TASONParser.ObjectValueContext objectValue => ObjectValue(objectValue),
            TASONParser.TypeInstanceValueContext typeInstanceValue => TypeInstanceValue(typeInstanceValue),
            _ => throw new ArgumentException($"Unsupported value type: {ctx.GetType().Name}"),
        };
    }

    internal static bool BooleanValue(TASONParser.BooleanValueContext ctx)
    {
        return ctx.boolean().GetText() switch
        {
            "true" => true,
            _ => false,
        };
    }

    internal static string StringValue(TASONParser.StringValueContext ctx)
    {
        return GetTextValue(ctx.STRING());
    }

    internal static double NumberValue(TASONParser.NumberValueContext ctx)
    {
        return PrimitiveHelpers.ParseTasonNumber(ctx.number().GetText());
    }

    internal object?[] ArrayValue(TASONParser.ArrayValueContext ctx)
    {
        return Array(ctx.array().value());
    }    
    
    internal object?[] Array(TASONParser.ValueContext[] ctx)
    {
        return ctx.Select(ValueContext).ToArray();
    }

    internal Dictionary<string, object?> ObjectValue(TASONParser.ObjectValueContext ctx) {
        return Object(ctx.@object());    
    }

    internal Dictionary<string, object?> Object(TASONParser.ObjectContext ctx)
    {
        var obj = new Dictionary<string, object?>();
        var pairs = ctx.pair().Select(Pair);
        if (!options.AllowDuplicatedKeys)
        {
            var keys = pairs.Select(p => p.Key).Distinct();
            if (keys.Count() != pairs.Count())
            {
                throw new ArgumentException("Duplicate keys in object");
            }
        }

        foreach (var pair in pairs)
        {
            obj[pair.Key] = pair.Value;
        }
        return obj;
    }

    internal KeyValuePair<string, object?> Pair(TASONParser.PairContext ctx)
    {
        return new (Key(ctx.key()), ValueContext(ctx.value()));
    }

    internal static string Key(TASONParser.KeyContext ctx) {
        if (ctx is TASONParser.IdentifierContext identifier)
        {
            return identifier.GetText();
        }
        return GetTextValue((ctx as TASONParser.StringKeyContext)!.STRING());
    }

    internal object TypeInstanceValue(TASONParser.TypeInstanceValueContext ctx)
    {
        var typeInstance = ctx.typeInstance();
        return typeInstance switch
        {
            TASONParser.ScalarTypeInstanceContext scalarType => ScalarTypeInstance(scalarType),
            TASONParser.ObjectTypeInstanceContext objectType => ObjectTypeInstance(objectType),
            _ => throw new ArgumentException($"Unsupported type instance type: {typeInstance.GetType().Name}"),
        };
    }

    internal object ScalarTypeInstance(TASONParser.ScalarTypeInstanceContext ctx) 
    {
        var typeName = ctx.IDENTIFIER().GetText();
        var str = GetTextValue(ctx.STRING());

        return CreateTypeInstance(typeName, str);
    }
    internal object ObjectTypeInstance(TASONParser.ObjectTypeInstanceContext ctx) 
    {
        var typeName = ctx.IDENTIFIER().GetText();

        return CreateTypeInstance(typeName, ctx.@object());
    }    

    private object CreateTypeInstance(string typeName, TASONParser.ObjectContext obj) 
    {
        if (registry.GetDefaultType(typeName) is not ITasonObjectType typeInfo)
        {
            throw new ArgumentException($"Unregistered type: {typeName}");
        }

        return registry.CreateInstance(typeInfo, TypedObjectArg(obj, typeInfo.Type));
    }
    

    private object CreateTypeInstance(string typeName, string value) 
    {
        if (registry.GetDefaultType(typeName) is not ITasonScalarType typeInfo)
        {
            throw new ArgumentException($"Unregistered type: {typeName}");
        }

        return registry.CreateInstance(typeInfo, value);
    }   

    private static string GetTextValue(ITerminalNode node)
    {
        var str = node.GetText();
        if (str.Length < 2)
        {
            throw new ArgumentException("Invalid string literal");
        }
        str = str[1..^1];
        return PrimitiveHelpers.Unescape(str);

    }
}
