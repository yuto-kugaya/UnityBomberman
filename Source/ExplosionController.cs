using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
	FieldController fieldController;
	Vector3Int pos;
	Animator animator;
	bool initFlag = false;

	void Awake()
	{
		fieldController = transform.parent.GetComponent<FieldController>();
		pos = GetPosition();
		animator = gameObject.GetComponent<Animator>();
		initFlag = true;
	}

	private void Update()
	{
		if (!initFlag) return;
		//爆風アニメが終了した場合
		if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
		{
			Destroy(gameObject);
		}
		else
		{
			//爆風による接触判定
			CheckExplosion();

		}
	}

	//爆風による燃焼判定
	void CheckExplosion()
	{
		//爆弾がある場合は誘爆する
		if (fieldController.IsBomb(pos))
		{
			fieldController.GetBomb(pos).Explosion();

		}
	}

	public Vector3Int GetPosition()
	{
		return fieldController.grid.WorldToCell(gameObject.transform.position);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Character")
		{
			collision.gameObject.GetComponent<CharacterController>().Dead();
		}
	}

}
