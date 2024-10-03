using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DevMashup{
    namespace GamePlay {
        public class InvSlot : MonoBehaviour, IDropHandler {
            public void OnDrop(PointerEventData eventData) {
                if(transform.childCount == 0) {
                    GameObject droppedObjUI = eventData.pointerDrag;

                    DraggableItems draggableItem = droppedObjUI.GetComponent<DraggableItems>();
                    draggableItem.parentAfterDrag = transform;
                }
                else {
                    GameObject dropped = eventData.pointerDrag;
                    DraggableItems draggingItem = dropped.GetComponent<DraggableItems>();
                    GameObject current = transform.GetChild(0).gameObject;
                    DraggableItems draggedItem = current.GetComponent<DraggableItems>();

                    if(World.Instance.playerScript.wholeInv.items[draggedItem.invValue] == null && World.Instance.playerScript.wholeInv.items[draggingItem.invValue] == null) {
                        Debug.Log("Cannot drag. Bothitems are null");
                    }
                    else {
                        draggedItem.transform.SetParent(draggingItem.parentAfterDrag);
                        draggingItem.parentAfterDrag = transform;
                        for(int i = 0; i < 40; i++) {
                            if(draggingItem.parentAfterDrag.name == World.Instance.playerScript.wholeInv.itemRect[i].name || current.transform.parent.name == World.Instance.playerScript.wholeInv.itemRect[1].name) {
                                World.Instance.playerScript.wholeInv.DeleteAndRefrenceRowOne("Refresh", draggingItem.invValue, draggedItem.invValue);
                            }
                        }
                    }
                } 
            }
        }
    }
}
