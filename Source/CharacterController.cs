using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//キャラクターのスーパークラス
public class CharacterController : MonoBehaviour
{
    //アニメーション情報
    protected Animator animator;
    //フィールド情報
    protected FieldController fieldController;

    #region inspector
    [Header("アニメーション名設定")]
    [SerializeField] protected string animNameStopFore = "StopFore";
    [SerializeField] protected string animNameStopLeft = "StopLeft";
    [SerializeField] protected string animNameStopRight = "StopRight";
    [SerializeField] protected string animNameStopBack = "StopBack";
    [SerializeField] protected string animNameWalkFore = "WalkFore";
    [SerializeField] protected string animNameWalkLeft = "WalkLeft";
    [SerializeField] protected string animNameWalkRight = "WalkRight";
    [SerializeField] protected string animNameWalkBack = "WalkBack";
    [SerializeField] protected string animNameDead = "Dead";
    [Header("初期アニメーション名")]
    [SerializeField] protected string animNameInitial = "StopFore";
    #endregion

    public enum Direction
    {
        FORE, LEFT, RIGHT, BACK
    }
    public enum AnimState
    {
        STOP, WALK
    }

    // Start is called before the first frame update
    virtual protected void Start()
    {
        fieldController = transform.parent.GetComponent<FieldController>();
        animator = gameObject.GetComponent<Animator>();
        animator.Play(animNameInitial);
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        
    }

    public Vector3Int GetPosition()
    {
        return fieldController.grid.WorldToCell(gameObject.transform.position);
    }

    public void Dead()
    {
        animator.Play(animNameDead);
    }
}
