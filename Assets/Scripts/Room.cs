 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private Transform[] enemySpawns;
    public GameObject enemyPrefab;
    // Start is called before the first frame update
    void Start()
    {
        enemySpawns = GetComponentsInChildren<Transform>();

        StartCoroutine("SpawnEnemies");

    }

    IEnumerator SpawnEnemies()
    {
        foreach (Transform i in enemySpawns)
        {
            if (i != this.gameObject.transform)
            {
                Instantiate(enemyPrefab, i.position, Quaternion.identity);
                yield return new WaitForSeconds(.1f);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
