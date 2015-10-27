using UnityEngine;
using System;

namespace N {

  public class Meta2d {

    public Meta meta;

    public Meta2d(Meta parent) {
      this.meta = parent;
    }

    /// Get the linear velocity of the object
    public Vector2 velocity {
      get {
        return (this.meta.cmp<Rigidbody2D>()).velocity;
      }
    }

    /// Shuffle slightly towards the target
    public bool move_towards(float x, float y, float z, float distance) {
      return this.move_towards(new Vector3(x, y, z), distance);
    }

    /// Shuffle slightly towards the target
    public bool move_towards(Meta2d target, float distance) {
      return this.move_towards(target.meta.raw.transform.position, distance);
    }

    /// Shuffle slightly towards the target
    public bool move_towards(GameObject target, float distance) {
      return this.move_towards(target.transform.position, distance);
    }

    /// Shuffle slightly towards the target
    public bool move_towards(Vector3 target, float distance) {
      var rdist = this.distance(target);
      if (rdist <= distance) {
        this.move(target);
        return true;
      }
      var delta = (target - this.meta.raw.transform.position).normalized * distance;
      this.meta.raw.transform.position += delta;
      return this.distance(target) < 0.01;
    }

    /// Shuffle slightly in the given direction
    public bool move_relative(Vector3 axis, float distance) {
      axis.Normalize();
      var offset = this.meta.raw.transform.position + axis * distance;
      return this.move_towards(offset, distance);
    }

    /// Shuffle slightly towards the target
    public Meta2d force_torwards(float x, float y, float z, float magn) {
      this.force_towards(new Vector3(x, y, z), magn);
      return this;
    }

    /// Shuffle slightly towards the target
    public Meta2d force_towards(Meta2d target, float magn) {
      this.force_towards(target.meta.raw.transform.position, magn);
      return this;
    }

    /// Shuffle slightly towards the target
    public Meta2d force_towards(GameObject target, float magn) {
      this.force_towards(target.transform.position, magn);
      return this;
    }

    /// Shuffle slightly towards the target
    public Meta2d force_towards(Vector3 target, float magn) {
      var d = magn * (target - this.meta.raw.transform.position).normalized;
      var d2 = new Vector2(d.x, d.y);
      this.apply_force(d2);
      return this;
    }

    /// Calculate the distance between points
    public float distance(Meta2d target) {
      return this.distance(target.meta.raw);
    }

    /// Calculate the distance between points
    public float distance(GameObject target) {
      return Vector3.Distance(this.meta.raw.transform.position, target.transform.position);
    }

    /// Calculate the distance between points
    public float distance(Vector3 target) {
      return Vector3.Distance(this.meta.raw.transform.position, target);
    }

    /// Calculate the distance between points
    public float distance(Vector2 target) {
      return Vector3.Distance(this.meta.raw.transform.position, new Vector3(target[0], target[1], this.meta.raw.transform.position[2]));
    }

    /// Calculate the force between two rigidbody objects using graivty
    public float gravity(Meta2d target, float gravity) {
      var b1 = this.meta.cmp<Rigidbody2D>(true);
      var b2 = target.meta.cmp<Rigidbody2D>(true);
      var m1 = b1 == null ? 0f : b1.mass;
      var m2 = b2 == null ? 0f : b2.mass;
      var d = this.distance(target) / 2f;
      return gravity * m1 * m2 / d;
    }

    /// Apply a force to this target
    public Meta2d apply_force(float x, float y) {
      this.apply_force(new Vector2(x, y));
      return this;
    }

    /// Apply a force to this target
    public Meta2d apply_force(Vector2 vector) {
      var rb = this.meta.cmp<Rigidbody2D>(true);
      if (rb != null) {
        rb.AddForce(vector);
      }
      return this;
    }

    /// Sync the x/y position in 2d of this to the target
    public Meta2d move(GameObject target) {
      if (this.meta.raw.transform.position.x != target.transform.position.x) {
        var pos = this.meta.raw.transform.position;
        pos.x = target.transform.position.x;
        this.meta.raw.transform.position = pos;
      }
      if (this.meta.raw.transform.position.y != target.transform.position.y) {
        var pos = this.meta.raw.transform.position;
        pos.y = target.transform.position.y;
        this.meta.raw.transform.position = pos;
      }
      return this;
    }

    /// Sync the x/y position in 2d of this to the target
    public Meta2d move(Vector2 target) {
      if (this.meta.raw.transform.position.x != target.x) {
        var pos = this.meta.raw.transform.position;
        pos.x = target.x;
        this.meta.raw.transform.position = pos;
      }
      if (this.meta.raw.transform.position.y != target.y) {
        var pos = this.meta.raw.transform.position;
        pos.y = target.y;
        this.meta.raw.transform.position = pos;
      }
      return this;
    }

    /// Stop moving this object
    public Meta2d stop() {
      var rb = this.meta.cmp<Rigidbody2D>();
      rb.velocity = new Vector3(0,0,0);
      return this;
    }

    /// Calculate mangitude of a vector in the direction of this target
    public Vector2 directed_force(float x, float y) {
      return directed_force(new Vector2(x, y));
    }

    /// Calculate mangitude of a vector in the direction of this target
    public Vector2 directed_force(Vector2 value) {
      var rotation = this.meta.raw.transform.localEulerAngles.z * Math.PI / 180.0;
      var cs = Math.Cos(rotation);
      var sn = Math.Sin(rotation);
      var nx = (float) (value.x * cs - value.y * sn);
      var ny = (float) (value.x * sn + value.y * cs);
      return new Vector2(nx, ny);
    }
  }
}
