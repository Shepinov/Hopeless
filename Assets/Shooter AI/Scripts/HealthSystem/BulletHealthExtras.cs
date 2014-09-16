//attach to bullets to customize their hit values

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletHealthExtras : MonoBehaviour {

public float extraHitValue; //the amount of damage the bullet does,. extra to the normal hit area
public float backupForce = 1000f; //the amount of force if the gun didnt give force


	public float m_SparkFactor = 0.5f;		// chance of bullet impact generating a spark

	// these gameobjects will all be spawned at the point and moment
	// of impact. technically they could be anything, but their
	// intended uses are as follows:
	public GameObject m_ImpactPrefab = null;	// a flash or burst illustrating the shock of impact
	public GameObject m_DustPrefab = null;		// evaporating dust / moisture from the hit material
	public GameObject m_SparkPrefab = null;		// a quick spark, as if hitting stone or metal
	public GameObject m_DebrisPrefab = null;	// pieces of material thrust out of the bullet hole and / or falling to the ground

	// sound
	protected AudioSource m_Audio = null;
	public List<AudioClip> m_ImpactSounds = new List<AudioClip>();	// list of impact sounds to be randomly played
	public Vector2 SoundImpactPitch = new Vector2(1.0f, 1.5f);	// random pitch range for impact sounds

	

//this is for the hit variables
public Vector3 hitPoint;
public Vector3 hitNormal;
public Transform hitM;

	
	//anti-friendly fire
	public bool allowFriendlyFire = false; //whether friendly fire is on or not
	public string tagOfFriendly; //the tag of a friendly
	
	
//this is so that the bullet doesnt hit own player
public float minDistance = 1.5f; //the distance after which the bullet becomes active
private Vector3 startPlace; //the start vector3
private bool startDeactivated = false; //whether we have activated the bullet yet thrugh deactivation


private int amountOfHits = 0; //how many hits (against ricochet errors, we only allow the visual effects to play if the amount of hits is 1)

void Start()
{


startPlace = transform.position;

StartCoroutine(CheckBackupForce());

}

//the is to check whether we should use ectra force or not
IEnumerator CheckBackupForce()
{
yield return new WaitForFixedUpdate();
yield return new WaitForFixedUpdate();
//this is to fire ouselves even if we werent fired
if(rigidbody.velocity.magnitude < 10f)
{
//Debug.Log("Backup force used");
rigidbody.AddForce(transform.forward * backupForce);
}

}


//this is to ignore own player
void Update()
{
if(Vector3.Distance(startPlace, transform.position) < minDistance && collider != null)
{
GetComponent<Collider>().enabled = false;
startDeactivated = true;

}

if(Vector3.Distance(startPlace, transform.position) > minDistance && startDeactivated == true && collider != null)
{
GetComponent<Collider>().enabled = true;
startDeactivated = false;
}
		

}




//this is for extra ragdoll effects so that they react to the shot
public IEnumerator BulletHit(Transform hit2)
{		
//Debug.Log(hit2);
yield return new WaitForFixedUpdate();
yield return new WaitForFixedUpdate();
yield return new WaitForFixedUpdate();
StopReactingToEnvironment();
ImpactVisuals(gameObject.transform, hit2, hitPoint, hitNormal);
DestroyBullet();

}

void OnTriggerEnter(Collider collision)
{


		//check whether this is friendly fire
		if(allowFriendlyFire == false)
		{
			int layersToCheck = 10;
			Transform testObject = collision.transform;
			
			for(int currentLayer = 0; currentLayer < layersToCheck; currentLayer ++)
			{
				if(testObject.tag == tagOfFriendly)
				{
					return;
				}
			else 
				{
					if(testObject.parent != null)
					{
						testObject = testObject.parent;
					}
				}
				
			}
		}


if(collision.gameObject.GetComponent<Detecthit>() == null)
{

collision.gameObject.SendMessageUpwards("Damage", extraHitValue, SendMessageOptions.DontRequireReceiver);
collision.gameObject.SendMessageUpwards("ApplyDamage", extraHitValue, SendMessageOptions.DontRequireReceiver);
			
}

hitM = collision.transform;
hitPoint = transform.position;
hitNormal = transform.eulerAngles;

StartCoroutine("BulletHit", hitM);

}

void OnCollisionEnter(Collision collision)
{

if(collision.gameObject.GetComponent<Detecthit>() == null)
{

collision.gameObject.SendMessageUpwards("Damage", extraHitValue, SendMessageOptions.DontRequireReceiver);
collision.gameObject.SendMessageUpwards("ApplyDamage", extraHitValue, SendMessageOptions.DontRequireReceiver);
			
}

hitM = collision.transform;
hitPoint = collision.contacts[0].point;
hitNormal = collision.contacts[0].normal;

StartCoroutine("BulletHit", hitM);	
}


//this is to make the bullet stob reacting to the game world
void StopReactingToEnvironment()
{
		if(GetComponent<Collider>() != null)
		{
			GetComponent<Collider>().enabled = false;
		}
			
		if(GetComponent<MeshRenderer>() != null)
		{
			GetComponent<MeshRenderer>().enabled = false;
		}
}


//this does all the impact visuals
void ImpactVisuals(Transform t, Transform hit, Vector3 hitPoint, Vector3 hitNormal)
{
m_Audio = audio;

Vector3 scale = t.localScale;

amountOfHits += 1;

if(amountOfHits > 1)
{
return;
}

if(hit == null)
{
return;
}


t.parent = hit.transform;
t.localPosition = hit.transform.InverseTransformPoint(hitPoint);

t.rotation = Quaternion.LookRotation(hitNormal);					// face away from hit surface
		
		
		if (hit.transform.lossyScale == Vector3.one)								// if hit object has normal scale
				t.Rotate(Vector3.forward, Random.Range(0, 360), Space.Self);	// spin randomly
			else
			{
				// rotated child objects will get skewed if the parent object has been
				// unevenly scaled in the editor, so on scaled objects we don't support
				// spin, and we need to unparent, rescale and reparent the decal.
				t.parent = null;
				t.localScale = scale;
				t.parent = hit.transform;
			}

			// spawn impact effect
			if (m_ImpactPrefab != null)
				Object.Instantiate(m_ImpactPrefab, t.position, t.rotation);

			// spawn dust effect
			if (m_DustPrefab != null)
				Object.Instantiate(m_DustPrefab, t.position, t.rotation);

			// spawn spark effect
			if (m_SparkPrefab != null)
			{
				if (Random.value < m_SparkFactor)
					Object.Instantiate(m_SparkPrefab, t.position, t.rotation);
			}

			// spawn debris particle fx
			if (m_DebrisPrefab != null)
				Object.Instantiate(m_DebrisPrefab, t.position, t.rotation);

			// play impact sound
			if (m_ImpactSounds.Count > 0)
			{
				m_Audio.pitch = Random.Range(SoundImpactPitch.x, SoundImpactPitch.y) * Time.timeScale;
				m_Audio.PlayOneShot(m_ImpactSounds[(int)Random.Range(0, (m_ImpactSounds.Count))]);
			}
}


/// <summary>
/// Correctly destroys the bullet.
/// </summary>
void DestroyBullet()
	{
		//destroy the non-effectcomponents
		Destroy( GetComponent<Collider>() );
		Destroy( rigidbody);
		Destroy( GetComponent<DontGoThroughThings>() );
		Destroy( GetComponent<MeshFilter>() );
        Destroy( GetComponent<BulletAutoDestroy>() );
		
		//continue playing sound if its still on
		if(audio.isPlaying == false)
		{
			Destroy( audio);
            Destroy(gameObject);
		}
		else
		{
			Destroy( audio, audio.clip.length - audio.time);
            Destroy(gameObject, audio.clip.length - audio.time);
            Destroy(this, audio.clip.length - audio.time);
		}
        
        
        
				
	}





}
