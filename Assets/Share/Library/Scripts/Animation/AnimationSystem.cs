using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
	\brief Define the animation path
*/
public enum AnimationPath {
	Linear,		/**< Linear straight line */
	Sine		/**< Sine wave*/		
}

/**
	\brief	Define the animation mode		
*/
public enum AnimationMode {
	PlayOnce,	/**< Play only one time, stop at end point */
	Cycle,		/**< Play cycle, the animation will start over again after the end point reached*/
	Oscillate,	/**< Play oscillate, the start end point will swap after the end point reached */		
	CycleWithOffset, /**< Play cycle, but the start point and end point will be offset by the difference */
	FixedCycle,	/**< Play fixed number of cycle, the animation will start over again after the end point reached*/
	FixedOscillate,	/**< Play oscillate, the start end point will swap after the end point reached */	
	FixedCycleWithOffset,	/**< Play fixed number of cycle, the animation will start over again after the end point reached*/
}

/**
	\brief Define the animation state
*/
public enum AnimationStates {
	Idle,	/**< Idle state of animation, the animation did not start. TransformType values would be the same as start point*/
	Move,	/**< Moving state of animation, TransformType values would be changing while updating */
	Finished	/**< Finished state of animation, Transform Type values will set to end point once*/
}
/**
 	\brief	Point calculation base requires the input of start and end point. While Velocity calculation base requires only the input of start point, the end point would be automcatically calcualate according to the start and end velocity.
 	
*/
public enum CalculationBase {
	Point,
	Velocity
}

public class AnimationSystem<T> : MonoBehaviour {
//	public GameObject callbackListener;	/**< Hold the reference of call back game object receiver, will send a message named "AnimationSystemCallback" when PlayOnce finished */
//	public string callbackFunction;
//	public string callbackArgument;
	
	public Callback callback;
	
	public T startPoint;	/**< Value of the start point */	
	public T endPoint;		/**< Value of the end point */
	public T currentPoint;
	public T amplitude;	/**< Value of the sine amplitude */
	public T frequency;	/**< Value of the sine frequency */
	
	public bool useDuration;	/**< Option for user to set duration to the animation system instead of speed, use speed for calculation is default */
	public float speed;		/**< Value of the speed */
	public float duration;	/**< Value of duration, only take affect if useDuration is on(true) */ 	

	public CalculationBase calculationBase = CalculationBase.Point;

	public T startVelocity;	/**< Value of the start velocity */
	public T endVelocity;	/**< Value of the end velocity */
	public T acceleration;	/**< Value of the acceleration */

	public bool moveAtStart = false;	/**< If true, the animation system will function automatically, if false, the animation system will function only by calling "StartMove" */	
	public AnimationStates animState;			/**< Current AnimationState*/
	
	public AnimationPath animationPath;		/**< Define the AnimationPath of the animation */
	public EaseType ease;						/**< Define the EaseType of the animation */
	public AnimationMode animationMode;	/**< Define the AnimationMode of the animation */

	public int numberOfCycle = 0;		/**< If the animation mode is not playOnce, user can specify a counter for the animation to stop */
	private int animCounter = 0;

	public bool holdAfterCycle = false;	/**< If true, the animation system will hold the value of a specific of time in seconds, not affect in PlayOnce */
	public float holdDuration;	/**< Time in seconds of hold duration, only take affect if holdAfterCycle is on (true) */

	// for calculation of animation move time acculumation
	private float moveTime = 0f; 	/**< Private variable storing the move time which updating in Update( ) */
	
	
	// delegates which implement by inheritance class
	protected delegate T GetCurrentPointFunction ();
	protected GetCurrentPointFunction dGetCurrentPoint;
	
	protected delegate float GetDistanceFunction(T source, T target);
	protected GetDistanceFunction dGetDistance;
	
	protected delegate T CalculateMoveValueFunction ( float moveTime, EaseType ease);
	protected CalculateMoveValueFunction dCalculateMoveValue;
	
	protected delegate void ApplyMoveValueFunction (T moveValue);
	protected ApplyMoveValueFunction dApplyMoveValue;
	
