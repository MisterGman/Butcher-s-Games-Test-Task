using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class GroupRunners : MonoBehaviour
{
    [SerializeField]
    private PlayerRunner newPlayerRunnerPrefab;
    
    [SerializeField]
    private List<PlayerRunner> playerRunners;

    [SerializeField]
    private SplineFollower splineFollower;

    public List<PlayerRunner> PlayerRunners => playerRunners;

    private static readonly int Victory = Animator.StringToHash("Victory");
    private static readonly int StartGame = Animator.StringToHash("StartGame");

    public void StartRunning()
    {
        splineFollower.follow = true;
        
        foreach (var runner in playerRunners)
        {
            runner.PlayerAnimator.SetTrigger(StartGame);
            runner.InitGroupRunners(this);
        }
    }
    
    public void EndLevel()
    {
        foreach (var runner in playerRunners)
        {
            runner.PlayerAnimator.SetTrigger(Victory);
        }
    }

    public void AddRunner(Vector3 position)
    {
        PlayerRunner runner = Instantiate(newPlayerRunnerPrefab, position, Quaternion.identity, transform);
        playerRunners.Add(runner);
        
        runner.PlayerAnimator.SetTrigger(StartGame);
        runner.InitGroupRunners(this);
    }

    public void RemoveRunner(PlayerRunner runner)
    {
        runner.transform.SetParent(null);
        runner.PlayerAnimator.enabled = false;
        playerRunners.Remove(runner);

        if (playerRunners.Count <= 0)
            GameOver();
    }

    private void GameOver()
    {
        splineFollower.follow = false;
        UIManager.instance.ShowRestartButton();
    }
}
