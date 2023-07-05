namespace CuddleLibs.Interfaces;

/// <summary>
/// Representation of a an outcrop drop data.
/// </summary>
public class OutcropDropData
{
    /// <summary>
    /// TechType of the resource to spawn.
    /// </summary>
    public required TechType resourceTechType;

    /// <summary>
    /// Percentage of chance to make it drop when the resource is broken (between 0 and 1).
    /// </summary>
    public float chance = 0.5f;

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
}
