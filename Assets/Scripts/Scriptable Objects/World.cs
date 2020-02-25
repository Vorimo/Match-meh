using UnityEngine;

namespace Scriptable_Objects {
    [CreateAssetMenu (fileName = "World", menuName = "World")]
    public class World : ScriptableObject {
        public Level[] levels;
    }
}
