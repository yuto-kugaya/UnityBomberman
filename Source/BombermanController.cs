using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombermanController : CharacterController
{
    Rigidbody2D rigidBody = new Rigidbody2D();
    #region inspector
    [Header("爆弾数")]
    [SerializeField] int numBomb = 1;
    [Header("火力")]
    [SerializeField] int numFire = 2;
    [Header("スピード")]
    [SerializeField] int numSpeed = 1;
    [Header("デフォルト爆弾プレハブ")]
    [SerializeField] BombBehaviour prefabBombBehaviour;
    [Header("爆弾位置補正")]
    [SerializeField] Vector2 bombPosOffset = new Vector2(0, -0.372f);
    [Header("足音")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip audioClipFoot;
    #endregion
    bool inputMoveFlag = true;
    bool inputBombFlag = true;
    Direction _direction = Direction.FORE;
    float defaultSpeed = 5f;
    int _nowSetBomb = 0;
    float speed;

    //このボンバーマンが設置したボムリスト
    List<BombBehaviour> bombs;

    //フィールド情報
    //FieldController fieldController;

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        bombs = new List<BombBehaviour>();
        speed = defaultSpeed + numSpeed;
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();

        //死亡したなら
        if(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == animNameDead)
        {
            //アニメが終わったなら
            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                Destroy(gameObject);
            }
            //以降の処理を飛ばす
            return;
        }

        //移動判定とアニメーション名取得
        AnimState animState = MoveController(ref _direction);
        //これは芸術点高い一行（自画自賛）
        //string nowAnimName = GetAnimationName(MoveController(ref _direction), _direction);
        string nowAnimName = GetAnimationName(animState, _direction);
        //爆弾設置判定
        BombController();

        //アニメーション変更
        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != nowAnimName)
        {
            animator.Play(nowAnimName);
        }
        //足音
        PlayFootStepAudio(animState == AnimState.WALK);
    }

    //爆弾ボタン入力
    void BombController()
    {
        if (inputBombFlag)
        {
            if(Input.GetButtonDown("Submit"))
            {
                //爆弾生成上限を超えている、あるいはもうすでに爆弾がある場合はスキップ
                if (bombs.Count >= numBomb || fieldController.IsBomb(GetPosition())) return;
                //爆弾生成
                BombBehaviour bomb = Instantiate(prefabBombBehaviour, gameObject.transform.position + (Vector3) bombPosOffset, Quaternion.identity, transform.parent);
                bomb.setFire(numFire, this);
                bombs.Add(bomb);
                fieldController.SetBomb(GetPosition(), bomb);
                inputBombFlag = false;
            }
        }
        else
            inputBombFlag = true;
    }

    //爆破後に爆弾数を戻す処理
    public void DeleteBomb(BombBehaviour bomb, Vector3Int pos)
    {
        fieldController.DeleteBomb(pos);
        bombs.Remove(bomb);
    }
    //十字キー入力
    AnimState MoveController(ref Direction direction)
    {
        if (inputMoveFlag)
        {
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                rigidBody.transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
                inputMoveFlag = false;
                direction = Direction.RIGHT;
                return AnimState.WALK;
            }
            else if (Input.GetAxisRaw("Horizontal") < 0)
            {
                rigidBody.transform.position += new Vector3(-1f * speed * Time.deltaTime, 0, 0);
                inputMoveFlag = false;
                direction = Direction.LEFT;
                return AnimState.WALK;
            }
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                rigidBody.transform.position += new Vector3(0, speed * Time.deltaTime, 0);
                inputMoveFlag = false;
                direction = Direction.BACK;
                return AnimState.WALK;
            }
            else if (Input.GetAxisRaw("Vertical") < 0)
            {
                rigidBody.transform.position += new Vector3(0, -1f * speed * Time.deltaTime, 0);
                inputMoveFlag = false;
                direction = Direction.FORE;
                return AnimState.WALK;
            }

            //移動入力なし
            return AnimState.STOP;

        }
        else
        {
            inputMoveFlag = true;
            return AnimState.WALK;
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

    //足音
    private void PlayFootStepAudio(bool isPlay)
    {
        if(!isPlay)
        {
            audioSource.Stop();
            return;
        }

        if(!audioSource.isPlaying)
        {
            audioSource.clip = audioClipFoot;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
