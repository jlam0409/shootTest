using UnityEngine;
using System.Collections;

/**
	\brief	Define the Texture type, currently support sprite sheet and image sequence
*/
public enum TextureType{
	SpriteSheet,	/**< Sprite Sheet */		
	SequenceImage		/**< Sequence Image */
}

// can further divide int and uv animation system
public class UVAnimation : AnimationSystem<int> {
	public GameObject animObject;
	private Material animMaterial;
	public TextureType textureType ;	/**< Define the TextureType of the texture resource*/
	
	public int spriteWidth;
	public int spriteHeight;
	
	public int currentFrame = -1;
	public int[] animFrameSequence; /**< anim uv frame sequence*/
	public Texture[] animFrameTexture;
	public bool setOffsetOnly = false;	// set this to true if the UV tile set properly already in 3D softwares.
	
	
	public void Awake(){
		base.dGetCurrentPoint = new GetCurrentPointFunction (GetCurrentPointInteger);
		base.dGetDistance = new GetDistanceFunction(GetDistanceInteger);
		base.dCalculateMoveValue = new CalculateMoveValueFunction (CalculateMoveValueInteger);
		base.dApplyMoveValue = new ApplyMoveValueFunction (ApplyMoveValueInteger);
		base.dApplyEndValue = new ApplyEndValueFunction (ApplyEndValueInteger);
		base.dAccumulateEndValue = new AccumulateEndValueFunction (AccumulateEndValueInteger);
		
		InitData();
	}	
	
	/**
		\brief	incase the uvanimation is added as component by script instead of added in inspector of unity, this should be invoke.
	*/
	public void InitData(){
		if (animObject != null){
			Renderer animRenderer = animObject.GetComponent<Renderer>() as Renderer;
			if (animRenderer == null){
				Debug.LogError (gameObject.name + " no renderer found");
			} else {
				animMaterial = animRenderer.material;
			}
		} else{
			Debug.LogWarning (gameObject.name + " missing assignment on animObject");
		}
	}
	
	protected int GetCurrentPointInteger (){
		return currentFrame;
	}
	
	protected float GetDistanceInteger(int source, int target){
		return (target-source)*1.0f;
	}
	
	protected int CalculateMoveValueInteger ( float moveTime, EaseType ease){
		int newValue = -1;
		switch (animationPath){
			default:
			case AnimationPath.Linear:
				currentFrame = (int)Mathf.Lerp(startPoint, endPoint,  Ease.GetTime( moveTime/duration , ease));
				if (animFrameSequence.Length == 0){// go with sequential order
					switch (textureType){
						case TextureType.SpriteSheet:	
							currentFrame = currentFrame % (spriteWidth * spriteHeight);
							break;
						case TextureType.SequenceImage:
							currentFrame = currentFrame % animFrameTexture.Length;
							break;
					}
				
					newValue = currentFrame;
				} else { // go with special order
					currentFrame = currentFrame % animFrameSequence.Length;
					newValue = animFrameSequence[currentFrame];
				}
				break;
		}
		return newValue;
	}
	
	protected void ApplyMoveValueInteger (int newValue){
		SetFrame (newValue);
	}
	
	protected void ApplyEndValueInteger (int endValue){
		if (animFrameSequence.Length != 0){// go with special order
			
			if ((endValue-1) >= animFrameSequence.Length){
				Debug.LogError (gameObject.name + "  Index out of order, endValue: " + endValue + " animFrameSequenceLength: " + animFrameSequence.Length);
			} else {
				endValue = animFrameSequence[endValue-1];
			}
		}
		ApplyMoveValueInteger (endValue);
	}
	
	protected int AccumulateEndValueInteger  (int endPoint_, int startPoint_){
		int diff = endPoint_ - startPoint_;
		return (endPoint_ + diff);
	}
	
	
	public void SetFrame (int newValue){
		
		if (animMaterial == null){
			return;	
		}
		
		//currentFrame = newValue;
		switch (textureType){
			case TextureType.SpriteSheet:
				
			
				int newFrameX = newValue % spriteWidth;
				int newFrameY = newValue / spriteWidth;
				Vector2 size = new Vector2 (1.0f / spriteWidth, 1.0f / spriteHeight);
			
				float newFrameU = newFrameX * size.x;
				float newFrameV = 1.0f - size.y - newFrameY * size.y;
				
				//if (gameObject.name.StartsWith("En005A_BaseNumber"))
				//	DebugText.Log ("SetUVFrame - frame" + newValue+ "newFrameU: " + newFrameU + "\tnewFrameV: " + newFrameV);
				
				animMaterial.SetTextureOffset ("_MainTex", new Vector2(newFrameU, newFrameV));
			
				if (!setOffsetOnly)
					animMaterial.SetTextureScale ("_MainTex", size);
				break;
			case TextureType.SequenceImage:
				if (newValue >= animFrameTexture.Length){
					Debug.LogError(gameObject.name + "animFrameTexture out of range, newValue: " + newValue + " animFrameTextureLength: " + animFrameTexture.Length);
				} else {
					animMaterial.mainTexture = animFrameTexture[newValue];
				}
				break;
		}
	}
}