	protected delegate void ApplyEndValueFunction (T endValue);
	protected ApplyEndValueFunction dApplyEndValue;
	
	protected delegate T AccumulateEndValueFunction (T endValue, T startValue);
	protected AccumulateEndValueFunction dAccumulateEndValue;

	protected delegate T CalculateAccelerationFunction (T startVelocity, T endVelocity, float duration);
	protected CalculateAccelerationFunction dCalculateAcceleration;

	protected delegate T CalculateEndPointFunction (T startPoint, T startVelocity, float duration, T acceleration);
	protected CalculateEndPointFunction dCalculateEndPoint;
	
		
	/**
		\fn	void	Start ( )
		\brief	Start function inherit from MonoBehaviour
	*/
	public void Start(){		
		if (moveAtStart){
			animState = AnimationStates.Move;
		}
		// set duration if use speed
		switch (calculationBase){
			case CalculationBase.Point:
				if (!useDuration){
					duration = SpeedToDuration(startPoint, endPoint, speed);
				}
			break;
			case CalculationBase.Velocity:
				acceleration = dCalculateAcceleration(startVelocity, endVelocity, duration);
				endPoint = dCalculateEndPoint(startPoint, startVelocity, duration, acceleration);
			break;
		}
	}
	
	/**
		\fn	void	Update ( )
		\brief	Update function inherit from MonoBehaviour
	*/
	void Update (){
		if (animState != AnimationStates.Move)
			return;
		
		moveTime += Time.deltaTime;			
		if (moveTime < duration ){
			T newValue = dCalculateMoveValue ( moveTime, ease);
			currentPoint = newValue;
			dApplyMoveValue (newValue);
		} else {
			currentPoint = endPoint;
			dApplyEndValue (endPoint);
			ExecuteAnimationMode();
		}
	}
	
	/**
	 	\fn	float SpeedToDuration ( T source , T destination, float walkSpeed)
		\brief	Calculate input speed and distance between and return time required to travel between the source and destination
		\param[in]	SourceVector3	source vector 3 values for calculation 
		\param[in]	DestinationVector3	destination vector 3 values for calculation
		\return	The distance between input source and destination
	*/
	protected float SpeedToDuration ( T source , T destination, float walkSpeed){
		float dis = dGetDistance (source, destination);
		return dis / walkSpeed ;
	}
	
	private void ExecuteAnimationMode (){
		switch (animationMode){
			case AnimationMode.FixedOscillate:
				if (animCounter +2 > numberOfCycle ){
					SwapStartEndPoint();
					animState = AnimationStates.Finished;
					LaunchCallback();
				} else {
					SwapStartEndPoint();
					StartCoroutine ( ResetMove(holdDuration) );
				}
				break;
			case AnimationMode.Oscillate:	
				SwapStartEndPoint();
				StartCoroutine ( ResetMove(holdDuration) );
				break;
			case AnimationMode.FixedCycleWithOffset:
				if (animCounter +2 > numberOfCycle ){
					animState = AnimationStates.Finished;
					LaunchCallback();
				} else {
					//T buffer = endPoint;
					endPoint = dAccumulateEndValue (endPoint, startPoint);
					startPoint = endPoint;
					StartCoroutine ( ResetMove(holdDuration) );
				}
			
				break;
			case AnimationMode.CycleWithOffset:
				//T buffer = endPoint;
				endPoint = dAccumulateEndValue (endPoint, startPoint);
				startPoint = endPoint;
				StartCoroutine ( ResetMove(holdDuration) );
				break;
				
			case AnimationMode.FixedCycle:					
				if (animCounter +2 > numberOfCycle ){
					animState = AnimationStates.Finished;
					LaunchCallback();
				} else {
					StartCoroutine ( ResetMove(holdDuration) );
				}
			break;
			case AnimationMode.Cycle:
				StartCoroutine ( ResetMove(holdDuration) );
				break;					
			case AnimationMode.PlayOnce:
				animState = AnimationStates.Finished;
				LaunchCallback();
				break;
		}	
	}
	
