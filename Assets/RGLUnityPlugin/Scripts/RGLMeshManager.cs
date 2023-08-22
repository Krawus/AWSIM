using System.Collections.Generic;
using UnityEngine;

namespace RGLUnityPlugin
{
    public class RGLMeshManager
    {
        private static Dictionary<int, RGLMesh> sharedMeshes = new Dictionary<int, RGLMesh>(); // <Identifier, RGLMesh>
        private static Dictionary<int, int> sharedMeshesUsageCount = new Dictionary<int, int>(); // <RGLMesh Identifier, count>

        public static RGLMesh RegisterRGLMeshInstance(Mesh unityMesh)
        {
            var meshId = unityMesh.GetInstanceID();
            if (!sharedMeshes.ContainsKey(meshId))
            {
                var rglMesh = new RGLMesh(meshId, unityMesh);
                sharedMeshes.Add(meshId, rglMesh);
                sharedMeshesUsageCount.Add(meshId, 1);
            }
            else
            {
                sharedMeshesUsageCount[meshId]++;
            }

            return sharedMeshes[meshId];
        }

        public static void UnregisterRGLMeshInstance(int meshId)
        {
            if (sharedMeshes[meshId] is null)
            {
                Debug.LogWarning($"Trying to unregister absent in RGLMeshManager mesh of id: {meshId}, ignoring request");
                return;
            }

            sharedMeshesUsageCount[meshId]--;
            if (sharedMeshesUsageCount[meshId] == 0)
            {
                sharedMeshes[meshId].DestroyFromRGL();
                sharedMeshes.Remove(meshId);
                sharedMeshesUsageCount.Remove(meshId);
            }
        }

        public static void ClearAllMeshes()
        {
            foreach (var mesh in sharedMeshes)
            {
                mesh.Value.DestroyFromRGL();
            }
        }
    }
}