using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]

public class Agent : MonoBehaviour 
{
	protected Animator			m_Animator;
	protected NavMeshAgent		m_Agent;
	protected Object 			m_ParticleClone;
	protected bool				m_StartedMatchTarget;

	public GameObject			m_Particle;

	// Use this for initialization
	void Start () 
	{
		m_Animator = GetComponent<Animator>();
		m_Agent = GetComponent<NavMeshAgent>();
		m_ParticleClone = null;

		m_Agent.updateRotation = false;
	}

	void OnDestroy ()
	{
		if (m_ParticleClone != null)
		{
			Destroy(m_ParticleClone);
		}
		m_ParticleClone = null;
	}

	void Update () 
	{

		m_Animator.SetBool("Walk",false);
		m_Animator.SetBool("HalfTurn",false);
		m_Animator.SetBool("TurnLeft",false);
		m_Animator.SetBool("TurnRight",false);
		
		if (AgentDone())
		{
			if (m_ParticleClone != null)
			{
				GameObject.Destroy(m_ParticleClone);
				m_ParticleClone = null;
			}
		}
		else
		{
			float angle = GetAngleToDesiredForward();

			float fullTurnAngle = 135.0f;
			float halfTurnAngle = 70.0f;

			if(Mathf.Abs(angle) > fullTurnAngle || angle < -fullTurnAngle)
			{
				m_Animator.SetBool("HalfTurn",true);
			}
			else if(angle > halfTurnAngle)
			{
				m_Animator.SetBool("TurnRight",true);
			}
			else if(angle < -halfTurnAngle)
			{
				m_Animator.SetBool("TurnLeft",true);
			}
			else
			{
				m_Animator.SetBool("Walk",true);
			}

		}

		MatchTurnTargets();

		if (Input.GetButtonDown ("Fire1"))
		{
			SetDestination ();
		}
	}

	void OnAnimatorMove()
	{	
		AnimatorStateInfo currentStateInfo = m_Animator.GetCurrentAnimatorStateInfo (0);
		AnimatorStateInfo nextStateInfo = m_Animator.GetNextAnimatorStateInfo (0);

		bool rotateToMatchDesiredVelocity;
		if(currentStateInfo.IsName("MoveWalk_F"))
		{
			rotateToMatchDesiredVelocity = true;
		}
		else if (nextStateInfo.IsName("MoveWalk_F"))
		{
			rotateToMatchDesiredVelocity = !currentStateInfo.IsName("Idle");
		}
		else
		{
			rotateToMatchDesiredVelocity = false;
		}

		if (rotateToMatchDesiredVelocity)
		{
			float desiredAngle = GetAngleToDesiredForward();
			float desiredAngleSpeed = desiredAngle / Time.deltaTime;
			float maxAngleSpeed = 180.0f;
			desiredAngleSpeed = Mathf.Clamp (desiredAngleSpeed, -maxAngleSpeed, maxAngleSpeed);
			desiredAngle = desiredAngleSpeed * Time.deltaTime;
			
			transform.rotation *= Quaternion.Euler (0, desiredAngle, 0);
		} 
		else 
		{
			transform.rotation = m_Animator.rootRotation;
		}

		m_Agent.velocity = m_Animator.deltaPosition / Time.deltaTime;

	}

	protected void MatchTurnTargets()
	{
		AnimatorStateInfo currentStateInfo = m_Animator.GetCurrentAnimatorStateInfo (0);
		// must be playing a turn animation
		if (currentStateInfo.IsTag("Turn")) 
		{
			// must still want to turn
			if (m_Animator.GetBool("HalfTurn") || m_Animator.GetBool("TurnRight") || m_Animator.GetBool("TurnLeft"))
			{
				// must not be in transition
				if (!m_Animator.IsInTransition(0) && currentStateInfo.normalizedTime < 0.5f)
				{
					m_Animator.MatchTarget(
						transform.position,
						Quaternion.LookRotation(m_Agent.desiredVelocity.normalized, Vector3.up),
						AvatarTarget.Root,
						new MatchTargetWeightMask(Vector3.zero, 0.9f),
						Mathf.Max(0.1f, currentStateInfo.normalizedTime),
						1f);
						m_StartedMatchTarget = true;
				}
			}
		}
		else
		{
			// reset this flag when not turning
			m_StartedMatchTarget = false;
		}
	}

	protected float GetAngleToDesiredForward()
	{
		Vector3 desiredVelocity = Quaternion.Inverse (transform.rotation) * m_Agent.desiredVelocity;
		return Mathf.Atan2 (desiredVelocity.x, desiredVelocity.z) * 180.0f / Mathf.PI;
	}

	protected bool AgentDone()
	{
		return !m_Agent.pathPending && m_Agent.remainingDistance <= m_Agent.stoppingDistance;
	}

	protected void SetDestination()
	{
		if(SelectableCamera.CurrentCamera != null)
		{
			var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit = new RaycastHit ();

			int inputLayerMask = 1 << LayerMask.NameToLayer("Clickable");

			if (Physics.Raycast (ray, out hit, Mathf.Infinity, inputLayerMask )) 
			{
				if (m_ParticleClone != null) 
				{
					GameObject.Destroy (m_ParticleClone);
					m_ParticleClone = null;
				}

				Debug.DrawLine(hit.point, hit.point + hit.normal, Color.cyan, 3.0f);
				
				// Create a particle if hit
				Quaternion q = new Quaternion ();
				q.SetLookRotation (hit.normal, Vector3.forward);
				if (m_Particle != null)
				{
					m_ParticleClone = Instantiate (m_Particle, hit.point, q);
				}
				
				m_Agent.destination = hit.point;
			}
		}
	}
}
