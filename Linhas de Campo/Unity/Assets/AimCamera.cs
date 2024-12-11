using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AimCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 targetOffset;
    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = .6f;
    public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;
    public int yMinLimit = 0;
    public int yMaxLimit = 80;
    public int zoomRate = 40;
    public float panSpeed = 0.3f;
    public float zoomDampening = 5.0f;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;


    private bool clicking = false;

    public float rayCastDistance = 1000f;
   
   
    void Start() { Init(); }
    void OnEnable() { Init(); }
    public void Init()
    {
        if (!target)
        {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * distance);
            target = go.transform;
        }

        distance = Vector3.Distance(transform.position, target.position);
        currentDistance = distance;
        desiredDistance = distance;

        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = Vector3.Angle(Vector3.up, transform.up);

    }

    void Update () {
        Time.timeScale = 5.0f;
    }

    void LateUpdate()
    {
		if(Input.GetKey(KeyCode.LeftShift))
		{
            if (Input.GetMouseButton(2) )
            {
            desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate * 0.125f * Mathf.Abs(desiredDistance);
        }

        if (Input.GetMouseButtonUp(0))
        {
            clicking = false;
        }
       
       
        if (Input.GetMouseButton(0))
        {
           
            if (clicking == false)
            {
                clicking = true;

                //Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                RaycastHit hit;
                if (Physics.Raycast (ray, out hit, rayCastDistance))
                {
                    //draw invisible ray cast/vector
                    Debug.DrawLine (ray.origin, hit.point);
                    //log hit area to the console
                    //Debug.Log(hit.point);
                    target.transform.position = hit.point;
                } 
     
               
              //  Debug.Log("cliik");


            }
           
 
               xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
               yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

               ////////OrbitAngle

               //Clamp the vertical axis for the orbit
               yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
               // set camera rotation
               desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
               currentRotation = transform.rotation;

               rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
               transform.rotation = rotation;
                    
        }

        // otherwise if right mouse is selected, we pan by way of transforming the target in screenspace
        else if (Input.GetMouseButton(1))
        {
            //grab the rotation of the camera so we can move in a psuedo local XY space
            target.rotation = transform.rotation;
            target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
            target.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);
        }
        
        ////////Orbit Position

        // affect the desired Zoom distance if we roll the scrollwheel
        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        //clamp the zoom min/max
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        // For smoothing of the zoom, lerp distance
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

        // calculate position based on the new currentDistance
        position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
        transform.position = position;
    }
	}

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}