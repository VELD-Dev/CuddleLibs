# 🫂 `CuddleLibs`
![Lines of code](https://img.shields.io/tokei/lines/github/VELD-Dev/Nautilus-Extensions)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/735e6021dfd8498689e12e43aa30ca88)](https://app.codacy.com/gh/VELD-Dev/CuddleLibs/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade)
![GitHub all releases](https://img.shields.io/github/downloads/VELD-Dev/CuddleLibs/total)
![GitHub tag (with filter)](https://img.shields.io/github/v/tag/VELD-Dev/CuddleLibs?label=cuddlelibs)
![GitHub tag](https://img.shields.io/github/v/tag/SubnauticaModding/Nautilus?label=nautilus)
  
A bunch of libraries and extensions made by me for modders, they are globally only maintained by me and made for my needs, but I'm totally open to pull requests, suggestions, issues etc...  
I've been working on that because many of my ideas would probably bloat Nautilus more than it is already, so I just make a bunch of additional libs.  
  
They work on a very simple concept called modules. Actually there is one main file containing all the main helpers, but in the future it may have many modules so the library is not bloated with things that are not useful to every users or every modders. The plan is to let the core library download these files automatically and auto-update those when possible.

### If you install the extension as a modder or as an user
You can put the `.dll` file(s) into the Nautilus folder if you wish, but you can also put those in `SubnauticaGame/BepInEx/plugins/CuddleLib/`.  
You must absolutely have Nautilus installed, these libs does not work without Nautilus since they globally extend Nautilus.

### As a modder
Then, all you have to do is to add it as a reference and you're ready to go, the namespaces always start with CuddleLibs so you're not lost and you can find the features you want in the same namespace, after that, it depends, for example the outcrops utils are in `CuddleLibs.Utils`, etc...

---

## 🪨 `CuddleLibs key features`
  
An non-exhaustive list of all the key features of CuddleLibs. I cannot put all of them on one page if someday there is too many features.

### Outcrops helper and utils
CuddleLibs has an incorporated module that lets developers easily add their ores to drops of certain chunks/outcrops, for example I can add a Coal drop to Limestones by doing this:

```csharp
// coalTechType may be obtained with whatever library you're using, with Nautilus you can get the
// TechType with customPrefab.Info.TechType; With SMLHelper you can use MyClass.TechType;
OutcropsUtils.EnsureOutcropDrop(coalTechType, TechType.LimestoneChunk, chance: 0.2f);

// If we want to override the spawn chance of an existing ore:
OutcropsUtils.EnsureOutcropDrop(TechType.Copper, TechType.LimestoneChunk, chance: 0.6f);  // Spawn chances are extremely high, here.

// Or if we want to add an existing ore to an other outcrop
OutcropsUtils.EnsureOutcropDrop(TechType.Gold, TechType.BreakableLead, chance: 0.025f);  // And here, spawn chances are extremely low.
```

I hope you'll use it if you make mods like that, the good point is that it does not break the compatibility with other mods as long as they use this lib too

## 📥 `Install`
Here are some instructions on how to install CuddleLibs, even if it's like every BepInEx mod, you may need that just in case.

### Dependencies
- [**Tobey's BepInEx Pack**](https://github.com/toebeann/BepInEx.Subnautica/releases) **OR** [**BepInEx 5.4.21+**](https://github.com/BepInEx/BepInEx/releases) (you can as well only download BepInEx, it works very well too)

### Dependants
- The great [**Alterra Weaponry**](https://github.com/VELD-Dev/Alterra-Weaponry/releases) ! (made by me lol)
