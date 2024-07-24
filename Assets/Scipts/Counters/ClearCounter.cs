using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter    {

    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    

    public override void Interact(Player player) {
            if(!HasKitchenObject()){
                //There is No KitchenObject here
                if(player.HasKitchenObject()){
                    //player is carrying something
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                } else {
                    // player no carry anything
                }
            } else {
                if(player.HasKitchenObject()){
                    //there is a kitchen object here
                    if(player.HasKitchenObject()) {
                        //player Is carrying something
                        if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)){
                            //player is holding a Plate
                            if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                                GetKitchenObject().DestroySelf();
                            }
                        } else {
                            // Player is not carrying Plate but something else
                            if (GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
                                //Counter is Holding a Plate
                                if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) {
                                    player.GetKitchenObject().DestroySelf();
                                }
                            }
                        }
                    }
                } else {
                    GetKitchenObject().SetKitchenObjectParent(player);
                }
            }
    }

    
}
