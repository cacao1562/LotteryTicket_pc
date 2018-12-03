using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Select : MonoBehaviour {



	public void clickImageMode() {

		SceneManager.LoadScene("imgMode");
	}

	public void clickTextMode() {

		SceneManager.LoadScene("textMode");
	}
}
