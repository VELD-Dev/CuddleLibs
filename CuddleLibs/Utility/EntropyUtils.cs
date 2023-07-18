namespace CuddleLibs.Utility;

/// <summary>
/// Utils for player entropy. Basically all the things related to
/// </summary>
public static class EntropyUtils
{
    /// <summary>
    /// This is mandatory. This is a class that checks the chance of spawning the techtype.
    /// <para>Normally you shouldn't have to set this manually, it is handled by all the functions adding a new drop.</para>
    /// </summary>
    /// <param name="techType"><see cref="TechType"/> of the item to ensure the <see cref="PlayerEntropy.TechEntropy"/> for.</param>
    /// <returns>An instance of the <see cref="PlayerEntropy.TechEntropy"/> created or found.</returns>
    public static PlayerEntropy.TechEntropy EnsureTechEntropy(TechType techType)
    {
        PlayerEntropy playerEntropy = Player.main.gameObject.GetComponent<PlayerEntropy>();
        PlayerEntropy.TechEntropy techEntropy = playerEntropy.randomizers.Find((pete) => pete.techType == techType);
        if (techEntropy == null)
        {
            techEntropy = new PlayerEntropy.TechEntropy()
            {
                techType = techType,
                entropy = new FairRandomizer()
            };

            playerEntropy.randomizers.Add(techEntropy);
        }
        return techEntropy;
    }
}
