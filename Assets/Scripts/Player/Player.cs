
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Player : Singleton<Player>
{
    [Header("Player Settings")]
    [SerializeField] Transform player;
    [SerializeField] float speed = 5;
    [SerializeField] float rotationSpeed = 500;
    public float currentPower;
    public float currentMass;
    public int currentScore;
    public bool isPlayerDeath;

    private Vector2 startTouch;
    private Vector2 swipeDelta;

    private Touch touch;

    Vector3 touchDown;
    Vector3 touchUp;
    bool dragStarted;

    Rigidbody rigidbody;

    protected override void Awake()
    {
        base.Awake();
        rigidbody = GetComponent<Rigidbody>();

        currentMass = 1.2f;
        currentPower = 2;
        rigidbody.mass = currentMass;
        currentScore = 0;
    }

    private void Update()
    {
        CheckPlayerYPosition();
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        Movement();
        rigidbody.mass = currentMass;
    }

    //CalculateMovement
    void Movement()
    {
        gameObject.transform.Translate(Vector3.forward * Time.deltaTime * speed);
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                dragStarted = true;
                touchDown = touch.position;
                touchUp = touch.position;
            }
        }
        if (dragStarted)
        {
            if (touch.phase == TouchPhase.Moved)
            {
                touchDown = touch.position;
            }
            if (touch.phase == TouchPhase.Ended)
            {
                touchDown = touch.position;
                dragStarted = false;
            }
            gameObject.transform.rotation = Quaternion.RotateTowards(transform.rotation, CalculateRotation(), rotationSpeed * Time.deltaTime);
        }
    }
    Quaternion CalculateRotation()
    {
        Quaternion _temp = Quaternion.LookRotation(CalculateDirection(), Vector3.up);
        return _temp;
    }
    Vector3 CalculateDirection()
    {
        Vector3 _temp = (touchDown - touchUp).normalized;
        _temp.z = _temp.y;
        _temp.y = 0;
        return _temp;
    }


    /// <summary>
    /// Check if the collided object's tag is enemy and verify that enemy has rigidbody and EnemyAI scripts
    /// if so check their mass's and add force according to the given parameters
    /// 
    /// if the collided object is food add power to the player
    /// </summary>
    private void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.tag == "Enemy")
        {
            if (other.gameObject.TryGetComponent(out Rigidbody enemyRigidbody) && other.gameObject.TryGetComponent(out EnemyAI enemy))
            {
                if (this.currentMass > enemy.currentMass)
                {
                    //Throw Back enemy
                    Vector3 awayFromPlayer = (other.gameObject.transform.position - this.transform.position);
                    enemyRigidbody.AddForce(awayFromPlayer * this.currentPower, ForceMode.Impulse);
                }
                else if (this.currentMass < enemy.currentMass)
                {
                    //Throw Back player
                    Vector3 awayFromEnemy = (this.transform.position - other.gameObject.transform.position);
                    rigidbody.AddForce(awayFromEnemy * enemy.currentPower, ForceMode.Impulse);
                }
                else if (this.currentMass == enemy.currentMass)
                {
                    //Throw back two of them
                    Vector3 awayFromEnemy = (this.transform.position - other.gameObject.transform.position);
                    Vector3 awayFromPlayer = (other.gameObject.transform.position - this.transform.position);
                    rigidbody.AddForce(awayFromEnemy * enemy.currentPower, ForceMode.Impulse);
                    enemyRigidbody.AddForce(awayFromPlayer * this.currentPower, ForceMode.Impulse);
                }
            }
            else
            {
                return;
            }
        }

        if(other.gameObject.tag == "Food")
        {
            this.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
            currentScore += 100;
            currentMass += 0.1f;
            currentPower += 2;
            Destroy(other.gameObject);
        }
    }

    void CheckPlayerYPosition()
    {
        if(this.transform.position.y < 0)
        {
            isPlayerDeath = true;
            this.gameObject.SetActive(false);
            UIManager.Instance.gameStates = Enums.GameStates.GameFinished;
        }
    }


}
