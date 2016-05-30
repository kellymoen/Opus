using UnityEngine;

public class FloatingScript : MonoBehaviour
{
  private Rigidbody rBody;

  public float height = 1f;

  void Start(){
    rBody = GetComponent<Rigidbody> ();
  }

  void Update(){
    //Make the object hover above the ground
    Ray ray = new Ray (transform.position, -transform.up);
    RaycastHit downCast;

    if (Physics.Raycast (ray, out downCast, height)) {
      if (downCast.distance < height) {
        rBody.useGravity = false;
        rBody.AddForce(0, (height - downCast.distance)/downCast.distance, 0);
      }
    } else {
      rBody.useGravity = true;
    }
  }

}
