using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class ImageSelect : MonoBehaviour {

	public Sprite selected;
	public Sprite unselected;

	private SpriteRenderer img;
	private bool isSelected = false;

	// Use this for initialization
	void Start () {
		img = GetComponent<SpriteRenderer> ();
	}
	
	public void toggle(){
		isSelected = !isSelected;
		img.sprite = (isSelected ? selected : unselected);
	}

	public bool setSelect(bool s){
		bool temp = isSelected;
		isSelected = s;
		img.sprite = (isSelected ? selected : unselected);
		return temp;
	}
}
