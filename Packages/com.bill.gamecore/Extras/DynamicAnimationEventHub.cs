using BillInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Diagnostics;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BillGameCore
{
    public class DynamicAnimationEventHub : MonoBehaviour
    {
        [System.Serializable]
        public struct EventMapping
        {
            [field: SerializeField] public string EventID { get; private set; }
            [field: SerializeField] public UnityEvent ActionsToTrigger { get; private set; }
        }

        [BillTitle("Dynamic Animation Event Hub")]
        [BillInfoBox("Maps event IDs to actions. Stored as a List<struct> so prefab overrides work.")]
        [SerializeField]
        [BillListDrawerSettings(ShowItemCount = true, DraggableItems = true)]
        private List<EventMapping> eventMappings = new List<EventMapping>();

        private readonly Dictionary<string, UnityEvent> _runtimeEventHub = new Dictionary<string, UnityEvent>();

        private void Awake()
        {
            InitializeFromInspector();
        }

        private void InitializeFromInspector()
        {
            _runtimeEventHub.Clear();
            for (int i = 0; i < eventMappings.Count; i++) // for loop: no enumerator allocation
            {
                var mapping = eventMappings[i];
                if (string.IsNullOrEmpty(mapping.EventID))
                {
                    LogWarning($"[DynamicEventHub] Empty EventID in the mappings on '{gameObject.name}'.");
                    continue;
                }

                if (_runtimeEventHub.ContainsKey(mapping.EventID))
                {
                    LogWarning($"[DynamicEventHub] Duplicate EventID '{mapping.EventID}' on '{gameObject.name}'; the first entry is used.");
                    continue;
                }

                _runtimeEventHub.Add(mapping.EventID, mapping.ActionsToTrigger);
            }
        }

        public void Trigger(string eventID)
        {
            if (string.IsNullOrEmpty(eventID))
            {
                LogWarning($"[DynamicEventHub] Received an empty EventID on '{gameObject.name}'.");
                return;
            }

            if (_runtimeEventHub.TryGetValue(eventID, out UnityEvent actionsToTrigger))
            {
                actionsToTrigger?.Invoke();
            }
            else
            {
                LogWarning($"[DynamicEventHub] EventID '{eventID}' not found on '{gameObject.name}'.");
            }
        }

        // ------------------------------------------------------------------------------------
        // Public runtime API for other scripts
        // ------------------------------------------------------------------------------------

        public bool HasEvent(string eventID)
        {
            return !string.IsNullOrEmpty(eventID) && _runtimeEventHub.ContainsKey(eventID);
        }

        public bool AddListener(string eventID, UnityAction call)
        {
            if (string.IsNullOrEmpty(eventID) || call == null) return false;

            if (!_runtimeEventHub.TryGetValue(eventID, out UnityEvent actionsToTrigger))
            {
                actionsToTrigger = new UnityEvent(); // allocates, but only when a new event ID is added at runtime
                _runtimeEventHub.Add(eventID, actionsToTrigger);
            }

            actionsToTrigger.AddListener(call);
            return true;
        }

        public bool RemoveListener(string eventID, UnityAction call)
        {
            if (string.IsNullOrEmpty(eventID) || call == null) return false;

            if (_runtimeEventHub.TryGetValue(eventID, out UnityEvent actionsToTrigger))
            {
                actionsToTrigger.RemoveListener(call);
                // The entry stays: runtime listener counts are not readable without reflection
                return true;
            }

            return false;
        }

        public bool RemoveAllListeners(string eventID)
        {
            if (string.IsNullOrEmpty(eventID)) return false;

            if (_runtimeEventHub.TryGetValue(eventID, out UnityEvent actionsToTrigger))
            {
                actionsToTrigger.RemoveAllListeners();
                // Remove the entry only when no persistent listeners are left
                if (actionsToTrigger.GetPersistentEventCount() == 0)
                {
                    _runtimeEventHub.Remove(eventID);
                }
                return true;
            }

            return false;
        }

        // Editor-only logging, stripped from builds
        [Conditional("UNITY_EDITOR")]
        private void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning(message, this);
        }

        // ------------------------------------------------------------------------------------
        // Scene view labels
        // ------------------------------------------------------------------------------------

    #if UNITY_EDITOR
        private void OnDrawGizmosSelected() // selected objects only, to avoid clutter
        {
            if (_runtimeEventHub.Count == 0) return;


            Vector3 labelPosition = transform.position + Vector3.up * 1.5f;


            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 12;

            // title
            Handles.Label(labelPosition, "Dynamic Event Hub Events:", style);
            labelPosition += Vector3.down * 0.2f;

            // event IDs
            foreach (var eventID in _runtimeEventHub.Keys)
            {
                Handles.Label(labelPosition, $"- {eventID}", style);
                labelPosition += Vector3.down * 0.2f; // next line
            }
        }
    #endif
    }
}
