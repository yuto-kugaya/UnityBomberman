using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// GridとTilemapを包括して管理するクラス
// Gridにアタッチすること。
public class FieldController : MonoBehaviour
{
    [System.NonSerialized] public Grid grid;
    [System.NonSerialized] public GridInformation gridInfo;
    [System.NonSerialized] public Tilemap tilemapFloor;
    [System.NonSerialized] public Tilemap tilemapHardBlock;
    [System.NonSerialized] public Tilemap tilemapSoftBlock;

    string keystr_Bomb = "Bomb";
    const int NONE = 0;
    const int EXIST = 1;

    Dictionary<Vector3Int, BombBehaviour> bombPosition;

    // Start is called before the first frame update
    void Start()
    {
        grid = gameObject.GetComponent<Grid>();
        gridInfo = gameObject.GetComponent<GridInformation>();
        tilemapFloor = transform.Find("Floor").GetComponent<Tilemap>();
        tilemapHardBlock = transform.Find("HardBlock").GetComponent<Tilemap>();
        tilemapSoftBlock = transform.Find("SoftBlock").GetComponent<Tilemap>();
        bombPosition = new Dictionary<Vector3Int, BombBehaviour>();
    }
    
    public void SetBomb(Vector3Int position, BombBehaviour bomb)
    {
        //すでにある場合は設置しない
        if (IsBomb(position)) return;

        gridInfo.SetPositionProperty(position, keystr_Bomb, EXIST);
        bombPosition.Add(position, bomb);
    }

    public bool IsBomb(Vector3Int position)
    {
        return gridInfo.GetPositionProperty(position, keystr_Bomb, NONE) == EXIST;
    }

    public BombBehaviour GetBomb(Vector3Int position)
    {
        if (!IsBomb(position)) return null;

        return bombPosition[position];
    }

    public void DeleteBomb(Vector3Int position)
    {
        if (!IsBomb(position)) return;

        gridInfo.SetPositionProperty(position, keystr_Bomb, NONE);
        bombPosition.Remove(position);
    }

    public void ExplodeSoftBlock(Vector3Int position)
    {
        if (tilemapSoftBlock.GetTile(position) == null) return;
        //TODO燃焼アニメ
        if(tilemapSoftBlock.GetTile(position).GetType() == typeof(BurnableTile))
        {
            Instantiate<DestroyInAnimEnd>(
                (tilemapSoftBlock.GetTile(position) as BurnableTile).animationObject,
                grid.CellToLocalInterpolated(position + new Vector3(0.5f, 0.5f)),
                Quaternion.identity, transform);
        }

        tilemapSoftBlock.SetTile(position, null);

    }

    //ブロックがあるかどうかを返す
    public bool IsBlock(Vector3Int position)
    {
        return (tilemapSoftBlock.GetTile(position) != null) || (tilemapHardBlock.GetTile(position) != null); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
