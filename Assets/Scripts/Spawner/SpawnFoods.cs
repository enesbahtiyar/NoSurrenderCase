using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFoods : Singleton<SpawnFoods>
{
    [SerializeField] GameObject foodObject;
    [SerializeField] float timer = 3f;
    [SerializeField] Transform foodHolder;

    Vector3 spawnPoints;
    float spawnX, spawnZ, spawnY;

    protected override void Awake()
    {
        base.Awake();

    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            SpawnBoost(Random.Range(1, 3));
            timer = 3f;
        }
    }


    public void SpawnBoost(float spawnCount)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            spawnX = Random.Range(-20f, 20f);
            spawnZ = Random.Range(-20f, 20f);
            spawnY = 4f;
            spawnPoints = new Vector3(spawnX, spawnY, spawnZ);
            Instantiate(foodObject, spawnPoints, Quaternion.identity, foodHolder);
        }
    }
}



