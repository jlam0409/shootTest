using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	private Callback mWinGameCallback;
	private Callback mLoseGameCallback;

	public DebrisEmitter debrisEmitter;
	public BulletEmitter bulletEmitter;

	public List<DebrisController> debrisList = new List<DebrisController>();
	public List<BulletController> bulletList = new List<BulletController>();

	private int debrisGeneratedCounter = 0;
	public TextMesh bulletText;

	private static string DEBRIS_GENERATED_KEY = "DebrisGenerated";
	private static string DEBRIS_COUNT_KEY = "DebrisCount";
	private static string DEBRIS_DATA_KEY = "DebrisData";
	private static string BULLET_COUNT_KEY = "BulletCount";
	private static string BULLET_DATA_KEY = "BulletData";
	
	public void Init(Callback winGameCallback, Callback loseGameCallback){
		mWinGameCallback = winGameCallback;
		mLoseGameCallback = loseGameCallback;

		debrisList = new List<DebrisController>();
		debrisGeneratedCounter = 0;
		bulletText.text = PlayerData.bullet.ToString();
	}

	public void StartGenerate(){
		StartCoroutine ( "StartGenerateDebris" );
	}

	public void StopGenerate(){
		StopCoroutine( "StartGenerateDebris" );
	}

	public IEnumerator StartGenerateDebris(){
		for (int i=debrisGeneratedCounter; i<GameConfigData.instance.debrisCount; i++){
			CreateDebris( i == (GameConfigData.instance.debrisCount-1) );
			debrisGeneratedCounter ++;
			yield return new WaitForSeconds (GameConfigData.instance.debrisInterval);
		}
	}

	public void PauseObjectMovement(){
		foreach (DebrisController eachController in debrisList){
			eachController.Stop();
		}
		foreach (BulletController eachController in bulletList){
			eachController.Stop();
		}
	}

	public void ResumeObjectMovement(){
		foreach (DebrisController eachController in debrisList){
			eachController.Move();
		}
		foreach (BulletController eachController in bulletList){
			eachController.Move();
		}
	}

	public void CreateDebris(bool isBoss){
		DebrisController debrisController = debrisEmitter.EmitDebris(isBoss);
		SetDebrisCallback(debrisController);
		debrisList.Add (debrisController);
	}

	public void SetDebrisCallback(DebrisController debrisController){
		debrisController.SetDebrisCallback (
			(data) => { HitDebris(data);}, 
			(bulletData) => { DestroyBullet(bulletData.bulletController);},
			(controller)=> {MissDebris (controller);} ); 
	}

	public void HitDebris(CollisionData collisionData){

		// add score and bullet
		PlayerData.score += collisionData.debrisController.GetReward();
		UpdateBulletCount(collisionData.debrisController.GetReward());

		// the player win the game if defected the boss
		if (collisionData.debrisController.GetDebrisType() == DebrisType.BOSS){
			mWinGameCallback.SendCallback(this);
		}

		DestroyDebris(collisionData.debrisController);
		DestroyBullet(collisionData.bulletController);

	}

	public void MissDebris(DebrisController controller){
		// the player lose the game if missed the boss
		if (controller.GetDebrisType() == DebrisType.BOSS){
			mLoseGameCallback.SendCallback(this);
		}
		DestroyDebris (controller);
	}

	public void DestroyDebris(DebrisController controller){
		if (debrisList.Contains(controller) ){
			debrisList.Remove (controller);
			Destroy (controller.gameObject);
		}
	}

	public void DestroyAllObject(){
		foreach (DebrisController eachController in debrisList){
			Destroy (eachController.gameObject);
		}
		debrisList.Clear();
		foreach (BulletController eachController in bulletList){
			Destroy (eachController.gameObject);
		}
		bulletList.Clear();
	}

	public void ShootBullet(float screenAxis){
		if (PlayerData.bullet > 0){
			UpdateBulletCount (-1);

			BulletController bulletController = bulletEmitter.EmitBullet(screenAxis);
			SetBulletCallback (bulletController);

			bulletList.Add (bulletController);
		}
	}

	public void SetBulletCallback(BulletController bulletController){
		bulletController.SetBulletCallback((controller)=> {MissBullet (controller);} );
	}

	public void MissBullet(BulletController controller){
		DestroyBullet (controller);
	}

	public void DestroyBullet(BulletController controller){
		if (bulletList.Contains (controller) ){
			bulletList.Remove(controller);
			Destroy (controller.gameObject);
		}

		// player will lose the game if the bullet list is zero and bullet count is 0
		if (bulletList.Count == 0 && PlayerData.bullet == 0){
			mLoseGameCallback.SendCallback(this);
		}
	}

	public void UpdateBulletCount(int count){
		PlayerData.bullet += count;
		bulletText.text = PlayerData.bullet.ToString();
	}



	public void SaveStateData(){
		PlayerPrefs.SetInt (DEBRIS_COUNT_KEY, debrisList.Count);
		for (int i=0; i<debrisList.Count; i++){
			PlayerPrefs.SetString((DEBRIS_DATA_KEY + i), SerializeDebrisData(debrisList[i]));
		}
		PlayerPrefs.SetInt (BULLET_COUNT_KEY, bulletList.Count);
		for (int i=0; i<bulletList.Count; i++){
			PlayerPrefs.SetString ((BULLET_DATA_KEY + i), SerializeBulletData(bulletList[i]));
		}

		PlayerPrefs.SetInt (DEBRIS_GENERATED_KEY, debrisGeneratedCounter);
	}
	
	public void LoadStateData(){
		int debrisCount = PlayerPrefs.GetInt(DEBRIS_COUNT_KEY);
		for (int i=0; i<debrisCount; i++){
			string debrisData = PlayerPrefs.GetString (DEBRIS_DATA_KEY + i);
			DeserializeDebrisData (debrisData);
			PlayerPrefs.DeleteKey(DEBRIS_DATA_KEY + i);
		}
		PlayerPrefs.DeleteKey(DEBRIS_COUNT_KEY);

		int bulletCount = PlayerPrefs.GetInt(BULLET_COUNT_KEY);
		for (int i=0; i<bulletCount; i++){
			string bulletData = PlayerPrefs.GetString (BULLET_DATA_KEY + i);
			DeserializeBulletData (bulletData);
			PlayerPrefs.DeleteKey(BULLET_DATA_KEY + i);
		}
		PlayerPrefs.DeleteKey(BULLET_COUNT_KEY);

		debrisGeneratedCounter = PlayerPrefs.GetInt (DEBRIS_GENERATED_KEY);
		PlayerPrefs.DeleteKey(DEBRIS_GENERATED_KEY);
	}
	
	public string SerializeDebrisData(DebrisController debrisController){
		string data = debrisController.transform.localPosition.x + "$" + debrisController.transform.localPosition.y + "$" + debrisController.transform.localPosition.z + ",";
		data += (int)debrisController.GetDebrisType() + ",";
		data += debrisController.GetReward() + ",";
		data += debrisController.GetSpeed() + ",";
		data += debrisController.GetHP();

		return data;			
	}
	
	public DebrisController DeserializeDebrisData(string debrisData){
		string[] debrisDataTok = debrisData.Split(',');
		string[] startData = debrisDataTok[0].Split('$');

		Vector3 startPosition = new Vector3 ( float.Parse(startData[0]), float.Parse(startData[1]), float.Parse(startData[2]));
		int debrisType = int.Parse(debrisDataTok[1]);
		int debrisReward = int.Parse(debrisDataTok[2]);
		float speed = float.Parse(debrisDataTok[3]);
		int hp = int.Parse (debrisDataTok[4]);
		DebrisController debrisController = debrisEmitter.EmitDebris(debrisType, debrisReward, startPosition, speed, hp);
		SetDebrisCallback (debrisController);
		debrisList.Add (debrisController);

		return debrisController;
	}

	public string SerializeBulletData(BulletController bulletController){
		string data = bulletController.transform.localPosition.x + "$" + bulletController.transform.localPosition.y + "$" + bulletController.transform.localPosition.z + ",";
		return data;			
	}
	
	public BulletController DeserializeBulletData(string bulletData){
		string[] bulletDataTok = bulletData.Split(',');
		string[] startData = bulletDataTok[0].Split('$');
		
		Vector3 startPosition = new Vector3 ( float.Parse(startData[0]), float.Parse(startData[1]), float.Parse(startData[2]));

		BulletController bulletController = bulletEmitter.EmitBullet(startPosition);
		SetBulletCallback (bulletController);
		bulletList.Add (bulletController);
		
		return bulletController;
	}
}
