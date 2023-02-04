using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] Transform EnemyParent;
    public float time = 30;
    public bool AllTheEnemiesAreDead;
    public bool TimeRunOut;

    private void Update()
    {
        CheckTimer();
        CheckIfThereisEnemy();
    }

    void CheckTimer()
    {
        time -= Time.deltaTime;
        if(time <= 0)
        {
            TimeRunOut= true;
            UIManager.Instance.gameStates = Enums.GameStates.GameFinished;
        }
    }

    void CheckIfThereisEnemy()
    {
        if(EnemyParent.childCount <= 0)
        {
            AllTheEnemiesAreDead = true;
            UIManager.Instance.gameStates = Enums.GameStates.GameFinished;
        }
    }
}
