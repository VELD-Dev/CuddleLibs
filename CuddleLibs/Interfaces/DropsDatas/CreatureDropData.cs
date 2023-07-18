using System;
using System.Collections.Generic;
namespace CuddleLibs.Interfaces.DropDatas;

/// <summary>
/// Representation of a creature drop data.
/// </summary>
public class CreatureDropData : DropData
{
    /// <summary>
    /// Wether the resource can spawn with other drops (of other mods maybe) or if it must be an unique spawn, that spawns all alone.<br/>
    /// Unique spawns are priorised.
    /// </summary>
    public bool unique = false;

    /// <summary>
    /// How many of this resource must drop of the 
    /// </summary>
    public ushort dropAmount = 1;
}
