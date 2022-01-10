using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Core
{
    public class Minimap : MonoBehaviour
    {
        [SerializeField] Transform player;
        [SerializeField] RectTransform playerSprite;
        [SerializeField] Transform worldZeroPoint;
        [SerializeField] float scaleDifference; // Native Size scale difference: 20.43

        [SerializeField] Vector2 minPos;
        [SerializeField] Vector2 maxPos;

        [SerializeField] float minX;
        [SerializeField] float maxX;
        [SerializeField] float minZ;
        [SerializeField] float maxZ;

        RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

        }

        private void LateUpdate()
        {
            Vector2 playerMapPosition = new Vector2(
                worldZeroPoint.position.x - player.position.x,
                worldZeroPoint.position.z - player.position.z);

            playerMapPosition *= scaleDifference;

            rectTransform.anchoredPosition = new Vector2(
                Mathf.Clamp(playerMapPosition.x,minPos.x,maxPos.x),
                Mathf.Clamp(playerMapPosition.y,minPos.y,maxPos.y));

            playerSprite.anchoredPosition = rectTransform.anchoredPosition - playerMapPosition;
            playerSprite.rotation = Quaternion.Euler(new Vector3(0, 0, -player.rotation.eulerAngles.y-45));
        }
    }

}