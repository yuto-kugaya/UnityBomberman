using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class BurnableTile : TileBase
{
    public Sprite m_DefaultSprite;
    public DestroyInAnimEnd animationObject;
	public Tile.ColliderType m_DefaultColliderType;

	public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = m_DefaultSprite;
		tileData.colliderType = this.m_DefaultColliderType;
		base.GetTileData(position, tilemap, ref tileData);
    }

	public override void RefreshTile(Vector3Int position, ITilemap tilemap)
	{
		base.RefreshTile(position, tilemap);
	}

	public override bool StartUp(Vector3Int location, ITilemap tilemap, GameObject go)
	{
		return base.StartUp(location, tilemap, go);
	}
}
