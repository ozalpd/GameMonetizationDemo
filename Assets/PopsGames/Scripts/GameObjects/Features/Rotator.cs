using UnityEngine;

namespace Pops.GameObjects.Features
{
    public class Rotator : MonoBehaviour
    {
        private void Start()
        {
            rotationSpeed = Random.Range(1.0f, 3.0f) * 70;
        }

        private float rotationSpeed;

        private void Update()
        {
            transform.Rotate(new Vector3(0, 0, rotationSpeed) * Time.deltaTime);
        }
    }
}