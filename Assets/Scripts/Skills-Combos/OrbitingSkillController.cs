using System;
using Unity.Mathematics;
using UnityEngine;

public class OrbitingSkillController : MonoBehaviour
{
    [Header("Configuração da Habilidade")]
    public GameObject spherePrefab;
    public float orbitDistance = 1.5f;
    public float rotationSpeed = 100f;

    [Header("Estatísticas (Nível)")]
    //public int sphereCount = 3;
    private bool hasSpawned = false;

    void Start()
    {
        //SpawnSpheres();
        
    }
    void FixedUpdate()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        if (hasSpawned && this.transform.childCount == 0)
        {
            Destroy(this.gameObject);
        }
    }

    //private void SpawnSpheres()
    //{
      //  for (int i = 0; i < sphereCount; i++)
        //{
          //  float angle = i * (360f / sphereCount);
//
  //          Vector2 localPos = Quaternion.Euler(0, 0, angle) * (Vector2.up * orbitDistance);
//
  //          GameObject sphere = Instantiate(spherePrefab, (Vector2)transform.position + localPos, Quaternion.identity, this.transform);
//
  //          sphere.transform.localPosition = localPos;
    //    }
      //  hasSpawned = true;
    //}
}
