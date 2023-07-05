using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuddleLibs.Utility;

/// <summary>
/// Contains all the main information about CuddleLibs
/// </summary>
public sealed class LibInfo : BepInEx.PluginInfo
{
    public const string LIB_GUID = "com.velddev.cuddlelibs";
    public const string LIB_NAME = "CuddleLibs";
    public const string LIB_DISPLAYNAME = "Cuddle Libs";
    public const string LIB_VERS = "1.0.1";
    public const string LIB_LONGVERSION = "1.0.1.0";
}
