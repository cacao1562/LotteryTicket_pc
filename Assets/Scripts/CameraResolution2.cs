using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CameraResolution2 : MonoBehaviour {


    public Camera pixelCam; 
    public float heightV; 
    public float widthV;
    public float x, y; // 카메라의 x,y만큼 떨어진 좌표이다.
    public float Size; // 카메라의 사이즈이다.

	// public List<string> itemArr = new List<string>(); //각 개수만큼 인덱스 번호 저장됨
	public List<string> randomArr = new List<string>(); //랜덤 섞은 리스트
	public List<string> itemTextList = new List<string>(); //상품 이름
	// public List<Sprite> spriteList = new List<Sprite>(); //이미지만 
	// public TextMesh textMesh;
	// private TextMesh shadowText;
	private int itemCount = 0;  
	public GameObject lastText;
	// public GameObject soldOutimg;
	public Slider sliderW, sliderH;
	public Text textW, textH;
	public InputField inputW, inputH;
	public SpriteRenderer spRenederer; //뒤에 보이는 아이템 이미지
	// public GameObject plan;
	private string[] sizeValue;
	private string[] itemValue;
	private char st = '=';
	/// 글라스 정보 
	private	string path = @"c:\info\info.ini";
	/// 경품 이미지 이름과 개수
	private	string path2 = @"c:\info\text_item.txt";
	private string path4 = @"c:\info\Logo\Logo.png";
	public GameObject drawObj;
	public GameObject errorPopup;
	public Text errorText; 
	private List<string> errorImgName = new List<string>();
	private string errorMessage;
	private bool errorCheck = false;
	public GameObject blackPlane;
	// public Text debugText;
	public SocketManager socketManager;
	public TextMesh itemText;
	public Drawable drawable;


    void Awake () {

	
		try {

			sizeValue = System.IO.File.ReadAllLines(path);

		}catch {
			
			errorPopup.SetActive(true);
			errorText.text = "c:/info/info.ini 파일이 없습니다.";
			return;
		}

		try {

			itemValue = System.IO.File.ReadAllLines(path2);

		}catch {
			
			errorPopup.SetActive(true);
			errorText.text = "c:/info/text_item.txt 파일이 없습니다";
			return;
		}

		try {

			byte[] b = System.IO.File.ReadAllBytes(path4);
			Texture2D texture2 = new Texture2D(0,0);
			texture2.LoadImage(b);
			drawable.logoTexture = texture2;
			// spriteList.Add( Sprite.Create(texture2, new Rect(0,0,texture2.width,texture2.height), new Vector2(0.5f, 0.5f)) );	

		}catch {

			errorPopup.SetActive(true);
			errorText.text = "c:/info/Logo/Logo.png 파일이 없습니다";
			
			return;
		}

	    // string[] childs = System.IO.Directory.GetFiles(@"c:\info\img\"); //하위 파일들 목록 가져옴
		// foreach ( string s in childs) {
		// 	Debug.Log(" file = "  + s) ;
		// }

		getSize();

		getItem();

		// getImg();
	
		if (errorCheck) {

			foreach (string st in errorImgName) {

				errorMessage += st + " 찾을 수 없음 " + "\n" ;
			}
			
			errorPopup.SetActive(true);
			errorText.text = errorMessage;	
			// GetComponent<CameraResolution>().enabled = false;
		
		}else {
			
			shakeArray();
		
		
			// shadowText = textMesh.transform.GetChild(0).GetComponent<TextMesh>();
			// textMesh.text = tarr[itemCount];
			// shadowText.text = tarr[itemCount];

			itemText.text = randomArr[itemCount];
			// spRenederer.sprite = spriteList[int.Parse(randomArr[itemCount])]; //처음 0번째 이미지 배치
			// float s = imageScaleChange();
			// spRenederer.transform.localScale = new Vector3(s, s, s);
			itemCount++;

			Screen.SetResolution(1024, 768, true);

			// float scaleFactor; // 캔버스 스케일
			// if(widthV > heightV) { //가로가 더 클때
			// 	scaleFactor = heightV / 768;
			// }else {	
			// 	scaleFactor = widthV / 1024;
			// }
			// Debug.Log("Canvas scale = " + scaleFactor );

			changTempCamVal();
			// Cursor.visible = false;
		}		
		
		
    }


	/// info.ini 파일 한줄씩 읽어서 값 저장
	private void getSize() {

		for (int i =0; i < sizeValue.Length; i++){
			
			if (sizeValue[i] == "") {
				continue;
			}

			string[] glassinfo = sizeValue[i].Split(st);

			if (glassinfo != null) {

				if(glassinfo[0] == "offsetX"){

					x = float.Parse(glassinfo[1]);

				}else if(glassinfo[0] == "offsetY"){

					y = float.Parse(glassinfo[1]);

				}else if(glassinfo[0] == "glassWidth"){

					widthV = float.Parse(glassinfo[1]) ;
					sliderW.value = (int)widthV;
					inputW.text = ((int)widthV).ToString();
					textW.text = "W = " + sliderW.value.ToString();

				}else if(glassinfo[0] == "glassHeight"){

					heightV = float.Parse(glassinfo[1]) ;
					sliderH.value = (int)heightV;
					inputH.text = ((int)heightV).ToString();
					textH.text = "H = " + sliderH.value.ToString();
				} 
			}
			
		}
	}

	/// item.txt 파일 한줄씩 읽어서 값 저장
	private void getItem() {

		//item.txt 파일 내용 읽는 부분
		for (int i = 0; i < itemValue.Length; i++){
			
			if (itemValue[i] == "") {
				continue;
			}

			string[] glassinfo = itemValue[i].Split(st);

			if (glassinfo != null) {

				for (int j=0; j < int.Parse(glassinfo[1]); j++ ) { //각각 적어논 개수만큼 돌고 인덱스 번호로 저장

					itemTextList.Add(glassinfo[0]); //이미지 이름만 저장 path 가져올때 사용하려고
					// itemArr.Add(i.ToString()); //아이템 이름대신 인덱스로 저장
				}
			}
			
		}

		
		//텍스처를 읽어서 스프라이트로 생성 info폴더 아래 img 폴더 아래 있는 이미지들 
	
	}

	// void getImg() {
	// 		for (int i = 0; i < itemTextList.Count; i++) {
			
			
	// 		try {

	// 			byte[] b = System.IO.File.ReadAllBytes(path3 + itemTextList[i]);
	// 			Texture2D texture2 = new Texture2D(0,0);
	// 			texture2.LoadImage(b);
	// 			spriteList.Add( Sprite.Create(texture2, new Rect(0,0,texture2.width,texture2.height), new Vector2(0.5f, 0.5f)) );
				

	// 		}catch {

	// 			debugText.text = "erro";
	// 			errorCheck = true;
	// 			// Debug.Log("EEEEEEE");
	// 			errorImgName.Add(itemTextList[i]);
				
				
	// 		}

	// 	}
	// }



	/// itemArr에서 랜덤으로 하나씩 가져와서 radomArr에 저장
	private void shakeArray() {

		bool[] barr = new bool[itemTextList.Count];

		for (int i=0; i<barr.Length; i++) {

			barr[i] = true;
		}
		int w = 0;
		int r;

		while (w < barr.Length) { //랜덤섞기

			r = Random.Range(0, barr.Length);

			if(barr[r]) {

				barr[r] = false;
				randomArr.Add(itemTextList[r]);
				w++; 
			}
		}
	}

	
	/// camera rect 생성하고 배치
    private void changTempCamVal()
    {
		sliderW.value = (int)widthV;
		sliderH.value = (int)heightV;
		inputW.text = ((int)widthV).ToString();
		inputH.text = ((int)heightV).ToString();

		pixelCam.rect = new Rect(0, ((768f - heightV) / 768f), widthV / 1024f, heightV / 768f);
		
		// float rate = (1024f / widthV) * (heightV / 768f);
		// float rate2 = (widthV / 1024f) * (768f / heightV);
		// Debug.Log("rate = " + rate + " rate2 = " + rate2);
		// pixelCam[i].orthographicSize = 70f * rate;
		float r = widthV / heightV;

		if (1.0f < r && r < 1.34f) { //정사각형 비율에 가까울때

			pixelCam.orthographicSize = 5f;
			pixelCam.transform.position = new Vector3(x, y, pixelCam.transform.position.z);

			float radius = 5f * widthV / heightV; //카메라에 보이는 가로 반지름
			float s = radius * 2 / 10f; // 카메라 가로만큼 꽉 차는 스케일값
			blackPlane.transform.localScale = new Vector3(s,s,s);
			return;
		}

		pixelCam.orthographicSize = 2.5f; // 768 / 100 / 2 = 3.84
		pixelCam.transform.position = new Vector3(x, y, pixelCam.transform.position.z);

		float radius1 = 5f * widthV / heightV;
		float s1 = radius1 * 2 / 10f;
		blackPlane.transform.localScale = new Vector3(s1,s1,s1);

		// if(Size != 0)
		// {

		// 	pixelCam.orthographicSize = Size;

		// }

    }

	private bool soldout = true; //아이템 총 개수 만큼 돌아가게

	///앱에서 reset 버튼 눌렀을때 이미지 교체
	public void setItem() {

		if (soldout) {

			itemText.text = randomArr[itemCount];
			// spRenederer.sprite = spriteList[int.Parse(randomArr[itemCount])];
			// float s = imageScaleChange();
			// spRenederer.transform.localScale = new Vector3(s, s, s);
			itemCount++;
			// textMesh.text = tarr[itemCount];
			// shadowText.text = tarr[itemCount];
			// itemCount++;

			if (itemCount == randomArr.Count) {

				itemCount = 0;
				soldout = false;
				// lastText.SetActive(true);
				
			}

		}else {

			// spRenederer.gameObject.SetActive(false);
			drawObj.SetActive(false);
			socketManager.soldOut();
			// soldOutimg.SetActive(true);
			// lastText.SetActive(false);
			itemText.text = "SOLD OUT";
			// textMesh.text = "Sold Out";
			// shadowText.text = "Sold Out";
		}

	}

	///이미지 스케일 조정
	private float imageScaleChange() {

		float sw = spRenederer.sprite.rect.width;
		float sh = spRenederer.sprite.rect.height;
		float a = sw / sh;
		float sum = sw + sh;
		
		if (1.0f <= a && a <= 1.4f ) { // 이미지가 정사각형 비율에 가까울때
			// if (sum > 1792) {
			// 	return (1792 / sum) - 1;
			// }else {
			// 	return (1792 - sum) / 1000f + 1f; 
			// }
			// return ((1024f / sw) + (768f / sh)) / 2 ;
			return 768f / sh;
		}
		if (sw < sh) {

			return 500f / sh;

		}else {

			return 1024f / sw;
		}

		// if (sum > 1792) {

		// 	float x = sum / 1792;
		// 	return 1 / x; 

		// }else {

		// 	return 1792 / sum;
		// }

	}

	/// 슬라이더 가로픽셀
	public void sliderValueChanged_W(){

		int w = (int)sliderW.value;
		textW.text = "W = " + w.ToString();
		widthV = w;
		changTempCamVal();
	}

	/// 슬라이더 세로픽셀
	public void sliderValueChanged_H(){

		int h = (int)sliderH.value;
		textH.text = "H = " + h.ToString();
		heightV = h;
		changTempCamVal();
	}

	public void saveButton() {

		int w = int.Parse(inputW.text);
		int h = int.Parse(inputH.text);
		widthV = w;
		heightV = h;
		changTempCamVal();
	}


	// void test(){ // 누적확률
	// 	float dr = Random.Range(0.0f, 1.0f) * 100.0f;
	// 	// float dr = rr * 100.0f;
	// 	// Debug.Log("dr = " + dr + " %");
	// 	float[] p = {5.0f, 15.0f, 15.0f, 30.0f, 35.0f};
	// 	string[] pp = {"티비", "자전거", "휴지","사탕","꽝"};
	// 	float sum = 0.0f;

	// 	for (int i=0; i<p.Length; i++) {

	// 		sum += p[i];

	// 		if ( dr <= sum) {

	// 			// Debug.Log("result = " + pp[i]);
	// 			break;
	// 		}
	// 	}
	// }



}
