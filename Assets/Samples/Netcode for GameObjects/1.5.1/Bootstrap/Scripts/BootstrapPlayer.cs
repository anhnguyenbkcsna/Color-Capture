using UnityEngine;

namespace Unity.Netcode.Samples
{
    /// <summary>
    /// Component attached to the "Player Prefab" on the `NetworkManager`.
    /// </summary>
    public class BootstrapPlayer : NetworkBehaviour
    {

        [ServerRpc]
        public void RandomTeleportServerRpc()
        {
            var oldPosition = transform.position;
            transform.position = GetRandomPositionOnXYPlane();
            var newPosition = transform.position;
            print($"{nameof(RandomTeleportServerRpc)}() -> {nameof(OwnerClientId)}: {OwnerClientId} --- {nameof(oldPosition)}: {oldPosition} --- {nameof(newPosition)}: {newPosition}");
        }

        private static Vector3 GetRandomPositionOnXYPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f);
        }
    }
}
