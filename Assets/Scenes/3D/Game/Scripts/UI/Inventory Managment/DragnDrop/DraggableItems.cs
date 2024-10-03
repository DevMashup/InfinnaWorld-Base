using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace DevMashup {
    namespace GamePlay {
        public class DraggableItems : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
            Image image;
            [HideInInspector]public Transform parentAfterDrag;
            [HideInInspector]public int invValue;

            void Start() {
                image = gameObject.GetComponent<Image>();
            }
            
            
            
            
            public void OnBeginDrag(PointerEventData eventData) {
                parentAfterDrag = transform.parent;
                transform.SetParent(World.Instance.uiScript.gameObject.transform); //!st layer of trasform in hierachy
                transform.SetAsLastSibling();
                image.raycastTarget = false;
            }

            public void OnDrag(PointerEventData eventData) {
                transform.position = Input.mousePosition;

            }

            public void OnEndDrag(PointerEventData eventData) {
                transform.SetParent(parentAfterDrag);
                image.raycastTarget = true;
            }
        }
    }
}