	private void SwapStartEndPoint(){
		//swap start and end
		T buffer = startPoint;
		startPoint = endPoint;
		endPoint = buffer;
	}
	
	/**
		\fn	void	StartMove()
		\brief	Change the AnimationState to Move immediately
	*/
	public void StartMove(){
		moveTime = 0;
		animCounter = 0;
		animState = AnimationStates.Move;
	}
	
	/**
		\fn	IEnumerator	StartMove(float duration_)
		\brief	Change the AnimationState to Move after input time duration
		\param[in]	duration_	Time duration in seconds delay before start
	*/
	public IEnumerator StartMove(float duration_){
		yield return new WaitForSeconds (duration_);
		moveTime = 0;
		animCounter = 0;
		animState = AnimationStates.Move;
	}
	
	/**
		\fn	void	StopMove()
		\brief	Change the AnimationState to Finished immediately
	*/
	public void StopMove(){
		animState = AnimationStates.Finished;
//		StartCoroutine( StopMove (0f) );
	}
	
	/**
		\fn	IEnumerator StopMove(float duration_)
		\brief	Change the AnimationState to Finished after input time duration
		\param[in]	duration_	Time duration in seconds delay before stop
	*/
	public IEnumerator StopMove(float duration_){
		yield return new WaitForSeconds (duration_);
		animState = AnimationStates.Finished;
	}
	
	/**
		\fn	void PauseMove()
		\brief	Change the AnimationState to Idle immediately
	*/
	public void PauseMove(){
//		StartCoroutine( PauseMove (0f) );
		animState = AnimationStates.Idle;
	}
	
	/**
		\fn	IEnumerator PauseMove(float duration_)
		\brief	Change the AnimationState to Idle after input time duration
		\param[in]	duration_	Time duration in seconds delay before pause
	*/
	public IEnumerator PauseMove(float duration_){
		yield return new WaitForSeconds (duration_);
		animState = AnimationStates.Idle;
	}
	
	/**
		\fn	void ResumeMove()
		\brief	Change the AnimationState to Move immediately
	*/
	public void ResumeMove(){
		animState = AnimationStates.Move;
		//StartCoroutine( ResumeMove (0f) );
	}
	
	/**
		\fn	IEnumerator ResumeMove(float duration_)
		\brief	Change the AnimationState to Move immediately
		\param[in]	duration_	Time duration in seconds delay before resume
	*/	
	public IEnumerator ResumeMove(float duration_){
		yield return new WaitForSeconds (duration_);
		animState = AnimationStates.Move;
	}
	
	/**
		\fn	void	ResetMove()
		\brief	Restart the AnimationSystem immediately
	*/
	public void ResetMove(){
		StartCoroutine (ResetMove (0f) );
	}
	
	/**
		\fn	IEnumerator ResetMove(float duration_)
		\brief	Restart the AnimationSystem after input time duration
		\param[in]	duration	Time duration in seconds delay before restart
	*/
	public IEnumerator ResetMove(float duration_){
		moveTime = 0;
		if (numberOfCycle > 0){		
			animCounter ++;				
			if (animCounter >= numberOfCycle ){
				animState = AnimationStates.Finished;
				if (duration_ > 0)
					yield return new WaitForSeconds (duration_);
				animState = AnimationStates.Move;
				animCounter = 0;
			} else {
				animState = AnimationStates.Move;
			}
		} else {
			animState = AnimationStates.Finished;
			if (duration_ > 0)
				yield return new WaitForSeconds (duration_);
			animState = AnimationStates.Move;
		}
	}
	
	/**
		\fn	void MoveTo(T endPoint_)
		\brief	Move the current point to end point according to existing speed
		\param[in]	endPoint_	End point of the animation
	*/
	public void MoveTo(T endPoint_){
		MoveToSpeed (endPoint_, speed);	
	}

