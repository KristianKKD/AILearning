using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

    public float x,y = 0;

    [SerializeField]
    float aiInputX, aiInputY = 0;

    public float acceleration = .5f;
    public float friction = .5f;
    public float maxSpeed = 10f;
    public float axisMaxSpeed = 5;

    Canvas background;

    private void Awake() {
        background = FindObjectOfType<Canvas>();
    }

    private void Update() {
        //float tempX = Input.GetAxisRaw("Horizontal");
        //float tempY = Input.GetAxisRaw("Vertical");

        //AIInput(tempX, tempY);

        x = AxisDamping(x);
        y = AxisDamping(y);
    }

    float AxisDamping(float dir) {
        if (Mathf.Abs(x) < Time.deltaTime * acceleration)
            dir = 0;
        else if (Mathf.Abs(dir) > acceleration * Time.deltaTime)
            dir += (dir > 0) ? -Time.deltaTime * friction : Time.deltaTime * friction;

        return dir;
    }

    public void AIInput(float aiX, float aiY) {
        aiInputX = aiX;
        aiInputY = aiY;

        x = UpdateDir(x, aiX);
        y = UpdateDir(y, aiY);
    }

    float UpdateDir(float original, float dir) {
        original += dir * acceleration * Time.deltaTime;

        return Mathf.Clamp(original, -axisMaxSpeed, axisMaxSpeed);
    }

    public void ResetVelocity() {
        x = 0f;
        y = 0f;
    }

    private void FixedUpdate() {
        transform.position += Vector3.ClampMagnitude(new Vector3(x, y, 0), maxSpeed);

        if (transform.position.x > background.pixelRect.width ||
               transform.position.y > background.pixelRect.height ||
               transform.position.x < 0 || transform.position.y < 0)
            GetComponent<AIInterface>().Death(true);

    }

}
