using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DigitalArtsAFAD
{

    /// <summary>
    /// Spawns objects at the given location.
    /// The spawned objects have no parent unless parent is specified.
    /// 
    /// Objects are spawned either manually by calling Spawn() method 
    /// or automatically after a random period of time defined by minInterval, maxInterval values.
    /// 
    /// </summary>
    public class Spawner : MonoBehaviour
    {
        [Tooltip("Prefabs (or gameobjects) that spawned objects will be cloned from.")]
        public GameObject[] prefabs;

        [Tooltip("Spawned objects are children of spawner gameobject unless parent is specified")]
        public Transform parent;

        [Tooltip("Specify maximum of spawned objects. If the limit is reached, oldest objects are destroyed")]
        public int maxCount = 0;

        [Space(5)]
        public UnityEvent<GameObject> onSpawn;
        float timer = 0;
        Queue<GameObject> spawns = new Queue<GameObject>();

        Queue<GameObject> toSpawn = new Queue<GameObject>();

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    Spawn();
                }
            }
        }

        private void FixedUpdate()
        {
            while (toSpawn.Count > 0)
            {
                GameObject go = toSpawn.Dequeue();
                DelayedSpawn(go);
            }
        }

        public void Spawn()
        {
            if (prefabs.Length > 0)
            {
                GameObject go = prefabs[Random.Range(0, prefabs.Length)];
                Spawn(go);
                onSpawn.Invoke(go);
            }
        }

        void DelayedSpawn(GameObject go)
        {
            GameObject spawned = Instantiate(go);
            spawned.transform.position = CalculatePosition();
            if (parent == null)
            {
                spawned.transform.SetParent(transform, true);
            }
            else
            {
                spawned.transform.SetParent(parent, true);
            }


            if (maxCount > 0)
            {
                spawns.Enqueue(spawned);
                while (spawns.Count > maxCount)
                {
                    Destroy(spawns.Dequeue());
                }
            }
        }

        public void Spawn(GameObject go)
        {
            toSpawn.Enqueue(go);
        }

        Vector3 CalculatePosition()
        {
            Vector3 result = transform.position;

            BoxCollider bc;
            if ((bc = GetComponent<BoxCollider>()) != null)
            {
                return PositionInsideBoxCollider(bc);
            }

            SphereCollider sc;
            if ((sc = GetComponent<SphereCollider>()) != null)
            {
                return PositionInsideSphereCollider(sc);
            }

            return result;
        }

        Vector3 PositionInsideBoxCollider(BoxCollider collider)
        {
            Vector3 pos = collider.center + new Vector3(
                Random.Range(-0.5f * collider.size.x, 0.5f * collider.size.x),
                Random.Range(-0.5f * collider.size.y, 0.5f * collider.size.y),
                Random.Range(-0.5f * collider.size.z, 0.5f * collider.size.z)
                );
            return transform.TransformPoint(pos);
        }

        Vector3 PositionInsideSphereCollider(SphereCollider collider)
        {
            Vector3 pos = collider.center + Random.insideUnitSphere * collider.radius;
            return transform.TransformPoint(pos);
        }
    }
}