	/**
		\fn	void MoveToTime( T endPoint_, float duration_)
		\brief	Move the current point to end point according to input time
		\param[in]	endPoint_	End point of the animation
		\param[in]	duration_	Time duration in seconds 
	*/
	public void MoveToTime( T endPoint_, float duration_){
		startPoint = dGetCurrentPoint();
		endPoint = endPoint_;
		duration = duration_;
		calculationBase = CalculationBase.Point;
		ResetMove();
	}
	
	/**
		\fn	void MoveToSpeed (T endPoint_, float speed_)
		\brief	Move the current point to end point according to input speed
		\param[in]	endPoint_	End point of the animation
		\param[in]	speed_	New speed of the animation
	*/
	public void MoveToSpeed (T endPoint_, float speed_){
		startPoint = dGetCurrentPoint();		
		endPoint = endPoint_;
		duration = SpeedToDuration (startPoint, endPoint_, speed_);
		calculationBase = CalculationBase.Point;
		ResetMove();
	}
	
	/**
		\fn	void MoveFromToTime (T startPoint_, T endPoint_, float duration_)
		\brief	Move the start point to end point according to input time
		\param[in]	startPoint_	Start point of the animation
		\param[in]	endPoint_	End point of the animation
		\param[in]	duration_	Time duration in seconds 
	*/
	public void MoveFromToTime (T startPoint_, T endPoint_, float duration_){
		startPoint = startPoint_;
		endPoint = endPoint_;
		duration = duration_;
		calculationBase = CalculationBase.Point;
		ResetMove();
	}
	
	/**
		\fn	void MoveFromToSpeed (T startPoint_, T endPoint_, float speed_)
		\brief	Move from the current point to end point according to input speed
		\param[in]	startPoint_	Start point of the animation
		\param[in]	endPoint_	End point of the animation
		\param[in]	speed_	new speed of the animation
	*/
	public void MoveFromToSpeed (T startPoint_, T endPoint_, float speed_){
		startPoint = startPoint_;
		endPoint = endPoint_;
		speed = speed_;
		duration = SpeedToDuration (startPoint, endPoint_, speed_);
		calculationBase = CalculationBase.Point;
		ResetMove();
	}
	
	/**
		\fn	void MoveFromToVelocity (T startPoint_, T startVelocity_, T endVelocity_, float duration_)
		\brief	Move from the current point and start velocity to end velocity according to input duration
		\param[in]	startPoint_	start point of the animation
		\param[in]	startVelocity_	Start velocity of the animation
		\param[in]	endVelocity_	End velocity of the animation
		\param[in]	duration_	Time duration in seconds 
	*/
	public void MoveFromToVelocity (T startPoint_, T startVelocity_, T endVelocity_, float duration_) {
		calculationBase = CalculationBase.Velocity;
		startPoint = startPoint_;
		startVelocity = startVelocity_;
		endVelocity = endVelocity_;
		duration = duration_;
		acceleration = dCalculateAcceleration(startVelocity, endVelocity, duration);
		endPoint = dCalculateEndPoint(startPoint, startVelocity, duration, acceleration);
		ResetMove();
	}
	
	/**
		\fn	void LaunchCallback()
		\brief	Send a message to callback receiver with user defined callbackFunction
		\pre	callbackListener have GameObject's reference
		\post	callbackListener have function named same as defined callbackFunction for callback
	*/
	private void LaunchCallback(){
		if (callback != null)
			callback.SendCallback(this);
//		ExecSendCallback(callbackListener, callbackFunction, callbackArgument);
	}
	
//	private void ExecSendCallback(GameObject listener_, string command_, string comandArgument_){
//		if (listener_ != null){
//			if (comandArgument_ != ""){
//				switch (comandArgument_){
//					case "this":	
//						listener_.SendMessage (command_, this);	
//					break;
//					case "gameObject":
//					case "GameObject":	
//						listener_.SendMessage (command_, gameObject);	
//					break;
//					case "true":
//						listener_.SendMessage (command_, true);
//					break;
//					case "false":
//						listener_.SendMessage (command_, false);
//					break;
//					default: 
//						listener_.SendMessage (command_, comandArgument_);		
//					break;
//				}			
//			}else {
//				listener_.SendMessage (command_);
//			}
//		}
//	}
}
