using UnityEngine;

public class enemyAnimator : MonoBehaviour
{
    Animator animator;
    const string ATTACK = "Attack";
    [SerializeField] enemyAI enemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void PlayAttack()
    {
        animator.SetTrigger(ATTACK);
    }
    public void ResetAttack()
    {
        animator.ResetTrigger(ATTACK);
    }
}
