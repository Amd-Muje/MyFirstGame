using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent {

    public static Player Instance { get; private set; }


    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;   
    [SerializeField] private Transform KitchenObjectHoldPoint;


    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Awake(){
        if(Instance != null){  
            Debug.LogError("ADA ERROR LEBIH DARI 1 PLAYER");
        }  

        Instance = this;
    } 
    private void Start () {
        gameInput.OnInteractAction += GameInput_OnInteractAction; 
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }
    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e) {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;
        
        if(selectedCounter != null){
            selectedCounter.InteractAlternate(this);
        }
    }
    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null) {
            selectedCounter.Interact(this);  
        }
    }
    //private = accessor
    private void Update() {
        HandleMovement();
        HandleInteractions();
    }   

    private void HandleInteractions(){
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        
        Vector3 moveDir = new Vector3(inputVector.x , 0f , inputVector.y);

        if (moveDir !=  Vector3.zero) {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir,out RaycastHit raycastHit, interactDistance, countersLayerMask)){
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)){
                //Has ClearCounter
                if (baseCounter != selectedCounter) {
                    SetSelectedCounter(baseCounter);

                }
            } else {
                SetSelectedCounter(null);

            }
        } else {
            SetSelectedCounter(null);

        }
    }
    public bool IsWalking(){
        return isWalking;
    }

    private void HandleMovement () {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        
        Vector3 moveDir = new Vector3(inputVector.x , 0f , inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove =! Physics.CapsuleCast(transform.position,transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);
        
    if(!canMove){

        //Tidak bisa jalan Ke moveDir
        //Coba jalan ke X

        Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
        canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position,transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

        if (canMove){
            // bisa gerak ke x saja
            moveDir = moveDirX;
        } else {
            // tidak bisa jalan hanya ke X

            // Coba jalan ke Z
            Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
            canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position,transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove){
                // bisa gerak ke z saja
                moveDir = moveDirZ;
            }   else {
                //tidak bisa gerak kemana - mana;
            }
        }    
    }


        if(canMove){
            transform.position += moveDir * moveDistance;
        }
        isWalking = moveDir != Vector3.zero;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
        
    }

    private void SetSelectedCounter(BaseCounter selectedCounter){
        this.selectedCounter = selectedCounter;
        
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform(){
        return KitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null) {
            OnPickedSomething?.Invoke(this, OnSelectedCounterChangedEventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }

    public void ClearKitchenObject() {
        kitchenObject = null;
    }

    public bool HasKitchenObject(){
        return kitchenObject != null;
    }
}
