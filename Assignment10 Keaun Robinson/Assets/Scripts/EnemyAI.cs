using UnityEngine;
using System.Collections;
using System.Collections.Generic;//needed or else list wonk work, which is stupid

public class EnemyAI : MonoBehaviour {
	#region variables
	private EnemyStates state = EnemyStates.Patrolling;
	static private List<GameObject> patrolPoints = null;

	public float walkingSpeed = 3.0f;//base walking speed
	public float chasingSpeed = 15.0f;//base chasing speed
	public float attackingSpeed = 1.5f;//base attacking speed
	public float attackingDistance = 1.0f;//attack distance 

	private GameObject patrollingInterestPoint;
	private GameObject playerOfInterest;
	#endregion

	#region Start method-making the partol points and adding them to a list 
	void Start () {
		if(patrolPoints==null) {
			patrolPoints = new List<GameObject>();
			foreach(GameObject go in GameObject.FindGameObjectsWithTag("PatrolPoints")) {//foreach is eaiser than doing a for loop and checking if list.Contains is true
				Debug.Log("Patrol Point: " + go.transform.position);
				patrolPoints.Add(go);
			}
		}
		SwitchToPatrolling();//default state
	}
	#endregion

	#region AI implementation
	void Update () {
		switch(state) {
		case EnemyStates.Attacking:
			OnAttackingUpdate();
			break;
		case EnemyStates.Chasing:
			OnChasingUpdate();
			break;
		case EnemyStates.Patrolling:
			OnPatrollingUpdate();//always called first as its default
			break;
		}
	}
	#endregion

	#region patrolling default setting
	void SwitchToPatrolling() {//called in start so it kinda starts the "loop" seen in the update
		state = EnemyStates.Patrolling;//default state
		GetComponent<Renderer>().material.color = new Color(0.0f, 1.0f, 0.0f);//get renderer component, change color
		SelectRandomPatrolPoint();//choose patrol point from random list cell of 0 1 2 or 3
		playerOfInterest = null;
	}

	void OnPatrollingUpdate() {
		float step = walkingSpeed * Time.deltaTime;//base walking speed implemented for fluid motion
		print ("Time.deltaTime: " + Time.deltaTime);//debug
		transform.position = Vector3.MoveTowards(transform.position, patrollingInterestPoint.transform.position, step);
		//moves this game object to position of a partolling point by (base walking speed implemented for fluid motion)
		float distance = Vector3.Distance(transform.position, patrollingInterestPoint.transform.position);//delta of distance between first parameter and second
		if(distance==0) {//if distance is zero...
			SelectRandomPatrolPoint();//choose another partoll point
		}
	}

	void SelectRandomPatrolPoint() {
		int choice = Random.Range(0,patrolPoints.Count);//list.count = array.Length
		patrollingInterestPoint = patrolPoints[choice];//random list cell of 0 1 2 or 3, not sure if there is a three thought because of cell zero
		Debug.Log("Enemy going to patrol to point " + patrollingInterestPoint.name);//debug
	}
	#endregion

	#region chasing setting
	void OnTriggerEnter(Collider collider) {//have a region around the enemy AI so that, when entered...
		SwitchToChasing(collider.gameObject);//chasing can be intitalized
	}

	void OnTriggerExit(Collider collider) {//when aformentioned region is exited by the collider...
		SwitchToPatrolling();//switch back to the default state
	}

	void SwitchToChasing(GameObject target) {//called when player enters the aformentioned region
		state = EnemyStates.Chasing;//chasing state
		GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 0.0f);//get renderer component, change color
		playerOfInterest = target;//makes the target the playerOfInterest, since the player would be the one entering 
								  //the aforementioned region
	}

	void OnChasingUpdate() {
		float step = chasingSpeed * Time.deltaTime;//base chasing speed implemented for fluid motion
		transform.position = Vector3.MoveTowards(transform.position, playerOfInterest.transform.position, step);
		//moves this game object to position of playerOfInterest by (base chasing speed implemented for fluid motion)
		float distance = Vector3.Distance(transform.position, playerOfInterest.transform.position);//delta of distance between first parameter and second
		if(distance<=attackingDistance) {//if distance is within the attacking range...
			SwitchToAttacking(playerOfInterest);//switch to attacking
		}
	}
	#endregion

	#region attacking setting
	void SwitchToAttacking(GameObject target) {//called when the distance from the player is less than the attacking range in the OnChasing method
		state = EnemyStates.Attacking;//switch state to attacking so OnAttacking method can be initialized
		GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f);//get renderer component, change color
	}

	void OnAttackingUpdate() {
		float step = attackingSpeed * Time.deltaTime;//base attacking speed implemented for fluid motion
		transform.position = Vector3.MoveTowards(transform.position, playerOfInterest.transform.position, step);
		//moves this game object to position of playerOfInterest by (base attacking speed implemented for fluid motion)
		float distance = Vector3.Distance(transform.position, playerOfInterest.transform.position);//delta of distance between first parameter and second
		if(distance>attackingDistance) {//if outside attacking range...
			SwitchToChasing(playerOfInterest);//switch back to chasing
		}
	}
	#endregion
}
