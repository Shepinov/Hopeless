	
	//this is the main ai state machine script, the main brain of the whole ai
	
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	
	public enum CurrentState {patrol, engage, chase, investigate, cover}
	public enum EngagementState { free, busy, cover}
	
	[ RequireComponent( typeof(ShooterAIAudioControl) ) ]
	
	public class AIStateManager : MonoBehaviour {
	
	public bool allowedToThink = true; //whether allowed to think and take action
	public bool onlyBasicFunctionality = false; //whether the brain should function on the most basic level
	public CurrentState currentState; //the current state: "patrol", "engage", "chase", "investigate", "cover"
	public EngagementState engagementStatus; //this variable is used during engagement: "free", "busy", "cover"
	public float tickBarrier; //this if for internal patrolling; the lower the barrier, the more often it checks 
	public GameObject patrolManager; //the patrol manager object
	public GameObject ears; //the ear sensor object
	public GameObject sight; //the sight sensor object
	public GameObject healthManager; //the health manager
	public GameObject animationManager; //the animation manager
	public float investigationTime; //the amount of seconds the ai will proceed to investigate before its gives up and goes back to patrolling
	public List<string> targetVisualCheck = new List<string>(); //add this script to object; once we see the player we will run this script with void "VisualCheck", if it returns true we engage the player
	public List<float> targetVisualCheckChance = new List<float>(); //the chance for the visual check
	public float timeUntilNextCheck; //after stepping down, how many seconds to wait until trying to engage again
	public float shockTime; //the amount of time the ai takes to realize and react
	public float timeBarrierEngageToInvestigate; //after how much time (frames) when we don't see the player should we go into investigation mode
	public float freezeTime; //the amount of seconds the ai freezes
	public float minCoverTime; //whats the minimum amount of seconds that the ai will try and spend in cover
	public float maxCoverTime; //whats the maximum amount of seconds that the ai will try and spend in cover
	public float chanceForFight; //the fight or flight chance (this variable specifies the probability that it will continue to fight instead of going for cover), in percent
	//different speed: WARNING: THESE ARE NOT THE ULTIMATE MAXIMUM SPEEDS, BUT JUST REFERENCES FOR EMOTIONS!!!!!!
	public float maxSpeedPatrol; //the maximum speed when patroling
	public float maxSpeedEngage; //once the fighting starts, whats the maximum speed
	public float maxSpeedChase; //the maximum speed when chasing
	public float maxSpeedCover; //the maximum speed when cover
	public float crouchSpeedFactor = 0.7f; //the factor with which to apply speed when crouching
	public float strafinSpeedFactor = 0.3f; //the factor with which to apply speed when strafing
	//distance to attack
	public float distanceToStopWalking; //at what distance to start the close range battle logic
	public bool allowCrouching = true; //whether we're even allowed to crouch
	public bool crouching = false; //whether we're crouching or not
	public float crouchingFactor = 1.5f; //the factor of which to bend down the objects related to crouching
	//debug
	public bool debug; //whether we should debug or not
	//emotions and hormones
	public float andrenaline = 0f; //the andrenaline scale; goes from 0 to any number you need; andrenaline influences how quick the person reacts; mostly probablt in the 0f-5f range
	public float fear; //how much the character is influenced by fear; 0 is none, 100 is petrified
	public bool panic; //whether we're panicing or not
	//this is for other scripts that are dependant on the tags
	public string tagToAvoidPrimary; //the primary tag that are the envemy character
	public string tagToAvoidSecondary; //the second tag of dangerous flying stuff, aka bullets
	//this is for the ai aiming
	public Vector3 enemyHeight; //the enemies offset from the base to compute aiming
	public Vector3 aimOffset; //the offset in aiming that results in other factors such as fear, andrenaline etc...
	public float offsetFactor = 1f; //the offset factor, to make the enemy aim better or less
	//this is for melee attack
	public float meleeAttackDistance = 1f; //the distance at which to attack with melee	
	public int meleeAttackUsed = 0; //the mele attack used by int; see below
	public string[] meleeAttackType = {"none","hybrid","only"}; //type of melee attack: none means no melee; hybrid means to attack with melee once we have no ammo; only meens we only use melee to attack
	//this if for search optimisation
	public float baseFramesToSearchCover = 70f; //once how many frames to check; the higher the better the peformace but worse cover optimisation
	//this is for weapons to indicate that they're out of range
	public bool turnToEnemyWeaponAngle = false;
	public bool enemyIsInUnobstructedView = false; //whether the enemy is in an unobstructed view but now in our field of view, e.g. behind us	
	//this is for multiple weapons
	public List<GameObject> otherWeapons = new List<GameObject>(); //other weapons that can be used
	public List<int> otherWeaponsMelee = new List<int>(); //other weapons' melee configurations
	//this is for strafing
	public bool strafing = false;
	
	
	
	private float ticks = 0f; //internal clock
	private AIMovementSwitcher movement; //for quicker movement
	private bool lastSeen; //whether the last sense to register the player was the eyes or the ears (eyes true, ears false)
	private bool currentlyInShock = false; //whether we're currently in shock and can't do anything
	private float ticksPlayerNotSeen = 0f; //of we are in engaged mode, and we lose the player's sight, then we start this tick counter and after a certian period of time, the ai goes into investigate mode
	private bool hit = false; //whether we've been hit or not
	private string engage = ""; //whether to engage or not: "engage", "stepdown", ""
	private bool alreadyEngaged = false; //whether we've already engaged the player once
	private float ticksInvestigate = 0f; //how many ticks have gone by; used by the investigative branch
	private float ticksInvestigateBarrier = 50f; //after how many ticks can tha ai try and investigate 
	private float ticksForFindinCover = 0f; //the auto cover search calls once every number of frames
	private float distanceToEnemy; //the distance to enemy; this gets checked automatically by the script at intervals
	private float enemyDistanceCheckFrame; 
	private float enemyDistanceCheckFrameBarrier = 100f;
	private float lastFiredEngagement = 0f; //how many frames since we last fired off trying to get into attack
	private float firedEngagementCriticalFrames = 300f; //only after this many frames may the engagemtn signal be fired again
	private bool allowedToStartEngagementCoroutine = true; //whether we're allowed to fire the coroutine
	
	
	
	//reference variables
	private float initFreezeTime;
	private float initShockTime;
	private float initFOV;
	private float initChanceForFight;
	private float initFear;
	private float initHeight = 0f;
	private float initYScale;
	
	//caches
	private ShooterAIAudioControl audioControl;
	private AIWeaponController weaponController;
	private AIMovementSwitcher movementManager;
	private SearchCover coverManager;
	private Sight sightManager;
	private Auditory earsManager;
	private PatrolManager patrolM;
	
	
	
	void Awake()
	{
		
		//set the caches correctly
		weaponController = GetComponent<AIWeaponController>();
		movementManager = GetComponent<AIMovementSwitcher>();
		coverManager = GetComponent<SearchCover>();
		sightManager = sight.GetComponent<Sight>();
		earsManager = ears.GetComponent<Auditory>();
		patrolM = patrolManager.GetComponent<PatrolManager>();
		
		
		//at the start the current state is patroling
		currentState = CurrentState.patrol;
		//and the engagment is set to free
		engagementStatus = EngagementState.free;
		
		//some variables that we will later use for reference
		initFreezeTime = freezeTime;
		initShockTime = shockTime;
		initFOV = sightManager.fieldOfView;
		initChanceForFight = chanceForFight;
		initFear = fear;
		audioControl = GetComponent<ShooterAIAudioControl>();
		
		if(GetComponent<NavMeshAgent>() != null)
		{
			initHeight = GetComponent<NavMeshAgent>().height;
		}

		
		initYScale = GetComponentInChildren<HealthManager>().gameObject.transform.localScale.y;
		
		movement = movementManager;
		
		
		
		
		//anti-bottlenecking procedure
		enemyDistanceCheckFrameBarrier += Random.Range(-30f, 30f);
	
		//find the health manager automatically
		healthManager = GetComponentInChildren<HealthManager>().gameObject;
		
		//setup multiple weapons correctly
		otherWeapons.Add( weaponController.weaponHoldingObject );
		otherWeaponsMelee.Add( meleeAttackUsed ); 
	}
	
	
	
	
	void Update()
	{
		
		//check whether we're allowed to think
		if(allowedToThink == false)
		{
			return;
		}
		
		//check whether we have to use basic level
		if( onlyBasicFunctionality == true )
		{
			goto BASIC_FUNCTION;
		}
		
		//setup multiple weapons correctly
		MultipleWeaponsConfigure();
		
		
		//update some variables
		enemyIsInUnobstructedView = CheckIfFree( sightManager.objectToSeek );
		if(distanceToEnemy < 1.5f)
		{
			enemyIsInUnobstructedView = true;
		}	
		
		
		
		//anti-bottlenecking procedure
		lastFiredEngagement += 1f;
		enemyDistanceCheckFrame += 1f;
		if(enemyDistanceCheckFrame > enemyDistanceCheckFrameBarrier)
		{
			enemyDistanceCheckFrame = 0f;
			if(sightManager.objectToSeek != null)
			{
				distanceToEnemy = earsManager.CalculatePathLength(sightManager.objectToSeek.transform.position);
				//coverManager.FindCoverPostion( sightManager.eyeLevel, false );
			}
		}
		
		//patrol part
		if(currentState == CurrentState.patrol)
		{
			
			//disable crouching
			if(crouching == true)
			{
				crouching = false;
			}
			
			//internal clock
			ticks += 1f;
			
			if(ticks > tickBarrier)
			{
				//if we need to check for updated position
				ticks = 0f;
				//set our new waypoint
				if(patrolM != null)
				{
					movement.SetNewDestination(patrolM.nextDestination);
				}
				if(patrolManager.GetComponent<LocationManager>() != null)
				{
					movement.SetNewDestination(patrolManager.GetComponent<LocationManager>().nextDestination);
				}
				
				
				if(debug)
				{
					Debug.Log("Going to next waypoint");
				}
				
			}
		}
		
		
		
		
		//if we suddenly hear/see player and we are in patrol and we are not engaging/stepping down
		if((currentState == CurrentState.patrol || currentState == CurrentState.chase || (currentState == CurrentState.investigate && ticksInvestigate > ticksInvestigateBarrier)) && engage == "")
		{
			
			//if we can hear the target
			if(earsManager.canHearTarget == true)
			{
				//investigate what we heard there
				movement.SetNewDestination(earsManager.lastHeardPosition);
				//set the correct state
				currentState = CurrentState.chase;
				//we heard last
				lastSeen = false;
				
				if(debug)
				{
					Debug.Log("Chase using ears");
				}
				
			}
			
	
			//if we saw the target, investigate
			if(sightManager.canSeeObject == true)
			{
				//investigate what we saw there
				movement.SetNewDestination(sightManager.lastSeenLocation);
				//set the correct state
				currentState = CurrentState.chase;
				//we saw last
				lastSeen = true;
				
				if(debug)
				{
					Debug.Log("Chase using eyes");
				}
				
			}
			
			//if we heard a bullet
			if(earsManager.canHearTarget2 == true)
			{
	
				if(debug)
				{
					Debug.Log("Chase where we heard bullet.");
				}
				
				//investigate what we heard from the area where the bullet originated
				movement.SetNewDestination( earsManager.lastHeardPositionSecondary );
				//set the correct state
				currentState = CurrentState.chase;
				//we heard last
				lastSeen = false;
				
			}
			
		}
		
		
		
		//if we were chasing him but lose sight/hearing of him, change to investigate
		if(currentState == CurrentState.chase && earsManager.canHearTarget == false && sightManager.canSeeObject == false &&
		   earsManager.canHearTarget2 == false)
		{
			//change to investigate
			currentState = CurrentState.investigate;
			StartCoroutine( StartInvestigationCountdown() );
			crouching = false;
			
			if(debug)
			{
				Debug.Log("Going from chase to investigate, because we can't locate enemy.");
			}
			
		}
		
		
		
		
		//if we're investigating and we're past the barrier
		if(currentState == CurrentState.investigate && ticksInvestigate > ticksInvestigateBarrier)
		{
			//lets go to the last seen location if we last saw the player, else to last heard location
			if(lastSeen == true)
			{
				
				if(debug)
				{
					Debug.Log("New last seen pos");
				}
				
				ticksInvestigate = 0f;
				movement.SetNewDestination(sightManager.lastSeenLocation);
			}
			else
			{
				
				if(debug)
				{
					Debug.Log("New last heard pos");
				}
				
				ticksInvestigate = 0f;
				
				if( earsManager.lastHeardPosition != Vector3.zero )
				{
					movement.SetNewDestination( earsManager.lastHeardPosition);
				}
				else
				{
					movement.SetNewDestination( earsManager.lastHeardPositionSecondary);
				}
			
			}
		}
		
		
	ticksInvestigate += 1f;
		
		
	
		
		
		//once we see the player
		//if we suddenly hear/see player and we are in patrol/chase/investigate
		if(currentState == CurrentState.patrol || currentState == CurrentState.chase || currentState == CurrentState.investigate)
		{
			
			//if we can see the target
			if(sightManager.canSeeObject == true)
			{
				gameObject.AddComponent( FindCorrectVisualCheckString() );
				
				//check if we should enage or not
				SendMessage("VisualCheck");
				
				//start again for reset, so that if the player does act in the rules of engagement, we wont miss it
				StartCoroutine("ResetEngagement");
				
				
				if(alreadyEngaged == false)
				{
					
					//if we have yet engaged the player once
					if(engage == "engage")
					{
						//if we should engage
						currentState = CurrentState.engage;
						EngageShooting();
						//go in shock to higher the reaction time
						StartCoroutine("Shock");
						//say that we've already engaged the player
						alreadyEngaged = true;
						
						if(debug)
						{
							Debug.Log("We see enemy, engage!");
						}
						
					}
					else
					{
						//else we step down and go back to patroling
						currentState = CurrentState.patrol;
						
						if(debug)
						{
							Debug.Log("Step down.");
						}
					}
					
				}
				else
				{
					//if we have engaged the player already, ATTACK!
					currentState = CurrentState.engage;
					EngageShooting();
					
					if(debug)
					{
						Debug.Log("We see enemy again! Attack!");
					}
				}
				
				
			}
		
		}
		
		
		//if we're in cover and suddenly see the player
		if(currentState == CurrentState.cover && movement.velocity.magnitude < 0.2f && enemyIsInUnobstructedView == true)
		{
			//go to engage
			currentState = CurrentState.engage;
			
			//reset previous variables
			engagementStatus = EngagementState.free;
			
			//start engagement cycle
			EngageShooting();
			
		}
		
		
		
		
		//what to do if we directly see the player and we are engaging it
		if(meleeAttackUsed != 2 && sightManager.canSeeObject == true && sightManager.canSeeObjectFromWeapon == true && currentState == CurrentState.engage && currentlyInShock == false)
		{
			//shoot
			ActualShoot();
			
			
			//stay on the spot if we have ammo
			if( weaponController.weaponHoldingObject != null && weaponController.weaponHoldingObject.GetComponent<AIWeaponSelected>().reloading == false)
			{
				
				if(debug)
				{
					Debug.Log("Frozen: We see enemy and are engaged");
				}
				
				movement.Freeze();
			}
			else
			{
				//run to cover if we're reloading
				
				movement.Defreeze();
				coverManager.FindCoverPostion( sightManager.eyeLevel, false );
				movement.SetNewDestination( coverManager.coverPostion );
				currentState = CurrentState.cover;
				StartCoroutine( ComeOutOfCover() );
				
					if(debug)
					{
						Debug.Log("We're reloading, run for cover!");
					}

			}
			
			
			//if we're too close, try switching weapons or get back
			if( meleeAttackUsed == 0 && sightManager.objectToSeek != null && Vector3.Distance( transform.position, sightManager.objectToSeek.transform.position) < (meleeAttackDistance + 1.5f) )
			{
				
				//find a weapon melee
				int newWeaponIndex = FindMeleeWeapon();
				
				if(newWeaponIndex != -1)
				{
					//create new weapon and set it up correctly
					GameObject newWeapon = Instantiate( otherWeapons[newWeaponIndex] ) as GameObject;
					newWeapon.SetActive( true);
					newWeapon.transform.parent = weaponController.weaponHoldingLocation;
					newWeapon.transform.position = weaponController.weaponHoldingObject.transform.position;
					newWeapon.transform.rotation = weaponController.weaponHoldingObject.transform.rotation;
					
					//switch it for the previous
					GameObject prevWeapon = weaponController.weaponHoldingObject;
					weaponController.weaponHoldingObject = newWeapon;
					prevWeapon.SetActive(false);
					
					
					
				}
				else
				{
					
					//get to a cover
					currentState = CurrentState.cover;
					
					movement.Defreeze();
					coverManager.FindCoverPostion( sightManager.eyeLevel, false );
					movement.SetNewDestination( coverManager.coverPostion );
					
					StartCoroutine( ComeOutOfCover() );
					
					if(debug)
					{
						Debug.Log("Engagement: Too close to enemy. Getting to cover.", transform);
					}
				}
				
			}
			
		}
	
		
		
		//if we get shot whilst we are engaged
		if(currentState == CurrentState.engage && hit == true)
		{
			//test our chance for either fight or flight
			if(FightOrFlight() == false)
			{
				//we should search for cover
				coverManager.FindCoverPostion( sightManager.eyeLevel, false );
				//change current state
				currentState = CurrentState.cover;
				//then go there
				movement.SetNewDestination(coverManager.coverPostion);
				//reset the cycle
				StartCoroutine( ComeOutOfCover() );
				
				if(debug)
				{
					Debug.Log("Cover");
				}
				
			}
			else
			{
				//else, ignore the pain and CHARGE!
				hit = false;
				//move towards where we think the player is
				movement.SetNewDestination(sightManager.lastSeenLocation);
				//audio
				if(audioControl != null)
				{
					audioControl.PlaySound( ShooterAIAudioOptions.Charge );
				}
				
				
				if(debug)
				{
					Debug.Log("Charge");
				}
				
			}
		}
	
		
		
		
		//if we lose sight of the player whilst we are in engagement mode
		if(currentState == CurrentState.engage && sightManager.canSeeObject == false)
		{
			ticksPlayerNotSeen += 1f;
		}
		
		//reset clock if we see the player again
		if(sightManager.canSeeObject == true && ticksPlayerNotSeen > 0f)
		{
			ticksPlayerNotSeen = 0;
		}
		
		
		//if we dont see the player for a specific amount of time
		if(currentState == CurrentState.engage && ticksPlayerNotSeen > timeBarrierEngageToInvestigate)
		{
			//go into investigative mode
			currentState = CurrentState.investigate;
			engagementStatus = EngagementState.free;
			StartCoroutine( StartInvestigationCountdown() );
			crouching = false;
			
			if(debug)
			{
				Debug.Log("Go into investigation mode");
			}
			
		}
		
		
		//this part is for when the ai runs out of ammo, it will take a chance to run and melee the enemy else go into cover
		if(currentState == CurrentState.engage && weaponController.amountOfAmmo <= 0f && engagementStatus == EngagementState.free)
		{
			if(FightOrFlight())
			{
				movement.SetNewDestination(sightManager.lastSeenLocation);
				andrenaline += 2f;
				engagementStatus = EngagementState.busy;
			}
			else
			{
				currentState = CurrentState.cover;
				//then go to the cover
				coverManager.FindCoverPostion( sightManager.eyeLevel, false );
				movement.SetNewDestination(coverManager.coverPostion);
				StartCoroutine( ComeOutOfCover() );
			}
			
		}
		
		
		
		
		//this is the part that is used for the most basic function, without turning off the brain completely
	BASIC_FUNCTION:
		
		//this is the fight logic managing code
		if(currentState == CurrentState.engage)
		{
			//choose the correct type of fight logic, based on distance to us or availability of object, BUT we can't be only melee attack
			if(sightManager.objectToSeek != null && distanceToEnemy > distanceToStopWalking && meleeAttackUsed != 2)
			{
				
				DuringEngagement();
				
			}
			else
			{
				
				//if we're either to far for melee or not using melee at all
				if((sightManager.objectToSeek != null && distanceToEnemy > meleeAttackDistance && meleeAttackUsed == 1) || (meleeAttackUsed == 0 && sightManager.objectToSeek != null && distanceToEnemy < meleeAttackDistance ) )				
				{
					
					//choose the close range fighting logic
					CloseRangeFightLogic();
				}
				
			}	
			
			
			if( (sightManager.objectToSeek != null && distanceToEnemy > meleeAttackDistance && meleeAttackUsed == 2) ||
			   (sightManager.objectToSeek != null && distanceToEnemy < meleeAttackDistance && meleeAttackUsed == 1))
			{
				MeleeFightLogic();
			}
			
		}
		
		
		//control the speed
		SpeedControl();
	
		
		//the ai searches for cover once every number of seconds
		FindCover();
		
		//sight control
		SightControl();
		
		
		
		
		//crouching control
		if(allowCrouching == true)
		{
			ControlCrouching();
		}
		
		
		
		//this has to be last, so that the preseration logic always kicks in
		SelfPreservation();
		
		//take care of all emotions
		EmotionsControl();
		
		
		//this script does ik head stuff
		
		//if we're engaged, then look at where we're aiming, else just look forward
		if(weaponController.weaponHoldingObject != null && currentState == CurrentState.engage /*&&
		   Vector3.Distance( transform.position, weaponController.debugAimingPos) > 1.5f && sightManager.canSeeObject == true */)
		{
			
			
			if(weaponController.debugAimingPos != Vector3.zero)
			{
				animationManager.GetComponent<HandIK>().headShouldLook = true;
				animationManager.GetComponent<HandIK>().headLook = weaponController.debugAimingPos;
			}
			else
			{
				
				animationManager.GetComponent<HandIK>().headShouldLook = true;
				animationManager.GetComponent<HandIK>().headLook = sightManager.lastSeenLocation + enemyHeight;
				
			}
	
		}
		else
		{
			animationManager.GetComponent<HandIK>().headShouldLook = false;
		}
		
		//max weapon turn angle stuff, to make it more nice looking
		if(turnToEnemyWeaponAngle == true && currentState == CurrentState.engage)
		{
			movement.SetNewDestination(transform.position - (transform.position - sightManager.lastSeenLocation).normalized);
		}
		
		
	}
	
	
	
	
	
	
	//after the specified amount of seconds to reutrn back to patroling
	public IEnumerator StartInvestigationCountdown()
	{
		yield return new WaitForSeconds(investigationTime);
		currentState = CurrentState.patrol;
		
		if(debug)
		{
			Debug.Log("Go to investigation mode");
		}
	
		
	}
	
	
	//if we should engage enemy
	public void Engage()
	{
		engage = "engage";
	}
	
	//if we should just step down
	public void StepDown()
	{
		engage = "stepdown";
	}
	
	//this is to reset how we engage
	IEnumerator ResetEngagement()
	{
		yield return new WaitForSeconds(timeUntilNextCheck);
		engage = "";
	}
	
	//once we go to and from shock
	IEnumerator Shock()
	{
		currentlyInShock = true;
		
		//this is for emotion effects
		andrenaline += 2f;
		
		yield return new WaitForSeconds(Random.Range(shockTime/2, shockTime));
		currentlyInShock = false;
		
	}
	
	
	//what to do once we start to engage shooting
	public void EngageShooting()
	{
	
		if(lastFiredEngagement > firedEngagementCriticalFrames)
		{
			//move to a cover and shoot from there
			movement.SetNewDestination( movement.FindClosestHalfCover() );
			
			if(allowCrouching == true)
			{
				crouching = true;
			}
			
			lastFiredEngagement = 0f;
		}
		
		if(debug)
		{
			Debug.Log("Engage shooting");
		}
	
		
		
	}
	
	
	
	//the fight logic during engagement
	void DuringEngagement()
	{
		
		//strafing
		if( sightManager.canSeeObject == true && turnToEnemyWeaponAngle == false)
		{
			
			//if we can see the enemy and we are not over the turn to enemy weapon angle
			movementManager.TurnOffRotation();
			strafing = true;
			
			//Debug.LogError("TEST: Strafing!", gameObject);
		}
		else
		{
			strafing = false;
			movement.TurnOnRotation();
			
		}
		
		
		
		//start main fight logic
		if( allowedToStartEngagementCoroutine == true)
		{
			//start the far range engagement logic, if we're not already in it
			StartCoroutine( FightingLogicController() );	
		}
		
	}
	
	
	
	IEnumerator FightingLogicController()
	{
		
		
		allowedToStartEngagementCoroutine = false;
		
		
		//if we are currently free, determine what type of situation we are in and react to it
		if(engagementStatus == EngagementState.free || engagementStatus == EngagementState.cover)
		{
			
			//the situation that we are stationary and probably in cover, come out, try to shoot the player and then go to the next cover
			if(movement.velocity.magnitude > -0.1f && movement.velocity.magnitude < 0.1f)
			{
				
				
				engagementStatus = EngagementState.busy;
				
				
				if(debug)
				{
					Debug.Log("Engage: Started computing best location");
				}
				
				Vector3 attackPosition = movement.FindClosestHalfCover();
				
				//movement.Defreeze();
				//now lets move to that position
				movement.SetNewDestination(attackPosition);
				
				
				//go in to crouching
				if(allowCrouching == true)
				{
					crouching = true;
				}
				
				
				if(debug)
				{
					Debug.Log("Engage: Moving to attack position: " + attackPosition);
				}
				
				
				//wait till we get to the cover
			TEST_IF_AT_HALF_COVER:
					if(Vector3.Distance(transform.position, coverManager.coverPostion) > 2f )
				{
					yield return new WaitForFixedUpdate();
					goto TEST_IF_AT_HALF_COVER;
				}
				else
				{
					if(debug)
					{
						Debug.LogError("We're in cover; face enemy");
					}
					
					//face the general direction of the enemy
					movement.SetNewDestination( transform.position + ( sightManager.lastSeenLocation - transform.position).normalized * 1.1f);
				}
				
				
				
				//lets wait a random number of seconds while we're attacking the enemy
				yield return new WaitForSeconds(Random.Range(minCoverTime/2f, maxCoverTime/2f));
				
				
				if(debug)
				{
					Debug.Log("Engage: Deciding what to do");
				}
					
				//stand up
				crouching = false;
				
				//if we heard a bullet, go to cover, or else come closer to the last spotted area, or if the fight instict kicks in, STORM!
				if(earsManager.canHearTarget2 == true)
				{
					movement.SetNewDestination(coverManager.coverPostion);
					engagementStatus = EngagementState.cover;
					
					if(debug)
					{
						Debug.Log("Engage: Going back to cover");
					}
					
				}
				else
				{
					movement.SetNewDestination(sightManager.lastSeenLocation);
					engagementStatus = EngagementState.free;
					
					if(debug)
					{
						Debug.Log("Engage: Going back to cover/checking what happened");
					}
						
				}
				
			}
			
			
		}
		
		allowedToStartEngagementCoroutine = true;
	}
	
	
	
	//this is the close range fight logic
	void CloseRangeFightLogic()
	{
		
		//this is so that we can see the player from an attack position, if we cant see the enemy and we've stopped
		if(sightManager.canSeeObject == false && movement.velocity.magnitude > -0.1f && movement.velocity.magnitude < 0.1f && CheckIfFree(sightManager.objectToSeek) == false)
		{
			
			Vector3 attackPosition = Vector3.zero;
				
			
			if(GetComponent<NavMeshAgent>() != null)
			{
				
				//find the nearest clear point from which we can see the enemy, and try to attack from their
				//create variable
				NavMeshAgent agent = GetComponent<NavMeshAgent>();
				// Create a path and set it based on a target position.
				NavMeshPath path = new NavMeshPath();
				if(agent.enabled)
					agent.CalculatePath(sightManager.lastSeenLocation, path);
				
				// Create an array of points which is the length of the number of corners in the path + 2.
				Vector3 [] allWayPoints = new Vector3[path.corners.Length + 2];
				
				// The first point is our position.
				allWayPoints[0] = transform.position;
					
				// The last point is the target position.
				allWayPoints[allWayPoints.Length - 1] = sightManager.lastSeenLocation;
				
				// The points inbetween are the corners of the path.
				for(int i = 0; i < path.corners.Length; i++)
				{
					allWayPoints[i + 1] = path.corners[i];
				}
				
				//now find and return the last corner before the final position (as this is the last "cover" standing between the enemy and the position)
				attackPosition = allWayPoints[allWayPoints.Length - 3];
				
				
				//go though each waypoint in the navmesh and see from whats the closest point from which we can see the enemy
				int waypointTestId = 0;
				while(waypointTestId < allWayPoints.Length)
				{
					RaycastHit hitCheckWay;
					
					if(debug)
					{
						Debug.DrawRay(transform.position+coverManager.eyePosition, transform.position+coverManager.eyePosition - allWayPoints[waypointTestId], Color.cyan);
					}
					
					if(Physics.Raycast(transform.position+coverManager.eyePosition, transform.position+coverManager.eyePosition - allWayPoints[waypointTestId], out hitCheckWay))
					{
						if(hitCheckWay.transform.tag == coverManager.tagToAvoid)
						{
							break;
						}
						}
					
					waypointTestId += 1;
				}
				
				}
			else 
			{
				
				
				
				//Pathfinding.Path path = GetComponent<Seeker>().StartPath( transform.position, sightManager.lastSeenLocation);
				Pathfinding.Path path = GetComponent<Seeker>().GetNewPath(transform.position, sightManager.lastSeenLocation);
				
				Vector3[] allWayPoints = path.vectorPath.ToArray();
				
				if(allWayPoints.Length < 1)
				{
					attackPosition = transform.position;
					goto GoToAttackPos;
				}
				
				// The first point is our position.
				allWayPoints[0] = transform.position;
				
				// The last point is the target position.
				allWayPoints[allWayPoints.Length - 1] = sightManager.lastSeenLocation;
				
				// The points inbetween are the corners of the path.
				for(int i = 0; i < path.vectorPath.ToArray().Length; i++)
				{
					allWayPoints[i + 1] = path.vectorPath[i];
				}
				
				//now find and return the last corner before the final position (as this is the last "cover" standing between the enemy and the position)
				attackPosition = allWayPoints[allWayPoints.Length - 3];
				
				
				//go though each waypoint in the navmesh and see from whats the closest point from which we can see the enemy
				int waypointTestId = 0;
				while(waypointTestId < allWayPoints.Length)
				{
					RaycastHit hitCheckWay;
					
					if(debug)
					{
						Debug.DrawRay(transform.position+coverManager.eyePosition, transform.position+coverManager.eyePosition - allWayPoints[waypointTestId], Color.cyan);
					}
					
					if(Physics.Raycast(transform.position+coverManager.eyePosition, transform.position+coverManager.eyePosition - allWayPoints[waypointTestId], out hitCheckWay))
					{
						if(hitCheckWay.transform.tag == coverManager.tagToAvoid)
						{
							break;
						}
					}
					
					waypointTestId += 1;
				}
				
			}
				
			
			
			if(debug)
			{
				Debug.Log("Close engage: go to position where we can see player");
			}
			
			
			//now lets move to that position
		GoToAttackPos:
				movement.SetNewDestination(attackPosition);
			
		}
		
	
		//if close and ears can hear but cant see, just stop and turn towards enemy
		if(earsManager.canHearTarget == true && CheckIfFree(sightManager.objectToSeek) == true && sightManager.canSeeObject == false)
		{
			movement.SetNewDestination(transform.position + (sightManager.objectToSeek.transform.position - transform.position).normalized);
			
			if(debug)
			{
				Debug.Log("Turn towards enemy " + (transform.position + (sightManager.objectToSeek.transform.position - transform.position).normalized));
			}
		}
		
		//new theory: if we can hear the enemy and we're close, 
		
		
		
	}
	
	//this is for the melee fight logic
	void MeleeFightLogic()
	{
		//reserved for special use in melee logic
		
		//turn towards the enemy if we're not looking in its direction
		if(sightManager.objectToSeek != null && Vector3.Angle(transform.position, sightManager.objectToSeek.transform.position) > sightManager.fieldOfView/10f)
		{
			movement.SetNewDestination((sightManager.objectToSeek.transform.position - transform.position).normalized + transform.position);
			
			if(debug)
			{
				Debug.Log("Melee face enemy");
			}
			
		}
		
		if(debug)
		{
			Debug.Log("Melee attack logic");
		}
		
		//attack
		ActualShoot();
		
		if(sightManager.objectToSeek != null && distanceToEnemy > meleeAttackDistance)
		{
			movement.SetNewDestination(sightManager.lastSeenLocation);
			
			if(debug)
			{
				Debug.Log("Melee run to enemy");
			}
			
		}
		
		
	}
	
	//this logic is self preservation, so that the ai doesnt want to run up to the enemy if it has no ammo etc...
	void SelfPreservation()
	{
	
		
	/*
	//if we've stopped, and we're not facing the direction of the enemy, face in the general direction of where we last saw the enemy
	if(sightManager.objectToSeek != null && sightManager.lastSeenLocation != Vector3.zero && GetComponent<NavMeshAgent>().velocity.magnitude > -0.1f && GetComponent<NavMeshAgent>().velocity.magnitude < 0.1f && Vector3.Angle(transform.position, sightManager.lastSeenLocation) > sightManager.fieldOfView/4f)
	{
	
	if(debug)
	{
	Debug.Log("Face enemy");
	}
	
	movement.SetNewDestination(transform.position + sightManager.lastSeenLocation.normalized);
	}
	*/
	
	
	/*
	//stop walking closer if we are very close to the enemy and looking at him
	if(sightManager.objectToSeek != null && sightManager.canSeeObject == true && Vector3.Distance(transform.position, sightManager.lastSeenLocation) < distanceToStopWalking && Vector3.Angle(transform.position, sightManager.lastSeenLocation) < sightManager.fieldOfView/2f)
	{
	
	if(debug)
	{
	Debug.Log("Very close to enemy");
	}
	
	movement.FreezePosition();
	}
	else
	{
	movement.DefreezePosition();
	}
	*/
	
	
	
	//reasurre that we are in cover when we should be in cover
		if(engagementStatus ==  EngagementState.cover && currentState == CurrentState.engage && coverManager.SpotTrueCover(transform.position + coverManager.eyePosition) == false)
		{
			
			
			Vector3 newPos = coverManager.coverPostion;
			
			
			if(debug)
			{
				Debug.Log("Find real cover! " + newPos);
			}
			
			movement.SetNewDestination(newPos);
		}
		
		
		
		//this is if we lose track of the player visually during engagement and he isnt directly behing us
		if(sightManager.canSeeObject == false && earsManager.canHearTarget == true && engagementStatus !=  EngagementState.cover && currentState == CurrentState.engage && CheckIfFree(sightManager.objectToSeek) == false)
		{
			movement.SetNewDestination(earsManager.lastHeardPosition);
			
			if(debug)
			{
				Debug.Log("Try and find the player using ears");
			}
			
		}
		
		
		
		
		
		//ragdoll control
		if(animationManager.GetComponent<Animator>().GetCurrentAnimatorStateInfo(1).IsTag("StandUpFromBack") || 
		   animationManager.GetComponent<Animator>().GetCurrentAnimatorStateInfo(1).IsTag("StandUpFromBelly") ||
		   animationManager.GetComponent<RagdollHelper>().state == RagdollHelper.RagdollState.blendToAnim  ||
		   animationManager.GetComponent<RagdollHelper>().state == RagdollHelper.RagdollState.ragdolled)
		{
			movement.SetNewDestination( transform.position);
			movement.SetSpeed( 0f );
			movement.Freeze();
			
		}
		
		
		//if we're too close to the enemy, run for cover
		if(currentState == CurrentState.engage && distanceToEnemy < 1.5f)
		{
			
			if(debug)
			{
				Debug.Log("Too close in engagement. Get to cover");
			}
			
			movement.Defreeze();
			coverManager.FindCoverPostion( sightManager.eyeLevel, false );
			movement.SetNewDestination( coverManager.coverPostion );
			currentState = CurrentState.cover;
			
			StartCoroutine( ComeOutOfCover() );
		}
		
		
	}
	
	
	//this is the actual function to shoot
	public void ActualShoot()
	{
		
		//apply aimoffset
		aimOffset = aimOffset * offsetFactor;
		
		
		if( distanceToEnemy < 1.5f)
		{
			aimOffset = Vector3.zero;
		}
		
		//if we're allowed to shoot
		if(sightManager.canSeeObject == true && meleeAttackUsed != 2)
		{
			//aim
			weaponController.AimAtTarget(sightManager.lastSeenLocation + enemyHeight + aimOffset);
			//shoot player
			weaponController.ShootAi("ranged");
		}
		
		//if we're only melee test whether we can attack
		if(sightManager.canSeeObject == true && meleeAttackUsed == 2 && sightManager.objectToSeek != null && earsManager.CalculatePathLength(sightManager.objectToSeek.transform.position) < meleeAttackDistance)
		{
			//attack enemy using melee
			weaponController.ShootAi("melee");
		}
		
	}
	
	
	
	
	
	//if we get hit
	public void AiHit()
	{
		hit = true;
	}
	
	
	//come out of cover
	public IEnumerator ComeOutOfCover()
	{
		
		//crouch
		if(allowCrouching)
		{
			crouching = true;
		}
		
		
		//wait till we get to the cover
	TEST_IF_AT_COVER:
			if(Vector3.Distance(transform.position, coverManager.coverPostion) > 2f )
		{
			yield return new WaitForFixedUpdate();
			goto TEST_IF_AT_COVER;
		}
		else
		{
			if(debug)
			{
				Debug.LogError("We're in cover; face enemy");
			}
			
			//face the general direction of the enemy
			movement.SetNewDestination( transform.position + ( sightManager.lastSeenLocation - transform.position).normalized * 1.1f);
		}
		
		
		//wait random amount of seconds
		yield return new WaitForSeconds(Random.Range(minCoverTime, maxCoverTime));
		
		//reset hit
		hit = false;
		
		//go back to engage
		currentState = CurrentState.engage;
		engagementStatus = EngagementState.free;
		
		//move towards where we think the player is
		movement.SetNewDestination(sightManager.lastSeenLocation);
		
		
		if(debug)
		{
			Debug.Log("Come out of cover");
		}
		
	}
	
	
	
	//this returns either true of false to fight
	bool FightOrFlight()
	{
	
		float randomNum = Random.Range(0f, 100f);
		
		
		//more debugging
		if(debug)
		{
			Debug.Log("Chosen Random Number: "+randomNum);
		}
		
		
		if(randomNum < chanceForFight)
		{
			//if we should fight
	return true;
		}
		else
		{
			//else we should run for cover
			return false;
		}
		
		
	}
	
	
	//this controls the different speed in different modes 
	void SpeedControl()
	{
	
		//this variable computes the speed that the andrenaline adds
		float applyFactor = 1f;
		if(crouching == true)
		{
			applyFactor = crouchSpeedFactor;
		}
		if(strafing == true)
		{
			applyFactor = strafinSpeedFactor;
		}
		float andrenalineSpeed = Mathf.Sqrt(andrenaline) * applyFactor;
		
		
		switch(currentState)
		{
		case CurrentState.engage:
			movement.SetSpeed( (maxSpeedEngage+andrenalineSpeed) * applyFactor);
			break;
		case CurrentState.cover:
			movement.SetSpeed( (maxSpeedCover+andrenalineSpeed) * applyFactor);
			break;
		case CurrentState.patrol:
			movement.SetSpeed( (maxSpeedPatrol+andrenalineSpeed) * applyFactor);
			break;
		case CurrentState.chase:
			movement.SetSpeed( (maxSpeedChase+andrenalineSpeed) * applyFactor);
			break;
			
			
		}
	}
	
	
	//this is where all the emotions take place
	void EmotionsControl()
	{
		
		
		//andrenaline deacreases constantly
		if(andrenaline > 0.1f)
		{
			andrenaline -= 0.01f;
		}
		
		//if we're hit, increase andrenaline
		/*
	if(hit == true)
	{
			andrenaline += 0.03f;
	}*/
		
	//if we're in combat, increase andrenaline
		if(currentState == CurrentState.engage)
		{
			andrenaline += 0.011f;
		}
		
		//cap out the adrenaline
		andrenaline = Mathf.Clamp( andrenaline, 0f, 7f);
		
		
		
		
		
		//andrenaline effects and different clamps
		freezeTime = initFreezeTime - andrenaline/2f;   //different components are influenced differently by andrenaline
		freezeTime = Mathf.Clamp(freezeTime, 0f, initFreezeTime + 5f);
		shockTime = initShockTime - andrenaline/2f;
		shockTime = Mathf.Clamp(shockTime, 0f, initShockTime + 5f);
		sightManager.fieldOfView = initFOV + (andrenaline * 10f);
		sightManager.fieldOfView = Mathf.Clamp(sightManager.fieldOfView, 20f, initFOV+30f);
		chanceForFight = initChanceForFight + (andrenaline * 10f);
		
		
		
		//different effects of fear
		freezeTime = freezeTime + (fear*fear)/2000f;
		shockTime = shockTime + (fear*fear)/2000f;
		sightManager.fieldOfView = sightManager.fieldOfView - fear;
		andrenaline += 0.005f;
		chanceForFight = chanceForFight - fear * 4f;
		
		chanceForFight = Mathf.Clamp(chanceForFight, 0f, 100f);
		
		//these are effects taken by fear and andrenaline
		aimOffset = Random.insideUnitSphere * (fear/20f + andrenaline/20f);
		
	
		fear = Mathf.Clamp(fear, 0f, 100f);
		
		
		//if we're hit, increase fear
		if(hit)
		{
			fear += initFear/500f; 
		}
		
		//if we go over the 80 percent barrier, go into freeze and  go into panic
		if(fear > 80f)
		{
			StartCoroutine("Freeze");
			Panic();
		}
		else
		{
			panic = false;
		}
		
		
	}
	
	
	//what happens when the ai panics
	void Panic()
	{
		
		//randombly shoot without disconcerning what we're shooting
		if(meleeAttackUsed == 2)
		{
			weaponController.ShootAi("melee");
		}
		else
		{
			weaponController.ShootAi("ranged");
		}
		
		fear -= 0.01f;
		
		
		panic = true;
		
		
	}
	
	
	//this functions lets the ai auto find the optimum cover once every few seconds
	void FindCover()
	{
		
		coverManager.tagToAvoid = tagToAvoidPrimary;
		
		ticksForFindinCover += 1f;
	
		if(ticksForFindinCover > (baseFramesToSearchCover + Random.Range(10f, 100f))  && currentState != CurrentState.patrol)
		{
			//search for cover
			coverManager.FindCoverPostion( coverManager.eyePosition, false);
			//and reset
			ticksForFindinCover = 0f;
		}
		
	}
	
	
	//this function controls everything to do with the eyes
	void SightControl()
	{
		
		sightManager.eyeLevel = coverManager.eyePosition;
	
	}
	
	
	//helper function that checks whether their is something between point a and point b
	/// <summary>
	/// Checks if free.
	/// </summary>
	/// <returns>
	/// The if free.
	/// </returns>
	/// <param name='objectToCheck'>
	/// If set to <c>true</c> object to check.
	/// </param>
	public bool CheckIfFree(GameObject objectToCheck)
	{
		
		
		bool foundCorrectObj = false;
		
		if(objectToCheck != null)
		{
	
			RaycastHit hit;
			Vector3 rayToCheck = objectToCheck.transform.position - transform.position;
			
			if(debug)
			{
				Debug.DrawRay( transform.position + sightManager.eyeLevel/2f, rayToCheck, Color.green);
			}
	
			//send out the ray
			if(Physics.Raycast(transform.position + sightManager.eyeLevel/2f, rayToCheck, out hit))
			{
				
				
				
				//if the object we see is the one we're seeking	
				int layersToCheck = 25;
				Transform testObject = hit.transform;
				
				for(int currentLayer = 0; currentLayer < layersToCheck; currentLayer ++)
				{
					if(testObject.gameObject == objectToCheck)
					{
						foundCorrectObj = true;
						continue;
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
		}
	
	
		if(debug == true)
		{
			Debug.Log("Direct contact: " + foundCorrectObj);
		}
		
		
		//return a value
		return foundCorrectObj;
		
		
	}
	
	
	
	/// <summary>
	/// Checks if there is something between the eyes and the given position.
	/// </summary>
	/// <returns>
	/// Whether if its a direct view to the specified location. True if its direct, false if not
	/// </returns>
	/// <param name='posToCheck'>
	/// The end position
	/// </param>
	public bool CheckIfFree(Vector3 posToCheck)
	{
		
		bool foundCorrectObj = true;
		
		
		RaycastHit hit;
		Vector3 rayToCheck = posToCheck + sightManager.eyeLevel;
		
		if(debug)
		{
			Debug.DrawRay(sight.transform.position, rayToCheck, Color.green);
		}
		
		//send out the ray
		if(!Physics.Raycast( transform.position, rayToCheck, out hit))
		{
			
			foundCorrectObj = false;
			
		}
		
		
		
	
		if(debug == true)
		{
			Debug.Log("Direct contact: " + foundCorrectObj);
		}
		
		
		//return a value
		return foundCorrectObj;
		
		
		
	}
	
	
	
	
	/// <summary>
	/// Finds the correct visual check string.
	/// </summary>
	/// <returns>
	/// The correct visual check string.
	/// </returns>
	string FindCorrectVisualCheckString()
	{
		if (targetVisualCheck == null || targetVisualCheck.Count == 0)
			return "";
		var total = 0f;
		float chance = 0.0f;
		var chances = targetVisualCheckChance;
		foreach (float elem in chances)
		{
			total += elem;
		}
		var randomPoint = Random.value * total;
		for (float i = 0; i < chances.Count - 1; i++)
		{
			if (randomPoint < chances[(int)i])
				chance = i;
			else
				randomPoint -= chances[(int)i];
		}
		//chance = chances.Count - 1;
		if(chance >= 0  &&  chance < targetVisualCheck.Count)
			return targetVisualCheck[(int)chance];
		return "";
	}
	
		
	
	
		/// <summary>
		/// Controls the crouching.
		/// </summary>
		void ControlCrouching()
		{
			 
			
			if(crouching == true && ( ( GetComponent<NavMeshAgent>() != null && GetComponent<NavMeshAgent>().height > (initHeight/crouchingFactor + 0.1f) ) || ( healthManager.gameObject.transform.localScale.y > (healthManager.gameObject.transform.localScale.y/crouchingFactor) + 0.1f) ) )
			{
				if(debug)
				{
					//Debug.Log("Crouching: Initiate crouchin. Setting correct height.");
				}
				
				if(GetComponent<NavMeshAgent>() != null)
				{
					GetComponent<NavMeshAgent>().height = initHeight/crouchingFactor;
				}
				
				healthManager.gameObject.transform.localScale = new Vector3(healthManager.gameObject.transform.localScale.x, initYScale/crouchingFactor, healthManager.gameObject.transform.localScale.z);
				
			}
			else
			{
				if(crouching == false && ( ( GetComponent<NavMeshAgent>() != null && GetComponent<NavMeshAgent>().height < (initHeight - 0.1f) ) || ( healthManager.gameObject.transform.localScale.y < (initYScale - 0.1f) ) ) )
				{
				if(debug)
				{
					//Debug.Log("Crouching: No more crouching. Setting correct height.");
				}
				
				if(GetComponent<NavMeshAgent>() != null)
				{
					GetComponent<NavMeshAgent>().height = initHeight;
				}
				
				healthManager.gameObject.transform.localScale = new Vector3(healthManager.gameObject.transform.localScale.x, initYScale, healthManager.gameObject.transform.localScale.z);
				
				}
			}
			
		}
	
	
	//if we suddenly freeze from something like a bullet
	public IEnumerator Freeze()
	{
		
		if(debug)
		{
			Debug.Log("Freeze");
		}
		
		movement.Freeze();
		yield return new WaitForSeconds(freezeTime);
		movement.Defreeze();
		
		if(debug)
		{
			Debug.Log("Unfreeze");
		}
		
	}
	
	
	
	/// <summary>
	/// Configures the variables to use for multiple weapons
	/// </summary>
	void MultipleWeaponsConfigure()
	{
		
		
		//comprehend the current weapon
		GameObject yieldingWeapon = weaponController.weaponHoldingObject;
		
		if(yieldingWeapon == null)
		{
			return;
		}
		
		GameObject currentWeapon = null;
		int currentWeaponIndex = -1;
		for(int x = 0; x < otherWeapons.Count; x++)
		{
			if(otherWeapons[x] == yieldingWeapon)
			{
				currentWeapon = yieldingWeapon;
				currentWeaponIndex = x;
			}
		}
		
		
		//test for errors
		if( currentWeapon == null)
		{
			Debug.LogError("Shooter AI: AI not yielding weapon in list", gameObject);
			return;
		}

		if( currentWeaponIndex > otherWeaponsMelee.Count)
		{
			Debug.LogError("Shooter AI: Melee settings not setup correctly", gameObject);
			return;
		}
		
		
		
		//set the correct index, if its not setup correctly
		if( otherWeaponsMelee[currentWeaponIndex]  != meleeAttackUsed)
		{
			meleeAttackUsed = otherWeaponsMelee[currentWeaponIndex];
		}
		
	}
	
	
	/// <summary>
	/// Finds a melee weapon from the AI's inventory.
	/// </summary>
	/// <returns>The melee weapon.</returns>
	int FindMeleeWeapon()
	{
		int indexMelee = -1;
		
		for(int x = 0; x < otherWeaponsMelee.Count; x ++)
		{
			if(otherWeaponsMelee[x] == 1 || otherWeaponsMelee[x] == 2)
			{
				indexMelee = x;
				continue;
			}
		}
		
		return indexMelee;
	}
	
	
	
	void OnDrawGizmosSelected ()
	{
	
		if(sight.GetComponent<Sight>().objectToSeek != null && debug == true)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(movement.FindClosestHalfCover(), 0.7f);
		}
		
		if(movement != null && debug)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(movement.destinationPosition, 0.3f);
	}
		
		
	}
	
	
	
	
}
