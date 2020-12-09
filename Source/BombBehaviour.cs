using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Layer
{
    BOMBERMAN = 8,
    BOMBPASS = 9,
    BOMB = 10,
    ENEMY = 12
}
public class BombBehaviour : MonoBehaviour
{
    BoxCollider2D boxCollider;
    [SerializeField] float _bombTime = 2.0f;
    [SerializeField] int _fire = 2;
    [SerializeField] ExplosionController explosion;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip audioClipBomSet;
    [SerializeField] AudioClip audioClipBomb;

    float nowTime = 0;
    FieldController fieldController;
    BombermanController _bomberman;
    bool[] isExploded = new bool[4];

    private void Awake()
    {
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        fieldController = transform.parent.GetComponent<FieldController>();
        audioSource.PlayOneShot(audioClipBomSet);
    }

    private void Update()
    {
        if(nowTime + Time.deltaTime >= _bombTime)
        {
            Explosion();
        }
        else
            nowTime += Time.deltaTime;
    }

    //爆風生成
    public void Explosion()
    {
        Vector3Int bombPos = new Vector3Int();
        try
        {
            bombPos = GetBombPos();
        }
        catch(MissingReferenceException)
        {
            return;
        }
        //爆風
        AudioSource.PlayClipAtPoint(audioClipBomb, Camera.main.transform.position, 1.0f);
        //ボム情報の消去（これを最初にやらないと誘爆が無限ループする）
        _bomberman.DeleteBomb(this, bombPos);

        ExplodeCell(bombPos);

        //広がり
        for (int i = 0; i < isExploded.Length; i++) isExploded[i] = true;

        for (int i = 1; i <= _fire; i++) 
        {
            //セルを燃やす
            foreach(BombermanController.Direction direction 
                in System.Enum.GetValues(typeof(BombermanController.Direction)))
            {
                if (isExploded[(int)direction])
                    isExploded[(int)direction] = ExplodeCell(bombPos + i * UnitVector(direction));
            }
        }

        //消滅
        Destroy(gameObject);


    }

    //セルを燃やす
    private bool ExplodeCell(Vector3Int pos)
    {
        //ハードブロックがある場合は止める
        if (fieldController.tilemapHardBlock.GetTile(pos) != null) return false;

        //ソフトブロック
        if (fieldController.tilemapSoftBlock.GetTile(pos) != null)
        {
            fieldController.ExplodeSoftBlock(pos);
            return false;
        }

        //爆風を出現
        Instantiate(explosion, fieldController.grid.CellToLocalInterpolated(pos + new Vector3(0.5f, 0.5f)), Quaternion.identity, transform.parent);

        //燃え広がる
        return true;
    }

    //ボムの置かれているGrid座標を計算する
    public Vector3Int GetBombPos()
    {
        return fieldController.grid.WorldToCell(gameObject.transform.position);
    }

    //ボンバーマンと重なっている（初期状態）ではすり抜け設定
    //ボンバーマンが離れたらすり抜けられなくする。（ボム通過がない場合）
    private void OnTriggerExit2D(Collider2D collision)
    {
        boxCollider.isTrigger = false;
    }

    //ドクロ効果などで爆発時間が変化する場合
    public void setBombTime(float time)
    {
        _bombTime = time;
    }

    public void setFire(int fire, BombermanController bomberman)
    {
        _fire = fire;
        _bomberman = bomberman;
        //座標修正
        gameObject.transform.position = fieldController.grid.CellToLocalInterpolated(GetBombPos() + new Vector3(0.5f, 0.5f));
        nowTime = 0f;
    }

    Vector3Int UnitVector(BombermanController.Direction direction)
    {
        switch(direction)
        {
            case BombermanController.Direction.FORE:
                return new Vector3Int(0, -1, 0);
            case BombermanController.Direction.LEFT:
                return new Vector3Int(-1, 0, 0);
            case BombermanController.Direction.RIGHT:
                return new Vector3Int(1, 0, 0);
            case BombermanController.Direction.BACK:
                return new Vector3Int(0, 1, 0);
        }
        return new Vector3Int(0, 0, 0);
    }

}
