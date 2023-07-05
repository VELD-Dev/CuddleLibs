# Nautilus Extensions by V E L D
A bunch of extensions made by me for modders, they are globally only maintained by me and made for my needs, but I'm totally open to pull requests, suggestions, issues etc...  
I've been working on that because many of my ideas would probably bloat Nautilus more than it is already, so I just make a bunch of additional libs.

## If you install the extension as a modder or as an user
You must put the `.dll` file(s) into the Nautilus folder already existing, so they are all next to them.  
You must absolutely have Nautilus installed, these libs does not work without Nautilus since they globally extend Nautilus.

### As a modder
Then, all you have to do is to add it as a reference and you're ready to go, the namespaces always start with Nautilus so you're not lost and you can find the features you want in the same namespace, after that, it depends, for example the outcrops helper is in Nautilus.OutcropsHelper.

---

## 🪨 `Nautilus.OutcropsHelper`
Nautilus Outcrops Helper is a lib that lets developers easily add their ores to drops of certain chunks/outcrops, for example I can add a Coal drop to Limestones by doing this:

```csharp
// The first argument is the Outcrop TechType.
// The second argument is the spawn chance between 0 and 1, based on something UWE called "Player Entropy" which globally computes the luck of the player.
coalCustomPrefab.SetOutcropDrop(TechType.LimestoneChunk, 0.71f)

// And if I want to add it to several outcrops, here it is:
coalCustomPrefab.SetOutcropDrop(
    new KeyValuePair<TechType, float>(TechType.BreakableLead, 0.56f),
    new(TechType.BreakableSilver, 0.2f),
    new(TechType.ObsidianChunk, 0.47f)
);
```

I hope you'll use it if you make mods like that, the good point is that it does not break the compatibility with other mods as long as they use this lib too
