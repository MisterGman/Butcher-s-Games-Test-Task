using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunner : MonoBehaviour
{
    [SerializeField]
    private Animator playerAnimator; 
    
    [SerializeField]
    private ParticleSystem bloodVisual;
    public Animator PlayerAnimator => playerAnimator;

    private GroupRunners _groupRunners;

    public void InitGroupRunners(GroupRunners groupRunners)
    {
        _groupRunners = groupRunners;
    }
    
    private void OnTriggerEnter(Collider obstacle)
    {
        if (obstacle.CompareTag("Gem"))
        {
            UIManager.instance.UpdateScore();
            Destroy(obstacle.gameObject);
        }

        if (obstacle.CompareTag("Saw"))
        {
            _groupRunners.RemoveRunner(this);
            bloodVisual.Play();
        }        
        
        if (obstacle.CompareTag("NewRunner"))
        {
            _groupRunners.AddRunner(obstacle.transform.position);
            Destroy(obstacle.gameObject);
        }
    }
}
