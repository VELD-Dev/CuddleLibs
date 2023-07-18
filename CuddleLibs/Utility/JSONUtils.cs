using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuddleLibs.Utility;

public class JSONUtils
{
    /// <summary>
    /// Ensures custom data files and folders exists. If not, create them.
    /// </summary>
    /// <param name="path">Path to the data folder.</param>
    /// <returns></returns>
    internal protected static bool EnsureCustomDataFiles(string path = null)
    {
        path ??= Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "data");
        InternalLogger.Debug(path);
        var assembly = Assembly.GetExecutingAssembly();
        List<Type> DataClasses = new();
        foreach(var type in assembly.GetTypes())
        {
            string ns = type.Namespace;
            if (ns.EndsWith("DropDatas"))
                DataClasses.Add(type);
        }

        if(!Directory.Exists(path))
            Directory.CreateDirectory(path);


        // Create paths for data classes and some samples.
        foreach (var c in DataClasses)
        {
            var datapath = Path.Combine(Path.GetDirectoryName(path), c.Name);
            InternalLogger.Debug(datapath);
            if (!Directory.Exists(datapath))
                Directory.CreateDirectory(datapath);

            var samplefilepath = Path.Combine(Path.GetDirectoryName(datapath), "sample.json");
            InternalLogger.Debug(samplefilepath);
            string sample;
            switch(true) {
                case true when c == typeof(CreatureDropData):
                {
                    var sampledata = new Dictionary<TechType, List<CreatureDropData>>()
                    {
                        {
                            TechType.ReaperLeviathan,
                            new()
                            {
                                new CreatureDropData() { TechType = TechType.TitaniumIngot, chance = 1f, dropAmount = 2, unique = false },
                                new CreatureDropData() { TechType = TechType.Kyanite, chance = 0.1f, dropAmount = 1, unique = true }
                            }
                        }
                    };

                    sample = JsonConvert.SerializeObject(sampledata);
                    break;
                }
                case true when c == typeof(OutcropDropData):
                {
                    var sampledata = new Dictionary<TechType, List<OutcropDropData>>()
                    {
                        {
                            TechType.LimestoneChunk,
                            new()
                            {
                                new OutcropDropData() { TechType = TechType.Lithium, chance = 0.05f },
                                new OutcropDropData() { TechType = TechType.Aerogel, chance = 0.3f }
                            }
                        }
                    };

                    sample = JsonConvert.SerializeObject(sampledata);
                    break;
                }
                default:
                    throw new NotImplementedException("DropData sample not implemented.");
            };

            File.WriteAllText(samplefilepath, sample);
        }
        return true;
    }

    internal protected static Dictionary<TechType, List<OutcropDropData>> LoadOutcropsFromFile(string path = null)
    {
        path ??= Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "data", nameof(OutcropDropData));

        var files = Directory.GetFiles(path);

        Dictionary<TechType, List<OutcropDropData>> outcropDrops = new();

        foreach(var file in files)
        {
            if (!File.Exists(file))
                continue;
            Dictionary<TechType, List<OutcropDropData>> newDrops = new();
            JsonConvert.PopulateObject(File.ReadAllText(file), newDrops);
            outcropDrops.Union(newDrops);
        }

        return outcropDrops;
    }

    internal protected static Dictionary<TechType, List<CreatureDropData>> LoadCreatureDropsFromFile(string path = null)
    {
        path ??= Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "data", nameof(CreatureDropData));

        var files = Directory.GetFiles(path);

        Dictionary<TechType, List<CreatureDropData>> creatureDrops = new();

        foreach(var file in files)
        {
            if(!File.Exists(file))
                continue;
            Dictionary<TechType, List<CreatureDropData>> newDrops = new();
            JsonConvert.PopulateObject(File.ReadAllText(file), newDrops);
            creatureDrops.Union(newDrops);
        }

        return creatureDrops;
    }
}
