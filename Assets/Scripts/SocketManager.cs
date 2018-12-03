using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SocketManager : MonoBehaviour
{
	private SocketIOComponent socket;
	private string roomNumber = "ticket";  //방 번호
	private JSONObject mobj;
	private bool check = false;
	private Drawable drawable;
	public CameraResolution cameraResolution;
	public CameraResolution2 cameraResolution2;
	public bool fading = false;
	public string sceneName;
	void Awake()
	{
		drawable = GameObject.Find("draw").GetComponent<Drawable>(); //로고 이미지 이름 draw로 고정
	}

	public void Start() 
	{
		sceneName = SceneManager.GetActiveScene().name;
		// Screen.sleepTimeout = SleepTimeout.NeverSleep;
		// Screen.SetResolution(Screen.width, Screen.width / 9 * 16 , true);
		
        Debug.Log("roomNum = "+roomNumber);
		socket = GameObject.Find ("SocketIO").GetComponent<SocketIOComponent> ();
		socket.On ("open", OnOpen);
		socket.On ("drawing", OnGetValue);
		socket.On ("error", OnError);
		socket.On ("close", OnClose);
		
		
	}

	public void startFadeOut() {

		JsonModel jm = new JsonModel();
		jm.sendStr = "startFade";
		JSONObject jo = new JSONObject(JsonUtility.ToJson(jm) );
		socket.Emit("send", jo );
	}

	public void endFadeOut() {

		JsonModel jm = new JsonModel();
		jm.sendStr = "endFade";
		JSONObject jo = new JSONObject(JsonUtility.ToJson(jm) );
		socket.Emit("send", jo );
	}

		public void soldOut() {

		JsonModel jm = new JsonModel();
		jm.sendStr = "soldOut";
		JSONObject jo = new JSONObject(JsonUtility.ToJson(jm) );
		socket.Emit("send", jo );
	}



	void Update()
	{


		if(!socket.IsConnected){

			Debug.Log("소켓 연결 안됨");
			return;
		}

		if(mobj == null){
			return;
		}

		if(mobj.Count <= 0){
			return;
		}	
   		
		

		if (check) {

			JsonModel jm = JsonUtility.FromJson<JsonModel>(mobj.ToString());


			if (jm.sendStr == "drawing") {
				
				string v2 = jm.result.Substring(1, jm.result.Length -2 );
				string[] vv = v2.Split(',');
				// Debug.Log("v0 = " + vv[0] + " v1 = " + vv[1] );
				drawable.v2 = new Vector2( float.Parse(vv[0]) , float.Parse(vv[1]) ); //앱에서 받은 좌표 넘겨줌
				drawable.receiveSocket = true;
				check = false;
				
			}else if (jm.sendStr == "reset") {

				drawable.receiveSocket = false;
				if (fading) { //fade out중 일때는 리셋 안되게

					check = false;
					return;

				}else {

					if (sceneName == "imgMode") {

						cameraResolution.setItem();
						
					}else if (sceneName == "textMode") {

						cameraResolution2.setItem();
					}
					
					// textAnimation.SetActive(true);
					drawable.ResetCanvas();
					check = false;
				}
			
			}
			
		}else{

		
		}

		
	}

	public void OnOpen(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Open(): " + e.data);
	
		socket.Emit("joinRoom", JSONObject.StringObject(roomNumber)); //방번호
		// sendConnect();	
	}
	
	public void OnGetValue(SocketIOEvent e)
	{
		// Debug.Log("get_Value: " + e.data);
		
		if (e.data == null) {
			// Debug.Log(" data nulllllll ");
			return; 
		}
		mobj = e.data; 
		check = true;
		
	}
	
	public void OnError(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Error(): " + e.data);
	}
	
	public void OnClose(SocketIOEvent e)
	{	
		Debug.Log("[SocketIO] Close(): " + e.data);
	}


}

[System.Serializable]
public class JsonModel{
	public string sendStr;
	public string result;
}


