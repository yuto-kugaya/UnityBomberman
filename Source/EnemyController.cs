using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class EnemyController : CharacterController
{
    #region Inspector
    [Header("スピード")]
    [SerializeField] int numSpeed = 1;
    #endregion

    Rigidbody2D rigidBody = new Rigidbody2D();
    Vector3Int nowPosition = new Vector3Int();
    bool isMoving = false;
    Direction _direction = Direction.FORE;
    float defaultSpeed = 0f;
    float speed;
    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        speed = defaultSpeed + numSpeed;
        isMoving = false;
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();

        //死亡したなら
        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == animNameDead)
        {
            //アニメが終わったなら
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                Destroy(gameObject);
            }
            //以降の処理を飛ばす
            return;
        }

        //移動判定とアニメーション名取得
        AnimState animState = MoveController(ref _direction);
        string nowAnimName = GetAnimationName(animState, _direction);
        //アニメーション変更
        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != nowAnimName)
        {
            animator.Play(nowAnimName);
        }
        
    }

    //移動
    AnimState MoveController(ref Direction direction)
    {
        //移動中かどうか
        //セル間を移動中の場合方向を維持する。
        if(isMoving)
        {
            switch (direction)
            {
                case Direction.FORE:
                    rigidBody.transform.position += new Vector3(0, -1f * speed * Time.deltaTime, 0);
                    break;
                case Direction.LEFT:
                    rigidBody.transform.position += new Vector3(-1f * speed * Time.deltaTime, 0, 0);
                    break;
                case Direction.RIGHT:
                    rigidBody.transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
                    break;
                case Direction.BACK:
                    rigidBody.transform.position += new Vector3(0, speed * Time.deltaTime, 0);
                    break;
            }
            isMoving = (nowPosition == GetPosition());
            return AnimState.WALK;

        }

        //ここに来るということは位置が前回で変わっているので座標更新
        nowPosition = GetPosition();

        switch (UnityEngine.Random.Range(0, 3))
        {
            case 0:
                if (fieldController.IsBlock(fieldController.grid.WorldToCell(gameObject.transform.position + new Vector3(speed * Time.deltaTime, 0, 0))))
                    return AnimState.STOP;
                rigidBody.transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
                isMoving = (nowPosition == GetPosition());
                direction = Direction.RIGHT;
                return AnimState.WALK;
            case 1:
                if (fieldController.IsBlock(fieldController.grid.WorldToCell(gameObject.transform.position + new Vector3(0, speed * Time.deltaTime, 0))))
                    return AnimState.STOP;
                rigidBody.transform.position += new Vector3(0, speed * Time.deltaTime, 0);
                isMoving = (nowPosition == GetPosition());
                direction = Direction.BACK;
                return AnimState.WALK;
            case 2:
                if (fieldController.IsBlock(fieldController.grid.WorldToCell(gameObject.transform.position + new Vector3(-1f * speed * Time.deltaTime, 0, 0))))
                    return AnimState.STOP;
                rigidBody.transform.position += new Vector3(-1f * speed * Time.deltaTime, 0, 0);
                isMoving = (nowPosition == GetPosition());
                direction = Direction.LEFT;
                return AnimState.WALK;
            case 3:
                if (fieldController.IsBlock(fieldController.grid.WorldToCell(gameObject.transform.position + new Vector3(0, -1f * speed * Time.deltaTime, 0))))
                    return AnimState.STOP;
                rigidBody.transform.position += new Vector3(0, -1f * speed * Time.deltaTime, 0);
                isMoving = (nowPosition == GetPosition());
                direction = Direction.FORE;
                return AnimState.WALK;
            default:
                return AnimState.STOP;

        }

    }

    string GetAnimationName(AnimState animState, Direction direction)
    {
        switch (animState)
        {
            case AnimState.STOP:
                switch (direction)
                {
                    case Direction.FORE:
                        return animNameStopFore;
                    case Direction.LEFT:
                        return animNameStopLeft;
                    case Direction.RIGHT:
                        return animNameStopRight;
                    case Direction.BACK:
                        return animNameStopBack;
                }
                break;
            case AnimState.WALK:
                switch (direction)
                {
                    case Direction.FORE:
                        return animNameWalkFore;
                    case Direction.LEFT:
                        return animNameWalkLeft;
                    case Direction.RIGHT:
                        return animNameWalkRight;
                    case Direction.BACK:
                        return animNameWalkBack;
                }
                break;
        }

        return animNameStopFore;
    }
}
