using UnityEngine;
using System.Collections.Generic;

public class TaskRunner : MonoBehaviour {
	private List<BaseTask> tasks = new List<BaseTask>();
	private int lastActive = 0;

	public bool inOrder = true;
	public bool deleteOnFinish = true;

	protected void AddTask(BaseTask task) {
		tasks.Add(task);
		task.SetRunner(this);
	}

	protected void RemoveTask(BaseTask task) {
		tasks.Remove(task);
		task.SetRunner(null);
	}

	protected virtual void Start() {
		SetToStart();
	}

	private void OnEnable() {
		SetToStart();
	}

	private void SetToStart() {
		foreach(BaseTask task in tasks) {
			task.Enabled = false;
		}

		tasks[0].Enabled = true;
	}

	/// <summary>
	/// Enable the previous task. If a base task it will enable the task before it, if no task is provided then the last active
	/// task will be re-enabled. If no task is provided and no element is enabled then the first task will be re-enabled.
	/// </summary>
	/// <param name="task">The task who previous to it should be activated (optional)</param>
	/// <param name="finishTask">If true the provided task (if not null) will be automatically disabled</param>
	public void EnablePrevious(BaseTask task = null, bool finishTask = true) {
		int index = 0;

		if(task == null) {
			index = lastActive - 1;
		}
		else {
			index = tasks.IndexOf(task) - 1;

			if(finishTask) { task.Finished(); }
		}

		if(index < 0) { index = 0; }

		lastActive = index;
		tasks[index].Enabled = true;
	}

	private void CollisionEnter(Collision collisionInfo) {
		foreach(BaseTask task in tasks) {
			if(task.Enabled) {
				task.CollisionEnter(collisionInfo);
			}
		}
	}

	private void CollisionExit(Collision collisionInfo) {
		foreach(BaseTask task in tasks) {
			if(task.Enabled) {
				task.CollisionExit(collisionInfo);
			}
		}
	}

	private void CollisionStay(Collision collisionInfo) {
		foreach(BaseTask task in tasks) {
			if(task.Enabled) {
				task.CollisionStay(collisionInfo);
			}
		}
	}

	private void ControllerColliderHit(ControllerColliderHit collider) {
		foreach(BaseTask task in tasks) {
			if(task.Enabled) {
				task.ControllerColliderHit(collider);
			}
		}
	}

	private void TriggerEnter(Collider other) {
		foreach(BaseTask task in tasks) {
			if(task.Enabled) {
				task.TriggerEnter(other);
			}
		}
	}

	private void TriggerExit(Collider other) {
		foreach(BaseTask task in tasks) {
			if(task.Enabled) {
				task.TriggerExit(other);
			}
		}
	}

	private void TriggerStay(Collider other) {
		foreach(BaseTask task in tasks) {
			if(task.Enabled) {
				task.TriggerStay(other);
			}
		}
	}

	private void Update() {
		int newLastActive = -1;

		for(int i=0; i<tasks.Count; i++) {
			BaseTask task = tasks[i];

			if(task.Enabled) {
				newLastActive = i;
				task.Update();
			}

			task.SetRunner(this);
		}

		if(inOrder && newLastActive == -1) {
			lastActive++;

			if(lastActive >= tasks.Count) {
				Finished();
			}
			else {
				tasks[lastActive].Enabled = true;
			}
		}
	}

	private void FixedUpdate() { }
	private void LateUpdate() { }

	protected virtual void Finished() {
		enabled = false;

		if(deleteOnFinish) {
			Destroy(this);
		}
	}
}