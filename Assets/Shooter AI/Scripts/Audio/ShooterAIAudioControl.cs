using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ShooterAIAudioOptions { Patrol, Investigate, Engage, Cover, Panic, Charge};

public class ShooterAIAudioControl : MonoBehaviour {
	
	
	public bool soundEnabled = true; //whether sound is enabled or not
	
	[Range(0,1f)]
	public float basePitch = 1f; //the base pitch
	
	[Range(0, 0.9f)]
	public float randomPitch = 0.1f; //the random pitch +- relative value
	
	
	[ Range( 0, 1f) ]
	public float chanceForSound = 0.5f; //the chance for the audio to play
		
	public AudioClip[] patrolSounds; //the patrol sounds
	public AudioClip[] investigateSounds; //the investigate sounds
	public AudioClip[] engageSounds; //the engage sounds
	public AudioClip[] coverSounds; //the cover sounds
	public AudioClip[] panicSounds; //the panic sounds
	public AudioClip[] chargeSounds; //the charge sounds
	
	
	//cache
	private AudioSource c_Audio;
	private AIStateManager brain;
	private CurrentState previousState;
	private bool previousPanic;
	
	
	
	void Awake()
	{
		//iniate audio source and cache variables
		if(GetComponent<AudioSource>() == null)
		{
			c_Audio = gameObject.AddComponent<AudioSource>();
		}
		else
		{
			c_Audio = GetComponent<AudioSource>();
		}
		
		brain = GetComponent<AIStateManager>();
		previousState = brain.currentState;
		previousPanic = brain.panic;
		
		//set the pitch correctly
		c_Audio.pitch = basePitch;
		c_Audio.pitch = c_Audio.pitch + Random.Range( -randomPitch, randomPitch);
		
	}
	
	
	
	
	
	void Update()
	{
		//set the correct sounds
		CheckAndSetCorrectSounds();
	}
	
	
	
	
	/// <summary>
	/// Checks and sets the correct sounds.
	/// </summary>
	void CheckAndSetCorrectSounds()
	{
		
		//if we've changed the state in the last frame
		if(previousState != brain.currentState)
		{
			//set the correct sound
			switch(brain.currentState)
			{
			case CurrentState.patrol: PlaySound(ShooterAIAudioOptions.Patrol); break;
			case CurrentState.investigate: PlaySound( ShooterAIAudioOptions.Investigate); break;
			case CurrentState.engage: PlaySound( ShooterAIAudioOptions.Engage); break;
			case CurrentState.cover: PlaySound( ShooterAIAudioOptions.Cover); break;
			}	
			
		}
		
		//check if we've started panicking
		if( previousPanic != brain.panic && brain.panic == true)
		{
			PlaySound( ShooterAIAudioOptions.Panic);
		}
		
		
		//set the current state as the last state
		previousState = brain.currentState;
	}
	
	
	
	
	
	
	/// <summary>
	/// Plays a sound.
	/// </summary>
	/// <param name="typeOfSound">Type of sound.</param>
	public void PlaySound( ShooterAIAudioOptions typeOfSound)
	{
		//test if we're even allowed to play sounds
		if( soundEnabled == false)
		{
			return;
		}
		
		//check if the chances come together
		if( Random.Range(0f, 1f) > chanceForSound )
		{
			return;
		}
		
		//initiate the sound that we will play
		AudioClip soundToPlay = null;
		
		//find out which sound to play
		switch( typeOfSound)
		{
		case ShooterAIAudioOptions.Patrol: soundToPlay = patrolSounds[ (int)Random.Range(0, patrolSounds.Length) ]; break;
		case ShooterAIAudioOptions.Investigate: soundToPlay = investigateSounds[ (int)Random.Range(0, investigateSounds.Length) ]; break;
		case ShooterAIAudioOptions.Engage: soundToPlay = engageSounds[ (int)Random.Range(0, engageSounds.Length) ]; break;
		case ShooterAIAudioOptions.Cover: soundToPlay = coverSounds[ (int)Random.Range(0, coverSounds.Length) ]; break;
		case ShooterAIAudioOptions.Panic: soundToPlay = panicSounds[ (int)Random.Range(0, panicSounds.Length) ]; break;
		case ShooterAIAudioOptions.Charge: soundToPlay = chargeSounds[ (int)Random.Range(0, chargeSounds.Length) ]; break;
			
		}
		
		
		//play the sound
		c_Audio.PlayOneShot( soundToPlay);
		
	}
	
	
	
	
	
}
