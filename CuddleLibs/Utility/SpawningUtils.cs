namespace CuddleLibs.Utility;

/// <summary>
/// A bunch of functions to spawn easily some prefabs.
/// </summary>
public static class SpawningUtils
{
    /// <summary>
    /// Spawns a resource using its <see cref="TechType"/>
    /// </summary>
    /// <param name="parent">Instance of <see cref="BreakableResource"/>.</param>
    /// <param name="techType"><see cref="TechType"/> the resource to spawn.</param>
    public static void SpawnResourceFromTechType(GameObject parent, TechType techType)
    {
        CoroutineHost.StartCoroutine(SpawnResourceAsync(techType, parent.gameObject.transform.position + parent.gameObject.transform.up * 0.01f, parent.gameObject.transform));
    }

    /// <summary>
    /// Makes spawn asynchronely a resource at a position with a certain transform.
    /// </summary>
    /// <param name="techType">TechType of the prefab to spawn.</param>
    /// <param name="position">Position at spawn of the prefab.</param>
    /// <param name="transform">Transform of the parent gameobject.</param>
    /// <returns></returns>
    public static IEnumerator SpawnResourceAsync(TechType techType, Vector3 position, Transform transform)
    {
        CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(techType);
        yield return task;
        try
        {
            var prefab = task.GetResult();
            if (prefab == null)
            {
                InternalLogger.Error("Failed to spawn {0}: Prefab is null.", techType);
                yield break;
            }
            Rigidbody rigidbody;
            if (prefab.gameObject.GetComponentInChildren<Transform>() != null)
            {
                EditorModifications.Instantiate(prefab, transform, position, new Quaternion(0, 0, 0, 0), true);
                rigidbody = prefab.EnsureComponent<Rigidbody>();
                UWE.Utils.SetIsKinematicAndUpdateInterpolation(rigidbody, false, false);
                rigidbody.AddTorque(Vector3.right * (float)UnityEngine.Random.Range(3, 6));
                rigidbody.AddForce(new Vector3(0f, 90f, 0f) * 0.1f);
                yield break;
            }
            EditorModifications.Instantiate(prefab, position, new(), true);
            rigidbody = prefab.EnsureComponent<Rigidbody>();
            UWE.Utils.SetIsKinematicAndUpdateInterpolation(rigidbody, false, false);
            rigidbody.AddTorque(Vector3.right * (float)UnityEngine.Random.Range(3, 6));
            rigidbody.AddForce(new Vector3(0f, 90f, 0f) * 0.1f);
        }
        catch (Exception e)
        {
            InternalLogger.Error($"An error ocurred while spawning Resource prefab ({techType}).\n{e}");
            yield break;
        }
    }
}
