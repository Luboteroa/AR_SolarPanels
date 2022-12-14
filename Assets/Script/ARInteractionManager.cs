using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;

public class ARInteractionManager : MonoBehaviour
{
    [SerializeField] private Camera arCamera;
    [SerializeField] private float scaleFactor = 2f;
    [SerializeField] private float scaleTolerance = 2f;
    private GameObject UI_Description;
    private ARRaycastManager aRRaycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private GameObject aRPointer;
    private GameObject item3DModel;
    private GameObject itemSelected;
    private Animator animator;

    private bool isInitialPosition;
    private bool isOverUI;
    private bool isOver3DModel;
    private Vector2 initialTouchPos;
    private float touchDis;

    public GameObject Item3DModel
    {
        set
        {
            item3DModel = value;
            animator = item3DModel.GetComponentInChildren<Animator>();
            item3DModel.transform.position = aRPointer.transform.position;
            item3DModel.transform.parent = aRPointer.transform; 
            isInitialPosition = true;
        }
    }

    private void Start()
    {
        aRPointer = transform.GetChild(0).gameObject;
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
        GameManager.instance.OnMainMenu += SetItemPosition;
    }

    private void Update()
    {
        if(isInitialPosition)
        {
            Vector2 middlePointScreen = new Vector2(Screen.width/2, Screen.height/2);
            aRRaycastManager.Raycast(middlePointScreen, hits, TrackableType.Planes);
            if(hits.Count>0)
            {
                transform.position = hits[1].pose.position;
                transform.rotation = hits[1].pose.rotation;
                aRPointer.SetActive(true);
                isInitialPosition = false;
            }
        }

        if(Input.touchCount>0)
        {
            Touch touchOne = Input.GetTouch(0);
            if(touchOne.phase == TouchPhase.Began)
            {
                var touchPosition = touchOne.position;
                isOverUI = IsTapOverUI(touchPosition);
                isOver3DModel = IsTapOver3DModel(touchPosition);
                if(animator == null) {return;}
                animator.SetTrigger("Trigger");
            }

            if(touchOne.phase == TouchPhase.Moved)
            {
                if(aRRaycastManager.Raycast(touchOne.position, hits, TrackableType.Planes))
                {
                    Pose hitPose = hits[0].pose;
                    if(!isOverUI && isOver3DModel) 
                    {
                        transform.position = hitPose.position;
                    }
                }
            }

            if(Input.touchCount == 2)
            {
                Touch touchTwo = Input.GetTouch(1);
                if(touchOne.phase == TouchPhase.Began || touchTwo.phase == TouchPhase.Began)
                {
                    initialTouchPos = touchTwo.position - touchOne.position;
                    touchDis = Vector2.Distance(touchTwo.position, touchOne.position);
                }

                if(touchOne.phase == TouchPhase.Moved || touchTwo.phase == TouchPhase.Moved)
                {
                    Vector2 currentTouchPos = touchTwo.position - touchOne.position;
                    float currentTouchDis = Vector2.Distance(touchTwo.position, touchOne.position);

                    float diffDis = currentTouchDis - touchDis;

                    //Rotation
                    float angle = Vector2.SignedAngle(initialTouchPos, currentTouchPos);
                    item3DModel.transform.rotation = Quaternion.Euler(0, item3DModel.transform.eulerAngles.y - angle, 0);

                    //Scale
                    Vector3 newScale = item3DModel.transform.localScale + Mathf.Sign(diffDis) * Vector3.one * scaleFactor;
                    item3DModel.transform.localScale = Vector3.Lerp(item3DModel.transform.localScale, newScale, 1f);

                    touchDis = currentTouchDis;
                    initialTouchPos = currentTouchPos; 
                }
            }

            if(isOver3DModel && item3DModel == null && !isOverUI)
            {
                GameManager.instance.ARPosition();
                item3DModel = itemSelected;
                animator = item3DModel.GetComponentInChildren<Animator>();
                itemSelected = null;
                aRPointer.SetActive(true);
                transform.position = item3DModel.transform.position;
                item3DModel.transform.parent = aRPointer.transform;
                if(animator == null) {return;}
                animator.SetTrigger("Trigger");
            }
        }
    }

    private bool IsTapOverUI(Vector2 touchPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(touchPosition.x, touchPosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count>0;
    }

    private bool IsTapOver3DModel(Vector2 touchPosition)
    {
        Ray ray = arCamera.ScreenPointToRay(touchPosition);
        if(Physics.Raycast(ray, out RaycastHit hit3DModel))
        {
            if(hit3DModel.collider.CompareTag("Item"))
            {
                itemSelected = hit3DModel.transform.gameObject;
                return true;
            }
        }
        return false;
    }

    private void SetItemPosition()
    {
        if(item3DModel != null)
        {
            animator.SetTrigger("Trigger");
            animator = null;
            item3DModel.transform.parent = null;
            aRPointer.SetActive(false);
            item3DModel = null;
        }
    }

    public void DeleteItem()
    {
        Destroy(item3DModel);
        aRPointer.SetActive(false);
        GameManager.instance.MainMenu();
    }
}
