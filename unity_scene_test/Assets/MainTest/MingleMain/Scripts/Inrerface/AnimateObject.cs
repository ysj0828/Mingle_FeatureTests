using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnimateObject : MonoBehaviour
{
  public Vector3 OrignalPosition { get; set; }
  public Quaternion OrignalRotation { get; set; }
  public Vector3 OrignalScale { get; set; }

  private Vector3 _target_position;
  private Quaternion _target_rotation;
  private Vector3 _target_localScale;
  private float _animation_start_time = 0f;
  private float _animation_duration = 0f;
  protected bool _animation_enabled = false;

  private void Start()
  {
    OrignalPosition = transform.localPosition;
    OrignalRotation = transform.localRotation;
    OrignalScale = transform.localScale;
  }
  private void FixedUpdate()
  {
    AnimatePosition();
  }

  void AnimatePosition()
  {
    if (!_animation_enabled) return;
    if (_animation_start_time + _animation_duration > Time.realtimeSinceStartup)
    {
      float val = (Time.realtimeSinceStartup - _animation_start_time) / _animation_duration;
      val = BeyondLerp(val);

      transform.localPosition = Vector3.LerpUnclamped(transform.localPosition, _target_position, val);
      if (_target_rotation != Quaternion.identity) transform.localRotation = Quaternion.LerpUnclamped(transform.localRotation, _target_rotation, val);
      transform.localScale = Vector3.LerpUnclamped(transform.localScale, _target_localScale, val);
    }
    else
    {
      _animation_enabled = false;
      this.transform.localPosition = _target_position;
      if (_target_rotation != Quaternion.identity) this.transform.localRotation = _target_rotation;
      this.transform.localScale = _target_localScale;
    }
  }

  float BeyondLerp(float val)
  {
    val = val * val * val * val;
    val = Mathf.Sin(val * Mathf.PI * 0.5f);
    return val;
  }

  //   public void UpdateTransformation(Transform target_transform, float animation_duration = 3f)
  public void UpdateTransformation(Vector3 target_position, Quaternion target_rotation,
                                    Vector3 target_localScale, float animation_duration = 0.5f)
  {
    _animation_enabled = true;
    _animation_start_time = Time.realtimeSinceStartup;
    _animation_duration = animation_duration;
    _target_position = target_position;
    _target_rotation = target_rotation;
    _target_localScale = target_localScale;
  }

  public void UpdatePositionScale(Vector3 target_position, Vector3 target_localScale, float animation_duration = 0.5f)
  {
    _animation_enabled = true;
    _animation_start_time = Time.realtimeSinceStartup;
    _animation_duration = animation_duration;
    _target_position = target_position;
    _target_rotation = Quaternion.identity;
    _target_localScale = target_localScale;
  }
}
