using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Bullet : MonoBehaviour
{
    private Enemy enemyScript;
    private GameObject player;
    private Player playerScript;
    public GameObject enemy;
    private Vector3 forward;
    public bool playerBullet;
    private Vector3 enemyPos;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerScript = GameObject.Find("Player").GetComponent<Player>();

        enemy = GameObject.Find("Enemy 1");
        enemyScript = GameObject.Find("Enemy 1").GetComponent<Enemy>();

        player = GameObject.Find("Player");
        forward = enemy.transform.forward;

        enemyPos = enemy.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += forward * Time.deltaTime * enemyScript.e_BulletSpeed;

        //If bullet distance goes too far
        float distance = Vector3.Distance(enemyPos, transform.position);
        if (enemyScript.e_BulletDist <= distance)
        {
            Death();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player.GetComponent<Player>().DecreaseHealth(enemyScript.e_BulletDamage);
            Death();
        }
    }
    void Death()
    {
        Destroy(gameObject);
    }
}
