using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace VRLabClass.Milestone3
{
    public class GoGo : MonoBehaviour
    {
        #region Properties

        [Header("Calculation Origin Configuration")]
        [SerializeField] private Transform _head; // Transform of user head --> used for origin calculation
        [SerializeField] private float _bodyCenterHeadOffset = .2f; // Vertical offset used to determine body center below users head

        private Vector3 _bodyCenter // returns position of body center used for calculation
        {
            get
            {
                Vector3 v = _head.position;
                v.y -= _bodyCenterHeadOffset;

                return v;
            }
        }

        [Header("GoGo Configuration")]
        [SerializeField] private Transform _hand; // Transform of users real hand
        [SerializeField] private Transform _gogoHand; // Hand transform to apply GoGo movement to
        [SerializeField] private GameObject _gogoVisual; // Hand visual that should be applied as soon as gogog hand exceeds the 1:1 mapping distance threshold
        [SerializeField][Range(0, 1)] private float _k = .167f; // value k in gogo equation
        [SerializeField][Range(0, 1)] private float _distanceThreshold = .4f; // value D in gogo equation

        #endregion

        #region MonoBehaviour Methods

        private void Start()
        {
            // Delete component if attached to remote users avatar
            if (GetComponentInParent<NetworkObject>() != null)
                if (!GetComponentInParent<NetworkObject>().IsOwner)
                {
                    Destroy(this);
                    return;
                }

            // set gogo hand to initial position and rotation, aligned with real hand
            _gogoHand.position = _hand.position;
            _gogoHand.rotation = _hand.rotation;

            // initially deactivate visuals
            _gogoVisual.SetActive(false);
        }

        private void Update()
        {
            ApplyGoGo();
        }

        #endregion

        #region GoGo Methods

        private void ApplyGoGo()
        {
            // Calculate distance from body center to hand in meters
            float distance = Vector3.Distance(_hand.position, _bodyCenter);

            // Convert distance to centimeters for GoGo equation (since the paper uses cm)
            float distanceCm = distance * 100f;

            // Check if hand exceeds distance threshold
            bool excedsThreshold = distance > _distanceThreshold;

            // Apply GoGo mapping or 1:1 mapping based on threshold
            if (excedsThreshold)
            {
                // GoGo equation: D' = D + k * (D - D_th)^2
                // Where D' is the extended distance, D is the hand distance, D_th is the threshold, k is the GoGo factor
                float extendedDistanceCm = distanceCm + _k * Mathf.Pow(distanceCm - (_distanceThreshold * 100f), 2f);

                // Convert back to meters
                float extendedDistance = extendedDistanceCm / 100f;

                // Calculate direction from body center to hand
                Vector3 direction = (_hand.position - _bodyCenter).normalized;

                // Apply extended position
                _gogoHand.position = _bodyCenter + direction * extendedDistance;

                // DEBUG: Log when GoGo is active
                Debug.Log($"GoGo ACTIVE - Hand Distance: {distance:F3}m | Extended Distance: {extendedDistance:F3}m");
            }
            else
            {
                // 1:1 mapping within threshold distance
                _gogoHand.position = _hand.position;
            }

            // Always sync rotation with real hand
            _gogoHand.rotation = _hand.rotation;

            // Activate/deactivate visuals based on threshold
            _gogoVisual.SetActive(excedsThreshold);

            // DEBUG: Draw debug lines in Scene view
            Debug.DrawLine(_bodyCenter, _hand.position, Color.green, 0f);  // Real hand
            Debug.DrawLine(_bodyCenter, _gogoHand.position, Color.red, 0f); // GoGo hand
        }

        #endregion
    }
}
