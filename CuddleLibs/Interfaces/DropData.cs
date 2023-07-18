namespace CuddleLibs.Interfaces;

/// <summary>
/// Rerpresentation of a drop data.
/// </summary>
public abstract class DropData
{
    /// <summary>
    /// <see cref="TechType"/> of the resource to drop.
    /// </summary>
    public required TechType TechType;

    /// <summary>
    /// Percentage of chance of spawning the item when it can drop.
    /// </summary>
    public float chance;

    /// <summary>
    /// Gives a string representation of the <see cref="OutcropDropData"/>.
    /// <example>
    /// For example:
    /// <code>
    /// {
    ///     float someField: 156.11f,
    ///     string someOtherField: "Hello world!"
    /// }
    /// </code>
    /// </example>
    /// </summary>
    /// <returns>A string representation of an <see cref="OutcropDropData"/></returns>
    public override string ToString()
    {
        var str = new StringBuilder();
        str.AppendLine("{");
        var fields = typeof(OutcropDropData).GetFields();
        for (int i = 0; i < fields.Length; i++)
        {
            var item = fields[i];
            str.AppendLine($"\t{item.FieldType} {item.Name}: {item.GetValue(this)}{(i != (fields.Length - 1) ? "," : string.Empty)}");
        }
        str.AppendLine("}");
        return str.ToString();
    }

    /// <summary>
    /// <inheritdoc cref="ToString()"/>
    /// </summary>
    /// <param name="prepend">A string that will be prepend to every line. It's better to use it with <c>\t</c> tabs for proper tabulation in logs.</param>
    /// <returns></returns>
    public string ToString(string prepend)
    {
        var str = new StringBuilder();
        str.AppendLine($"{prepend}{{");
        var fields = typeof(OutcropDropData).GetFields();
        for (int i = 0; i < fields.Length; i++)
        {
            var item = fields[i];
            str.AppendLine($"{prepend}\t{item.FieldType} {item.Name}: {item.GetValue(this)}{(i != (fields.Length - 1) ? "," : string.Empty)}");
        }
        str.AppendLine($"{prepend}}}");
        return str.ToString();
    }
}
