using UnityEngine;

public class BaseTask {
	private TaskRunner runner;
	private bool enabled = false;

	public bool Enabled {
		get { return enabled; }
		set {
			bool old = enabled;

			enabled = value;
			if(!old && value) {
				Init();
			}
		}
	}

	public void SetRunner(TaskRunner runner) {
		this.runner = runner;
	}

	protected TaskRunner Runner { get { return runner; } }

	public void Finished() { Enabled = false; }

	public virtual void CollisionEnter(Collision collisionInfo) { }
	public virtual void CollisionExit(Collision collisionInfo) { }
	public virtual void CollisionStay(Collision collisionInfo) { }
	public virtual void ControllerColliderHit(ControllerColliderHit collider) { }
	public virtual void TriggerEnter(Collider other) { }
	public virtual void TriggerExit(Collider other) { }
	public virtual void TriggerStay(Collider other) { }
	public virtual void Init() { }
	public virtual void FixedUpdate() { }
	public virtual void LateUpdate() { }
	public virtual void Update() { }
}