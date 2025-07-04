﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    /// <summary>  
    /// プレイヤー  
    /// </summary>  
    [SerializeField] private Player player_ = null;

    /// <summary>  
    /// ワールド行列   
    /// </summary>  
    private Matrix4x4 worldMatrix_ = Matrix4x4.identity;

    /// <summary>  
    /// ターゲットとして設定する  
    /// </summary>  
    /// <param name="enable">true:設定する / false:解除する</param>  
    public void SetTarget(bool enable)
    {
        // マテリアルの色を変更する  
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.materials[0].color = enable ? Color.red : Color.white;
    }

	/// <summary>
	/// 開始処理
	/// </summary>
	public void Start()
    {
		Matrix4x4 setpos = Matrix4x4.Translate(transform.position);
		Matrix4x4 setrot = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 0));

		worldMatrix_ = worldMatrix_ * (setpos * setrot);

		transform.position = worldMatrix_.GetColumn(3);
		transform.rotation = worldMatrix_.rotation;
		transform.localScale = worldMatrix_.lossyScale;
	}

    /// <summary>  
    /// 更新処理  
    /// </summary>  
    public void Update()
    {
		var normalZ = new Vector3(0, 0, 1);
		var myForward = worldMatrix_ * normalZ;

		var myViewCos = Mathf.Cos(20 * Mathf.Deg2Rad);

		var myToPlayer = (player_.transform.position - transform.position).normalized;

		var dot = Vector3.Dot(myForward, myToPlayer);

		if (myViewCos <= dot) //視野に入ったら実行
		{

			var target = (player_.transform.position - transform.position).normalized;

			var fowerd = transform.forward;

			var limitViewCos = Mathf.Cos(10 * Mathf.Deg2Rad);

			var dir_dot = Vector3.Dot(fowerd, target);

			Matrix4x4 rot = Matrix4x4.identity;

			if (0.99f > dir_dot) //約１より小さかったら
			{
				float radian = 1;

				if (limitViewCos <= dir_dot) // 10°以内だと1に近い
				{
					radian = Mathf.Acos(dir_dot);

					Debug.Log("<=10");
				}
				else
				{
					radian = Mathf.Acos(limitViewCos);

					Debug.Log(">10");
				}

				var cross = Vector3.Cross(fowerd, target);

				radian *= (cross.y / Mathf.Abs(cross.y));

				Matrix4x4 rotM = Matrix4x4.Rotate(Quaternion.Euler(0, Mathf.Rad2Deg * radian, 0));

				rot = rotM;
			}


			Vector3 vec=new Vector3();
			vec.z = 0.2f;
			var pos = Matrix4x4.Translate(vec);


			worldMatrix_ = worldMatrix_ * (pos * rot);
		}

		transform.position = worldMatrix_.GetColumn(3);
		transform.rotation = worldMatrix_.rotation;
		transform.localScale = worldMatrix_.lossyScale;
	}
}
