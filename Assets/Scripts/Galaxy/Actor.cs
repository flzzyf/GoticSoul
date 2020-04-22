using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//单位动画
public enum UnitAnim {
	Null,
	Stand,
	Hit,
	Slash,
	RisingSlash,
	Roll,
	WolfSword,
	TakeDown,
}

public class Actor : MonoBehaviour{
	Animator animator;

	Weapon weapon;

	public void Init() {
		animator = GetComponent<Animator>();

		clips = new List<KeyValuePair<AnimationClip, AnimationClip>>();
		((AnimatorOverrideController)animator.runtimeAnimatorController).GetOverrides(clips);

		weapon = GetComponentInChildren<Weapon>();
	}

	public void EmptyMethod() {

	}

	public void Flip() {
		//var scale = transform.localScale;
		//scale.x *= -1;
		//transform.localScale = scale;

		float rotationY = transform.eulerAngles.y == 0 ? 180 : 0;
		transform.rotation = Quaternion.Euler(0, rotationY, 0);
	}

	#region 动画播放

	//重载前动画和重载后动画List
	List<KeyValuePair<AnimationClip, AnimationClip>> clips;

	//播放动画
	public void PlayAnim(UnitAnim anim, float preswingTime = 0, float swingTime = 0, float backswingTime = 0, 
		Action onSwingStart = null, Action onSwingEnd = null, Action onPlayComplete = null) {
		StartCoroutine(PlayAnimCor(anim, preswingTime, swingTime, backswingTime, onSwingStart, onSwingEnd, onPlayComplete));
	}
	IEnumerator PlayAnimCor(UnitAnim anim, float preswingTime, float swingTime, float backswingTime,
		Action onSwingStart, Action onSwingEnd, Action onPlayComplete) {
		AnimationClip clip = GetClip(anim, true);

		animator.Play(anim.ToString(), -1, 0f);

		//有事件帧分割动画
		if(clip.events.Length > 0) {
			//前摇
			if (preswingTime > 0) {
				float preswingSpeed = clip.events[0].time / preswingTime;

				animator.speed = preswingSpeed;

				yield return new WaitForSeconds(preswingTime);
			}
			else {
				yield return new WaitForSeconds(clip.events[0].time);
			}

			onSwingStart?.Invoke();

			//摇
			if (swingTime > 0) {
				float swingSpeed = (clip.events[1].time - clip.events[0].time) / swingTime;

				animator.speed = swingSpeed;

				yield return new WaitForSeconds(swingTime);
			}
			else {
				yield return new WaitForSeconds(clip.events[1].time - clip.events[0].time);
			}

			onSwingEnd?.Invoke();

			//后摇
			if (backswingTime > 0) {
				float backswingSpeed = (clip.length - clip.events[1].time) / backswingTime;

				animator.speed = backswingSpeed;

				yield return new WaitForSeconds(backswingTime);
			}
			else {
				yield return new WaitForSeconds(clip.length - clip.events[1].time);
			}
		}
		//没有事件帧，且设置了释放时间，则调整整个动画时间
		else if(swingTime > 0) {
			float swingSpeed = clip.length / swingTime;

			animator.speed = swingSpeed;

			yield return new WaitForSeconds(swingTime);
		}

		//完成
		onPlayComplete?.Invoke();

		animator.speed = 1;
	}

	//获取动画（根据是否是重载的动画）
	public AnimationClip GetClip(UnitAnim anim, bool overrided) {
		for (int i = 0; i < clips.Count; i++) {
			if (clips[i].Key.name == anim.ToString()) {
				//如果是返回重载动画，且有重载的动画
				if (overrided && clips[i].Value != null) {
					return clips[i].Value;
				}
				else {
					return clips[i].Key;
				}
			}
		}

		Debug.LogError("未能找到动画：" + anim.ToString());

		return null;
	}

	#endregion

	//public bool showWeaponSlashEffect;

	//private void Update() {

	//	weapon.ShowSlashEffect(showWeaponSlashEffect);
	//}

	public void SetParticle() {
		GetComponentInChildren<Weapon>().SetParticle(true);
	}
	public void SetParticle2() {
		GetComponentInChildren<Weapon>().SetParticle(false);
	}

	public void StartWeaponSlashEffect() {
		weapon.ShowSlashEffect(true);
	}
	public void EndWeaponSlashEffect() {
		weapon.ShowSlashEffect(false);
	}
}
