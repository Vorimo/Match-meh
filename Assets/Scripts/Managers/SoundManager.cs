using UnityEngine;

namespace Managers {
    public class SoundManager : MonoBehaviour {

        public AudioSource[] destroyNoise;

        public void PlayRandomDestroyNoise() {
            var clipToPlay = Random.Range(0, destroyNoise.Length);
            destroyNoise[clipToPlay].Play();
        }
    }
}
